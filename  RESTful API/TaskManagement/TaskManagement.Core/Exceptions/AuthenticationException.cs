using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Exceptions
{
    /// <summary>
    /// Exception for authentication and authorization failures
    /// </summary>
    public class AuthenticationException : BusinessException
    {
        public AuthenticationException(string message) : base(message, "AUTH_ERROR")
        {
        }
    }

}
