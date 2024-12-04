using AuthAPI.Interfaces;
using Microsoft.Data.SqlClient;
using AuthAPI.Models.Entities;
using AuthAPI.Models;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using AuthAPI.Models.DTOs;

namespace AuthAPI.Services;

public class DatabaseService : IDatabaseService
{
	private readonly string _connectionString;

	public DatabaseService(string connectionString)
	{
		_connectionString = connectionString;
	}

	public string StoreVerificationCode(int accountID, string code)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"insert into AccountVerificationCode values (@AccountID,
																	  @Code,
																      GETUTCDATE())";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@AccountID", accountID);
				command.Parameters.AddWithValue("@Code", code);

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

			string query = @"select ID, TokenID, RoleID, Email, [Password], FirstName, LastName, Created, Verified from Account where ID = @ID";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@ID", accountID);

				using (SqlDataReader reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						return new Account
						{
							ID = Convert.ToInt32(reader["ID"]),
							TokenID = reader["TokenID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["TokenID"]),
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

	public Account GetAccountByEmail(string email)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"select ID, TokenID, RoleID, Email, [Password], FirstName, LastName, Created, Verified from Account where Email = @Email";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@Email", email);

				using (SqlDataReader reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						return new Account
						{
							ID = Convert.ToInt32(reader["ID"]),
							TokenID = reader["TokenID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["TokenID"]),
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

	public string CheckCode(string email, string code)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"select count(*) from AccountVerificationCode av
							 join Account a on av.AccountID = a.ID
							 where Email = @Email and Code = @Code and CreatedAt >= DATEADD(MINUTE, -10, GETUTCDATE())";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@Email", email);
				command.Parameters.AddWithValue("@Code", code);

				if (Convert.ToInt32(command.ExecuteScalar()) > 0)
				{
					return "true";
				}
			}

			query = @"select count(*) from AccountVerificationCode av
					  join Account a on av.AccountID = a.ID
					  where Email = @Email and Code = @Code";

			//If not found, lets check for an expired code to inform the user
			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@Email", email);
				command.Parameters.AddWithValue("@Code", code);

				if (Convert.ToInt32(command.ExecuteScalar()) > 0)
				{
					return "Code has expired";
				}
			}

			return "false";
		}
	}

	public string AssignToken(int id, string token)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string deleteQuery = @"delete from Token where UserID = @UserID";
			string insertquery = @"insert into Token values (@UserID, @Token, GETUTCDATE())";

			//start by removing old tokens
			using (SqlCommand command = new SqlCommand(deleteQuery, connection))
			{
				command.Parameters.AddWithValue("@UserID", id);

				//doesn't matter if nothing is deleted here so skip any checks
				command.ExecuteNonQuery();
			}

			//save new token
			using (SqlCommand command = new SqlCommand(insertquery, connection))
			{
				command.Parameters.AddWithValue("@UserID", id);
				command.Parameters.AddWithValue("@Token", token);

				//This time it matters so check a record is created
				if (command.ExecuteNonQuery() > 0)
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

	public string VerifyToken(string request)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"select Created from Token where Value = @Token order by Created desc";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@Token", request);

				using (SqlDataReader reader = command.ExecuteReader())
				{
					if (!reader.Read())
					{
						return null;
					}

					if (reader.IsDBNull(0))
					{
						return null;
					}

					return reader.GetDateTime(0).ToString();
				}
			}
		}
	}

	public string VerifyAccountEmail(string email)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"update Account set Verified = 1 where Email = @Email";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@Email", email);

				if (command.ExecuteNonQuery() > 0)
				{
					return "success";
				}
				else
				{
					return "failed";
				}
			}
		}
	}

	public CityRoleDTO GetCityRoleFromToken(string token)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"select r.Name, ad.City 
							 from Token t
							 join Account a on t.UserID = a.ID
							 left join Role r ON a.RoleID = r.ID
							 left join Address ad ON a.ID = ad.UserID
							 where t.Value = @Token
							   and (ad.IsDefault = 1 or ad.IsDefault is null)";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@Token", token);

				using (SqlDataReader reader = command.ExecuteReader())
				{
					if (!reader.Read())
					{
						return null;
					}

					return new CityRoleDTO
					{
						Role = reader.IsDBNull(0) == true ? "not found" : reader.GetString(0),
						City = reader.IsDBNull(1) == true ? "not found" : reader.GetString(1)
					};
				}
			}
		}
	}

	public string GetAccountRoleName(Account account)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"select Name from Role where ID = @ID";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@ID", account.RoleID);

				using (SqlDataReader reader = command.ExecuteReader())
				{
					if (!reader.Read())
					{
						return null;
					}

					return reader.GetString(0);
				}
			}
		}
	}
}
