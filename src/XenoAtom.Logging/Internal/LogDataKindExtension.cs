// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

internal static class LogDataKindExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LogDataKind ToDataKind<T>() where T : unmanaged
    {
        if (typeof(T) == typeof(sbyte)) return LogDataKind.Int8;
        if (typeof(T) == typeof(short)) return LogDataKind.Int16;
        if (typeof(T) == typeof(int)) return LogDataKind.Int32;
        if (typeof(T) == typeof(long)) return LogDataKind.Int64;
        if (typeof(T) == typeof(byte)) return LogDataKind.UInt8;
        if (typeof(T) == typeof(ushort)) return LogDataKind.UInt16;
        if (typeof(T) == typeof(uint)) return LogDataKind.UInt32;
        if (typeof(T) == typeof(ulong)) return LogDataKind.UInt64;
        if (typeof(T) == typeof(float)) return LogDataKind.Float;
        if (typeof(T) == typeof(double)) return LogDataKind.Double;
        if (typeof(T) == typeof(bool)) return LogDataKind.Bool;
        if (typeof(T) == typeof(char)) return LogDataKind.Char;
        if (typeof(T) == typeof(Guid)) return LogDataKind.Guid;
        if (typeof(T) == typeof(decimal)) return LogDataKind.Decimal;
        if (typeof(T) == typeof(DateTime)) return LogDataKind.DateTime;
        if (typeof(T) == typeof(DateTimeOffset)) return LogDataKind.DateTimeOffset;
        if (typeof(T) == typeof(TimeSpan)) return LogDataKind.TimeSpan;
        if (typeof(T) == typeof(DateOnly)) return LogDataKind.DateOnly;
        if (typeof(T) == typeof(TimeOnly)) return LogDataKind.TimeOnly;
        if (typeof(T) == typeof(Version)) return LogDataKind.Version;
        if (typeof(T).IsEnum) return LogDataKind.Enum;
        return LogDataKind.Unknown;
    }
}