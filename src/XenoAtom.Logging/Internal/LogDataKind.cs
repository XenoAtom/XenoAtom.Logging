// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

internal enum LogDataKind : byte
{
    Unknown,
    SpanChar,
    SpanByte,
    String,
    Int8,
    Int16,
    Int32,
    Int64,
    UInt8,
    UInt16,
    UInt32,
    UInt64,
    Float,
    Double,
    Bool,
    Char,
    Guid,
    Decimal,
    DateTime,
    DateTimeOffset,
    TimeSpan,
    DateOnly,
    TimeOnly,
    Version,
    Enum,
}