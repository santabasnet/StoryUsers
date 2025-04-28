using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace StoryUsers.Model;

public class StoryUser
{

    [Required]
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public String Email { get; set; }

    [Required]
    [MinLength(8)]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, contain at least one letter, one number, and one special character.")]
    public String Password { get; set; }
    public string Name { get; set; } = "";

    public String ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

/*record StoryUser(int Id, string Email, string Password, string FullName, string Address, string? Interests)
{
}*/

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

