﻿// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StatusGeneric
{
    /// <summary>
    /// This contains the error handling part of the GenericBizRunner
    /// </summary>
    public class StatusGenericHandler : IStatusGenericHandler
    {
        /// <summary>
        /// This is the default success message.
        /// </summary>
        public const string DefaultSuccessMessage = "Success";
        protected readonly List<ErrorGeneric> _errors = new List<ErrorGeneric>();
        private string _successMessage = DefaultSuccessMessage;

        /// <summary>
        /// This creates a StatusGenericHandler, with optional header (see Header property, and CombineStatuses)
        /// </summary>
        /// <param name="header"></param>
        public StatusGenericHandler(string header = "")
        {
            Header = header;
        }

        /// <summary>
        /// The header provides a prefix to any errors you add. Useful if you want to have a general prefix to all your errors
        /// e.g. a header if "MyClass" would produce error messages such as "MyClass: This is my error message."
        /// NOTE: Headers aren't supported by the https://github.com/JonPSmith/Net.LocalizeMessagesAndErrors library
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// This holds the list of ValidationResult errors. If the collection is empty, then there were no errors
        /// </summary>
        public IReadOnlyList<ErrorGeneric> Errors => _errors.AsReadOnly();

        /// <summary>
        /// This is true if there are no errors 
        /// </summary>
        public bool IsValid => !_errors.Any();

        /// <summary>
        /// This is true if any errors have been added 
        /// </summary>
        public bool HasErrors => _errors.Any();

        /// <summary>
        /// On success this returns the message as set by the business logic, or the default messages set by the BizRunner
        /// If there are errors it contains the message "Failed with NN errors"
        /// </summary>
        public string Message
        {
            get => IsValid
                ? _successMessage
                : $"Failed with {_errors.Count} error" + (_errors.Count == 1 ? "" : "s");
            set => _successMessage = value;
        }

        /// <summary>
        /// This allows statuses to be combined. This does the following
        /// 1. The status parameter's Errors are added to the current status
        /// 2. It updates the current Message if the status parameter's Message has the DefaultSuccessMessage
        /// NOTE: If you are using Headers, then it will combine the headers in any errors in combines
        /// e.g. If current Header is "MyClass" and the status parameter's Header is "MyProp", then
        /// each error in the status parameter's would start with "MyClass>MyProp:", e.g. "MyClass>MyProp: This is my error message."
        /// NOTE: Headers aren't supported by the https://github.com/JonPSmith/Net.LocalizeMessagesAndErrors library.
        /// </summary>
        /// <param name="status"></param>
        public IStatusGeneric CombineStatuses(IStatusGeneric status)
        {
            if (!status.IsValid)
            {
                _errors.AddRange(string.IsNullOrEmpty(Header)
                    ? status.Errors
                    : status.Errors.Select(x => new ErrorGeneric(Header, x)));
            }
            if (IsValid && status.Message != DefaultSuccessMessage)
                Message = status.Message;

            return this;
        }

        /// <summary>
        /// This is a simple method to output all the errors as a single string - returns "No errors" if no errors.
        /// Useful for feeding back all the errors in a single exception (also nice in unit testing)
        /// </summary>
        /// <param name="separator">if null then each errors is separated by Environment.NewLine, otherwise uses the separator you provide</param>
        /// <returns>a single string with all errors separated by the 'separator' string, or "No errors" if no errors.</returns>
        public string GetAllErrors(string separator = null)
        {
            separator = separator ?? Environment.NewLine;
            return _errors.Any() 
                ? string.Join(separator, Errors) 
                : "No errors";
        }

        /// <summary>
        /// This adds one error to the Errors collection
        /// NOTE: This is virtual so that the StatusGenericHandler.Generic can override it. That allows both to return a IStatusGeneric result
        /// </summary>
        /// <param name="errorMessage">The text of the error message</param>
        /// <param name="propertyNames">optional. A list of property names that this error applies to</param>
        public virtual IStatusGeneric AddError(string errorMessage, params string[] propertyNames)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));
            _errors.Add(new ErrorGeneric(Header, new ValidationResult(errorMessage, propertyNames)));
            return this;
        }

        /// <summary>
        /// This adds one error to the Errors collection and saves the exception's data to the DebugData property
        /// </summary>
        /// <param name="ex">The exception that you want to turn into a IStatusGeneric error.</param>
        /// <param name="errorMessage">The user-friendly text for the error message</param>
        /// <param name="propertyNames">optional. A list of property names that this error applies to</param>
        public IStatusGeneric AddError(Exception ex, string errorMessage, params string[] propertyNames)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));
            var errorGeneric = new ErrorGeneric(Header, new ValidationResult(errorMessage, propertyNames));
            errorGeneric.CopyExceptionToDebugData(ex);
            _errors.Add(errorGeneric);
            return this;
        }

        /// <summary>
        /// This adds one ValidationResult to the Errors collection
        /// </summary>
        /// <param name="validationResult"></param>
        public void AddValidationResult(ValidationResult validationResult)
        {
            _errors.Add(new ErrorGeneric(Header, validationResult));
        }

        /// <summary>
        /// This appends a collection of ValidationResults to the Errors collection
        /// </summary>
        /// <param name="validationResults"></param>
        public void AddValidationResults(IEnumerable<ValidationResult> validationResults)
        {
            _errors.AddRange(validationResults.Select(x => new ErrorGeneric(Header, x)));
        }
    }
}