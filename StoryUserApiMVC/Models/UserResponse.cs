// Models/UserResponseDto.cs
namespace StoryUserApi.Models
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
