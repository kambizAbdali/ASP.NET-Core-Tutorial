using DapperSample.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperSample.Core.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> GetCustomerWithOrdersAndAddressAsync(int customerId); // 1:1, 1:N
    }
}
