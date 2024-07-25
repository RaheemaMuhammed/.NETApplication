using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

       
        //registration
        public async Task RegisterUserAsync(string username, string passwordHash)
        {
            using (var connection = new SqlConnection(Database.GetDbConnection().ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("RegisterUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    await command.ExecuteNonQueryAsync();
                }



            }
        }

        //logging in users

        public async Task<bool> ValidateUserLoginAsync(string username, string passwordHash)
        {
            using (var connection = new SqlConnection(Database.GetDbConnection().ConnectionString))
            {

                await connection.OpenAsync();
                using (var command = new SqlCommand("ValidateUserLogin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    var result = (int)await command.ExecuteScalarAsync();
                    return result > 0;
                }
            }



        }

        //listing customers
        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            var customers = new List<Customer>();
            using (var connection = new SqlConnection(Database.GetDbConnection().ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("GetCustomers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            customers.Add(new Customer
                            {
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone"))

                            });
                        }
                    }
                }

            }
            return customers;
        }



        //adding customer data
        public async Task InsertCustomerAsync(string firstName, string lastName, string email, string phone)
        {
            using (var connection = new SqlConnection(Database.GetDbConnection().ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("InsertCustomer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Phone", phone);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        //updating customer data
        public async Task UpdateCustomerAsync(int customerId, string firstName, string lastName, string email, string phone)
        {
            using (var connection = new SqlConnection(Database.GetDbConnection().ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("UpdateCustomer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CustomerId", customerId);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Phone", phone);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        //delete customer
        public async Task DeleteCustomerAsync(int customerId)
        {
            using (var connection = new SqlConnection(Database.GetDbConnection().ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("DeleteCustomer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CustomerId", customerId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
