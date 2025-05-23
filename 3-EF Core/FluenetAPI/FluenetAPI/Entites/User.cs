﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public long KeyId { get; set; } 
        //public long KeyId2 { get; set; } // We have two primary keys and they identified in UserConfig class.
        public string NationalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }
    public class Address
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}