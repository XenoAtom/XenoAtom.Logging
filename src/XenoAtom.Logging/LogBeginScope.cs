// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Represents a scope handle returned by <see cref="Logger.BeginScope(LogProperties)"/>.
/// </summary>
public struct LogBeginScope : IDisposable
{
    private readonly LogBeginScopeToken _token;
    private bool _disposed;

    internal LogBeginScope(LogBeginScopeToken token)
    {
        _token = token;
        _disposed = false;
    }

    /// <summary>
    /// Ends the scope.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        var node = _token.Node;
        if (node is not null)
        {
            LogScopeContext.Pop(node);
        }

        _disposed = true;
    }
}
