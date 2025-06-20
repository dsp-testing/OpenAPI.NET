﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.MicrosoftExtensions;

/// <summary>
/// Extension element for OpenAPI to add tag the primary error message to use on error types. x-ms-primary-error-message
/// </summary>
public class OpenApiPrimaryErrorMessageExtension : IOpenApiExtension
{
    /// <summary>
    /// Name of the extension as used in the description.
    /// </summary>
    public static string Name => "x-ms-primary-error-message";

    /// <inheritdoc />
    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        if (writer is null) throw new ArgumentNullException(nameof(writer));
        writer.WriteValue(IsPrimaryErrorMessage);
    }

    /// <summary>
    /// Whether this property is the primary error message to use on error types.
    /// </summary>
    public bool IsPrimaryErrorMessage { get; set; }

    /// <summary>
    /// Parses the <see cref="JsonNodeExtension"/> to <see cref="OpenApiPrimaryErrorMessageExtension"/>.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <returns>The <see cref="OpenApiPrimaryErrorMessageExtension"/>.</returns>
    public static OpenApiPrimaryErrorMessageExtension Parse(JsonNode source)
    {
        if (source is not JsonValue rawObject) throw new ArgumentOutOfRangeException(nameof(source));
        return new()
        {
            IsPrimaryErrorMessage = rawObject.TryGetValue<bool>(out var value) && value
        };
    }
}
