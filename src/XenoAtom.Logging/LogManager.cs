// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Enum = System.Enum;

namespace XenoAtom.Logging;

public class LogManager
{
    // TODO

    public static Logger GetLogger(string name)
    {
        return new Logger();
    }

    private struct LoggerInfo
    {
        public string Name;

        public LogLevel Level;

    }
}

internal struct LogMessageRaw
{


    private void Protocol()
    {


    }



}