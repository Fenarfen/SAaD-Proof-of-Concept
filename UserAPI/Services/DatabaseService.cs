using UserAPI.Interfaces;
using Microsoft.Data.SqlClient;
using UserAPI.Models.Entities;
using UserAPI.Models.Dtos;

namespace UserAPI.Services;

public class DatabaseService : IDatabaseService
{
	private readonly string _connectionString;

	public DatabaseService(string connectionString)
	{
		_connectionString = connectionString;
	}

	public string CreateMemberUser(Account account)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"insert into Account values (null, 1, @Password, @Email, @FirstName, @LastName, GETDATE(), 0)";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@Email", account.Email);
				command.Parameters.AddWithValue("@FirstName", account.FirstName);
				command.Parameters.AddWithValue("@LastName", account.LastName);
				command.Parameters.AddWithValue("@Password", account.Password);

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

	public string EditAccount(int id, AccountUpdateDto account)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"update Account
							 Email = @Email,
							 Password = @Password,
							 FirstName = @FirstName,
							 LastName = @LastName
							 where ID = @ID";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@Email", account.Email);
				command.Parameters.AddWithValue("@Password", account.Password);
				command.Parameters.AddWithValue("@FirstName", account.FirstName);
				command.Parameters.AddWithValue("@LastName", account.LastName);
				command.Parameters.AddWithValue("@ID", id);

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

	public string CreateMemberAddress(MemberAddress memberAddress)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"insert into MemberAddress values (@AccountID, 
															   @FirstLine, 
															   @SecondLine, 
															   @ThirdLine, 
															   @FourthLine, 
															   @City, 
															   @County, 
															   @Country, 
															   @Postcode)";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@AccountID", memberAddress.AccountID);
				command.Parameters.AddWithValue("@FirstLine", memberAddress.FirstLine);
				command.Parameters.AddWithValue("@SecondLine", memberAddress.SecondLine);
				command.Parameters.AddWithValue("@ThirdLine", memberAddress.ThirdLine);
				command.Parameters.AddWithValue("@FourthLine", memberAddress.FourthLine);
				command.Parameters.AddWithValue("@City", memberAddress.City);
				command.Parameters.AddWithValue("@County", memberAddress.County);
				command.Parameters.AddWithValue("@Country", memberAddress.Country);
				command.Parameters.AddWithValue("@Postcode", memberAddress.PostCode);

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

	public Account GetAccountByID(int accountID)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"select TokenID, RoleID, Email, [Password], FirstName, LastName, Created, Verified from Account where ID = @ID";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				using (SqlDataReader reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						return new Account
						{
							ID = Convert.ToInt32(reader["ID"]),
							TokenID = Convert.ToInt32(reader["TokenID"]),
							RoleID = Convert.ToInt32(reader["RoleID"]),
							Password = reader["Password"].ToString(),
							Email = reader["Email"].ToString(),
							FirstName = reader["FirstName"].ToString(),
							LastName = reader["LastName"].ToString(),
							Created = Convert.ToDateTime(reader["Created"]),
							Verified = Convert.ToBoolean(reader["Verified"])
						};
					}
					else
					{
						return null;
					}
				}
			}
		}
	}

	public string EditMemberAddress(int id, MemberAddress memberAddress)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"update MemberAddress
							 set FirstLine = @FirstLine,
							 SecondLine = @SecondLine,
							 ThirdLine = @ThirdLine,
							 FourthLine = @FourthLine,
							 City = @City,
							 County = @County,
							 Country = @Country,
							 Postcode = @Postcode
							 where ID = @ID";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@FirstLine", memberAddress.FirstLine);
				command.Parameters.AddWithValue("@SecondLine", memberAddress.SecondLine);
				command.Parameters.AddWithValue("@ThirdLine", memberAddress.ThirdLine);
				command.Parameters.AddWithValue("@FourthLine", memberAddress.FourthLine);
				command.Parameters.AddWithValue("@City", memberAddress.City);
				command.Parameters.AddWithValue("@County", memberAddress.County);
				command.Parameters.AddWithValue("@Country", memberAddress.Country);
				command.Parameters.AddWithValue("@Postcode", memberAddress.PostCode);
				command.Parameters.AddWithValue("@ID", id);

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

	public string DeleteMemberAddress(int addressID)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"delete from MemberAddress where ID = @ID";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@ID", addressID);

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

	public string DoesEmailExist(string email)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"select count(*) from Account where Email = @Email";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@Email", email);

				int count = (int)command.ExecuteScalar();

				if (count > 0)
				{
					return "true";
				}
				else
				{
					return "false";
				}
			}
		}
	}
}
