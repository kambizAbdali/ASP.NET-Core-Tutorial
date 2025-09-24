using Dapper;
using DapperSample.Core.Entities;
using DapperSample.Core.Interfaces;
using DapperSample.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperSample.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public CustomerRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        // 1:1, 1:N Example - QueryMultiple
        public async Task<Customer> GetCustomerWithOrdersAndAddressAsync(int customerId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            string sql = @"SELECT * FROM Customers WHERE Id = @CustomerId;
                         SELECT * FROM Addresses WHERE CustomerId = @CustomerId;
                         SELECT * FROM Orders WHERE CustomerId = @CustomerId;";

            using var multi = await connection.QueryMultipleAsync(sql, new { CustomerId = customerId });

            var customer = await multi.ReadSingleOrDefaultAsync<Customer>();
            var address = await multi.ReadSingleOrDefaultAsync<Address>();
            var orders = await multi.ReadAsync<Order>();

            if (customer != null)
            {
                customer.Address = address;
                customer.Orders = orders.ToList();
            }

            return customer;
        }
    }
}