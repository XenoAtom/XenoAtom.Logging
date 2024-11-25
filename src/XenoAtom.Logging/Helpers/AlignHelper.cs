// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace XenoAtom.Logging.Helpers;

internal static class AlignHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AlignUp(int value, int alignment) => value + alignment - 1 & ~(alignment - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint AlignUp(nint value, int alignment) => value + alignment - 1 & ~(alignment - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint AlignDown(nint value, int alignment) => value & ~(alignment - 1);

}