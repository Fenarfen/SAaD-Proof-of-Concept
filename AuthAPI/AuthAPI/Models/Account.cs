namespace AuthAPI.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int TokenID { get; set; }
        public int AddressID { get; set; }
        public int RoleID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Created { get; set; }
        public Boolean Verified { get; set; }
    }
}
