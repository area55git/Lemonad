﻿using System;
using Xunit;

namespace Lemonad.ErrorHandling.Test.Result.Tests {
    public class DoWithTests {
        private static Result<double, string> Division(double left, double right) {
            if (right == 0)
                return $"Can not divide '{left}' with '{right}'.";

            return left / right;
        }

        [Fact]
        public void
            Result_With_Error__Expects_Action__Not_To_Be_Invoked() {
            var actionExectued = false;
            var result = Division(10, 0).DoWith(d => actionExectued = true);

            Assert.False(actionExectued, "Should not get exectued since there's an error.");
            Assert.Equal(default, result.Value);
            Assert.Equal("Can not divide '10' with '0'.", result.Error);
            Assert.True(result.HasError, "Result should have error.");
            Assert.False(result.HasValue, "Result should not have value.");
        }

        [Fact]
        public void
            Result_With_Value__Expects_Action_To_Be_Invoked() {
            var actionExectued = false;
            var result = Division(10, 2).DoWith(d => actionExectued = true);

            Assert.True(actionExectued, "Should not get exectued since there's an error.");
            Assert.Equal(5, result.Value);
            Assert.Equal(default, result.Error);
            Assert.False(result.HasError, "Result should not have error.");
            Assert.True(result.HasValue, "Result should have value.");
        }
    }
}