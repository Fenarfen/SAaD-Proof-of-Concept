namespace AuthAPI.Models.Requests
{
    public class VerifyCodeRequest
    {
        public required string Email { get; set; }
        public required string Code { get; set; }
    }
}
