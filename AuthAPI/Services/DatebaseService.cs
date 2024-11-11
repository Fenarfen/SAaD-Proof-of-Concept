using AuthAPI.Interfaces;
using Microsoft.Data.SqlClient;
using AuthAPI.Models.Entities;

namespace AuthAPI.Services;

public class DatabaseService : IDatabaseService
{
	private readonly string _connectionString;

	public DatabaseService(string connectionString)
	{
		_connectionString = connectionString;
	}

	public string AssignVerificationCode(int accountID, string code)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"insert into UserVerificationCode values (@AccountID,
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

			string query = @"select TokenID, RoleID, Email, [Password], FirstName, LastName, Created, Verified from Account where ID = @ID";

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

	public Account GetAccountByEmail(string email)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"select TokenID, RoleID, Email, [Password], FirstName, LastName, Created, Verified from Account where Email = @Email";

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

	public string CheckCode(int accountID, string code)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"select count(*) from UserVerificationCode where AccountID = @AccountID and Code = @Code and Created >= DATEADD(MINUTE, -30, GETUTCDATE());";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@AccountID", accountID);
				command.Parameters.AddWithValue("@Code", code);

				if (Convert.ToInt32(command.ExecuteScalar()) > 0)
				{
					return "true";
				}
			}

			query = @"select count(*) from UserVerificationCode where AccountID = @AccountID and Code = @Code";

			//If not found, lets check for an expired code to inform the user
			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@AccountID", accountID);
				command.Parameters.AddWithValue("@Code", code);

				if (Convert.ToInt32(command.ExecuteScalar()) > 0)
				{
					return "expired";
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
				if (Convert.ToInt32(command.ExecuteScalar()) > 0)
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

	public string VerifyAccount(int accountID)
	{
		using (SqlConnection connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			string query = @"update Account set Verified = 1 where ID = @ID";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@ID", accountID);

				if (Convert.ToInt32(command.ExecuteScalar()) > 0)
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
