// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

public sealed partial class Logger
{
    internal readonly ref struct InterpolatedLogMessageInternal
    {
        private readonly LogMessageWriter? _writer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InterpolatedLogMessageInternal(LogLevel level, string msg)
        {
            _writer = LogBufferManager.Current.Allocate();
            _writer.BeginMessage(level);
            _writer.AppendLiteral(msg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InterpolatedLogMessageInternal(LogLevel level, LogEventId eventId, string msg)
        {
            _writer = LogBufferManager.Current.Allocate();
            _writer.BeginMessage(level);
            _writer.AppendEventId(eventId);
            _writer.AppendLiteral(msg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InterpolatedLogMessageInternal(LogLevel level, Exception? exception, string msg)
        {
            _writer = LogBufferManager.Current.Allocate();
            _writer.BeginMessage(level);
            _writer.AppendException(exception);
            _writer.AppendLiteral(msg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InterpolatedLogMessageInternal(LogLevel level, LogEventId eventId, Exception? exception, string msg)
        {
            _writer = LogBufferManager.Current.Allocate();
            _writer.BeginMessage(level);
            _writer.AppendEventId(eventId);
            _writer.AppendException(exception);
            _writer.AppendLiteral(msg);
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            Logger logger,
            out bool enabled)
        {
            var localEnabled = logger.IsEnabled(level);
            enabled = localEnabled;
            if (localEnabled)
            {
                _writer = LogBufferManager.Current.Allocate();
                _writer.BeginMessage(logger, level);
            }
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            LogEventId eventId,
            Logger logger,
            out bool enabled)
        {
            var localEnabled = logger.IsEnabled(level);
            enabled = localEnabled;
            if (localEnabled)
            {
                _writer = LogBufferManager.Current.Allocate();
                _writer.BeginMessage(logger, level);
                _writer.AppendEventId(eventId);
            }
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            Exception? exception,
            Logger logger,
            out bool enabled)
        {
            var localEnabled = logger.IsEnabled(level);
            enabled = localEnabled;
            if (localEnabled)
            {
                _writer = LogBufferManager.Current.Allocate();
                _writer.BeginMessage(logger, level);
                _writer.AppendException(exception);
            }
        }

        public InterpolatedLogMessageInternal(
            LogLevel level,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int literalLength,
            [SuppressMessage("ReSharper", "UnusedParameter.Local")] int formattedCount,
            LogEventId eventId,
            Exception? exception,
            Logger logger,
            out bool enabled)
        {
            var localEnabled = logger.IsEnabled(level);
            enabled = localEnabled;
            if (localEnabled)
            {
                _writer = LogBufferManager.Current.Allocate();
                _writer.BeginMessage(logger, level);
                _writer.AppendEventId(eventId);
                _writer.AppendException(exception);
            }
        }

        public bool IsLoggerEnabled => _writer != null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendEventId(LogEventId eventId) => _writer!.AppendEventId(eventId);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendException(Exception? exception) => _writer!.AppendException(exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<char> s) => _writer!.AppendLiteral(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<byte> s) => _writer!.AppendLiteral(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(string s) => _writer!.AppendLiteral(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s, int alignment) => _writer!.AppendFormatted(s, alignment);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> s, int alignment) => _writer!.AppendFormatted(s, alignment);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> s, int alignment) => _writer!.AppendFormatted(s, alignment);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value) => _writer!.AppendFormatted(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value, int alignment) => _writer!.AppendFormatted(value, alignment);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value) where T : unmanaged, ISpanFormattable => _writer!.AppendFormatted(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment) where T : unmanaged, ISpanFormattable => _writer!.AppendFormatted(value, alignment);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment, string? format) where T : unmanaged, ISpanFormattable => _writer!.AppendFormatted(value, alignment, format);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value) where T : unmanaged, ISpanFormattable => _writer!.AppendFormatted(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment) where T : unmanaged, ISpanFormattable => _writer!.AppendFormatted(value, alignment);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment, string? format) where T : unmanaged, ISpanFormattable => _writer!.AppendFormatted(value, alignment, format);
    }
}