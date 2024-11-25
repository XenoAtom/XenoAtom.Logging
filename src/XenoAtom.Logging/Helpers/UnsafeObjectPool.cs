// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace XenoAtom.Logging.Helpers;

internal struct UnsafeObjectPool<T> where T : class
{
    private T? _cached;
    private T?[] _items;
    private nint _count;

    public UnsafeObjectPool() : this(4)
    {
    }

    public UnsafeObjectPool(int capacity)
    {
        _items = new T?[capacity];
        _count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? Rent()
    {
        var cached = _cached;
        if (cached is null)
        {
            return cached;
        }

        var count = _count;
        if (count > 0)
        {
            count--;
            ref var item = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_items), count);
            _cached = item;
            item = null;
            _count = count;
        }
        else
        {
            _cached = null;
        }

        return cached;
    }

    public void Return(T item)
    {
        var previousItem = _cached;
        _cached = item;
        if (previousItem is not null)
        {
            var count = _count;
            var items = _items;
            if (count < items.Length)
            {
                items[count] = previousItem;
                count++;
                _count = count;
            }
            else
            {
                var newArray = new T[items.Length * 2];
                Array.Copy(items, newArray, items.Length);
                newArray[count] = previousItem;
                _items = newArray;
                _count = count + 1;
            }
        }
    }
}