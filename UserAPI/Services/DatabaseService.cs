using UserAPI.Interfaces;
using Microsoft.Data.SqlClient;
using UserAPI.Models.Entities;
using UserAPI.Models.Dtos;
using Microsoft.Identity.Client;
using System.Data;

namespace UserAPI.Services;

public class DatabaseService : IDatabaseService
{
	private readonly IDbConnection _connection;

	public DatabaseService(IDbConnection connection)
	{
		_connection = connection;
	}

	public string CreateMemberUser(AccountCreateDto account)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"insert into Account output inserted.ID values (null, 1, @Email, @Password, @FirstName, @LastName, GETDATE(), 0)";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@Email", account.Email);
				AddParameter(command, "@FirstName", account.FirstName);
				AddParameter(command, "@LastName", account.LastName);
				AddParameter(command, "@Password", account.Password);

				return Convert.ToString(command.ExecuteScalar()); // returns null if no record is created, otherwise will be the id of the record created
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

	public string EditAccount(ProfileManagementDTO account)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"update Account
							 set Email = @Email,
							 FirstName = @FirstName,
							 LastName = @LastName
							 where ID = @ID";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@Email", account.Email);
				AddParameter(command, "@FirstName", account.FirstName);
				AddParameter(command, "@LastName", account.LastName);
				AddParameter(command, "@ID", account.ID);

				int result = command.ExecuteNonQuery();

				if (result == 1)
				{
					SyncAddresses(account.Addresses);
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

	public void SyncAddresses(List<Address> addresses)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			if (addresses == null || addresses.Count == 0)
			{
				throw new ArgumentException("Address list cannot be null or empty.");
			}

			int userID = addresses.First().AccountID;

			List<Address> currentAddresses = new List<Address>();

			_connection.Open();

			string query = @"select * from Address where UserID = @UserID";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@UserID", userID);

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						currentAddresses.Add(new Address
						{
							ID = reader.GetInt32(0),
							AccountID = reader.GetInt32(1),
							FirstLine = reader.GetString(2),
							SecondLine = reader.IsDBNull(3) ? null : reader.GetString(3),
							ThirdLine = reader.IsDBNull(4) ? null : reader.GetString(4),
							FourthLine = reader.IsDBNull(5) ? null : reader.GetString(5),
							City = reader.GetString(6),
							County = reader.IsDBNull(7) ? null : reader.GetString(7),
							Country = reader.IsDBNull(8) ? null : reader.GetString(8),
							PostCode = reader.GetString(9),
							IsDefault = reader.GetBoolean(10)
						});
					}
				}
			}

			foreach (var currentAddress in currentAddresses)
			{
				// If the current address from the old list is not in the new list, delete it
				if (!addresses.Any(a => a.ID == currentAddress.ID))
				{
					DeleteAddress(currentAddress.ID);
				}
			}

			foreach (var address in addresses)
			{
				// Check if the address from the new list exists in the old list
				var existingAddress = currentAddresses.FirstOrDefault(ca => ca.ID == address.ID);

				if (existingAddress == null)
				{
					// If it doesn't exist, create a new address
					CreateAddress(address);
				}
				else
				{
					// If it exists, update the address
					UpdateAddress(address);
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

	public string CreateAddress(Address address)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"insert into Address values (@AccountID, 
														 @FirstLine, 
														 @SecondLine, 
														 @ThirdLine, 
														 @FourthLine, 
														 @City, 
														 @County, 
														 @Country, 
														 @Postcode,
                                                         @IsDefault)";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@AccountID", address.AccountID);             // required
				AddParameter(command, "@FirstLine", address.FirstLine);             // required
				AddParameter(command, "@SecondLine", address.SecondLine ?? "");
				AddParameter(command, "@ThirdLine", address.ThirdLine ?? "");
				AddParameter(command, "@FourthLine", address.FourthLine ?? "");
				AddParameter(command, "@City", address.City);                       // required
				AddParameter(command, "@County", address.County ?? "");
				AddParameter(command, "@Country", address.Country ?? "");
				AddParameter(command, "@Postcode", address.PostCode);               // required
				AddParameter(command, "@IsDefault", address.IsDefault);             // required

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

	public string UpdateAddress(Address address)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"update Address
							 set FirstLine = @FirstLine,
							 SecondLine = @SecondLine,
							 ThirdLine = @ThirdLine,
							 FourthLine = @FourthLine,
							 City = @City,
							 County = @County,
							 Country = @Country,
							 Postcode = @Postcode,
							 IsDefault = @IsDefault
							 where ID = @ID";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@FirstLine", address.FirstLine);             // required
				AddParameter(command, "@SecondLine", address.SecondLine ?? "");
				AddParameter(command, "@ThirdLine", address.ThirdLine ?? "");
				AddParameter(command, "@FourthLine", address.FourthLine ?? "");
				AddParameter(command, "@City", address.City);                       // required
				AddParameter(command, "@County", address.County ?? "");
				AddParameter(command, "@Country", address.Country ?? "");
				AddParameter(command, "@Postcode", address.PostCode);               // required
				AddParameter(command, "@IsDefault", address.IsDefault);             // required
				AddParameter(command, "@ID", address.ID);                           // required

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

	public string DeleteAddress(int addressID)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"delete from Address where ID = @ID";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@ID", addressID);

				int result = command.ExecuteNonQuery();

				if (result > 1)
				{
					return "something went very wrong";
				}
				else if (result == 1)
				{
					return "success";
				}
				else
				{
					return "not found";
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

				AddParameter(command, "ID", accountID);

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

	public ProfileManagementDTO GetProfileManagementDTOfromToken(string token)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			ProfileManagementDTO profileManagementDTO = null;

			_connection.Open();

			string accountQuery = @"select a.ID, r.Name, a.Email, a.FirstName, a.LastName, a.Created, a.Verified from Token t
									join Account a on t.UserID = a.ID
									join Role r on r.ID = a.RoleID
									where t.Value = @Token";

			string addressesQuery = @"select ad.* from Token t
									  join Account a on t.ID = a.TokenID
									  join Address ad on a.ID = ad.UserID
									  where t.Value = @Token";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = accountQuery;

				AddParameter(command, "@Token", token);

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						profileManagementDTO = new ProfileManagementDTO
						{
							ID = reader.GetInt32(0),
							Role = reader.GetString(1),
							Email = reader.GetString(2),
							FirstName = reader.GetString(3),
							LastName = reader.GetString(4),
							CreatedAt = reader.GetDateTime(5),
							Verified = reader.GetBoolean(6)
						};
					}

					if (profileManagementDTO == null)
					{
						return null;
					}
				}
			}

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = addressesQuery;

				AddParameter(command, "@Token", token);

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						profileManagementDTO.Addresses.Add(new Address
						{
							ID = reader.GetInt32(0),            //required
							AccountID = reader.GetInt32(1),     //required
							FirstLine = reader.GetString(2),    //required
							SecondLine = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
							ThirdLine = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
							FourthLine = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
							City = reader.GetString(6),         //required
							County = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
							Country = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
							PostCode = reader.GetString(9),     //required
							IsDefault = reader.GetBoolean(10),  //required
						});
					}
				}
			}

			return profileManagementDTO;
		}
		finally
		{
			if (_connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
		}
	}

	public string DoesEmailExist(string email)
	{
		try
		{
			if (_connection.State == ConnectionState.Open)
			{
				_connection.Close();
			}

			_connection.Open();

			string query = @"select count(*) from Account where Email = @Email";

			using (IDbCommand command = _connection.CreateCommand())
			{
				command.CommandText = query;

				AddParameter(command, "@Email", email);

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
