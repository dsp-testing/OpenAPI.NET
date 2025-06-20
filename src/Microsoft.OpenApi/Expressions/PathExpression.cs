﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Path expression, the name in path is case-sensitive.
    /// </summary>
    public sealed class PathExpression : SourceExpression
    {
        /// <summary>
        /// path. string
        /// </summary>
        public const string Path = "path.";

        /// <summary>
        /// Initializes a new instance of the <see cref="PathExpression"/> class.
        /// </summary>
        /// <param name="name">The name string, it's case-insensitive.</param>
        public PathExpression(string name)
            : base(name)
        {
            Utils.CheckArgumentNullOrEmpty(name);
        }

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        public override string Expression { get => Path + Value; }

        /// <summary>
        /// Gets the name string.
        /// </summary>
        public string? Name { get => Value; }
    }
}
