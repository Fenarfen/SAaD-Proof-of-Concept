using System;
using AuthAPI.Models;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthAPI.Services
{
	public class DatabaseService
	{
		private readonly string _connectionString;

		public DatabaseService(string connectionString)
		{
			_connectionString = connectionString;
		}

		public string CreateMemberUser(Account account)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					string query = @"insert into Account values (null, @AddressID, 1, @Username, @Password, @Email, @FirstName, @LastName, GETDATE(), 0)";

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@AddressID", account.AddressID);
						command.Parameters.AddWithValue("@Username", account.Username);
						command.Parameters.AddWithValue("@Password", account.Password);
						command.Parameters.AddWithValue("@Email", account.Email);
						command.Parameters.AddWithValue("@FirstName", account.FirstName);
						command.Parameters.AddWithValue("@LastName", account.LastName);

						int result = command.ExecuteNonQuery();

						if (result == 1)
						{
							return "success";
						}
						else
						{
							return "failure";
						}
					}
				}
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}

		public string CreateMemberAddress(MemberAddress memberAddress)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					string query = @"insert into MemberAddress values (@FirstLine, @SecondLine, @ThirdLine, @FourthLine, @City, @County, @Country, @PostCode); SELECT SCOPE_IDENTITY();";

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@FirstLine", memberAddress.FirstLine);
						command.Parameters.AddWithValue("@SecondLine", memberAddress.SecondLine);
						command.Parameters.AddWithValue("@ThirdLine", memberAddress.ThirdLine);
						command.Parameters.AddWithValue("@FourthLine", memberAddress.FourthLine);
						command.Parameters.AddWithValue("@City", memberAddress.City);
						command.Parameters.AddWithValue("@County", memberAddress.County);
						command.Parameters.AddWithValue("@Country", memberAddress.Country);
						command.Parameters.AddWithValue("@PostCode", memberAddress.PostCode);

						object result = command.ExecuteScalar();

						return result == null ? "Couldn't save address" : result.ToString();
					}
				}
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}
		public async Task<int> SaveDataAsync(string data)
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();

				string query = "INSERT INTO YourTable (DataColumn) VALUES (@Data)";
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@Data", data);

					return await command.ExecuteNonQueryAsync();
				}
			}
		}

		public async Task<string?> ReadDataAsync(int id)
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();

				string query = "SELECT DataColumn FROM YourTable WHERE Id = @Id";
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@Id", id);

					using (SqlDataReader reader = await command.ExecuteReaderAsync())
					{
						if (await reader.ReadAsync())
						{
							return reader.GetString(0);
						}
					}
				}
			}

			return null;
		}
	}
}
