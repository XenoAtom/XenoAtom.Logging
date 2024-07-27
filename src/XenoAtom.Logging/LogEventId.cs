// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// A log event id associated with a name.
/// </summary>
/// <param name="Id">The id of this log event.</param>
/// <param name="Name">The short name of this log event.</param>
public readonly record struct LogEventId(int Id, string? Name)
{
    public static LogEventId Empty { get; } = new(0, null);

    public bool IsEmpty => Id == 0 && Name is null;
}