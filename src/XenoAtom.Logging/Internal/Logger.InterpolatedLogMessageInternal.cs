// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

/// <inheritdoc />
public sealed partial class Logger
{
    [SkipLocalsInit]
    internal readonly ref struct InterpolatedLogMessageInternal
    {
        private readonly LogMessageInternal? _message;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static LoggerOverflowMode ResolveOverflowMode(Logger logger)
        {
            var mode = logger.OverflowMode;
            return mode == LoggerOverflowMode.Default ? LoggerOverflowMode.DropAndNotify : mode;
        }

        private static bool TryCreateMessage(
            Logger logger,
            LoggerOverflowMode overflowMode,
            out LogMessageInternal? message)
        {
            var processor = LogManager.Processor ?? ThrowLogManagerNotInitialized();

            if (processor is LogMessageAsyncProcessor asyncProcessor)
            {
                if (!asyncProcessor.TryRentMessage(overflowMode, out var asyncMessage))
                {
                    message = null;
                    return false;
                }

                asyncMessage!.Processor = processor;
                asyncMessage.OverflowMode = overflowMode;
                asyncMessage.SyncSlot = -1;
                message = asyncMessage;
                return true;
            }
            else if (!LogMessageInternalThreadCache.TryRent(out var syncMessage, out var syncSlot))
            {
                message = null;
                return false;
            }
            else
            {
                syncMessage.Processor = processor;
                syncMessage.OverflowMode = overflowMode;
                syncMessage.SyncSlot = syncSlot;
                message = syncMessage;
                return true;
            }
        }

        [DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
        private static LogMessageProcessor ThrowLogManagerNotInitialized() => throw new InvalidOperationException("The LogManager processor is not initialized.");

        private InterpolatedLogMessageInternal(
            Logger logger,
            LogLevel level,
            LogEventId eventId,
            LogPropertiesSnapshot properties,
            object? attachment,
            bool isMarkup)
        {
            if (TryCreateMessage(logger, ResolveOverflowMode(logger), out var message))
            {
                _message = message;
                message!.Initialize(
                    logger,
                    level,
                    LogManager.TimeProvider.GetUtcNow(),
                    Thread.CurrentThread,
                    LogScopeContext.CaptureSnapshot(),
                    eventId,
                    properties,
                    attachment,
                    isMarkup,
                    CultureInfo.InvariantCulture,
                    initialTextCapacity: 0);
            }
            else
            {
                _message = null;
                properties.Dispose();
            }
        }

        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, string msg)
            : this(logger, level, LogEventId.Empty, LogPropertiesSnapshot.Empty, attachment: null, isMarkup: false)
        {
            _message?.AppendLiteral(msg);
        }

        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, LogProperties properties, string msg)
            : this(logger, level, LogEventId.Empty, properties.Snapshot(), attachment: null, isMarkup: false)
        {
            _message?.AppendLiteral(msg);
        }

        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, LogEventId eventId, string msg)
            : this(logger, level, eventId, LogPropertiesSnapshot.Empty, attachment: null, isMarkup: false)
        {
            _message?.AppendLiteral(msg);
        }

        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, LogEventId eventId, LogProperties properties, string msg)
            : this(logger, level, eventId, properties.Snapshot(), attachment: null, isMarkup: false)
        {
            _message?.AppendLiteral(msg);
        }

        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, object? attachment, string msg)
            : this(logger, level, LogEventId.Empty, LogPropertiesSnapshot.Empty, attachment, isMarkup: false)
        {
            _message?.AppendLiteral(msg);
        }

        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, object? attachment, LogProperties properties, string msg)
            : this(logger, level, LogEventId.Empty, properties.Snapshot(), attachment, isMarkup: false)
        {
            _message?.AppendLiteral(msg);
        }

        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, LogEventId eventId, object? attachment, string msg)
            : this(logger, level, eventId, LogPropertiesSnapshot.Empty, attachment, isMarkup: false)
        {
            _message?.AppendLiteral(msg);
        }

        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, LogEventId eventId, object? attachment, LogProperties properties, string msg)
            : this(logger, level, eventId, properties.Snapshot(), attachment, isMarkup: false)
        {
            _message?.AppendLiteral(msg);
        }

        internal InterpolatedLogMessageInternal(
            Logger logger,
            LogLevel level,
            LogEventId eventId,
            LogPropertiesSnapshot properties,
            object? attachment,
            bool isMarkup,
            ReadOnlySpan<char> msg)
            : this(logger, level, eventId, properties, attachment, isMarkup)
        {
            _message?.AppendLiteral(msg);
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            Logger logger,
            out bool enabled)
            : this(level, literalLength, formattedCount, LogEventId.Empty, attachment: null, isMarkup: false, logger, properties: default, hasProperties: false, out enabled)
        {
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            Logger logger,
            LogProperties properties,
            out bool enabled)
            : this(level, literalLength, formattedCount, LogEventId.Empty, attachment: null, isMarkup: false, logger, properties, hasProperties: true, out enabled)
        {
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            LogEventId eventId,
            Logger logger,
            out bool enabled)
            : this(level, literalLength, formattedCount, eventId, attachment: null, isMarkup: false, logger, properties: default, hasProperties: false, out enabled)
        {
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            LogEventId eventId,
            Logger logger,
            LogProperties properties,
            out bool enabled)
            : this(level, literalLength, formattedCount, eventId, attachment: null, isMarkup: false, logger, properties, hasProperties: true, out enabled)
        {
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            object? attachment,
            Logger logger,
            out bool enabled)
            : this(level, literalLength, formattedCount, LogEventId.Empty, attachment, isMarkup: false, logger, properties: default, hasProperties: false, out enabled)
        {
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            object? attachment,
            Logger logger,
            LogProperties properties,
            out bool enabled)
            : this(level, literalLength, formattedCount, LogEventId.Empty, attachment, isMarkup: false, logger, properties, hasProperties: true, out enabled)
        {
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            LogEventId eventId,
            object? attachment,
            Logger logger,
            out bool enabled)
            : this(level, literalLength, formattedCount, eventId, attachment, isMarkup: false, logger, properties: default, hasProperties: false, out enabled)
        {
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            LogEventId eventId,
            object? attachment,
            Logger logger,
            LogProperties properties,
            out bool enabled)
            : this(level, literalLength, formattedCount, eventId, attachment, isMarkup: false, logger, properties, hasProperties: true, out enabled)
        {
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            object? attachment,
            bool isMarkup,
            Logger logger,
            out bool enabled)
            : this(level, literalLength, formattedCount, LogEventId.Empty, attachment, isMarkup, logger, properties: default, hasProperties: false, out enabled)
        {
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            object? attachment,
            bool isMarkup,
            Logger logger,
            LogProperties properties,
            out bool enabled)
            : this(level, literalLength, formattedCount, LogEventId.Empty, attachment, isMarkup, logger, properties, hasProperties: true, out enabled)
        {
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            LogEventId eventId,
            object? attachment,
            bool isMarkup,
            Logger logger,
            out bool enabled)
            : this(level, literalLength, formattedCount, eventId, attachment, isMarkup, logger, properties: default, hasProperties: false, out enabled)
        {
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            LogEventId eventId,
            object? attachment,
            bool isMarkup,
            Logger logger,
            LogProperties properties,
            out bool enabled)
            : this(level, literalLength, formattedCount, eventId, attachment, isMarkup, logger, properties, hasProperties: true, out enabled)
        {
        }

        private InterpolatedLogMessageInternal(
            LogLevel level,
            int literalLength,
            int formattedCount,
            LogEventId eventId,
            object? attachment,
            bool isMarkup,
            Logger logger,
            LogProperties properties,
            bool hasProperties,
            out bool enabled)
        {
            var localEnabled = logger.IsEnabled(level);
            enabled = localEnabled;
            if (!localEnabled)
            {
                _message = null;
                return;
            }

            var propertiesSnapshot = hasProperties ? properties.Snapshot() : LogPropertiesSnapshot.Empty;
            if (!TryCreateMessage(logger, ResolveOverflowMode(logger), out var message))
            {
                enabled = false;
                _message = null;
                propertiesSnapshot.Dispose();
                return;
            }

            _message = message;
            var estimatedTextLength = Math.Max(0, literalLength + (formattedCount * 8));
            message!.Initialize(
                logger,
                level,
                LogManager.TimeProvider.GetUtcNow(),
                Thread.CurrentThread,
                LogScopeContext.CaptureSnapshot(),
                eventId,
                propertiesSnapshot,
                attachment,
                isMarkup,
                CultureInfo.InvariantCulture,
                estimatedTextLength);
        }

        public bool IsLoggerEnabled => _message is not null;

        public void AppendEventId(LogEventId eventId)
        {
            if (_message is null)
            {
                return;
            }

            // EventId is immutable in the internal message once initialized for now.
        }

        public void AppendException(object? attachment)
        {
            if (_message is null)
            {
                return;
            }

            // Attachment is immutable in the internal message once initialized for now.
        }

        public void AppendLiteral(ReadOnlySpan<char> s)
        {
            _message?.AppendLiteral(s);
        }

        public void AppendLiteral(ReadOnlySpan<byte> s)
        {
            _message?.AppendLiteral(s);
        }

        public void AppendLiteral(string s)
        {
            _message?.AppendLiteral(s);
        }

        public void AppendFormatted(string s, int alignment)
        {
            _message?.AppendFormatted(s, alignment);
        }

        public void AppendFormatted(ReadOnlySpan<char> s, int alignment)
        {
            _message?.AppendFormatted(s, alignment);
        }

        public void AppendFormatted(ReadOnlySpan<byte> s, int alignment)
        {
            _message?.AppendFormatted(s, alignment);
        }

        public void AppendFormatted(bool value)
        {
            _message?.AppendFormatted(value);
        }

        public void AppendFormatted(bool value, int alignment)
        {
            _message?.AppendFormatted(value, alignment);
        }

        public void AppendFormatted<T>(T value) where T : unmanaged, ISpanFormattable
        {
            _message?.AppendFormatted(value);
        }

        public void AppendFormatted<T>(T value, int alignment) where T : unmanaged, ISpanFormattable
        {
            _message?.AppendFormatted(value, alignment);
        }

        public void AppendFormatted<T>(T? value) where T : unmanaged, ISpanFormattable
        {
            _message?.AppendFormatted(value);
        }

        public void AppendFormatted<T>(T? value, int alignment) where T : unmanaged, ISpanFormattable
        {
            _message?.AppendFormatted(value, alignment);
        }

        public void AppendFormatted<T>(T? value, int alignment, string? format) where T : unmanaged, ISpanFormattable
        {
            _message?.AppendFormatted(value, alignment, format);
        }

        public void AppendFormatted<T>(T value, int alignment, string? format) where T : unmanaged, ISpanFormattable
        {
            _message?.AppendFormatted(value, alignment, format);
        }

        public void Log()
        {
            var message = _message;
            if (message is null)
            {
                return;
            }

            var processor = message.Processor!;
            var syncSlot = message.SyncSlot;

            if (syncSlot < 0)
            {
                // Async path: ownership transfers to the async processor which returns it to the pool.
                processor.Log(message, message.OverflowMode);
                return;
            }

            // Sync path
            try
            {
                processor.Log(message, message.OverflowMode);
            }
            finally
            {
                message.Reset();
                LogMessageInternalThreadCache.Return(syncSlot);
            }
        }
    }
}
