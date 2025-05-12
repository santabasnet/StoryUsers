using System.ComponentModel.DataAnnotations;

namespace StoryUserApi.Models
{
    public class StoryUser
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[!@#$%^&*(),.?""':{}|<>]).+$", 
            ErrorMessage = "Password must contain at least one special character.")]
        public string Password { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
