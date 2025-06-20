﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Source expression.
    /// </summary>
    public abstract class SourceExpression : RuntimeExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryExpression"/> class.
        /// </summary>
        /// <param name="value">The value string.</param>
        protected SourceExpression(string? value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        protected string? Value { get; }

        /// <summary>
        /// Build the source expression from input string.
        /// </summary>
        /// <param name="expression">The source expression.</param>
        /// <returns>The built source expression.</returns>
        public new static SourceExpression Build(string expression)
        {
            if (!string.IsNullOrWhiteSpace(expression))
            {
                var expressions = expression.Split('.');
                if (expressions.Length == 2)
                {
                    if (expression.StartsWith(HeaderExpression.Header, StringComparison.Ordinal))
                    {
                        // header.
                        return new HeaderExpression(expressions[1]);
                    }

                    if (expression.StartsWith(QueryExpression.Query, StringComparison.Ordinal))
                    {
                        // query.
                        return new QueryExpression(expressions[1]);
                    }

                    if (expression.StartsWith(PathExpression.Path, StringComparison.Ordinal))
                    {
                        // path.
                        return new PathExpression(expressions[1]);
                    }
                }

                // body
                if (expression.StartsWith(BodyExpression.Body, StringComparison.Ordinal))
                {
                    var subString = expression.Substring(BodyExpression.Body.Length);
                    if (string.IsNullOrEmpty(subString))
                    {
                        return new BodyExpression();
                    }

                    return new BodyExpression(new(subString));
                }
            }

            throw new OpenApiException(string.Format(SRResource.SourceExpressionHasInvalidFormat, expression));
        }
    }
}
