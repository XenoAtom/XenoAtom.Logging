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
                object? attachment,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Trace, literalLength, formattedCount, attachment, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceInterpolatedLogMessage"/> struct.
        /// </summary>
        public TraceInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                object? attachment,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Trace, literalLength, formattedCount, attachment, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceInterpolatedLogMessage"/> struct.
        /// </summary>
        public TraceInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                object? attachment,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Trace, literalLength, formattedCount, eventId, attachment, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceInterpolatedLogMessage"/> struct.
        /// </summary>
        public TraceInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                object? attachment,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Trace, literalLength, formattedCount, eventId, attachment, logger, properties, out enabled);

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
                object? attachment,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Debug, literalLength, formattedCount, attachment, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInterpolatedLogMessage"/> struct.
        /// </summary>
        public DebugInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                object? attachment,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Debug, literalLength, formattedCount, attachment, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInterpolatedLogMessage"/> struct.
        /// </summary>
        public DebugInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                object? attachment,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Debug, literalLength, formattedCount, eventId, attachment, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInterpolatedLogMessage"/> struct.
        /// </summary>
        public DebugInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                object? attachment,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Debug, literalLength, formattedCount, eventId, attachment, logger, properties, out enabled);

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
                object? attachment,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Info, literalLength, formattedCount, attachment, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoInterpolatedLogMessage"/> struct.
        /// </summary>
        public InfoInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                object? attachment,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Info, literalLength, formattedCount, attachment, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoInterpolatedLogMessage"/> struct.
        /// </summary>
        public InfoInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                object? attachment,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Info, literalLength, formattedCount, eventId, attachment, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoInterpolatedLogMessage"/> struct.
        /// </summary>
        public InfoInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                object? attachment,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Info, literalLength, formattedCount, eventId, attachment, logger, properties, out enabled);

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
                object? attachment,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Warn, literalLength, formattedCount, attachment, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="WarnInterpolatedLogMessage"/> struct.
        /// </summary>
        public WarnInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                object? attachment,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Warn, literalLength, formattedCount, attachment, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="WarnInterpolatedLogMessage"/> struct.
        /// </summary>
        public WarnInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                object? attachment,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Warn, literalLength, formattedCount, eventId, attachment, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="WarnInterpolatedLogMessage"/> struct.
        /// </summary>
        public WarnInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                object? attachment,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Warn, literalLength, formattedCount, eventId, attachment, logger, properties, out enabled);

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
                object? attachment,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Error, literalLength, formattedCount, attachment, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInterpolatedLogMessage"/> struct.
        /// </summary>
        public ErrorInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                object? attachment,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Error, literalLength, formattedCount, attachment, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInterpolatedLogMessage"/> struct.
        /// </summary>
        public ErrorInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                object? attachment,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Error, literalLength, formattedCount, eventId, attachment, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInterpolatedLogMessage"/> struct.
        /// </summary>
        public ErrorInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                object? attachment,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Error, literalLength, formattedCount, eventId, attachment, logger, properties, out enabled);

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
                object? attachment,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Fatal, literalLength, formattedCount, attachment, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalInterpolatedLogMessage"/> struct.
        /// </summary>
        public FatalInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                object? attachment,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Fatal, literalLength, formattedCount, attachment, logger, properties, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalInterpolatedLogMessage"/> struct.
        /// </summary>
        public FatalInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                object? attachment,
                Logger logger,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Fatal, literalLength, formattedCount, eventId, attachment, logger, out enabled);

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalInterpolatedLogMessage"/> struct.
        /// </summary>
        public FatalInterpolatedLogMessage(
                int literalLength,
                int formattedCount,
                LogEventId eventId,
                object? attachment,
                Logger logger,
                LogProperties properties,
                out bool enabled)
            => Internal = new InterpolatedLogMessageInternal(LogLevel.Fatal, literalLength, formattedCount, eventId, attachment, logger, properties, out enabled);

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
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The log message.</param>
    public static void Trace(this Logger logger, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        logger.Log(new(logger, LogLevel.Trace, msg));
    }


    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Trace(this Logger logger, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        logger.Log(new(logger, LogLevel.Trace, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Trace(this Logger logger, LogEventId eventId, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        logger.Log(new(logger, LogLevel.Trace, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Trace(this Logger logger, LogEventId eventId, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        logger.Log(new(logger, LogLevel.Trace, eventId, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Trace(this Logger logger, object? attachment, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;

        logger.Log(new(logger, LogLevel.Trace, attachment, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Trace(this Logger logger, object? attachment, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;

        logger.Log(new(logger, LogLevel.Trace, attachment, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Trace(this Logger logger, LogEventId eventId, object? attachment, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;

        logger.Log(new(logger, LogLevel.Trace, eventId, attachment, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Trace(this Logger logger, LogEventId eventId, object? attachment, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Trace)) return;

        logger.Log(new(logger, LogLevel.Trace, eventId, attachment, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Trace(this Logger logger, [InterpolatedStringHandlerArgument("logger")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Trace(this Logger logger, LogProperties properties, [InterpolatedStringHandlerArgument("logger", "properties")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Trace(this Logger logger, LogEventId eventId, [InterpolatedStringHandlerArgument("eventId", "logger")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Trace(this Logger logger, LogEventId eventId, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "logger", "properties")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Trace(this Logger logger, object? attachment, [InterpolatedStringHandlerArgument("attachment", "logger")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Trace(this Logger logger, object? attachment, LogProperties properties, [InterpolatedStringHandlerArgument("attachment", "logger", "properties")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Trace(this Logger logger, LogEventId eventId, object? attachment, [InterpolatedStringHandlerArgument("eventId", "attachment", "logger")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Trace(this Logger logger, LogEventId eventId, object? attachment, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "attachment", "logger", "properties")] ref Logger.TraceInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The log message.</param>
    public static void Debug(this Logger logger, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        logger.Log(new(logger, LogLevel.Debug, msg));
    }


    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Debug(this Logger logger, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        logger.Log(new(logger, LogLevel.Debug, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Debug(this Logger logger, LogEventId eventId, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        logger.Log(new(logger, LogLevel.Debug, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Debug(this Logger logger, LogEventId eventId, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        logger.Log(new(logger, LogLevel.Debug, eventId, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Debug(this Logger logger, object? attachment, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;

        logger.Log(new(logger, LogLevel.Debug, attachment, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Debug(this Logger logger, object? attachment, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;

        logger.Log(new(logger, LogLevel.Debug, attachment, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Debug(this Logger logger, LogEventId eventId, object? attachment, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;

        logger.Log(new(logger, LogLevel.Debug, eventId, attachment, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Debug(this Logger logger, LogEventId eventId, object? attachment, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;

        logger.Log(new(logger, LogLevel.Debug, eventId, attachment, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Debug(this Logger logger, [InterpolatedStringHandlerArgument("logger")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Debug(this Logger logger, LogProperties properties, [InterpolatedStringHandlerArgument("logger", "properties")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Debug(this Logger logger, LogEventId eventId, [InterpolatedStringHandlerArgument("eventId", "logger")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Debug(this Logger logger, LogEventId eventId, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "logger", "properties")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Debug(this Logger logger, object? attachment, [InterpolatedStringHandlerArgument("attachment", "logger")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Debug(this Logger logger, object? attachment, LogProperties properties, [InterpolatedStringHandlerArgument("attachment", "logger", "properties")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Debug(this Logger logger, LogEventId eventId, object? attachment, [InterpolatedStringHandlerArgument("eventId", "attachment", "logger")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Debug(this Logger logger, LogEventId eventId, object? attachment, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "attachment", "logger", "properties")] ref Logger.DebugInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The log message.</param>
    public static void Info(this Logger logger, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;
        logger.Log(new(logger, LogLevel.Info, msg));
    }


    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Info(this Logger logger, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;
        logger.Log(new(logger, LogLevel.Info, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Info(this Logger logger, LogEventId eventId, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;
        logger.Log(new(logger, LogLevel.Info, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Info(this Logger logger, LogEventId eventId, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;
        logger.Log(new(logger, LogLevel.Info, eventId, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Info(this Logger logger, object? attachment, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;

        logger.Log(new(logger, LogLevel.Info, attachment, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Info(this Logger logger, object? attachment, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;

        logger.Log(new(logger, LogLevel.Info, attachment, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Info(this Logger logger, LogEventId eventId, object? attachment, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;

        logger.Log(new(logger, LogLevel.Info, eventId, attachment, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Info(this Logger logger, LogEventId eventId, object? attachment, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Info)) return;

        logger.Log(new(logger, LogLevel.Info, eventId, attachment, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Info(this Logger logger, [InterpolatedStringHandlerArgument("logger")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Info(this Logger logger, LogProperties properties, [InterpolatedStringHandlerArgument("logger", "properties")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Info(this Logger logger, LogEventId eventId, [InterpolatedStringHandlerArgument("eventId", "logger")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Info(this Logger logger, LogEventId eventId, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "logger", "properties")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Info(this Logger logger, object? attachment, [InterpolatedStringHandlerArgument("attachment", "logger")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Info(this Logger logger, object? attachment, LogProperties properties, [InterpolatedStringHandlerArgument("attachment", "logger", "properties")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Info(this Logger logger, LogEventId eventId, object? attachment, [InterpolatedStringHandlerArgument("eventId", "attachment", "logger")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Info(this Logger logger, LogEventId eventId, object? attachment, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "attachment", "logger", "properties")] ref Logger.InfoInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The log message.</param>
    public static void Warn(this Logger logger, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;
        logger.Log(new(logger, LogLevel.Warn, msg));
    }


    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Warn(this Logger logger, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;
        logger.Log(new(logger, LogLevel.Warn, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Warn(this Logger logger, LogEventId eventId, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;
        logger.Log(new(logger, LogLevel.Warn, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Warn(this Logger logger, LogEventId eventId, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;
        logger.Log(new(logger, LogLevel.Warn, eventId, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Warn(this Logger logger, object? attachment, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;

        logger.Log(new(logger, LogLevel.Warn, attachment, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Warn(this Logger logger, object? attachment, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;

        logger.Log(new(logger, LogLevel.Warn, attachment, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Warn(this Logger logger, LogEventId eventId, object? attachment, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;

        logger.Log(new(logger, LogLevel.Warn, eventId, attachment, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Warn(this Logger logger, LogEventId eventId, object? attachment, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Warn)) return;

        logger.Log(new(logger, LogLevel.Warn, eventId, attachment, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Warn(this Logger logger, [InterpolatedStringHandlerArgument("logger")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Warn(this Logger logger, LogProperties properties, [InterpolatedStringHandlerArgument("logger", "properties")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Warn(this Logger logger, LogEventId eventId, [InterpolatedStringHandlerArgument("eventId", "logger")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Warn(this Logger logger, LogEventId eventId, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "logger", "properties")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Warn(this Logger logger, object? attachment, [InterpolatedStringHandlerArgument("attachment", "logger")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Warn(this Logger logger, object? attachment, LogProperties properties, [InterpolatedStringHandlerArgument("attachment", "logger", "properties")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Warn(this Logger logger, LogEventId eventId, object? attachment, [InterpolatedStringHandlerArgument("eventId", "attachment", "logger")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Warn(this Logger logger, LogEventId eventId, object? attachment, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "attachment", "logger", "properties")] ref Logger.WarnInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The log message.</param>
    public static void Error(this Logger logger, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;
        logger.Log(new(logger, LogLevel.Error, msg));
    }


    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Error(this Logger logger, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;
        logger.Log(new(logger, LogLevel.Error, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Error(this Logger logger, LogEventId eventId, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;
        logger.Log(new(logger, LogLevel.Error, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Error(this Logger logger, LogEventId eventId, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;
        logger.Log(new(logger, LogLevel.Error, eventId, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Error(this Logger logger, object? attachment, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;

        logger.Log(new(logger, LogLevel.Error, attachment, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Error(this Logger logger, object? attachment, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;

        logger.Log(new(logger, LogLevel.Error, attachment, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Error(this Logger logger, LogEventId eventId, object? attachment, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;

        logger.Log(new(logger, LogLevel.Error, eventId, attachment, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Error(this Logger logger, LogEventId eventId, object? attachment, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;

        logger.Log(new(logger, LogLevel.Error, eventId, attachment, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Error(this Logger logger, [InterpolatedStringHandlerArgument("logger")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Error(this Logger logger, LogProperties properties, [InterpolatedStringHandlerArgument("logger", "properties")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Error(this Logger logger, LogEventId eventId, [InterpolatedStringHandlerArgument("eventId", "logger")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Error(this Logger logger, LogEventId eventId, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "logger", "properties")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Error(this Logger logger, object? attachment, [InterpolatedStringHandlerArgument("attachment", "logger")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Error(this Logger logger, object? attachment, LogProperties properties, [InterpolatedStringHandlerArgument("attachment", "logger", "properties")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Error(this Logger logger, LogEventId eventId, object? attachment, [InterpolatedStringHandlerArgument("eventId", "attachment", "logger")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Error(this Logger logger, LogEventId eventId, object? attachment, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "attachment", "logger", "properties")] ref Logger.ErrorInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The log message.</param>
    public static void Fatal(this Logger logger, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;
        logger.Log(new(logger, LogLevel.Fatal, msg));
    }


    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Fatal(this Logger logger, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;
        logger.Log(new(logger, LogLevel.Fatal, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Fatal(this Logger logger, LogEventId eventId, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;
        logger.Log(new(logger, LogLevel.Fatal, eventId, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Fatal(this Logger logger, LogEventId eventId, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;
        logger.Log(new(logger, LogLevel.Fatal, eventId, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Fatal(this Logger logger, object? attachment, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;

        logger.Log(new(logger, LogLevel.Fatal, attachment, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Fatal(this Logger logger, object? attachment, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;

        logger.Log(new(logger, LogLevel.Fatal, attachment, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    public static void Fatal(this Logger logger, LogEventId eventId, object? attachment, string msg)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;

        logger.Log(new(logger, LogLevel.Fatal, eventId, attachment, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The log message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    public static void Fatal(this Logger logger, LogEventId eventId, object? attachment, string msg, LogProperties properties)
    {
        if (!logger.IsEnabled(LogLevel.Fatal)) return;

        logger.Log(new(logger, LogLevel.Fatal, eventId, attachment, properties, msg));
    }

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Fatal(this Logger logger, [InterpolatedStringHandlerArgument("logger")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Fatal(this Logger logger, LogProperties properties, [InterpolatedStringHandlerArgument("logger", "properties")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Fatal(this Logger logger, LogEventId eventId, [InterpolatedStringHandlerArgument("eventId", "logger")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Fatal(this Logger logger, LogEventId eventId, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "logger", "properties")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Fatal(this Logger logger, object? attachment, [InterpolatedStringHandlerArgument("attachment", "logger")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Fatal(this Logger logger, object? attachment, LogProperties properties, [InterpolatedStringHandlerArgument("attachment", "logger", "properties")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Fatal(this Logger logger, LogEventId eventId, object? attachment, [InterpolatedStringHandlerArgument("eventId", "attachment", "logger")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);

    /// <summary>
    /// Logs a message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventId">The event identifier associated with the message.</param>
    /// <param name="attachment">An optional attachment associated with the message.</param>
    /// <param name="properties">Structured properties associated with the message.</param>
    /// <param name="msg">The interpolated log message handler.</param>
    public static void Fatal(this Logger logger, LogEventId eventId, object? attachment, LogProperties properties, [InterpolatedStringHandlerArgument("eventId", "attachment", "logger", "properties")] ref Logger.FatalInterpolatedLogMessage msg)
        => logger.Log(msg.Internal);
}
