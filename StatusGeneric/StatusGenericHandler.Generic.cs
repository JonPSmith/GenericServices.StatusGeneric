// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace StatusGeneric
{
    /// <summary>
    /// This contains the error handling part of the GenericBizRunner
    /// </summary>
    public class StatusGenericHandler<T> : StatusGenericHandler, IStatusGeneric<T>
    {
        private T _result;

        /// <summary>
        /// This is the returned result
        /// </summary>
        public T Result => IsValid ? _result : default(T);

        /// <summary>
        /// This sets the result to be returned
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public StatusGenericHandler<T> SetResult(T result)
        {
            _result = result;
            return this;
        }

        /// <summary>
        /// This adds one error to the Errors collection
        /// </summary>
        /// <param name="errorMessage">The text of the error message</param>
        /// <param name="propertyNames">optional. A list of property names that this error applies to</param>
        public new IStatusGeneric<T> AddError(string errorMessage, params string[] propertyNames)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));
            _errors.Add(new ErrorGeneric(Header, new ValidationResult(errorMessage, propertyNames)));
            return this;
        }
    }
}