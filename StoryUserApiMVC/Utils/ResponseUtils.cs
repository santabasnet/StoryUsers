namespace StoryUserApi.Utils;

public static class ResponseUtils
{

    public static dynamic invalidEmailOrPassword(){
        return new { message = "Invalid email or password" };
    }

    public static dynamic emailAlreadyExists(){
        return new { message = "Email already exists." };
    }

}