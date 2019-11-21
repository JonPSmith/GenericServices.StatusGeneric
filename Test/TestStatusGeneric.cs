﻿// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using StatusGeneric;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test
{
    public class TestStatusGeneric
    {
        [Fact]
        public void TestGenericStatusOk()
        {
            //SETUP 

            //ATTEMPT
            var status = new StatusGenericHandler();

            //VERIFY
            status.IsValid.ShouldBeTrue();
            status.Errors.Any().ShouldBeFalse();
            status.Message.ShouldEqual("Success");
        }

        [Fact]
        public void TestGenericStatusSetMessageOk()
        {
            //SETUP 
            var status = new StatusGenericHandler();

            //ATTEMPT
            status.Message = "New message";

            //VERIFY
            status.IsValid.ShouldBeTrue();
            status.Errors.Any().ShouldBeFalse();
            status.Message.ShouldEqual("New message");
        }

        [Fact]
        public void TestGenericStatusWithErrorOk()
        {
            //SETUP 
            var status = new StatusGenericHandler();

            //ATTEMPT
            status.AddError("This is an error.");

            //VERIFY
            status.IsValid.ShouldBeFalse();
            status.Errors.Single().ToString().ShouldEqual("This is an error.");
            status.Message.ShouldEqual("Failed with 1 error");
        }

        [Fact]
        public void TestGenericStatusCombineStatusesWithErrorsOk()
        {
            //SETUP 
            var status1 = new StatusGenericHandler();
            var status2 = new StatusGenericHandler();

            //ATTEMPT
            status1.AddError("This is an error.");
            status2.CombineStatuses(status1);

            //VERIFY
            status2.IsValid.ShouldBeFalse();
            status2.Errors.Single().ToString().ShouldEqual("This is an error.");
            status2.Message.ShouldEqual("Failed with 1 error");
        }

        [Fact]
        public void TestGenericStatusCombineStatusesIsValidWithMessageOk()
        {
            //SETUP 
            var status1 = new StatusGenericHandler();
            var status2 = new StatusGenericHandler();

            //ATTEMPT
            status1.Message = "My message";
            status2.CombineStatuses(status1);

            //VERIFY
            status2.IsValid.ShouldBeTrue();
            status2.Message.ShouldEqual("My message");
        }

        [Fact]
        public void TestGenericStatusHeaderAndErrorOk()
        {
            //SETUP 
            var status = new StatusGenericHandler("MyClass");

            //ATTEMPT
            status.AddError("This is an error.");

            //VERIFY
            status.IsValid.ShouldBeFalse();
            status.Errors.Single().ToString().ShouldEqual("MyClass: This is an error.");
        }

        [Fact]
        public void TestGenericStatusHeaderCombineStatusesOk()
        {
            //SETUP 
            var status1 = new StatusGenericHandler("MyClass");
            var status2 = new StatusGenericHandler("MyProp");

            //ATTEMPT
            status2.AddError("This is an error.");
            status1.CombineStatuses(status2);

            //VERIFY
            status1.IsValid.ShouldBeFalse();
            status1.Errors.Single().ToString().ShouldEqual("MyClass>MyProp: This is an error.");
            status1.Message.ShouldEqual("Failed with 1 error");
        }

        [Fact]
        public void TestCaptureException()
        {
            //SETUP
            var status = new StatusGenericHandler();

            //ATTEMPT
            try
            {
                MethodToThrowException();
            }
            catch (Exception e)
            {
                status.CopyExceptionToDebugData(e);
            }

            //VERIFY
            var lines = status.DebugData.Split(Environment.NewLine);
            lines.Length.ShouldEqual(6);
            lines[0].ShouldEqual("This is a test");
            lines[1].ShouldStartWith("StackTrace:   at Test.TestStatusGeneric.MethodToThrowException()");
            lines[3].ShouldEqual("Data: data1\t1");
            lines[4].ShouldEqual("Data: data2\t2");
        }

        private void MethodToThrowException()
        {
            var ex = new Exception("This is a test");
            ex.Data.Add("data1", 1);
            ex.Data.Add("data2", "2");
            throw ex;
        }

        //------------------------------------

        [Fact]
        public void TestGenericStatusGenericOk()
        {
            //SETUP 

            //ATTEMPT
            var status = new StatusGenericHandler<string>();

            //VERIFY
            status.IsValid.ShouldBeTrue();

            status.Result.ShouldEqual(null);
        }

        [Fact]
        public void TestGenericStatusGenericSetResultOk()
        {
            //SETUP 

            //ATTEMPT
            var status = new StatusGenericHandler<string>();
            status.SetResult("Hello world");

            //VERIFY
            status.IsValid.ShouldBeTrue();
            status.Result.ShouldEqual("Hello world");
        }

        [Fact]
        public void TestGenericStatusGenericSetResultThenErrorOk()
        {
            //SETUP 

            //ATTEMPT
            var status = new StatusGenericHandler<string>();
            status.SetResult("Hello world");
            status.AddError("This is an error.");

            //VERIFY
            status.IsValid.ShouldBeFalse();
            status.Result.ShouldEqual(null);
        }
    }
}