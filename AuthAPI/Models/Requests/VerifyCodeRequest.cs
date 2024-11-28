namespace AuthAPI.Models.Requests
{
    public class VerifyCodeRequest
    {
        public int id { get; set; }
        public required string Code { get; set; }
    }
}
