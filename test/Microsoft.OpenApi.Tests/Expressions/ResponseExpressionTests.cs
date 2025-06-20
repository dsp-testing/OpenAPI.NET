﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class ResponseExpressionTests
    {
        [Fact]
        public void ResponseExpressionConstructorThrows()
        {
            // Arrange & Act
            Action test = () => new ResponseExpression(source: null);

            // Assert
            Assert.Throws<ArgumentNullException>("source", test);
        }

        [Fact]
        public void ResponseExpressionConstructorWorks()
        {
            // Arrange
            var source = SourceExpression.Build("header.accept");

            // Act
            var response = new ResponseExpression(source);

            // Assert
            Assert.Same(source, response.Source);
            Assert.Equal("$response.header.accept", response.Expression);
            Assert.Equal("$response.header.accept", response.ToString());
        }
    }
}
