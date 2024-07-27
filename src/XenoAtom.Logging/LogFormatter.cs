// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Base class for formatting a <see cref="LogMessage"/> into a <see cref="Span{T}"/> of characters.
/// </summary>
public abstract class LogFormatter
{
    /// <summary>
    /// Formats a <see cref="LogMessage"/> into a <see cref="Span{T}"/> of characters.
    /// </summary>
    /// <param name="logMessage">The log message to format.</param>
    /// <param name="destination">The destination characters span.</param>
    /// <param name="charsWritten">The number of characters written.</param>
    /// <param name="segments">The segments computed during the formatting. (optional)</param>
    /// <returns><c>true</c> if the format was successful; otherwise <c>false</c> if the characters span doesn't have enough space.</returns>
    public abstract bool TryFormat(in LogMessage logMessage, Span<char> destination, out int charsWritten, ref LogMessageFormatSegments segments);
}