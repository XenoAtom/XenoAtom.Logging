// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Declares a source-generated <see cref="LogFormatter"/> implementation from a compile-time template.
/// </summary>
/// <remarks>
/// This attribute can be applied to:
/// <list type="bullet">
/// <item><description>A <see langword="partial"/> type inheriting <see cref="LogFormatter"/>.</description></item>
/// <item><description>A <see langword="static"/> <see langword="partial"/> property returning <see cref="LogFormatter"/> inside a <see langword="partial"/> type.</description></item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class LogFormatterAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogFormatterAttribute"/> class.
    /// </summary>
    /// <param name="template">The formatter template.</param>
    /// <exception cref="ArgumentNullException"><paramref name="template"/> is <see langword="null"/>.</exception>
    public LogFormatterAttribute(string template)
    {
        ArgumentNullException.ThrowIfNull(template);
        Template = template;
    }

    /// <summary>
    /// Gets the formatter template.
    /// </summary>
    public string Template { get; }
}
