// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

internal static class LogMessageInternalThreadCache
{
    [ThreadStatic]
    private static LogMessageInternal? _message0;

    [ThreadStatic]
    private static LogMessageInternal? _message1;

    [ThreadStatic]
    private static int _inUseMask;

    public static bool TryRent(out LogMessageInternal message, out int slot)
    {
        slot = -1;
        message = null!;

        var mask = _inUseMask;
        if ((mask & 1) == 0)
        {
            _inUseMask = mask | 1;
            _message0 ??= new LogMessageInternal();
            message = _message0;
            slot = 0;
            return true;
        }

        if ((mask & 2) == 0)
        {
            _inUseMask = mask | 2;
            _message1 ??= new LogMessageInternal();
            message = _message1;
            slot = 1;
            return true;
        }

        return false;
    }

    public static void Return(int slot)
    {
        if ((uint)slot > 1u)
        {
            return;
        }

        _inUseMask &= ~(1 << slot);
    }
}

