// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Represents a logical logging scope.
/// </summary>
public readonly ref struct LogScope
{
    private readonly LogScopeSnapshot? _snapshot;

    internal LogScope(LogScopeSnapshot snapshot)
    {
        _snapshot = snapshot;
    }

    /// <summary>
    /// Gets an empty scope.
    /// </summary>
    public static LogScope Empty => new(LogScopeSnapshot.Empty);

    /// <summary>
    /// Gets the number of active nested scopes.
    /// </summary>
    public int Count => _snapshot?.Count ?? 0;

    /// <summary>
    /// Gets the properties for a nested scope by index.
    /// </summary>
    public LogPropertiesReader this[int index] => new((_snapshot ?? LogScopeSnapshot.Empty)[index]);
}
