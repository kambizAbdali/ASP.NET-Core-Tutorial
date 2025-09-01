using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwnedEntityTypesDemo.Application.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public AddressDTO HomeAddress { get; set; }
        public AddressDTO WorkPlaceAddress { get; set; }
    }
}
