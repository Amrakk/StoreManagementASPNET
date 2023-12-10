namespace StoreManagementAPI.Models.RequestSchemas
{
    public class CreateUserRequest
    {
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
