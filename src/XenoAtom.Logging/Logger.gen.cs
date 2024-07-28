// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
//
// This file is auto-generated. Edit Logger.gen.tt to modify it.
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

partial class Logger
{

    /// <summary>
    /// Interpolated string handler for <see cref="LogLevel.Trace"/> log level.
    /// </summary>
    [InterpolatedStringHandler]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public readonly ref struct TraceInterpolatedLogMessage
    {
        internal readonly InterpolatedLogMessageInternal Internal;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceInterpolatedLogMessage"/> struct.
        /// </summary>
        public TraceInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Trace, literalLength, formattedCount, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceInterpolatedLogMessage"/> struct.
        /// </summary>
        public TraceInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Trace, literalLength, formattedCount, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceInterpolatedLogMessage"/> struct.
        /// </summary>
        public TraceInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Trace, literalLength, formattedCount, eventId, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceInterpolatedLogMessage"/> struct.
        /// </summary>
        public TraceInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Trace, literalLength, formattedCount, eventId, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceInterpolatedLogMessage"/> struct.
        /// </summary>
        public TraceInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Exception? exception,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Trace, literalLength, formattedCount, exception, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceInterpolatedLogMessage"/> struct.
        /// </summary>
        public TraceInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Exception? exception,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Trace, literalLength, formattedCount, exception, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceInterpolatedLogMessage"/> struct.
        /// </summary>
        public TraceInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Exception? exception,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Trace, literalLength, formattedCount, eventId, exception, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceInterpolatedLogMessage"/> struct.
        /// </summary>
        public TraceInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Exception? exception,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Trace, literalLength, formattedCount, eventId, exception, logger, properties, out enabled);

        /// <summary>
        /// Appends a literal span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<char> s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a literal span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<byte> s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a literal string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral([ConstantExpected] string s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a formatted string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a formatted string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s, int alignment)
            => Internal.AppendFormatted(s, alignment);

        /// <summary>
        /// Appends a formatted span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> value)
            => Internal.AppendLiteral(value);

        /// <summary>
        /// Appends a formatted span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted span of bytes to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> value)
            => Internal.AppendLiteral(value);

        /// <summary>
        /// Appends a formatted span of bytes to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted boolean to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value)
            => Internal.AppendFormatted(value);

        /// <summary>
        /// Appends a formatted boolean to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value);
            
        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment, string? format) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment, format);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment, string? format) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment, format);
    }

    /// <summary>
    /// Interpolated string handler for <see cref="LogLevel.Debug"/> log level.
    /// </summary>
    [InterpolatedStringHandler]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public readonly ref struct DebugInterpolatedLogMessage
    {
        internal readonly InterpolatedLogMessageInternal Internal;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInterpolatedLogMessage"/> struct.
        /// </summary>
        public DebugInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Debug, literalLength, formattedCount, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInterpolatedLogMessage"/> struct.
        /// </summary>
        public DebugInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Debug, literalLength, formattedCount, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInterpolatedLogMessage"/> struct.
        /// </summary>
        public DebugInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Debug, literalLength, formattedCount, eventId, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInterpolatedLogMessage"/> struct.
        /// </summary>
        public DebugInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Debug, literalLength, formattedCount, eventId, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInterpolatedLogMessage"/> struct.
        /// </summary>
        public DebugInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Exception? exception,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Debug, literalLength, formattedCount, exception, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInterpolatedLogMessage"/> struct.
        /// </summary>
        public DebugInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Exception? exception,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Debug, literalLength, formattedCount, exception, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInterpolatedLogMessage"/> struct.
        /// </summary>
        public DebugInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Exception? exception,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Debug, literalLength, formattedCount, eventId, exception, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInterpolatedLogMessage"/> struct.
        /// </summary>
        public DebugInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Exception? exception,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Debug, literalLength, formattedCount, eventId, exception, logger, properties, out enabled);

        /// <summary>
        /// Appends a literal span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<char> s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a literal span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<byte> s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a literal string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral([ConstantExpected] string s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a formatted string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a formatted string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s, int alignment)
            => Internal.AppendFormatted(s, alignment);

        /// <summary>
        /// Appends a formatted span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> value)
            => Internal.AppendLiteral(value);

        /// <summary>
        /// Appends a formatted span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted span of bytes to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> value)
            => Internal.AppendLiteral(value);

        /// <summary>
        /// Appends a formatted span of bytes to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted boolean to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value)
            => Internal.AppendFormatted(value);

        /// <summary>
        /// Appends a formatted boolean to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value);
            
        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment, string? format) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment, format);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment, string? format) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment, format);
    }

    /// <summary>
    /// Interpolated string handler for <see cref="LogLevel.Info"/> log level.
    /// </summary>
    [InterpolatedStringHandler]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public readonly ref struct InfoInterpolatedLogMessage
    {
        internal readonly InterpolatedLogMessageInternal Internal;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoInterpolatedLogMessage"/> struct.
        /// </summary>
        public InfoInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Info, literalLength, formattedCount, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoInterpolatedLogMessage"/> struct.
        /// </summary>
        public InfoInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Info, literalLength, formattedCount, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoInterpolatedLogMessage"/> struct.
        /// </summary>
        public InfoInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Info, literalLength, formattedCount, eventId, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoInterpolatedLogMessage"/> struct.
        /// </summary>
        public InfoInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Info, literalLength, formattedCount, eventId, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoInterpolatedLogMessage"/> struct.
        /// </summary>
        public InfoInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Exception? exception,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Info, literalLength, formattedCount, exception, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoInterpolatedLogMessage"/> struct.
        /// </summary>
        public InfoInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Exception? exception,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Info, literalLength, formattedCount, exception, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoInterpolatedLogMessage"/> struct.
        /// </summary>
        public InfoInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Exception? exception,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Info, literalLength, formattedCount, eventId, exception, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoInterpolatedLogMessage"/> struct.
        /// </summary>
        public InfoInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Exception? exception,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Info, literalLength, formattedCount, eventId, exception, logger, properties, out enabled);

        /// <summary>
        /// Appends a literal span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<char> s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a literal span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<byte> s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a literal string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral([ConstantExpected] string s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a formatted string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a formatted string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s, int alignment)
            => Internal.AppendFormatted(s, alignment);

        /// <summary>
        /// Appends a formatted span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> value)
            => Internal.AppendLiteral(value);

        /// <summary>
        /// Appends a formatted span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted span of bytes to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> value)
            => Internal.AppendLiteral(value);

        /// <summary>
        /// Appends a formatted span of bytes to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted boolean to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value)
            => Internal.AppendFormatted(value);

        /// <summary>
        /// Appends a formatted boolean to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value);
            
        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment, string? format) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment, format);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment, string? format) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment, format);
    }

    /// <summary>
    /// Interpolated string handler for <see cref="LogLevel.Warn"/> log level.
    /// </summary>
    [InterpolatedStringHandler]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public readonly ref struct WarnInterpolatedLogMessage
    {
        internal readonly InterpolatedLogMessageInternal Internal;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarnInterpolatedLogMessage"/> struct.
        /// </summary>
        public WarnInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Warn, literalLength, formattedCount, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="WarnInterpolatedLogMessage"/> struct.
        /// </summary>
        public WarnInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Warn, literalLength, formattedCount, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="WarnInterpolatedLogMessage"/> struct.
        /// </summary>
        public WarnInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Warn, literalLength, formattedCount, eventId, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="WarnInterpolatedLogMessage"/> struct.
        /// </summary>
        public WarnInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Warn, literalLength, formattedCount, eventId, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="WarnInterpolatedLogMessage"/> struct.
        /// </summary>
        public WarnInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Exception? exception,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Warn, literalLength, formattedCount, exception, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="WarnInterpolatedLogMessage"/> struct.
        /// </summary>
        public WarnInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Exception? exception,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Warn, literalLength, formattedCount, exception, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="WarnInterpolatedLogMessage"/> struct.
        /// </summary>
        public WarnInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Exception? exception,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Warn, literalLength, formattedCount, eventId, exception, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="WarnInterpolatedLogMessage"/> struct.
        /// </summary>
        public WarnInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Exception? exception,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Warn, literalLength, formattedCount, eventId, exception, logger, properties, out enabled);

        /// <summary>
        /// Appends a literal span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<char> s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a literal span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<byte> s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a literal string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral([ConstantExpected] string s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a formatted string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a formatted string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s, int alignment)
            => Internal.AppendFormatted(s, alignment);

        /// <summary>
        /// Appends a formatted span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> value)
            => Internal.AppendLiteral(value);

        /// <summary>
        /// Appends a formatted span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted span of bytes to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> value)
            => Internal.AppendLiteral(value);

        /// <summary>
        /// Appends a formatted span of bytes to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted boolean to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value)
            => Internal.AppendFormatted(value);

        /// <summary>
        /// Appends a formatted boolean to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value);
            
        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment, string? format) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment, format);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment, string? format) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment, format);
    }

    /// <summary>
    /// Interpolated string handler for <see cref="LogLevel.Error"/> log level.
    /// </summary>
    [InterpolatedStringHandler]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public readonly ref struct ErrorInterpolatedLogMessage
    {
        internal readonly InterpolatedLogMessageInternal Internal;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInterpolatedLogMessage"/> struct.
        /// </summary>
        public ErrorInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Error, literalLength, formattedCount, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInterpolatedLogMessage"/> struct.
        /// </summary>
        public ErrorInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Error, literalLength, formattedCount, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInterpolatedLogMessage"/> struct.
        /// </summary>
        public ErrorInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Error, literalLength, formattedCount, eventId, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInterpolatedLogMessage"/> struct.
        /// </summary>
        public ErrorInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Error, literalLength, formattedCount, eventId, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInterpolatedLogMessage"/> struct.
        /// </summary>
        public ErrorInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Exception? exception,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Error, literalLength, formattedCount, exception, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInterpolatedLogMessage"/> struct.
        /// </summary>
        public ErrorInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Exception? exception,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Error, literalLength, formattedCount, exception, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInterpolatedLogMessage"/> struct.
        /// </summary>
        public ErrorInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Exception? exception,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Error, literalLength, formattedCount, eventId, exception, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInterpolatedLogMessage"/> struct.
        /// </summary>
        public ErrorInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Exception? exception,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Error, literalLength, formattedCount, eventId, exception, logger, properties, out enabled);

        /// <summary>
        /// Appends a literal span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<char> s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a literal span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<byte> s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a literal string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral([ConstantExpected] string s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a formatted string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a formatted string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s, int alignment)
            => Internal.AppendFormatted(s, alignment);

        /// <summary>
        /// Appends a formatted span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> value)
            => Internal.AppendLiteral(value);

        /// <summary>
        /// Appends a formatted span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted span of bytes to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> value)
            => Internal.AppendLiteral(value);

        /// <summary>
        /// Appends a formatted span of bytes to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted boolean to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value)
            => Internal.AppendFormatted(value);

        /// <summary>
        /// Appends a formatted boolean to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value);
            
        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment, string? format) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment, format);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment, string? format) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment, format);
    }

    /// <summary>
    /// Interpolated string handler for <see cref="LogLevel.Fatal"/> log level.
    /// </summary>
    [InterpolatedStringHandler]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public readonly ref struct FatalInterpolatedLogMessage
    {
        internal readonly InterpolatedLogMessageInternal Internal;

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalInterpolatedLogMessage"/> struct.
        /// </summary>
        public FatalInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Fatal, literalLength, formattedCount, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalInterpolatedLogMessage"/> struct.
        /// </summary>
        public FatalInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Fatal, literalLength, formattedCount, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalInterpolatedLogMessage"/> struct.
        /// </summary>
        public FatalInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Fatal, literalLength, formattedCount, eventId, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalInterpolatedLogMessage"/> struct.
        /// </summary>
        public FatalInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Fatal, literalLength, formattedCount, eventId, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalInterpolatedLogMessage"/> struct.
        /// </summary>
        public FatalInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Exception? exception,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Fatal, literalLength, formattedCount, exception, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalInterpolatedLogMessage"/> struct.
        /// </summary>
        public FatalInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                Exception? exception,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Fatal, literalLength, formattedCount, exception, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalInterpolatedLogMessage"/> struct.
        /// </summary>
        public FatalInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Exception? exception,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Fatal, literalLength, formattedCount, eventId, exception, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalInterpolatedLogMessage"/> struct.
        /// </summary>
        public FatalInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                Exception? exception,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Fatal, literalLength, formattedCount, eventId, exception, logger, properties, out enabled);

        /// <summary>
        /// Appends a literal span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<char> s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a literal span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(ReadOnlySpan<byte> s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a literal string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral([ConstantExpected] string s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a formatted string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s)
            => Internal.AppendLiteral(s);

        /// <summary>
        /// Appends a formatted string to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string s, int alignment)
            => Internal.AppendFormatted(s, alignment);

        /// <summary>
        /// Appends a formatted span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> value)
            => Internal.AppendLiteral(value);

        /// <summary>
        /// Appends a formatted span of chars to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted span of bytes to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> value)
            => Internal.AppendLiteral(value);

        /// <summary>
        /// Appends a formatted span of bytes to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<byte> value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted boolean to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value)
            => Internal.AppendFormatted(value);

        /// <summary>
        /// Appends a formatted boolean to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(bool value, int alignment)
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value);
            
        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, int alignment, string? format) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment, format);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment);

        /// <summary>
        /// Appends a formatted nullable value to the interpolated log message.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T? value, int alignment, string? format) where T : unmanaged, ISpanFormattable
            => Internal.AppendFormatted(value, alignment, format);
    }
}

partial class LoggerExtensions
{

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        logger.Log(new(logger, LogLevel.Trace, msg));
    }


    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        logger.Log(new(logger, LogLevel.Trace, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, LogEventId eventId, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        logger.Log(new(logger, LogLevel.Trace, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, LogEventId eventId, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        logger.Log(new(logger, LogLevel.Trace, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, Exception? exception, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;

        logger.Log(new(logger, LogLevel.Trace, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, Exception? exception, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;

        logger.Log(new(logger, LogLevel.Trace, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, LogEventId eventId, Exception? exception, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;

        logger.Log(new(logger, LogLevel.Trace, eventId, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, LogEventId eventId, Exception? exception, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;

        logger.Log(new(logger, LogLevel.Trace, eventId, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, [InterpolatedStringHandlerArgument("logger")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, LogProperties properties, [InterpolatedStringHandlerArgument("logger", "properties")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, LogEventId eventId, [InterpolatedStringHandlerArgument("eventId", "logger")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, LogEventId eventId, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "logger", "properties")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, Exception? exception, [InterpolatedStringHandlerArgument("exception", "logger")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, Exception? exception, LogProperties properties, [InterpolatedStringHandlerArgument("exception", "logger", "properties")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, LogEventId eventId, Exception? exception, [InterpolatedStringHandlerArgument("eventId", "exception", "logger")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void Trace(this Logger logger, LogEventId eventId, Exception? exception, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "exception", "logger", "properties")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        logger.Log(new(logger, LogLevel.Debug, msg));
    }


    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        logger.Log(new(logger, LogLevel.Debug, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, LogEventId eventId, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        logger.Log(new(logger, LogLevel.Debug, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, LogEventId eventId, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        logger.Log(new(logger, LogLevel.Debug, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, Exception? exception, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;

        logger.Log(new(logger, LogLevel.Debug, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, Exception? exception, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;

        logger.Log(new(logger, LogLevel.Debug, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, LogEventId eventId, Exception? exception, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;

        logger.Log(new(logger, LogLevel.Debug, eventId, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, LogEventId eventId, Exception? exception, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;

        logger.Log(new(logger, LogLevel.Debug, eventId, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, [InterpolatedStringHandlerArgument("logger")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, LogProperties properties, [InterpolatedStringHandlerArgument("logger", "properties")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, LogEventId eventId, [InterpolatedStringHandlerArgument("eventId", "logger")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, LogEventId eventId, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "logger", "properties")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, Exception? exception, [InterpolatedStringHandlerArgument("exception", "logger")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, Exception? exception, LogProperties properties, [InterpolatedStringHandlerArgument("exception", "logger", "properties")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, LogEventId eventId, Exception? exception, [InterpolatedStringHandlerArgument("eventId", "exception", "logger")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void Debug(this Logger logger, LogEventId eventId, Exception? exception, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "exception", "logger", "properties")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;
        logger.Log(new(logger, LogLevel.Info, msg));
    }


    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;
        logger.Log(new(logger, LogLevel.Info, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, LogEventId eventId, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;
        logger.Log(new(logger, LogLevel.Info, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, LogEventId eventId, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;
        logger.Log(new(logger, LogLevel.Info, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, Exception? exception, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;

        logger.Log(new(logger, LogLevel.Info, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, Exception? exception, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;

        logger.Log(new(logger, LogLevel.Info, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, LogEventId eventId, Exception? exception, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;

        logger.Log(new(logger, LogLevel.Info, eventId, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, LogEventId eventId, Exception? exception, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;

        logger.Log(new(logger, LogLevel.Info, eventId, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, [InterpolatedStringHandlerArgument("logger")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, LogProperties properties, [InterpolatedStringHandlerArgument("logger", "properties")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, LogEventId eventId, [InterpolatedStringHandlerArgument("eventId", "logger")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, LogEventId eventId, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "logger", "properties")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, Exception? exception, [InterpolatedStringHandlerArgument("exception", "logger")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, Exception? exception, LogProperties properties, [InterpolatedStringHandlerArgument("exception", "logger", "properties")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, LogEventId eventId, Exception? exception, [InterpolatedStringHandlerArgument("eventId", "exception", "logger")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void Info(this Logger logger, LogEventId eventId, Exception? exception, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "exception", "logger", "properties")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;
        logger.Log(new(logger, LogLevel.Warn, msg));
    }


    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;
        logger.Log(new(logger, LogLevel.Warn, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, LogEventId eventId, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;
        logger.Log(new(logger, LogLevel.Warn, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, LogEventId eventId, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;
        logger.Log(new(logger, LogLevel.Warn, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, Exception? exception, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;

        logger.Log(new(logger, LogLevel.Warn, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, Exception? exception, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;

        logger.Log(new(logger, LogLevel.Warn, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, LogEventId eventId, Exception? exception, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;

        logger.Log(new(logger, LogLevel.Warn, eventId, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, LogEventId eventId, Exception? exception, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;

        logger.Log(new(logger, LogLevel.Warn, eventId, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, [InterpolatedStringHandlerArgument("logger")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, LogProperties properties, [InterpolatedStringHandlerArgument("logger", "properties")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, LogEventId eventId, [InterpolatedStringHandlerArgument("eventId", "logger")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, LogEventId eventId, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "logger", "properties")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, Exception? exception, [InterpolatedStringHandlerArgument("exception", "logger")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, Exception? exception, LogProperties properties, [InterpolatedStringHandlerArgument("exception", "logger", "properties")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, LogEventId eventId, Exception? exception, [InterpolatedStringHandlerArgument("eventId", "exception", "logger")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void Warn(this Logger logger, LogEventId eventId, Exception? exception, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "exception", "logger", "properties")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;
        logger.Log(new(logger, LogLevel.Error, msg));
    }


    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;
        logger.Log(new(logger, LogLevel.Error, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, LogEventId eventId, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;
        logger.Log(new(logger, LogLevel.Error, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, LogEventId eventId, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;
        logger.Log(new(logger, LogLevel.Error, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, Exception? exception, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;

        logger.Log(new(logger, LogLevel.Error, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, Exception? exception, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;

        logger.Log(new(logger, LogLevel.Error, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, LogEventId eventId, Exception? exception, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;

        logger.Log(new(logger, LogLevel.Error, eventId, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, LogEventId eventId, Exception? exception, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;

        logger.Log(new(logger, LogLevel.Error, eventId, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, [InterpolatedStringHandlerArgument("logger")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, LogProperties properties, [InterpolatedStringHandlerArgument("logger", "properties")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, LogEventId eventId, [InterpolatedStringHandlerArgument("eventId", "logger")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, LogEventId eventId, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "logger", "properties")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, Exception? exception, [InterpolatedStringHandlerArgument("exception", "logger")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, Exception? exception, LogProperties properties, [InterpolatedStringHandlerArgument("exception", "logger", "properties")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, LogEventId eventId, Exception? exception, [InterpolatedStringHandlerArgument("eventId", "exception", "logger")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void Error(this Logger logger, LogEventId eventId, Exception? exception, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "exception", "logger", "properties")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;
        logger.Log(new(logger, LogLevel.Fatal, msg));
    }


    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;
        logger.Log(new(logger, LogLevel.Fatal, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, LogEventId eventId, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;
        logger.Log(new(logger, LogLevel.Fatal, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, LogEventId eventId, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;
        logger.Log(new(logger, LogLevel.Fatal, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, Exception? exception, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;

        logger.Log(new(logger, LogLevel.Fatal, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, Exception? exception, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;

        logger.Log(new(logger, LogLevel.Fatal, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, LogEventId eventId, Exception? exception, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;

        logger.Log(new(logger, LogLevel.Fatal, eventId, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, LogEventId eventId, Exception? exception, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;

        logger.Log(new(logger, LogLevel.Fatal, eventId, exception, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, [InterpolatedStringHandlerArgument("logger")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, LogProperties properties, [InterpolatedStringHandlerArgument("logger", "properties")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, LogEventId eventId, [InterpolatedStringHandlerArgument("eventId", "logger")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, LogEventId eventId, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "logger", "properties")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, Exception? exception, [InterpolatedStringHandlerArgument("exception", "logger")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, Exception? exception, LogProperties properties, [InterpolatedStringHandlerArgument("exception", "logger", "properties")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, LogEventId eventId, Exception? exception, [InterpolatedStringHandlerArgument("eventId", "exception", "logger")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void Fatal(this Logger logger, LogEventId eventId, Exception? exception, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "exception", "logger", "properties")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);
}