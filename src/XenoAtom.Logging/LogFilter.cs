// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// A filter for a log message.
/// </summary>
/// <param name="message">The message that is being logged.</param>
/// <returns><c>true</c> if the filter accepts this message; otherwise <c>false</c>.</returns>
public delegate bool LogFilter(in LogMessage message);