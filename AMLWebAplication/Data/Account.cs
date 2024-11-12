namespace AMLWebAplication.Data
{
    public class Account
    {
        public int ID { get; set; }
        public int AddressID { get; set; }
        public int RoleID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public bool Verified { get; set; }
    }

}
