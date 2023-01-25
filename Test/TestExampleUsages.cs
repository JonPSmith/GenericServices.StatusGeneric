// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test
{
    public class TestExampleUsages
    {
        private readonly ITestOutputHelper _output;

        public TestExampleUsages(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("xxx", true)]
        [InlineData(null, false)]
        public void TestNonStatusGenericString(string s, bool valid)
        {
            //SETUP 
            var service = new ExampleUsages();

            //ATTEMPT
            var status = service.NonStatusGenericString(s);

            //VERIFY
            status.IsValid.ShouldEqual(valid);
            if (valid)
            {
                status.Message.ShouldEqual("All went well");
            }
            else
            {
                status.Errors.Single().ToString().ShouldEqual("input must not be null");
                status.Message.ShouldEqual("Failed with 1 error");
            }
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(-1, false)]
        public void TestStatusGenericNumReturnString(int i, bool valid)
        {
            //SETUP 
            var service = new ExampleUsages();

            //ATTEMPT
            var status = service.StatusGenericNumReturnString(i);

            //VERIFY
            status.IsValid.ShouldEqual(valid);
            if (valid)
            {
                status.Result.ShouldEqual("1");
            }
            else
            {
                status.Errors.Single().ToString().ShouldEqual("input must be positive");
                status.Message.ShouldEqual("Failed with 1 error");
                status.Result.ShouldEqual(null);
            }
        }

        [Theory]
        [InlineData(1, true, null)]
        [InlineData(-1, false, "input must be positive")]
        [InlineData(10, false, "input must not be null")]
        public void TestNonStatusGenericNum(int i, bool valid, string error)
        {
            //SETUP 
            var service = new ExampleUsages();

            //ATTEMPT
            var status = service.NonStatusGenericNum(i);

            //VERIFY
            status.IsValid.ShouldEqual(valid);
            if (valid)
            {
                status.Message.ShouldEqual("All went well");
            }
            else
            {
                status.Errors.Single().ToString().ShouldEqual(error);
                status.Message.ShouldEqual("Failed with 1 error");
            }
        }

        [Theory]
        [InlineData("Ab1aaaaaaaaa", 0)]
        [InlineData("aaaaaaaaaa1", 1)]
        [InlineData("", 4)]
        public void TestCheckPassword(string s, int numErrors)
        {
            //SETUP 
            var service = new ExampleUsages();

            //ATTEMPT
            var status = service.CheckPassword(s);

            //VERIFY
            foreach (var errorGeneric in status.Errors)
            {
                _output.WriteLine($"Error: '{errorGeneric.ToString()}', members: {string.Join(", ",errorGeneric.ErrorResult.MemberNames)}");
            }
            status.Errors.Count.ShouldEqual(numErrors);
        }

        [Theory]
        [InlineData("me@gmail.com", "Ab1aaaaaaaaa", null)]
        [InlineData("me@gmail.com", "aaaaaaaaaa1", "A password must contain an upper case character")]
        [InlineData(null, "Ab1aaaaaaaaa", "The email must not be null")]
        public void TestLogin(string email, string password, string error)
        {
            //SETUP 
            var service = new ExampleUsages();

            //ATTEMPT
            var status = service.Login(email, password);

            //VERIFY
            status.IsValid.ShouldEqual(error == null);
            if (error != null)
            {
                status.Errors.Single().ToString().ShouldEqual(error);
            }
        }

    }
}