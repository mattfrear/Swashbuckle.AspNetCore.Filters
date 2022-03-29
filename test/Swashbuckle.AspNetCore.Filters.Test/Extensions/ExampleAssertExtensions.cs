﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Shouldly;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;

namespace Swashbuckle.AspNetCore.Filters.Test.Extensions
{
    public static class ExampleAssertExtensions
    {
        public static void ShouldMatch(this Title actualExample, Title expectedExample)
        {
            actualExample.ShouldBe(expectedExample);
        }

        public static void ShouldMatch(this PersonRequest actualExample, PersonRequest expectedExample)
        {
            actualExample.Title.ShouldBe(expectedExample.Title);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
            actualExample.Age.ShouldBe(expectedExample.Age);
        }

        public static void ShouldMatch(this PersonResponse actualExample, PersonResponse expectedExample)
        {
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.Title.ShouldBe(expectedExample.Title);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
            actualExample.LastName.ShouldBe(expectedExample.LastName);
            actualExample.Age.ShouldBe(expectedExample.Age);
            actualExample.Income.ShouldBe(expectedExample.Income);
        }

        public static void ShouldMatch<T>(this OpenApiExample actualExample, ISwaggerExample<T> expectedExample, Action<T,T> matcher)
        {
            actualExample.Summary.ShouldBe(expectedExample.Summary);
            var actualRequestValueSerialized = actualExample.Value.ShouldBeAssignableTo<OpenApiRawString>();
            var actualRequestValue = JsonConvert.DeserializeObject<T>(actualRequestValueSerialized.Value);
            matcher(actualRequestValue, expectedExample.Value);
        }

        public static void ShouldAllMatch<T>(
            this IDictionary<string, OpenApiExample> actualExamples,
            IEnumerable<ISwaggerExample<T>> expectedExamples,
            Action<T,T> matcher)
        {
            var expectedExamplesList = expectedExamples.ToList();

            // All expected examples should be in dictionary of actual examples
            foreach (var expectedExample in expectedExamplesList)
            {
                actualExamples.ShouldContainKey(expectedExample.Name);
                var actualExample = actualExamples[expectedExample.Name];
                actualExample.ShouldMatch(expectedExample, matcher);
            }

            // No other examples should occur in the dictionary of actual examples
            foreach (var actualExampleKey in actualExamples.Keys)
            {
                expectedExamplesList.ShouldContain(expectedExample => expectedExample.Name == actualExampleKey);
            }
        }
    }
}