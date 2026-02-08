// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Formatters;

/// <summary>
/// Options used to configure <see cref="JsonLogFormatter"/>.
/// </summary>
public sealed class JsonLogFormatterOptions
{
    /// <summary>
    /// Gets or sets the schema profile used for emitted JSON.
    /// </summary>
    public JsonLogSchemaProfile SchemaProfile { get; set; } = JsonLogSchemaProfile.Default;

    /// <summary>
    /// Gets or sets the field naming policy used in <see cref="JsonLogSchemaProfile.Default"/> mode.
    /// </summary>
    /// <remarks>
    /// This option is ignored when <see cref="SchemaProfile"/> is <see cref="JsonLogSchemaProfile.ElasticCommonSchema"/>,
    /// because ECS field names are fixed by the schema.
    /// </remarks>
    public JsonLogFieldNamingPolicy FieldNamingPolicy { get; set; } = JsonLogFieldNamingPolicy.Default;

    /// <summary>
    /// Gets or sets a value indicating whether the thread id field is included.
    /// </summary>
    public bool IncludeThreadId { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether event fields are included.
    /// </summary>
    public bool IncludeEventId { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether event names are included when an event id is present.
    /// </summary>
    public bool IncludeEventName { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether exception text is included.
    /// </summary>
    public bool IncludeException { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether message properties are included.
    /// </summary>
    public bool IncludeProperties { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether scope properties are included.
    /// </summary>
    public bool IncludeScopes { get; set; } = true;
}
