using System;

namespace TaskManagement.Core.Exceptions
{
    /// <summary>
    /// Custom exception for business rule violations
    /// </summary>
    public class BusinessException : Exception
    {
        public string ErrorCode { get; }

        public BusinessException(string message) : base(message)
        {
        }

        public BusinessException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public BusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}