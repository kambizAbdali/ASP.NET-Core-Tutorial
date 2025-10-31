using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Exceptions
{
    /// <summary>
    /// Exception for resource not found scenarios
    /// </summary>
    public class ResourceNotFoundException : BusinessException
    {
        public ResourceNotFoundException(string message) : base(message, "RESOURCE_NOT_FOUND")
        {
        }
    }
}
