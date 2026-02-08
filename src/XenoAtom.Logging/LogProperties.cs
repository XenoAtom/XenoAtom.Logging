// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Buffers.Binary;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace XenoAtom.Logging;

/// <summary>
/// Represents a mutable set of structured logging properties.
/// </summary>
/// <remarks>
/// This type is a mutable value type. Avoid copying instances after they are populated, and dispose only the original
/// instance that owns the rented buffer.
/// </remarks>
public struct LogProperties : IEnumerable, IDisposable
{
    private const int DefaultBufferSize = 256;
    private byte[]? _buffer;
    private int _byteCount;
    private int _count;

    /// <summary>
    /// Gets the number of properties in this instance.
    /// </summary>
    public int Count => _count;

    internal bool IsEmpty => _count == 0;

    /// <summary>
    /// Resets the current properties.
    /// </summary>
    public void Reset()
    {
        _byteCount = 0;
        _count = 0;
    }

    /// <summary>
    /// Adds a named scalar property.
    /// </summary>
    public void Add<T>((string Name, T Value) item) where T : unmanaged, ISpanFormattable
        => Add(item.Name.AsSpan(), item.Value);

    /// <summary>
    /// Adds a named nullable scalar property.
    /// </summary>
    public void Add<T>((string Name, T? Value) item) where T : unmanaged, ISpanFormattable
    {
        if (item.Value.HasValue)
        {
            Add(item.Name.AsSpan(), item.Value.Value);
        }
        else
        {
            AddCore(item.Name.AsSpan(), "null".AsSpan());
        }
    }

    /// <summary>
    /// Adds a named boolean property.
    /// </summary>
    public void Add((string Name, bool Value) item)
        => Add(item.Name, item.Value);

    /// <summary>
    /// Adds a named string property.
    /// </summary>
    public void Add((string Name, string Value) item)
        => Add(item.Name, item.Value);

    /// <summary>
    /// Adds a named text property.
    /// </summary>
    public void Add(string name, ReadOnlySpan<char> s)
        => AddCore(name.AsSpan(), s);

    /// <summary>
    /// Adds a named boolean property.
    /// </summary>
    public void Add(string name, bool value)
        => AddCore(name.AsSpan(), value ? bool.TrueString.AsSpan() : bool.FalseString.AsSpan());

    /// <summary>
    /// Adds a named string property.
    /// </summary>
    public void Add(string name, string value)
        => AddCore(name.AsSpan(), value.AsSpan());

    /// <summary>
    /// Adds an unnamed text property.
    /// </summary>
    public void Add(string text)
        => AddCore(ReadOnlySpan<char>.Empty, text.AsSpan());

    /// <summary>
    /// Adds an unnamed text property.
    /// </summary>
    public void Add(ReadOnlySpan<char> text)
        => AddCore(ReadOnlySpan<char>.Empty, text);

    /// <summary>
    /// Adds an unnamed scalar property.
    /// </summary>
    public void Add<T>(T value) where T : unmanaged, ISpanFormattable
        => Add(ReadOnlySpan<char>.Empty, value);

    /// <summary>
    /// Adds a named interpolated string property.
    /// </summary>
    public void Add(string name, ref DefaultInterpolatedStringHandler value)
        => Add(name, value.ToStringAndClear());

    /// <summary>
    /// Returns an enumerator for this property set.
    /// </summary>
    public Enumerator GetEnumerator() => new(_buffer, _byteCount, _count);

    IEnumerator IEnumerable.GetEnumerator() => new BoxingEnumerator(_buffer, _byteCount, _count);

    /// <summary>
    /// Clears any buffered state owned by this instance.
    /// </summary>
    public void Dispose()
    {
        var buffer = _buffer;
        if (buffer is null)
        {
            return;
        }

        ArrayPool<byte>.Shared.Return(buffer);
        _buffer = null;
        _byteCount = 0;
        _count = 0;
    }

    internal LogPropertiesSnapshot Snapshot()
    {
        var buffer = _buffer;
        if (_count == 0 || _byteCount == 0 || buffer is null)
        {
            return LogPropertiesSnapshot.Empty;
        }

        return LogPropertiesSnapshot.Create(buffer, _byteCount, _count);
    }

    /// <summary>
    /// Appends all properties from another <see cref="LogProperties"/> instance.
    /// </summary>
    /// <param name="properties">The source properties to append.</param>
    public void AddRange(LogProperties properties)
    {
        var sourceBuffer = properties._buffer;
        var sourceByteCount = properties._byteCount;
        var sourceCount = properties._count;
        if (sourceBuffer is null || sourceByteCount == 0 || sourceCount == 0)
        {
            return;
        }

        EnsureBuffer(sourceByteCount);
        sourceBuffer.AsSpan(0, sourceByteCount).CopyTo(_buffer!.AsSpan(_byteCount));
        _byteCount += sourceByteCount;
        _count += sourceCount;
    }

    private void Add(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
        => AddCore(name, value);

    private void Add<T>(ReadOnlySpan<char> name, T value) where T : unmanaged, ISpanFormattable
    {
        Span<char> stackBuffer = stackalloc char[128];
        if (value.TryFormat(stackBuffer, out var charsWritten, default, CultureInfo.InvariantCulture))
        {
            AddCore(name, stackBuffer[..charsWritten]);
            return;
        }

        var rentedBuffer = ArrayPool<char>.Shared.Rent(512);
        try
        {
            while (true)
            {
                if (value.TryFormat(rentedBuffer, out charsWritten, default, CultureInfo.InvariantCulture))
                {
                    AddCore(name, rentedBuffer.AsSpan(0, charsWritten));
                    return;
                }

                ArrayPool<char>.Shared.Return(rentedBuffer);
                rentedBuffer = ArrayPool<char>.Shared.Rent(rentedBuffer.Length * 2);
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rentedBuffer);
        }
    }

    private void AddCore(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
    {
        PrepareEntry(name.Length, value.Length, out var nameDestination, out var valueDestination);
        name.CopyTo(nameDestination);
        value.CopyTo(valueDestination);
    }

    private void PrepareEntry(int nameCharCount, int valueCharCount, out Span<char> nameDestination, out Span<char> valueDestination)
    {
        checked
        {
            var nameByteCount = nameCharCount * sizeof(char);
            var valueByteCount = valueCharCount * sizeof(char);
            var totalEntrySize = LogPropertiesEncoding.HeaderSize + nameByteCount + valueByteCount;
            EnsureBuffer(totalEntrySize);

            var buffer = _buffer!;
            var position = _byteCount;
            BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(position, sizeof(int)), nameCharCount);
            position += sizeof(int);
            BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(position, sizeof(int)), valueCharCount);
            position += sizeof(int);
            var nameBytes = buffer.AsSpan(position, nameByteCount);
            position += nameByteCount;
            var valueBytes = buffer.AsSpan(position, valueByteCount);
            position += valueByteCount;
            nameDestination = MemoryMarshal.Cast<byte, char>(nameBytes);
            valueDestination = MemoryMarshal.Cast<byte, char>(valueBytes);

            _byteCount = position;
            _count++;
        }
    }

    private void EnsureBuffer(int additionalRequiredBytes)
    {
        var requiredLength = checked(_byteCount + additionalRequiredBytes);
        var buffer = _buffer;
        if (buffer is null)
        {
            _buffer = ArrayPool<byte>.Shared.Rent(Math.Max(DefaultBufferSize, requiredLength));
            return;
        }

        if (buffer.Length >= requiredLength)
        {
            return;
        }

        var newBuffer = ArrayPool<byte>.Shared.Rent(Math.Max(buffer.Length * 2, requiredLength));
        buffer.AsSpan(0, _byteCount).CopyTo(newBuffer);
        ArrayPool<byte>.Shared.Return(buffer);
        _buffer = newBuffer;
    }

    /// <summary>
    /// Enumerator over <see cref="LogProperties"/>.
    /// </summary>
    public ref struct Enumerator
    {
        private readonly ReadOnlySpan<byte> _payload;
        private readonly int _count;
        private int _index;
        private int _position;
        private LogProperty _current;

        internal Enumerator(byte[]? buffer, int byteCount, int count)
        {
            _payload = buffer is null || byteCount == 0 ? ReadOnlySpan<byte>.Empty : buffer.AsSpan(0, byteCount);
            _count = count;
            _index = -1;
            _position = 0;
            _current = default;
        }

        /// <summary>
        /// Advances to the next property.
        /// </summary>
        /// <returns><see langword="true"/> when a property is available; otherwise <see langword="false"/>.</returns>
        /// <exception cref="InvalidOperationException">The underlying payload is invalid.</exception>
        public bool MoveNext()
        {
            if (_payload.IsEmpty)
            {
                return false;
            }

            var next = _index + 1;
            if ((uint)next >= (uint)_count)
            {
                return false;
            }

            if (!LogPropertiesEncoding.TryReadEntry(_payload, ref _position, out var nameOffset, out var nameCharCount, out var valueOffset, out var valueCharCount))
            {
                throw new InvalidOperationException("Invalid property payload.");
            }

            _current = new LogProperty(
                LogPropertiesEncoding.DecodeCharSpan(_payload, nameOffset, nameCharCount),
                LogPropertiesEncoding.DecodeCharSpan(_payload, valueOffset, valueCharCount));
            _index = next;
            return true;
        }

        /// <summary>
        /// Resets this enumerator to its initial position.
        /// </summary>
        public void Reset()
        {
            _index = -1;
            _position = 0;
            _current = default;
        }

        /// <summary>
        /// Gets the current property.
        /// </summary>
        public LogProperty Current => _current;
    }

    private sealed class BoxingEnumerator : IEnumerator
    {
        private readonly byte[]? _buffer;
        private readonly int _byteCount;
        private readonly int _count;
        private int _index;
        private int _position;
        private DictionaryEntry _current;

        internal BoxingEnumerator(byte[]? buffer, int byteCount, int count)
        {
            _buffer = buffer;
            _byteCount = byteCount;
            _count = count;
            _index = -1;
            _position = 0;
            _current = default;
        }

        public bool MoveNext()
        {
            if (_buffer is null)
            {
                return false;
            }

            var next = _index + 1;
            if ((uint)next >= (uint)_count)
            {
                return false;
            }

            var payload = _buffer.AsSpan(0, _byteCount);
            if (!LogPropertiesEncoding.TryReadEntry(payload, ref _position, out var nameOffset, out var nameCharCount, out var valueOffset, out var valueCharCount))
            {
                throw new InvalidOperationException("Invalid property payload.");
            }

            var name = LogPropertiesEncoding.DecodeCharSpan(payload, nameOffset, nameCharCount);
            var value = LogPropertiesEncoding.DecodeCharSpan(payload, valueOffset, valueCharCount);
            _current = new DictionaryEntry(name.ToString(), value.ToString());
            _index = next;
            return true;
        }

        public void Reset()
        {
            _index = -1;
            _position = 0;
            _current = default;
        }

        public object Current => _current;
    }
}

/// <summary>
/// Represents a single named property.
/// </summary>
public readonly ref struct LogProperty
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogProperty"/> struct.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The property value.</param>
    public LogProperty(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Gets the property name.
    /// </summary>
    public ReadOnlySpan<char> Name { get; }

    /// <summary>
    /// Gets the property value.
    /// </summary>
    public ReadOnlySpan<char> Value { get; }
}

internal sealed class LogPropertiesSnapshot : IDisposable
{
    internal static readonly LogPropertiesSnapshot Empty = new(Array.Empty<byte>(), 0, 0, pooled: false);

    private byte[] _buffer;
    private readonly int _byteCount;
    private readonly int _count;
    private readonly bool _pooled;
    private int _referenceCount;

    private LogPropertiesSnapshot(byte[] buffer, int byteCount, int count, bool pooled)
    {
        _buffer = buffer;
        _byteCount = byteCount;
        _count = count;
        _pooled = pooled;
        _referenceCount = pooled ? 1 : 0;
    }

    internal static LogPropertiesSnapshot Create(byte[] sourceBuffer, int byteCount, int count)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(byteCount);
        sourceBuffer.AsSpan(0, byteCount).CopyTo(buffer);
        return new LogPropertiesSnapshot(buffer, byteCount, count, pooled: true);
    }

    internal int Count => _count;

    internal ReadOnlySpan<byte> BufferSpan => _buffer.AsSpan(0, _byteCount);

    internal LogProperty this[int index] => DecodePropertyAt(index);

    internal bool Contains(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
    {
        var position = 0;
        var payload = _buffer.AsSpan(0, _byteCount);
        for (var i = 0; i < _count; i++)
        {
            if (!LogPropertiesEncoding.TryReadEntry(payload, ref position, out var currentNameOffset, out var currentNameCharCount, out var currentValueOffset, out var currentValueCharCount))
            {
                return false;
            }

            var currentName = LogPropertiesEncoding.DecodeCharSpan(payload, currentNameOffset, currentNameCharCount);
            var currentValue = LogPropertiesEncoding.DecodeCharSpan(payload, currentValueOffset, currentValueCharCount);
            if (currentName.SequenceEqual(name) && currentValue.SequenceEqual(value))
            {
                return true;
            }
        }

        return false;
    }

    private LogProperty DecodePropertyAt(int index)
    {
        if ((uint)index >= (uint)_count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        var position = 0;
        var payload = _buffer.AsSpan(0, _byteCount);
        for (var i = 0; i <= index; i++)
        {
            if (!LogPropertiesEncoding.TryReadEntry(payload, ref position, out var nameOffset, out var nameCharCount, out var valueOffset, out var valueCharCount))
            {
                throw new InvalidOperationException("Invalid property payload.");
            }

            if (i == index)
            {
                return new LogProperty(
                    LogPropertiesEncoding.DecodeCharSpan(payload, nameOffset, nameCharCount),
                    LogPropertiesEncoding.DecodeCharSpan(payload, valueOffset, valueCharCount));
            }
        }

        throw new ArgumentOutOfRangeException(nameof(index));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AddRef()
    {
        if (_pooled)
        {
            Interlocked.Increment(ref _referenceCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Release()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (!_pooled)
        {
            return;
        }

        if (Interlocked.Decrement(ref _referenceCount) != 0)
        {
            return;
        }

        var buffer = Interlocked.Exchange(ref _buffer, Array.Empty<byte>());
        if (buffer.Length == 0)
        {
            return;
        }

        ArrayPool<byte>.Shared.Return(buffer);
    }
}

internal static class LogPropertiesEncoding
{
    internal const int HeaderSize = sizeof(int) * 2;

    internal static bool TryReadEntry(
        ReadOnlySpan<byte> buffer,
        ref int position,
        out int nameOffset,
        out int nameCharCount,
        out int valueOffset,
        out int valueCharCount)
    {
        nameOffset = 0;
        nameCharCount = 0;
        valueOffset = 0;
        valueCharCount = 0;

        if (position < 0 || position > buffer.Length - HeaderSize)
        {
            return false;
        }

        nameCharCount = BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(position, sizeof(int)));
        position += sizeof(int);
        valueCharCount = BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(position, sizeof(int)));
        position += sizeof(int);

        if (nameCharCount < 0 || valueCharCount < 0)
        {
            return false;
        }

        if ((uint)nameCharCount > (uint)(int.MaxValue / sizeof(char)) || (uint)valueCharCount > (uint)(int.MaxValue / sizeof(char)))
        {
            return false;
        }

        var nameByteCount = nameCharCount * sizeof(char);
        var valueByteCount = valueCharCount * sizeof(char);

        if (position > buffer.Length - nameByteCount)
        {
            return false;
        }

        nameOffset = position;
        position += nameByteCount;

        if (position > buffer.Length - valueByteCount)
        {
            return false;
        }

        valueOffset = position;
        position += valueByteCount;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<char> DecodeCharSpan(ReadOnlySpan<byte> payload, int byteOffset, int charCount)
        => MemoryMarshal.Cast<byte, char>(payload.Slice(byteOffset, charCount * sizeof(char)));
}
