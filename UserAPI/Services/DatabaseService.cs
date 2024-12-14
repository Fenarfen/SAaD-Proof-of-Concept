using UserAPI.Interfaces;
using Microsoft.Data.SqlClient;
using UserAPI.Models.Entities;
using UserAPI.Models.Dtos;
using Microsoft.Identity.Client;

namespace UserAPI.Services;

public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string CreateMemberUser(AccountCreateDto account)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = @"insert into Account output inserted.ID values (null, 1, @Email, @Password, @FirstName, @LastName, GETDATE(), 0)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", account.Email);
                command.Parameters.AddWithValue("@FirstName", account.FirstName);
                command.Parameters.AddWithValue("@LastName", account.LastName);
                command.Parameters.AddWithValue("@Password", account.Password);

                return Convert.ToString(command.ExecuteScalar()); // returns null if no record is created, otherwise will be the id of the record created
            }
        }
    }

    public string EditAccount(ProfileManagementDTO account)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = @"update Account
							 set Email = @Email,
							 FirstName = @FirstName,
							 LastName = @LastName
							 where ID = @ID";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", account.Email);
                command.Parameters.AddWithValue("@FirstName", account.FirstName);
                command.Parameters.AddWithValue("@LastName", account.LastName);
                command.Parameters.AddWithValue("@ID", account.ID);

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
    }

    public void SyncAddresses(List<Address> addresses)
    {
        int userID = addresses.First().AccountID;

        List<Address> currentAddresses = new List<Address>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = @"select * from Address where UserID = @UserID";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", userID);

                using (SqlDataReader reader = command.ExecuteReader())
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

    public string CreateAddress(Address address)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

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

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@AccountID", address.AccountID);       //required
                command.Parameters.AddWithValue("@FirstLine", address.FirstLine);       //required
                command.Parameters.AddWithValue("@SecondLine", address.SecondLine?? "");
                command.Parameters.AddWithValue("@ThirdLine", address.ThirdLine?? "");
                command.Parameters.AddWithValue("@FourthLine", address.FourthLine?? "");
                command.Parameters.AddWithValue("@City", address.City);                 //required
                command.Parameters.AddWithValue("@County", address.County ?? "");
                command.Parameters.AddWithValue("@Country", address.Country ?? "");
                command.Parameters.AddWithValue("@Postcode", address.PostCode);         //required
				command.Parameters.AddWithValue("@IsDefault", address.IsDefault);       //required

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

    public string UpdateAddress(Address address)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

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

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FirstLine", address.FirstLine);       //required
                command.Parameters.AddWithValue("@SecondLine", address.SecondLine ?? "");
                command.Parameters.AddWithValue("@ThirdLine", address.ThirdLine ?? "");
                command.Parameters.AddWithValue("@FourthLine", address.FourthLine ?? "");
                command.Parameters.AddWithValue("@City", address.City);                 //required
                command.Parameters.AddWithValue("@County", address.County ?? "");
                command.Parameters.AddWithValue("@Country", address.Country ?? "");
                command.Parameters.AddWithValue("@Postcode", address.PostCode);         //required
                command.Parameters.AddWithValue("@IsDefault", address.IsDefault);       //required
                command.Parameters.AddWithValue("@ID", address.ID);

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

    public string DeleteAddress(int addressID)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = @"delete from Address where ID = @ID";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ID", addressID);

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
    }

    public Account GetAccountByID(int accountID)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = @"select ID, TokenID, RoleID, Email, [Password], FirstName, LastName, Created, Verified from Account where ID = @ID";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("ID", accountID);

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

    public ProfileManagementDTO GetProfileManagementDTOfromToken(string token)
    {
        ProfileManagementDTO profileManagementDTO = null;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string accountQuery = @"select a.ID, r.Name, a.Email, a.FirstName, a.LastName, a.Created, a.Verified from Token t
									join Account a on t.UserID = a.ID
									join Role r on r.ID = a.RoleID
									where t.Value = @Token";

            string addressesQuery = @"select ad.* from Token t
									  join Account a on t.ID = a.TokenID
									  join Address ad on a.ID = ad.UserID
									  where t.Value = @Token";

            using (SqlCommand command = new SqlCommand(accountQuery, connection))
            {
                command.Parameters.AddWithValue("@Token", token);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
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
                    
                    if(profileManagementDTO == null)
                    {
                        return null;
                    }
                }
            }

            using (SqlCommand command = new SqlCommand(addressesQuery, connection))
            {
                command.Parameters.AddWithValue("@Token", token);

                using (SqlDataReader reader = command.ExecuteReader())
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
        }

        return profileManagementDTO;
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
