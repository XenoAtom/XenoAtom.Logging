// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Formatters;

/// <summary>
/// Defines the JSON schema profile produced by <see cref="JsonLogFormatter"/>.
/// </summary>
public enum JsonLogSchemaProfile
{
    /// <summary>
    /// Uses the default XenoAtom.Logging JSON schema.
    /// </summary>
    Default,

    /// <summary>
    /// Uses an Elastic Common Schema-inspired flattened field layout.
    /// </summary>
    ElasticCommonSchema
}
