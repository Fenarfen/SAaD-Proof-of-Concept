using AuthAPI.Interfaces;
using Microsoft.Data.SqlClient;
using AuthAPI.Models.Entities;
using AuthAPI.Models;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using AuthAPI.Models.DTOs;
using System.Data;
using Newtonsoft.Json.Linq;

namespace AuthAPI.Services;

public class DatabaseService : IDatabaseService
{
	private readonly IDbConnection _connection;

	public DatabaseService(IDbConnection connection)
	{
		_connection = connection;
	}

	public string StoreVerificationCode(int accountID, string code)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"insert into AccountVerificationCode values (@AccountID,
																	 @Code,
																     GETUTCDATE())";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@AccountID", accountID);
				AddParameter(command, "@Code", code);

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
		finally
		{
			if (_connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
		}
	}

	public Account GetAccountByID(int accountID)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"select ID, TokenID, RoleID, Email, [Password], FirstName, LastName, Created, Verified from Account where ID = @ID";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@ID", accountID);

				using (IDataReader reader = command.ExecuteReader())
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
		finally
		{
			if (_connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
		}
	}

	public Account GetAccountByEmail(string email)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"select ID, TokenID, RoleID, Email, [Password], FirstName, LastName, Created, Verified from Account where Email = @Email";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@Email", email);

				using (IDataReader reader = command.ExecuteReader())
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
		finally
		{
			if (_connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
		}
	}

	public string CheckCode(string email, string code)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"select count(*) from AccountVerificationCode av
							 join Account a on av.AccountID = a.ID
							 where Email = @Email and Code = @Code and CreatedAt >= DATEADD(MINUTE, -10, GETUTCDATE())";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@Email", email);
				AddParameter(command, "@Code", code);

				if (Convert.ToInt32(command.ExecuteScalar()) > 0)
				{
					return "true";
				}
			}

			query = @"select count(*) from AccountVerificationCode av
					  join Account a on av.AccountID = a.ID
					  where Email = @Email and Code = @Code";

			//If not found, lets check for an expired code to inform the user
			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@Email", email);
				AddParameter(command, "@Code", code);

				if (Convert.ToInt32(command.ExecuteScalar()) > 0)
				{
					return "Code has expired";
				}
			}

			return "false";
		}
		finally
		{
			if (_connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
		}
	}

	public string AssignToken(int id, string token)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string deleteQuery = @"delete from Token where UserID = @UserID";
			string insertquery = @"insert into Token values (@UserID, @Token, GETUTCDATE())";
			string updateQuery = @"update Account
								   set TokenID = (select top 1 ID from Token where UserID = @UserID order by Created desc)
								   where ID = @UserID";


			//start by removing old tokens
			//using (SqlCommand command = new SqlCommand(deleteQuery, connection))
			//{
			//	command.Parameters.AddWithValue("@UserID", id);

			//	//doesn't matter if nothing is deleted here so skip any checks
			//	command.ExecuteNonQuery();
			//}

			//save new token
			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = insertquery;

				AddParameter(command, "@UserID", id);
				AddParameter(command, "@Token", token);

				//cancel if no record created
				if (command.ExecuteNonQuery() == 0)
				{
					return "false";
				}
			}

			//Update account
			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = updateQuery;

				AddParameter(command, "@UserID", id);

				//return result status
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
		finally
		{
			if (_connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
		}
	}

	public string VerifyToken(string request)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"select Created from Token where Value = @Token order by Created desc";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@Token", request);

				using (IDataReader reader = command.ExecuteReader())
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
		finally
		{
			if (_connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
		}
	}

	public string VerifyAccountEmail(string email)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"update Account set Verified = 1 where Email = @Email";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@Email", email);

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
		finally
		{
			if (_connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
		}
	}

	public CityRoleDTO GetCityRoleFromToken(string token)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"select r.Name, ad.City 
							 from Token t
							 join Account a on t.UserID = a.ID
							 left join Role r ON a.RoleID = r.ID
							 left join Address ad ON a.ID = ad.UserID
							 where t.Value = @Token
							   and (ad.IsDefault = 1 or ad.IsDefault is null)";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@Token", token);

				using (IDataReader reader = command.ExecuteReader())
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
		finally
		{
			if (_connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
		}
	}

	public string GetAccountRoleName(Account account)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"select Name from Role where ID = @ID";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@ID", account.RoleID);

				using (IDataReader reader = command.ExecuteReader())
				{
					if (!reader.Read())
					{
						return null;
					}

					return reader.GetString(0);
				}
			}
		}
		finally
		{
			if (_connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
		}
	}

	private void AddParameter(IDbCommand command, string parameterName, object value)
	{
		IDbDataParameter parameter = command.CreateParameter();
		parameter.ParameterName = parameterName;
		parameter.Value = value ?? DBNull.Value;
		command.Parameters.Add(parameter);
	}
}
