﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Tag Object Reference
    /// </summary>
    public class OpenApiTagReference : BaseOpenApiReferenceHolder<OpenApiTag, IOpenApiTag, BaseOpenApiReference>, IOpenApiTag
    {
        /// <summary>
        /// Resolved target of the reference.
        /// </summary>
        public override IOpenApiTag? Target
        {
            get
            {
                return Reference.HostDocument?.Tags?.FirstOrDefault(t => OpenApiTagComparer.StringComparer.Equals(t.Name, Reference.Id));
            }
        }

        /// <summary>
        /// Constructor initializing the reference object.
        /// </summary>
        /// <param name="referenceId">The reference Id.</param>
        /// <param name="hostDocument">The host OpenAPI document.</param>
        /// <param name="externalResource">Optional: External resource in the reference.
        /// It may be:
        /// 1. a absolute/relative file path, for example:  ../commons/pet.json
        /// 2. a Url, for example: http://localhost/pet.json
        /// </param>
        public OpenApiTagReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null) : base(referenceId, hostDocument, ReferenceType.Tag, externalResource)
        {
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="openApiTagReference">The reference to copy</param>
        private OpenApiTagReference(OpenApiTagReference openApiTagReference) : base(openApiTagReference)
        {

        }

        /// <inheritdoc/>
        public string? Description
        {
            get => Target?.Description;
        }

        /// <inheritdoc/>
        public OpenApiExternalDocs? ExternalDocs { get => Target?.ExternalDocs; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get => Target?.Extensions; }

        /// <inheritdoc/>
        public string? Name { get => Target?.Name ?? Reference?.Id; }
        /// <inheritdoc/>
        public override IOpenApiTag CopyReferenceAsTargetElementWithOverrides(IOpenApiTag source)
        {
            return source is OpenApiTag ? new OpenApiTag(this) : source;
        }

        /// <inheritdoc/>
        public IOpenApiTag CreateShallowCopy()
        {
            return new OpenApiTagReference(this);
        }
        /// <inheritdoc/>
        protected override BaseOpenApiReference CopyReference(BaseOpenApiReference sourceReference)
        {
            return new BaseOpenApiReference(sourceReference);
        }
    }
}
