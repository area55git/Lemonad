﻿using System;
using Xunit;

namespace Lemonad.ErrorHandling.Test.Result.Tests {
    public class MaperrorTests {
        [Fact]
        public void Result_String_Int_Whose_Property_HasValue_Is_False() {
            var isExectued = false;
            var mapOk = ResultParsers.Int("foo").MapError(i => {
                isExectued = true;
                return i;
            });

            Assert.True(isExectued, "Should get exectued.");
            Assert.True(mapOk.HasError, "Result should have a error value.");
            Assert.False(mapOk.HasValue, "Result should have error.");
            Assert.Equal("Could not parse type System.String(\"foo\") into System.Int32.", mapOk.Error);
            Assert.Equal(default(int), mapOk.Value);
        }

        [Fact]
        public void
            Result_String_Int_Whose_Property_HasValue_Is_False__Null_Selector__Expects_ArgumentNullExcetpion() {
            Assert.Throws<ArgumentNullException>(() => {
                Func<string, string> selector = null;
                var mapOk = ResultParsers.Int("foo").MapError(selector);
            });
        }

        [Fact]
        public void Result_String_Int_Whose_Property_HasValue_Is_True() {
            var isExectued = false;
            var mapOk = ResultParsers.Int("2").MapError(i => {
                isExectued = true;
                return i;
            });

            Assert.False(isExectued, "Should not get exectued.");
            Assert.True(mapOk.HasValue, "Result should have value.");
            Assert.False(mapOk.HasError, "Result should not have a error value.");
            Assert.Equal(default(string), mapOk.Error);
            Assert.Equal(2, mapOk.Value);
        }

        [Fact]
        public void
            Result_String_Int_Whose_Property_HasValue_Is_True__Null_Selector__Expects_No_ArgumentNullExcetpion() {
            var exception = Record.Exception(() => {
                Func<string, string> selector = null;
                var mapOk = ResultParsers.Int("2").MapError(selector);
                Assert.True(mapOk.HasValue, "Result should have value.");
                Assert.False(mapOk.HasError, "Result should not have a error value.");
                Assert.Equal(2, mapOk.Value);
                Assert.Equal(default(string), mapOk.Error);
            });
            Assert.Null(exception);
        }
    }
}