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
    internal readonly ref struct InterpolatedLogMessageInternal
    {
        private readonly LogMessageInternal? _message;
        private readonly LoggerOverflowMode _overflowMode;
        private readonly int _syncSlot;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static LoggerOverflowMode ResolveOverflowMode(Logger logger)
        {
            var mode = logger.OverflowMode;
            return mode == LoggerOverflowMode.Default ? LoggerOverflowMode.DropAndNotify : mode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryCreateMessage(
            Logger logger,
            LogLevel level,
            LoggerOverflowMode overflowMode,
            out LogMessageInternal? message,
            out int syncSlot)
        {
            syncSlot = -1;
            message = null;

            var processor = LogManager.Processor ?? throw new InvalidOperationException("The LogManager processor is not initialized.");

            if (processor is LogMessageAsyncProcessor asyncProcessor)
            {
                if (!asyncProcessor.TryRentMessage(overflowMode, out message))
                {
                    return false;
                }

                return true;
            }

            if (!LogMessageInternalThreadCache.TryRent(out var syncMessage, out syncSlot))
            {
                return false;
            }

            message = syncMessage;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitializeMessage(
            LogMessageInternal message,
            Logger logger,
            LogLevel level,
            LogEventId eventId,
            LogPropertiesSnapshot properties,
            object? attachment,
            bool isMarkup,
            int initialTextCapacity)
        {
            var timestamp = LogManager.TimeProvider.GetUtcNow();
            var thread = Thread.CurrentThread;
            var scope = LogScopeContext.CaptureSnapshot();

            message.Initialize(
                logger,
                level,
                timestamp,
                thread,
                scope,
                eventId,
                properties,
                attachment,
                isMarkup,
                CultureInfo.InvariantCulture,
                initialTextCapacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private InterpolatedLogMessageInternal(
            Logger logger,
            LogLevel level,
            LogEventId eventId,
            LogPropertiesSnapshot properties,
            object? attachment,
            bool isMarkup)
        {
            _overflowMode = ResolveOverflowMode(logger);

            if (TryCreateMessage(logger, level, _overflowMode, out var message, out _syncSlot))
            {
                _message = message;
                InitializeMessage(message!, logger, level, eventId, properties, attachment, isMarkup, initialTextCapacity: 0);
            }
            else
            {
                _message = null;
                _syncSlot = -1;
                properties.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, string msg)
            : this(logger, level, LogEventId.Empty, LogPropertiesSnapshot.Empty, attachment: null, isMarkup: false)
        {
            if (_message is not null)
            {
                _message.AppendLiteral(msg);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, LogProperties properties, string msg)
            : this(logger, level, LogEventId.Empty, properties.Snapshot(), attachment: null, isMarkup: false)
        {
            if (_message is not null)
            {
                _message.AppendLiteral(msg);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, LogEventId eventId, string msg)
            : this(logger, level, eventId, LogPropertiesSnapshot.Empty, attachment: null, isMarkup: false)
        {
            if (_message is not null)
            {
                _message.AppendLiteral(msg);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, LogEventId eventId, LogProperties properties, string msg)
            : this(logger, level, eventId, properties.Snapshot(), attachment: null, isMarkup: false)
        {
            if (_message is not null)
            {
                _message.AppendLiteral(msg);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, object? attachment, string msg)
            : this(logger, level, LogEventId.Empty, LogPropertiesSnapshot.Empty, attachment, isMarkup: false)
        {
            if (_message is not null)
            {
                _message.AppendLiteral(msg);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, object? attachment, LogProperties properties, string msg)
            : this(logger, level, LogEventId.Empty, properties.Snapshot(), attachment, isMarkup: false)
        {
            if (_message is not null)
            {
                _message.AppendLiteral(msg);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, LogEventId eventId, object? attachment, string msg)
            : this(logger, level, eventId, LogPropertiesSnapshot.Empty, attachment, isMarkup: false)
        {
            if (_message is not null)
            {
                _message.AppendLiteral(msg);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InterpolatedLogMessageInternal(Logger logger, LogLevel level, LogEventId eventId, object? attachment, LogProperties properties, string msg)
            : this(logger, level, eventId, properties.Snapshot(), attachment, isMarkup: false)
        {
            if (_message is not null)
            {
                _message.AppendLiteral(msg);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            if (_message is not null)
            {
                _message.AppendLiteral(msg);
            }
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
                _overflowMode = LoggerOverflowMode.Default;
                _syncSlot = -1;
                return;
            }

            _overflowMode = ResolveOverflowMode(logger);

            var propertiesSnapshot = hasProperties ? properties.Snapshot() : LogPropertiesSnapshot.Empty;
            if (!TryCreateMessage(logger, level, _overflowMode, out var message, out _syncSlot))
            {
                enabled = false;
                _message = null;
                propertiesSnapshot.Dispose();
                return;
            }

            _message = message;
            var estimatedTextLength = Math.Max(0, literalLength + (formattedCount * 8));
            InitializeMessage(message!, logger, level, eventId, propertiesSnapshot, attachment, isMarkup, estimatedTextLength);
        }

        public bool IsLoggerEnabled => _message is not null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendEventId(LogEventId eventId)
        {
            if (_message is null)
            {
                return;
            }

            // EventId is immutable in the internal message once initialized for now.
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendException(object? attachment)
        {
            if (_message is null)
            {
                return;
            }

            // Attachment is immutable in the internal message once initialized for now.
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<char> s)
        {
            if (_message is not null)
            {
                _message.AppendLiteral(s);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<byte> s)
        {
            if (_message is not null)
            {
                _message.AppendLiteral(s);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(string s)
        {
            if (_message is not null)
            {
                _message.AppendLiteral(s);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s, int alignment)
        {
            if (_message is not null)
            {
                _message.AppendFormatted(s, alignment);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> s, int alignment)
        {
            if (_message is not null)
            {
                _message.AppendFormatted(s, alignment);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> s, int alignment)
        {
            if (_message is not null)
            {
                _message.AppendFormatted(s, alignment);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value)
        {
            if (_message is not null)
            {
                _message.AppendFormatted(value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value, int alignment)
        {
            if (_message is not null)
            {
                _message.AppendFormatted(value, alignment);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value) where T : unmanaged, ISpanFormattable
        {
            if (_message is not null)
            {
                _message.AppendFormatted(value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment) where T : unmanaged, ISpanFormattable
        {
            if (_message is not null)
            {
                _message.AppendFormatted(value, alignment);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value) where T : unmanaged, ISpanFormattable
        {
            if (_message is not null)
            {
                _message.AppendFormatted(value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment) where T : unmanaged, ISpanFormattable
        {
            if (_message is not null)
            {
                _message.AppendFormatted(value, alignment);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment, string? format) where T : unmanaged, ISpanFormattable
        {
            if (_message is not null)
            {
                _message.AppendFormatted(value, alignment, format);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment, string? format) where T : unmanaged, ISpanFormattable
        {
            if (_message is not null)
            {
                _message.AppendFormatted(value, alignment, format);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log()
        {
            var message = _message;
            if (message is null)
            {
                return;
            }

            var processor = LogManager.Processor;
            if (processor is null)
            {
                return;
            }

            if (processor is LogMessageAsyncProcessor asyncProcessor)
            {
                // Ownership transfers to the async processor which returns it to the pool.
                processor.Log(message, _overflowMode);
                return;
            }

            try
            {
                processor.Log(message, _overflowMode);
            }
            finally
            {
                message.Reset();
                LogMessageInternalThreadCache.Return(_syncSlot);
            }
        }
    }
}
