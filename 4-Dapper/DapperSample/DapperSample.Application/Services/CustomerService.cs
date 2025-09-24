using DapperSample.Core.Entities;
using DapperSample.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperSample.Application.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Customer> GetCustomerWithOrdersAndAddressAsync(int customerId)
        {
            return await _customerRepository.GetCustomerWithOrdersAndAddressAsync(customerId);
        }
    }
}