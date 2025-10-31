using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Exceptions
{
    /// <summary>
    /// Exception for validation rule violations
    /// </summary>
    public class ValidationException : BusinessException
    {
        public ValidationException(string message) : base(message, "VALIDATION_ERROR")
        {
        }
    }   
}
