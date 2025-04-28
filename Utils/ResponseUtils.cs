using System.Text.RegularExpressions;

namespace StoryUsers.Utils;


public class ResponseUtils
{

    public static String notFound(int Id)
    {
        return $"User with ID {Id} not found.";
    }

    // Function to validate password
    public static bool isValidPassword(string password)
    {
        // Regular expression to enforce minimum 8 characters, at least one letter, one number, and one special character
        string pattern = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

        // Create a Regex object with the pattern
        Regex regex = new Regex(pattern);

        // Check if the password matches the regular expression
        return regex.IsMatch(password);
    }

}