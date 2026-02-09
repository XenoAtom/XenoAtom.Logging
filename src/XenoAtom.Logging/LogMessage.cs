// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Represents a decoded log message passed to <see cref="LogWriter"/>.
/// </summary>
public readonly ref struct LogMessage
{
    private readonly LogMessageInternal _internalMessage;

    internal LogMessage(LogMessageInternal message)
    {
        _internalMessage = message;
    }

    /// <summary>
    /// Gets the logger that emitted this message.
    /// </summary>
    public Logger Logger => _internalMessage.Logger;

    /// <summary>
    /// Gets the level associated with this message.
    /// </summary>
    public LogLevel Level => _internalMessage.Level;

    /// <summary>
    /// Gets the sequence id of this message.
    /// </summary>
    public long SequenceId => _internalMessage.SequenceId;

    /// <summary>
    /// Gets the timestamp associated with this message.
    /// </summary>
    public DateTime Timestamp => _internalMessage.Timestamp.UtcDateTime;

    /// <summary>
    /// Gets the timestamp associated with this message.
    /// </summary>
    /// <remarks>
    /// This property is an alias of <see cref="Timestamp"/>.
    /// </remarks>
    public DateTime DateTime => Timestamp;

    /// <summary>
    /// Gets the event id associated with this message.
    /// </summary>
    public LogEventId EventId => _internalMessage.EventId;

    /// <summary>
    /// Gets the thread that emitted this message.
    /// </summary>
    public Thread Thread => _internalMessage.Thread;

    /// <summary>
    /// Gets the active scope when this message was emitted.
    /// </summary>
    public LogScope Scope => new(_internalMessage.Scope);

    /// <summary>
    /// Gets the textual message payload.
    /// </summary>
    public ReadOnlySpan<char> Text => _internalMessage.Text;

    /// <summary>
    /// Gets the additional structured properties.
    /// </summary>
    public LogPropertiesReader Properties => new(_internalMessage.Properties);

    /// <summary>
    /// Gets the attachment associated with this message.
    /// </summary>
    public object? Attachment => _internalMessage.Attachment;

    /// <summary>
    /// Gets the exception attached to this message.
    /// </summary>
    /// <remarks>
    /// This property is a convenience view over <see cref="Attachment"/>.
    /// </remarks>
    public Exception? Exception => _internalMessage.Attachment as Exception;

    /// <summary>
    /// Gets a value indicating whether <see cref="Text"/> contains markup tags.
    /// </summary>
    public bool IsMarkup => _internalMessage.IsMarkup;

    /// <summary>
    /// Gets the format provider for format-sensitive rendering.
    /// </summary>
    public IFormatProvider FormatProvider => _internalMessage.FormatProvider;
}

/// <summary>
/// Represents a reader over structured log properties.
/// </summary>
public readonly ref struct LogPropertiesReader
{
    private readonly LogPropertiesSnapshot? _snapshot;

    internal LogPropertiesReader(LogPropertiesSnapshot snapshot)
    {
        _snapshot = snapshot;
    }

    /// <summary>
    /// Gets the number of properties in this reader.
    /// </summary>
    public int Count => _snapshot?.Count ?? 0;

    /// <summary>
    /// Gets a property by index.
    /// </summary>
    public LogProperty this[int index] => (_snapshot ?? LogPropertiesSnapshot.Empty)[index];

    /// <summary>
    /// Gets an enumerator over properties.
    /// </summary>
    public Enumerator GetEnumerator() => new(_snapshot ?? LogPropertiesSnapshot.Empty);

    /// <summary>
    /// Determines whether the reader contains an exact name/value property pair.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The property value.</param>
    /// <returns><see langword="true"/> if the property exists; otherwise <see langword="false"/>.</returns>
    public bool Contains(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
        => (_snapshot ?? LogPropertiesSnapshot.Empty).Contains(name, value);

    /// <summary>
    /// Enumerator for <see cref="LogPropertiesReader"/>.
    /// </summary>
    public ref struct Enumerator
    {
        private readonly ReadOnlySpan<byte> _payload;
        private readonly int _count;
        private int _index;
        private int _position;
        private LogProperty _current;

        internal Enumerator(LogPropertiesSnapshot snapshot)
        {
            _payload = snapshot.BufferSpan;
            _count = snapshot.Count;
            _index = -1;
            _position = 0;
            _current = default;
        }

        /// <summary>
        /// Advances to the next property in the reader.
        /// </summary>
        /// <returns><see langword="true"/> when a property is available; otherwise <see langword="false"/>.</returns>
        /// <exception cref="InvalidOperationException">The underlying payload is invalid.</exception>
        public bool MoveNext()
        {
            var next = _index + 1;
            if ((uint)next >= (uint)_count)
            {
                return false;
            }

            if (!LogPropertiesEncoding.TryReadEntry(_payload, ref _position, out var nameOffset, out var nameCharCount, out var valueOffset, out var valueCharCount))
            {
                throw new InvalidOperationException("Invalid property payload.");
            }

            _current = new LogProperty(
                LogPropertiesEncoding.DecodeCharSpan(_payload, nameOffset, nameCharCount),
                LogPropertiesEncoding.DecodeCharSpan(_payload, valueOffset, valueCharCount));
            _index = next;
            return true;
        }

        /// <summary>
        /// Gets the current property.
        /// </summary>
        public LogProperty Current => _current;
    }
}
