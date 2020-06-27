// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using StatusGeneric;
using Xunit.Abstractions;

namespace Test
{
    public class ExampleUsages
    {
        public IStatusGeneric NonStatusGenericString(string s)
        {
            var status = new StatusGenericHandler();

            //add error and return immediately
            if (s == null)
                //You can return just an error message, but adding the property name
                //will improve the error feedback in ASP.NET Core etc.
                return status.AddError("input must not be null", nameof(s));

            status.Message = "All went well";

            //If no errors were added then its returns a IsValid status with the message
            //If there are errors then the Message says something like "Failed with 1 error" and the HasErrors will be true, IsValid will be false
            return status;
        }

        public IStatusGeneric<string> StatusGenericNumReturnString(int i)
        {
            var status = new StatusGenericHandler<string>();

            //add error and return immediately
            if (i <= 0)
                return status.AddError("input must be positive", nameof(i));

            //This sets the Result property in the generic status
            status.SetResult(i.ToString());

            //If there are errors then the Result is set to the default value for generic type
            return status;
        }

        public IStatusGeneric NonStatusGenericNum(int i)
        {
            var status = new StatusGenericHandler();

            //add error and return immediately
            if (i <= 0)
                return status.AddError("input must be positive", nameof(i));

            //combine error from another non-StatusGeneric method and return if has errors
            status.CombineStatuses(NonStatusGenericString(i == 10 ? null : "good"));
            if (status.HasErrors)
                return status;

            //combine errors from another generic status, keeping the status to extract later
            var stringStatus = StatusGenericNumReturnString(i);
            if (status.CombineStatuses(stringStatus).HasErrors)
                return status;

            var stringResult = stringStatus.Result;
            //Other methods here 

            return status;
        }

        public IStatusGeneric<int> StatusGenericNum(int i)
        {
            var status = new StatusGenericHandler<int>();

            //series of tests and then return all the errors together
            //Good because the user gets all the errors at once
            if (i == 20)
                status.AddError("input must not be 20", nameof(i));
            if (i == 30)
                status.AddError("input must not be 30", nameof(i));
            if (i == 40)
                status.AddError("input must not be 40", nameof(i));
            if (i == 50)
                status.AddError("input must not be 50", nameof(i));
            if (!status.IsValid)
                return status;

            //add error and return immediately
            if (i <= 0)
                return status.AddError("input must be positive", nameof(i));

            //combine error from another non-StatusGeneric method and return if has errors
            status.CombineStatuses(NonStatusGenericString(i == 10 ? null : "good"));
            if (status.HasErrors)
                return status;

            //combine errors from another generic status, keeping the status to extract later
            var stringStatus = StatusGenericNumReturnString(i);
            if (status.CombineStatuses(stringStatus).HasErrors)
                return status;

            //Do something with the Result from the StatusGenericNumReturnString method
            var combinedNums = int.Parse(stringStatus.Result) + i;
            //Set the result to be returned from this method if there are no errors
            status.SetResult(combinedNums);

            //You can put tests just before a result would be returned  - any error will set the result to default value
            if (combinedNums <= 0)
                status.AddError("input must be positive", nameof(i));

            //If you return a IStatusGeneric<T> and there are errors then the result will be set to default
            return status;
        }
    }
}