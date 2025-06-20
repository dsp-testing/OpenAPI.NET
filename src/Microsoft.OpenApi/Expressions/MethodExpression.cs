﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Method expression.
    /// </summary>
    public sealed class MethodExpression : RuntimeExpression
    {
        /// <summary>
        /// $method. string
        /// </summary>
        public const string Method = "$method";

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        public override string Expression => Method;

        /// <summary>
        /// Private constructor.
        /// </summary>
        public MethodExpression()
        {
        }
    }
}
