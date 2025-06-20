﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Contains a set of OpenApi documents and document fragments that reference each other
    /// </summary>
    public class OpenApiWorkspace
    {
        private readonly Dictionary<string, Uri> _documentsIdRegistry = new();
        private readonly Dictionary<Uri, Stream> _artifactsRegistry = new();        
        private readonly Dictionary<Uri, IOpenApiReferenceable> _IOpenApiReferenceableRegistry = new(new UriWithFragmentEqualityComparer());

        private sealed class UriWithFragmentEqualityComparer : IEqualityComparer<Uri>
        {
            public bool Equals(Uri? x, Uri? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                return x.AbsoluteUri == y.AbsoluteUri;
            }

            public int GetHashCode(Uri obj)
            {
                return obj.AbsoluteUri.GetHashCode();
            }
        }

        /// <summary>
        /// The base location from where all relative references are resolved
        /// </summary>
        public Uri? BaseUrl { get; }
       
        /// <summary>
        /// Initialize workspace pointing to a base URL to allow resolving relative document locations.  Use a file:// url to point to a folder
        /// </summary>
        /// <param name="baseUrl"></param>
        public OpenApiWorkspace(Uri baseUrl)
        {
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// Initialize workspace using current directory as the default location.
        /// </summary>
        public OpenApiWorkspace()
        {
            BaseUrl = new Uri(OpenApiConstants.BaseRegistryUri);
        }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiWorkspace"/> object
        /// </summary>
        public OpenApiWorkspace(OpenApiWorkspace workspace) { }

        /// <summary>
        /// Returns the total count of all the components in the workspace registry
        /// </summary>
        /// <returns></returns>
        public int ComponentsCount()
        {
            return _IOpenApiReferenceableRegistry.Count + _artifactsRegistry.Count;
        }
        private const string ComponentSegmentSeparator = "/";

        /// <summary>
        /// Registers a document's components into the workspace
        /// </summary>
        /// <param name="document"></param>
        public void RegisterComponents(OpenApiDocument document)
        {
            if (document?.Components == null) return;

            string baseUri = getBaseUri(document);
            string location;

            // Register Schema
            if (document.Components.Schemas != null)
            {
                foreach (var item in document.Components.Schemas)
                {
                    location = item.Value.Id ?? baseUri + ReferenceType.Schema.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register Parameters
            if (document.Components.Parameters != null)
            {
                foreach (var item in document.Components.Parameters)
                {
                    location = baseUri + ReferenceType.Parameter.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register Responses
            if (document.Components.Responses != null)
            {
                foreach (var item in document.Components.Responses)
                {
                    location = baseUri + ReferenceType.Response.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register RequestBodies
            if (document.Components.RequestBodies != null)
            {
                foreach (var item in document.Components.RequestBodies)
                {
                    location = baseUri + ReferenceType.RequestBody.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register Links
            if (document.Components.Links != null)
            {
                foreach (var item in document.Components.Links)
                {
                    location = baseUri + ReferenceType.Link.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register Callbacks
            if (document.Components.Callbacks != null)
            {
                foreach (var item in document.Components.Callbacks)
                {
                    location = baseUri + ReferenceType.Callback.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register PathItems
            if (document.Components.PathItems != null)
            {
                foreach (var item in document.Components.PathItems)
                {
                    location = baseUri + ReferenceType.PathItem.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register Examples
            if (document.Components.Examples != null)
            {
                foreach (var item in document.Components.Examples)
                {
                    location = baseUri + ReferenceType.Example.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register Headers
            if (document.Components.Headers != null)
            {
                foreach (var item in document.Components.Headers)
                {
                    location = baseUri + ReferenceType.Header.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register SecuritySchemes
            if (document.Components.SecuritySchemes != null)
            {
                foreach (var item in document.Components.SecuritySchemes)
                {
                    location = baseUri + ReferenceType.SecurityScheme.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }
        }

        private static string getBaseUri(OpenApiDocument openApiDocument)
        {
            return openApiDocument.BaseUri + "#" + OpenApiConstants.ComponentsSegment;
        }

        /// <summary>
        /// Registers a component for a document in the workspace
        /// </summary>
        /// <param name="openApiDocument">The document to register the component for.</param>
        /// <param name="componentToRegister">The component to register.</param>
        /// <param name="id">The id of the component.</param>
        /// <typeparam name="T">The type of the component to register.</typeparam>
        /// <returns>true if the component is successfully registered; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">openApiDocument is null</exception>
        /// <exception cref="ArgumentNullException">componentToRegister is null</exception>
        /// <exception cref="ArgumentNullException">id is null or empty</exception>
        public bool RegisterComponentForDocument<T>(OpenApiDocument openApiDocument, T componentToRegister, string id)
        {
            Utils.CheckArgumentNull(openApiDocument);
            Utils.CheckArgumentNull(componentToRegister);
            Utils.CheckArgumentNullOrEmpty(id);

            var baseUri = getBaseUri(openApiDocument);

            var location = componentToRegister switch
            {
                IOpenApiSchema => baseUri + ReferenceType.Schema.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiParameter => baseUri + ReferenceType.Parameter.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiResponse => baseUri + ReferenceType.Response.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiRequestBody => baseUri + ReferenceType.RequestBody.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiLink => baseUri + ReferenceType.Link.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiCallback => baseUri + ReferenceType.Callback.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiPathItem => baseUri + ReferenceType.PathItem.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiExample => baseUri + ReferenceType.Example.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiHeader => baseUri + ReferenceType.Header.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiSecurityScheme => baseUri + ReferenceType.SecurityScheme.GetDisplayName() + ComponentSegmentSeparator + id,
                _ => throw new ArgumentException($"Invalid component type {componentToRegister!.GetType().Name}"),
            };

            return RegisterComponent(location, componentToRegister);
        }

        /// <summary>
        /// Registers a component in the component registry.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="component"></param>
        /// <returns>true if the component is successfully registered; otherwise false.</returns>
        internal bool RegisterComponent<T>(string location, T component)
        {
            var uri = ToLocationUrl(location);
            if (uri is not null)
            {
                if (component is IOpenApiReferenceable referenceable)
                {
                    if (!_IOpenApiReferenceableRegistry.ContainsKey(uri))
                    {
                        _IOpenApiReferenceableRegistry[uri] = referenceable;
                        return true;
                    }
                }
                else if (component is Stream stream && !_artifactsRegistry.ContainsKey(uri))
                {
                    _artifactsRegistry[uri] = stream;
                    return true;
                }
                return false;
            }            

            return false;
        }

        /// <summary>
        /// Adds a document id to the dictionaries of document locations and their ids.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddDocumentId(string? key, Uri? value)
        {
            if (!string.IsNullOrEmpty(key) && key is not null && value is not null && !_documentsIdRegistry.ContainsKey(key))
            {
                _documentsIdRegistry[key] = value;
            }
        }

        /// <summary>
        /// Retrieves the document id given a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The document id of the given key.</returns>
        public Uri? GetDocumentId(string? key)
        {
            if (key is not null && _documentsIdRegistry.TryGetValue(key, out var id))
            {
                return id;
            }

            return null;
        }

        /// <summary>
        /// Verify if workspace contains a component based on its URL.
        /// </summary>
        /// <param name="location">A relative or absolute URL of the file.  Use file:// for folder locations.</param>
        /// <returns>Returns true if a matching document is found.</returns>
        public bool Contains(string location)
        {
            var key = ToLocationUrl(location);
            if (key is null) return false;
            return _IOpenApiReferenceableRegistry.ContainsKey(key) || _artifactsRegistry.ContainsKey(key);
        }

        /// <summary>
        /// Resolves a reference given a key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <returns>The resolved reference.</returns>
        public T? ResolveReference<T>(string location)
        {
            if (string.IsNullOrEmpty(location)) return default;

            var uri = ToLocationUrl(location);
            if (uri is not null)
            {
                if (_IOpenApiReferenceableRegistry.TryGetValue(uri, out var referenceableValue) && referenceableValue is T referenceable)
                {
                    return referenceable;
                }
                else if (_artifactsRegistry.TryGetValue(uri, out var artifact) && artifact is T artifactValue)
                {
                    return artifactValue;
                }
            }            

            return default;
        }

        /// <summary>
        /// Recursively resolves a schema from a URI fragment.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="parentSchema">The parent schema to resolve against.</param>
        /// <returns></returns>
        internal IOpenApiSchema? ResolveJsonSchemaReference(string location, IOpenApiSchema parentSchema)
        {
            /* Enables resolving references for nested subschemas
             * Examples:
             * #/components/schemas/person/properties/address"
             * #/components/schemas/human/allOf/0
             */

            if (string.IsNullOrEmpty(location) || ToLocationUrl(location) is not Uri uri) return default;

#if NETSTANDARD2_1 || NETCOREAPP || NET5_0_OR_GREATER
            if (!location.Contains("#/components/schemas/", StringComparison.OrdinalIgnoreCase))
#else
            if (!location.Contains("#/components/schemas/"))
#endif
                throw new ArgumentException($"Invalid schema reference location: {location}. It should contain '#/components/schemas/'");

            var pathSegments = uri.Fragment.Split(['/'], StringSplitOptions.RemoveEmptyEntries);

            // Build the base path for the root schema: "#/components/schemas/person"
            var fragment = OpenApiConstants.ComponentsSegment + ReferenceType.Schema.GetDisplayName() + ComponentSegmentSeparator + pathSegments[3];
            var uriBuilder = new UriBuilder(uri)
            {
                Fragment = fragment
            }; // to avoid escaping the # character in the resulting Uri

            if (_IOpenApiReferenceableRegistry.TryGetValue(uriBuilder.Uri, out var schema) && schema is IOpenApiSchema targetSchema)
            {
                // traverse remaining segments after fetching the base schema
                var remainingSegments = pathSegments.Skip(4).ToArray();
                var stack = new Stack<IOpenApiSchema>();
                stack.Push(parentSchema);
                return ResolveSubSchema(targetSchema, remainingSegments, stack);
            }

            return default;          
        }
        
        internal static IOpenApiSchema? ResolveSubSchema(IOpenApiSchema schema, string[] pathSegments, Stack<IOpenApiSchema> visitedSchemas)
        {
            // Prevent infinite recursion in case of circular references
            if (visitedSchemas.Contains(schema))
            {
                if (schema is OpenApiSchemaReference openApiSchemaReference)
                    throw new InvalidOperationException($"Circular reference detected while resolving schema: {openApiSchemaReference.Reference.ReferenceV3}");
                else
                    throw new InvalidOperationException($"Circular reference detected while resolving schema");
            }
            visitedSchemas.Push(schema);
            // Traverse schema object to resolve subschemas
            if (pathSegments.Length == 0)
            {
                return schema;
            }
            var currentSegment = pathSegments[0];
            pathSegments = [.. pathSegments.Skip(1)]; // skip one segment for the next recursive call

            switch (currentSegment)
            {
                case OpenApiConstants.Properties:
                    var propName = pathSegments[0];
                    if (schema.Properties != null && schema.Properties.TryGetValue(propName, out var propSchema))
                        return ResolveSubSchema(propSchema, [.. pathSegments.Skip(1)], visitedSchemas);
                    break;
                case OpenApiConstants.Items:
                    return schema.Items is OpenApiSchema itemsSchema ? ResolveSubSchema(itemsSchema, pathSegments, visitedSchemas) : null;

                case OpenApiConstants.AdditionalProperties:
                    return schema.AdditionalProperties is OpenApiSchema additionalSchema ? ResolveSubSchema(additionalSchema, pathSegments, visitedSchemas) : null;
                case OpenApiConstants.AllOf:
                case OpenApiConstants.AnyOf:
                case OpenApiConstants.OneOf:
                    if (!int.TryParse(pathSegments[0], out var index)) return null;

                    var list = currentSegment switch
                    {
                        OpenApiConstants.AllOf => schema.AllOf,
                        OpenApiConstants.AnyOf => schema.AnyOf,
                        OpenApiConstants.OneOf => schema.OneOf,
                        _ => null
                    };

                    // recurse into the indexed subschema if valid
                    if (list != null && index < list.Count)
                        return ResolveSubSchema(list[index], [.. pathSegments.Skip(1)], visitedSchemas);
                    break;
            }

            return null;
        }

        private Uri? ToLocationUrl(string location)
        {
            if (BaseUrl is not null)
            {
                return new(BaseUrl, location);
            }
            return null;
        }
    }
}
