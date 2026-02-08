// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace XenoAtom.Logging;

/// <summary>
/// Allows to associate <see cref="LogMessageFormatSegmentKind"/> to a range of characters in a formatted log message.
/// </summary>
public unsafe ref struct LogMessageFormatSegments : IDisposable
{
    private byte[]? _segments;
    private int _count;
    private int _previousCharIndexEnd;

    /// <summary>
    /// Creates a new instance of <see cref="LogMessageFormatSegments"/>.
    /// </summary>
    /// <param name="enabled">True if the segments are enabled.</param>
    public LogMessageFormatSegments(bool enabled)
    {
        if (enabled)
        {
            _segments = ArrayPool<byte>.Shared.Rent(sizeof(LogMessageFormatSegment) * ((int)LogMessageFormatSegmentKind.Scalar + 8));
            _previousCharIndexEnd = -1;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is enabled.
    /// </summary>
    public bool IsEnabled => _segments is not null;

    /// <summary>
    /// Gets the number of segments.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Resets the state of the segments.
    /// </summary>
    public void Reset()
    {
        _count = 0;
        _previousCharIndexEnd = -1;
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        var segments = _segments;
        if (segments is null) return;
        ArrayPool<byte>.Shared.Return(segments);
    }

    /// <summary>
    /// Gets the span of segments currently stored in this instance.
    /// </summary>
    /// <remarks>
    /// Do not cache the returned span across calls that mutate this instance.
    /// </remarks>
    public readonly ReadOnlySpan<LogMessageFormatSegment> AsSpan()
    {
        var segments = _segments;
        return segments is null ? default : MemoryMarshal.Cast<byte, LogMessageFormatSegment>(segments).Slice(0, _count);
    }

    /// <summary>
    /// Adds a segment of a specified kind for a range of characters.
    /// </summary>
    /// <param name="startCharIndex">The index of the first character of this segment.</param>
    /// <param name="endCharIndex">The index of the last character of this segment.</param>
    /// <param name="kind">The kind of segment</param>
    /// <exception cref="ArgumentOutOfRangeException">If startCharIndex or endCharIndex are negative, or overlap with an existing segment.</exception>
    public void Add(int startCharIndex, int endCharIndex, LogMessageFormatSegmentKind kind)
    {
        var segments = _segments;
        if (segments is null) return;

        if (startCharIndex < 0) throw new ArgumentOutOfRangeException(nameof(startCharIndex));
        if (endCharIndex < 0) throw new ArgumentOutOfRangeException(nameof(endCharIndex));
        if (startCharIndex > endCharIndex) throw new ArgumentOutOfRangeException(nameof(startCharIndex));
        if (startCharIndex > _previousCharIndexEnd)
        {
            _previousCharIndexEnd = endCharIndex;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(startCharIndex), "Overlapping segments are not allowed");
        }

        var count = _count;
        if (count >= segments.Length / sizeof(LogMessageFormatSegment))
        {
            var newSegments = ArrayPool<byte>.Shared.Rent(segments.Length * 2);
            segments.CopyTo(newSegments, 0);
            ArrayPool<byte>.Shared.Return(segments);
            _segments = segments = newSegments;
        }

        Unsafe.WriteUnaligned(ref segments[count * sizeof(LogMessageFormatSegment)], new LogMessageFormatSegment(startCharIndex, endCharIndex - startCharIndex, kind));
        _count = count + 1;
    }
}

/// <summary>
/// Describes a typed segment in a formatted log message.
/// </summary>
/// <param name="Start">The start character index of the segment.</param>
/// <param name="Length">The segment length in characters.</param>
/// <param name="Kind">The semantic kind of the segment.</param>
public readonly record struct LogMessageFormatSegment(int Start, int Length, LogMessageFormatSegmentKind Kind);
