// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

internal readonly struct LogDataHeader
{
    private readonly uint _data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private LogDataHeader(uint data) => _data = data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LogDataHeader(LogDataPartKind kind, LogDataKind dataKind, ushort length)
        => _data = (uint)(byte)kind << 24 | (uint)dataKind << 16 | length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LogDataHeader Aligned(LogDataPartKind kind, LogDataKind dataKind, ushort length)
        => new((uint)((byte)kind | 0x40) << 24 | (uint)dataKind << 16 | length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LogDataHeader Formatted(LogDataPartKind kind, LogDataKind dataKind, ushort length)
        => new((uint)((byte)kind | 0x80) << 24 | (uint)dataKind << 16 | length);

    public LogDataPartKind Kind => (LogDataPartKind)(_data << 1 >> 24);

    public bool IsAligned => (int)(_data << 1) < 0;

    public bool IsFormatted => (int)_data < 0;

    public LogDataKind DataKind => (LogDataKind)(byte)(_data >> 16);

    public ushort Length => (ushort)_data;
}