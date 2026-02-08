// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;

namespace XenoAtom.Logging;

internal static class LogScopeContext
{
    private static readonly AsyncLocal<LogScopeNode?> Current = new();

    public static LogScopeNode? CurrentNode => Current.Value;

    public static LogBeginScopeToken Push(in LogProperties properties)
    {
        var snapshot = properties.Snapshot();
        var node = new LogScopeNode(Current.Value, snapshot);
        Current.Value = node;
        return new LogBeginScopeToken(node);
    }

    public static void Pop(LogScopeNode node)
    {
        var current = Current.Value;
        if (ReferenceEquals(current, node))
        {
            Current.Value = current.Parent;
            current.Release();
            return;
        }

        // Ignore out-of-order disposal and preserve the active scope stack.
    }

    public static LogScopeSnapshot CaptureSnapshot()
    {
        var current = Current.Value;
        if (current is null)
        {
            return LogScopeSnapshot.Empty;
        }

        var count = 0;
        var scan = current;
        while (scan is not null)
        {
            count++;
            scan = scan.Parent;
        }

        var scopes = ArrayPool<LogPropertiesSnapshot?>.Shared.Rent(count);
        scan = current;
        for (var index = count - 1; index >= 0; index--)
        {
            var properties = scan!.Properties;
            properties.AddRef();
            scopes[index] = properties;
            scan = scan.Parent;
        }

        return new LogScopeSnapshot(scopes, count, pooled: true);
    }
}

internal sealed class LogScopeNode
{
    private int _released;

    public LogScopeNode(LogScopeNode? parent, LogPropertiesSnapshot properties)
    {
        Parent = parent;
        Properties = properties;
    }

    public LogScopeNode? Parent { get; }

    public LogPropertiesSnapshot Properties { get; }

    public void Release()
    {
        if (Interlocked.Exchange(ref _released, 1) != 0)
        {
            return;
        }

        Properties.Release();
    }
}

internal readonly struct LogBeginScopeToken
{
    public LogBeginScopeToken(LogScopeNode? node)
    {
        Node = node;
    }

    public LogScopeNode? Node { get; }
}

internal sealed class LogScopeSnapshot : IDisposable
{
    public static readonly LogScopeSnapshot Empty = new(Array.Empty<LogPropertiesSnapshot?>(), 0, pooled: false);

    private LogPropertiesSnapshot?[] _scopes;
    private readonly int _count;
    private readonly bool _pooled;
    private int _disposed;

    public LogScopeSnapshot(LogPropertiesSnapshot?[] scopes, int count, bool pooled)
    {
        _scopes = scopes;
        _count = count;
        _pooled = pooled;
    }

    public int Count => _count;

    public LogPropertiesSnapshot this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _scopes[index] ?? LogPropertiesSnapshot.Empty;
        }
    }

    public void Dispose()
    {
        if (!_pooled)
        {
            return;
        }

        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        var scopes = Interlocked.Exchange(ref _scopes, Array.Empty<LogPropertiesSnapshot?>());
        for (var i = 0; i < _count; i++)
        {
            scopes[i]?.Release();
        }

        ArrayPool<LogPropertiesSnapshot?>.Shared.Return(scopes, clearArray: true);
    }
}
