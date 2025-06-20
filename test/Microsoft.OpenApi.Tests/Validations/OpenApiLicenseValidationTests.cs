﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiLicenseValidationTests
    {
        [Fact]
        public void ValidateFieldIsRequiredInLicense()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;
            var license = new OpenApiLicense();

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(license);

            errors = validator.Errors;
            var result = !errors.Any();

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            var error = Assert.Single(errors);
            Assert.Equal(string.Format(SRResource.Validation_FieldIsRequired, "name", "license"), error.Message);
        }
    }
}
