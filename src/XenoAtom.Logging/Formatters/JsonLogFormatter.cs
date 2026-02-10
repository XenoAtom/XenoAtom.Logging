// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Collections.Concurrent;
using System.Globalization;
using XenoAtom.Logging.Helpers;

namespace XenoAtom.Logging.Formatters;

/// <summary>
/// Formats a log message as a single JSON object.
/// </summary>
public sealed record class JsonLogFormatter : LogFormatter
{
    /// <summary>
    /// Gets a shared formatter instance that includes properties and scopes.
    /// </summary>
    public static JsonLogFormatter Instance { get; } = new();
    private static readonly ConcurrentDictionary<string, string> SnakeCaseNameCache = new(StringComparer.Ordinal);

    private readonly JsonLogSchemaProfile _schemaProfile;
    private readonly JsonLogFieldNamingPolicy _fieldNamingPolicy;
    private readonly bool _includeThreadId;
    private readonly bool _includeEventId;
    private readonly bool _includeEventName;
    private readonly bool _includeException;
    private readonly bool _includeProperties;
    private readonly bool _includeScopes;
    private readonly bool _useNestedEventObject;

    private readonly string _timestampField;
    private readonly string _levelField;
    private readonly string _loggerField;
    private readonly string _messageField;
    private readonly string _threadIdField;
    private readonly string _eventContainerField;
    private readonly string _eventIdField;
    private readonly string _eventNameField;
    private readonly string _exceptionField;
    private readonly string _propertiesField;
    private readonly string _scopesField;
    private readonly string _propertyNameField;
    private readonly string _propertyValueField;

    /// <summary>
    /// Initializes a new instance of <see cref="JsonLogFormatter"/> with default options.
    /// </summary>
    public JsonLogFormatter() : this(new JsonLogFormatterOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="JsonLogFormatter"/>.
    /// </summary>
    /// <param name="includeProperties">Indicates whether to include structured properties.</param>
    /// <param name="includeScopes">Indicates whether to include scope properties.</param>
    public JsonLogFormatter(bool includeProperties = true, bool includeScopes = true)
        : this(new JsonLogFormatterOptions { IncludeProperties = includeProperties, IncludeScopes = includeScopes })
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="JsonLogFormatter"/>.
    /// </summary>
    /// <param name="options">The formatter options.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public JsonLogFormatter(JsonLogFormatterOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _schemaProfile = options.SchemaProfile;
        _fieldNamingPolicy = options.FieldNamingPolicy;
        _includeThreadId = options.IncludeThreadId;
        _includeEventId = options.IncludeEventId;
        _includeEventName = options.IncludeEventName;
        _includeException = options.IncludeException;
        _includeProperties = options.IncludeProperties;
        _includeScopes = options.IncludeScopes;

        if (_schemaProfile == JsonLogSchemaProfile.Default)
        {
            _useNestedEventObject = true;
            _timestampField = ApplyNamingPolicy("timestamp");
            _levelField = ApplyNamingPolicy("level");
            _loggerField = ApplyNamingPolicy("logger");
            _messageField = ApplyNamingPolicy("message");
            _threadIdField = ApplyNamingPolicy("threadId");
            _eventContainerField = ApplyNamingPolicy("eventId");
            _eventIdField = ApplyNamingPolicy("id");
            _eventNameField = ApplyNamingPolicy("name");
            _exceptionField = ApplyNamingPolicy("exception");
            _propertiesField = ApplyNamingPolicy("properties");
            _scopesField = ApplyNamingPolicy("scopes");
            _propertyNameField = ApplyNamingPolicy("name");
            _propertyValueField = ApplyNamingPolicy("value");
        }
        else
        {
            _useNestedEventObject = false;
            _timestampField = "@timestamp";
            _levelField = "log.level";
            _loggerField = "log.logger";
            _messageField = "message";
            _threadIdField = "process.thread.id";
            _eventContainerField = string.Empty;
            _eventIdField = "event.code";
            _eventNameField = "event.action";
            _exceptionField = "error.stack_trace";
            _propertiesField = "labels";
            _scopesField = "xeno.scopes";
            _propertyNameField = "name";
            _propertyValueField = "value";
        }
    }

    /// <summary>
    /// Gets the schema profile used for emitted JSON.
    /// </summary>
    public JsonLogSchemaProfile SchemaProfile => _schemaProfile;

    /// <summary>
    /// Gets the field naming policy configured for <see cref="JsonLogSchemaProfile.Default"/> mode.
    /// </summary>
    /// <remarks>
    /// When <see cref="SchemaProfile"/> is <see cref="JsonLogSchemaProfile.ElasticCommonSchema"/>, ECS uses fixed field names
    /// and this policy does not affect emitted fields.
    /// </remarks>
    public JsonLogFieldNamingPolicy FieldNamingPolicy => _fieldNamingPolicy;

    /// <summary>
    /// Gets a value indicating whether thread id is included.
    /// </summary>
    public bool IncludeThreadId => _includeThreadId;

    /// <summary>
    /// Gets a value indicating whether event id fields are included.
    /// </summary>
    public bool IncludeEventId => _includeEventId;

    /// <summary>
    /// Gets a value indicating whether event name is included.
    /// </summary>
    public bool IncludeEventName => _includeEventName;

    /// <summary>
    /// Gets a value indicating whether exception text is included.
    /// </summary>
    public bool IncludeException => _includeException;

    /// <summary>
    /// Gets a value indicating whether message properties are included.
    /// </summary>
    public bool IncludeProperties => _includeProperties;

    /// <summary>
    /// Gets a value indicating whether active scopes are included.
    /// </summary>
    public bool IncludeScopes => _includeScopes;

    /// <inheritdoc />
    public override bool TryFormat(in LogMessage logMessage, Span<char> destination, out int charsWritten, ref LogMessageFormatSegments segments)
    {
        charsWritten = 0;
        var position = 0;
        var hasField = false;

        if (!TryAppend('{', destination, ref position))
        {
            return false;
        }

        if (!TryWriteFieldName(_timestampField, destination, ref position, ref hasField))
        {
            return false;
        }

        if (!TryWriteDateTime(logMessage.Timestamp, destination, ref position))
        {
            return false;
        }

        if (!TryWriteStringField(_levelField, logMessage.Level.ToLongString(), destination, ref position, ref hasField))
        {
            return false;
        }

        if (!TryWriteStringField(_loggerField, logMessage.Logger.Name, destination, ref position, ref hasField))
        {
            return false;
        }

        if (!TryWriteMessageField(logMessage, destination, ref position, ref hasField))
        {
            return false;
        }

        if (_includeThreadId &&
            !TryWriteNumberField(_threadIdField, logMessage.Thread.ManagedThreadId, destination, ref position, ref hasField))
        {
            return false;
        }

        if (_includeEventId && !logMessage.EventId.IsEmpty)
        {
            if (_useNestedEventObject)
            {
                if (!TryWriteFieldName(_eventContainerField, destination, ref position, ref hasField))
                {
                    return false;
                }

                if (!TryAppend('{', destination, ref position))
                {
                    return false;
                }

                var hasEventIdField = false;
                if (!TryWriteNumberField(_eventIdField, logMessage.EventId.Id, destination, ref position, ref hasEventIdField))
                {
                    return false;
                }

                if (_includeEventName && !string.IsNullOrEmpty(logMessage.EventId.Name))
                {
                    if (!TryWriteStringField(_eventNameField, logMessage.EventId.Name.AsSpan(), destination, ref position, ref hasEventIdField))
                    {
                        return false;
                    }
                }

                if (!TryAppend('}', destination, ref position))
                {
                    return false;
                }
            }
            else
            {
                if (!TryWriteNumberField(_eventIdField, logMessage.EventId.Id, destination, ref position, ref hasField))
                {
                    return false;
                }

                if (_includeEventName && !string.IsNullOrEmpty(logMessage.EventId.Name))
                {
                    if (!TryWriteStringField(_eventNameField, logMessage.EventId.Name.AsSpan(), destination, ref position, ref hasField))
                    {
                        return false;
                    }
                }
            }
        }

        if (_includeException && logMessage.Exception is not null)
        {
            if (!TryWriteStringField(_exceptionField, logMessage.Exception.ToString(), destination, ref position, ref hasField))
            {
                return false;
            }
        }

        if (_includeProperties && logMessage.Properties.Count > 0)
        {
            if (!TryWritePropertiesField(_propertiesField, _propertyNameField, _propertyValueField, logMessage.Properties, destination, ref position, ref hasField))
            {
                return false;
            }
        }

        if (_includeScopes && logMessage.Scope.Count > 0)
        {
            if (!TryWriteScopesField(_scopesField, _propertyNameField, _propertyValueField, logMessage.Scope, destination, ref position, ref hasField))
            {
                return false;
            }
        }

        if (!TryAppend('}', destination, ref position))
        {
            return false;
        }

        if (segments.IsEnabled)
        {
            segments.Add(0, position, LogMessageFormatSegmentKind.Text);
        }

        charsWritten = position;
        return true;
    }

    private static bool TryWriteScopesField(string name, string propertyNameField, string propertyValueField, LogScope scope, Span<char> destination, ref int position, ref bool hasField)
    {
        if (!TryWriteFieldName(name, destination, ref position, ref hasField))
        {
            return false;
        }

        if (!TryAppend('[', destination, ref position))
        {
            return false;
        }

        for (var scopeIndex = 0; scopeIndex < scope.Count; scopeIndex++)
        {
            if (scopeIndex > 0 && !TryAppend(',', destination, ref position))
            {
                return false;
            }

            if (!TryWritePropertyArray(scope[scopeIndex], propertyNameField, propertyValueField, destination, ref position))
            {
                return false;
            }
        }

        return TryAppend(']', destination, ref position);
    }

    private static bool TryWritePropertiesField(string name, string propertyNameField, string propertyValueField, LogPropertiesReader properties, Span<char> destination, ref int position, ref bool hasField)
    {
        if (!TryWriteFieldName(name, destination, ref position, ref hasField))
        {
            return false;
        }

        return TryWritePropertyArray(properties, propertyNameField, propertyValueField, destination, ref position);
    }

    private static bool TryWritePropertyArray(LogPropertiesReader properties, string propertyNameField, string propertyValueField, Span<char> destination, ref int position)
    {
        if (!TryAppend('[', destination, ref position))
        {
            return false;
        }

        var index = 0;
        foreach (var property in properties)
        {
            if (index > 0 && !TryAppend(',', destination, ref position))
            {
                return false;
            }

            if (!TryAppend('{', destination, ref position))
            {
                return false;
            }

            var hasPropertyField = false;
            if (!TryWriteStringField(propertyNameField, property.Name, destination, ref position, ref hasPropertyField))
            {
                return false;
            }

            if (!TryWriteStringField(propertyValueField, property.Value, destination, ref position, ref hasPropertyField))
            {
                return false;
            }

            if (!TryAppend('}', destination, ref position))
            {
                return false;
            }

            index++;
        }

        return TryAppend(']', destination, ref position);
    }

    private bool TryWriteMessageField(in LogMessage logMessage, Span<char> destination, ref int position, ref bool hasField)
    {
        var text = logMessage.Text;
        if (!logMessage.IsMarkup || text.IsEmpty)
        {
            return TryWriteStringField(_messageField, text, destination, ref position, ref hasField);
        }

        var rentedBuffer = ArrayPool<char>.Shared.Rent(MarkupStripper.GetMaxOutputLength(text.Length));
        try
        {
            var charsWritten = MarkupStripper.Strip(text, rentedBuffer);
            return TryWriteStringField(_messageField, rentedBuffer.AsSpan(0, charsWritten), destination, ref position, ref hasField);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rentedBuffer);
        }
    }

    private static bool TryWriteStringField(string name, string value, Span<char> destination, ref int position, ref bool hasField)
        => TryWriteStringField(name, value.AsSpan(), destination, ref position, ref hasField);

    private static bool TryWriteStringField(string name, ReadOnlySpan<char> value, Span<char> destination, ref int position, ref bool hasField)
    {
        if (!TryWriteFieldName(name, destination, ref position, ref hasField))
        {
            return false;
        }

        return TryWriteEscapedString(value, destination, ref position);
    }

    private static bool TryWriteNumberField(string name, int value, Span<char> destination, ref int position, ref bool hasField)
    {
        if (!TryWriteFieldName(name, destination, ref position, ref hasField))
        {
            return false;
        }

        if (!value.TryFormat(destination[position..], out var charsWritten, provider: CultureInfo.InvariantCulture))
        {
            return false;
        }

        position += charsWritten;
        return true;
    }

    private static bool TryWriteDateTime(DateTime value, Span<char> destination, ref int position)
    {
        if (!TryAppend('"', destination, ref position))
        {
            return false;
        }

        if (!value.TryFormat(destination[position..], out var charsWritten, "O", CultureInfo.InvariantCulture))
        {
            return false;
        }

        position += charsWritten;
        return TryAppend('"', destination, ref position);
    }

    private static bool TryWriteFieldName(string name, Span<char> destination, ref int position, ref bool hasField)
    {
        if (hasField && !TryAppend(',', destination, ref position))
        {
            return false;
        }

        if (!TryWriteEscapedString(name.AsSpan(), destination, ref position))
        {
            return false;
        }

        if (!TryAppend(':', destination, ref position))
        {
            return false;
        }

        hasField = true;
        return true;
    }

    // Characters requiring JSON escaping: all control chars (0x00-0x1F), '"' and '\'.
    private static readonly SearchValues<char> JsonEscapeChars = SearchValues.Create(
        "\"\\\u0000\u0001\u0002\u0003\u0004\u0005\u0006\u0007\b\t\n\u000b\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f");

    private static bool TryWriteEscapedString(ReadOnlySpan<char> value, Span<char> destination, ref int position)
    {
        if (!TryAppend('"', destination, ref position))
        {
            return false;
        }

        // Fast path: bulk-copy segments that need no escaping
        while (!value.IsEmpty)
        {
            var idx = value.IndexOfAny(JsonEscapeChars);
            if (idx < 0)
            {
                // No more chars to escape — copy remainder
                if (!TryAppend(value, destination, ref position))
                {
                    return false;
                }
                break;
            }

            // Copy the safe prefix
            if (idx > 0)
            {
                if (!TryAppend(value[..idx], destination, ref position))
                {
                    return false;
                }
            }

            // Escape the single character at idx
            var character = value[idx];
            switch (character)
            {
                case '"':
                    if (!TryAppend("\\\"", destination, ref position)) return false;
                    break;
                case '\\':
                    if (!TryAppend("\\\\", destination, ref position)) return false;
                    break;
                case '\b':
                    if (!TryAppend("\\b", destination, ref position)) return false;
                    break;
                case '\f':
                    if (!TryAppend("\\f", destination, ref position)) return false;
                    break;
                case '\n':
                    if (!TryAppend("\\n", destination, ref position)) return false;
                    break;
                case '\r':
                    if (!TryAppend("\\r", destination, ref position)) return false;
                    break;
                case '\t':
                    if (!TryAppend("\\t", destination, ref position)) return false;
                    break;
                default:
                    // Control character < 0x20
                    if (!TryAppend("\\u00", destination, ref position)) return false;
                    if (!TryAppend(GetHex(character >> 4), destination, ref position)) return false;
                    if (!TryAppend(GetHex(character), destination, ref position)) return false;
                    break;
            }

            value = value[(idx + 1)..];
        }

        return TryAppend('"', destination, ref position);
    }

    private static char GetHex(int value)
    {
        var nibble = value & 0xF;
        return nibble < 10 ? (char)('0' + nibble) : (char)('A' + (nibble - 10));
    }

    private static bool TryAppend(string text, Span<char> destination, ref int position)
        => TryAppend(text.AsSpan(), destination, ref position);

    private static bool TryAppend(ReadOnlySpan<char> text, Span<char> destination, ref int position)
    {
        if (text.Length > destination.Length - position)
        {
            return false;
        }

        text.CopyTo(destination[position..]);
        position += text.Length;
        return true;
    }

    private static bool TryAppend(char value, Span<char> destination, ref int position)
    {
        if ((uint)position >= (uint)destination.Length)
        {
            return false;
        }

        destination[position++] = value;
        return true;
    }

    private string ApplyNamingPolicy(string fieldName)
    {
        if (_fieldNamingPolicy == JsonLogFieldNamingPolicy.Default)
        {
            return fieldName;
        }

        return _fieldNamingPolicy switch
        {
            JsonLogFieldNamingPolicy.SnakeCase => ToSnakeCase(fieldName),
            _ => throw new ArgumentOutOfRangeException(nameof(_fieldNamingPolicy), _fieldNamingPolicy, null)
        };
    }

    private static string ToSnakeCase(string value)
    {
        if (value.Length == 0)
        {
            return value;
        }

        if (SnakeCaseNameCache.TryGetValue(value, out var cachedValue))
        {
            return cachedValue;
        }

        var buffer = value.Length <= 64
            ? stackalloc char[value.Length * 2]
            : new char[value.Length * 2];

        var writeIndex = 0;
        for (var index = 0; index < value.Length; index++)
        {
            var character = value[index];
            if (char.IsUpper(character))
            {
                if (index > 0 && (char.IsLower(value[index - 1]) || (index + 1 < value.Length && char.IsLower(value[index + 1]))))
                {
                    buffer[writeIndex++] = '_';
                }

                buffer[writeIndex++] = char.ToLowerInvariant(character);
            }
            else
            {
                buffer[writeIndex++] = character;
            }
        }

        var snakeCaseValue = new string(buffer[..writeIndex]);
        return SnakeCaseNameCache.GetOrAdd(value, snakeCaseValue);
    }
}
