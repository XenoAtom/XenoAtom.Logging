// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Formatters;

/// <summary>
/// Defines the field naming policy used by <see cref="JsonLogFormatter"/> in default schema mode.
/// </summary>
public enum JsonLogFieldNamingPolicy
{
    /// <summary>
    /// Uses formatter-defined field names.
    /// </summary>
    Default,

    /// <summary>
    /// Converts field names to snake_case.
    /// </summary>
    SnakeCase
}
