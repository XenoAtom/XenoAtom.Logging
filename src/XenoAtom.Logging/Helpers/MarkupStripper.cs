// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Helpers;

internal static class MarkupStripper
{
    public static int GetMaxOutputLength(int markupLength)
    {
        if (markupLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(markupLength));
        }

        return markupLength;
    }

    public static int Strip(ReadOnlySpan<char> markup, Span<char> destination)
    {
        if (destination.Length < markup.Length)
        {
            throw new ArgumentException("Destination span is too small.", nameof(destination));
        }

        var readIndex = 0;
        var writeIndex = 0;
        while (readIndex < markup.Length)
        {
            var current = markup[readIndex];
            if (current == '[')
            {
                if ((uint)(readIndex + 1) < (uint)markup.Length && markup[readIndex + 1] == '[')
                {
                    destination[writeIndex++] = '[';
                    readIndex += 2;
                    continue;
                }

                var closeIndex = markup[(readIndex + 1)..].IndexOf(']');
                if (closeIndex >= 0)
                {
                    readIndex += closeIndex + 2;
                    continue;
                }
            }
            else if (current == ']' && (uint)(readIndex + 1) < (uint)markup.Length && markup[readIndex + 1] == ']')
            {
                destination[writeIndex++] = ']';
                readIndex += 2;
                continue;
            }

            destination[writeIndex++] = current;
            readIndex++;
        }

        return writeIndex;
    }
}
