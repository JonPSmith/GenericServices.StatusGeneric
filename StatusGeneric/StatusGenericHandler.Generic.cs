// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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
    }
}