// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Collections;
using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

public struct LogProperties : IEnumerable, IDisposable
{
    internal byte[] Buffer;
    private int _position;

    // TODO: Add a Roslyn analyzer to check that this constructor is not used directly but only on authorized method calls (BeginScope/Logger.Info...etc.)
    public LogProperties()
    {
        Buffer = ArrayPool<byte>.Shared.Rent(256);
    }

    public void Reset()
    {
        _position = 0;
    }

    public void Add<T>((string Name, T Value) item) where T : unmanaged, ISpanFormattable
    {
    }

    public void Add<T>((string Name, T? Value) item) where T : unmanaged, ISpanFormattable
    {

    }

    public void Add((string Name, bool Value) item)
    {

    }
    public void Add((string Name, string Value) item)
    {

    }

    public void Add(string name, ReadOnlySpan<char> s)
    {
    }

    public void Add(string name, ReadOnlySpan<byte> s)
    {
    }

    public void Add(string name, bool value)
    {

    }
    public void Add(string name, string value)
    {
    }

    public void Add(string text)
    {
    }
    
    public void Add(ReadOnlySpan<char> text)
    {
    }

    public void Add(ReadOnlySpan<byte> text)
    {
    }

    public void Add<T>(T value) where T : unmanaged, ISpanFormattable
    {

    }

    public void Add(string name, ref DefaultInterpolatedStringHandler value)
    {
        // TODO: Replace DefaultInterpolatedStringHandler with our own copy here to support copy to Span and avoid the string allocation
        //value.ToStringAndClear()
    }

    public void Add<T>(ReadOnlySpan<byte> name, T value) where T : unmanaged, ISpanFormattable
    {
    }

    public void Add<T>(ReadOnlySpan<byte> name, T? value) where T : unmanaged, ISpanFormattable
    {
    }

    public void Add(ReadOnlySpan<byte> name, bool value)
    {
    }

    public void Add(ReadOnlySpan<byte> name, ReadOnlySpan<char> s)
    {
    }

    public void Add(ReadOnlySpan<byte> name, ReadOnlySpan<byte> s)
    {
    }

    public static void Tester()
    {
        LogProperties properties = new()
        {
            ("Name", true),
            ("Value", 1),
            ("ProductId", 123553),
            ("Price", 15.2f),
            {"Hello"u8, 1},
            {"Hello2"u8, 2},
        };

        LogProperties test = [("Name", true), ("Value", 1), ("ProductId", 123553), ("Price", 15.2f)];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        if (Buffer.Length > 0) ArrayPool<byte>.Shared.Return(Buffer);
        Buffer = Array.Empty<byte>();
    }
}