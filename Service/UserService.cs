using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using StoryUsers.Model;

namespace StoryUsers.UserService;

public class DemoUser
{

    private static List<StoryUser> userList = new List<StoryUser>{
        new StoryUser{Id = 1, Email = "s@gmail.com", Password = "$%12aRRRR", Name = "Ram Kumar"},
        new StoryUser{Id = 2, Email = "r@gmail.com", Password = "#%12bSSSS", Name = "Krishna Bdr"}
    };

    public static List<StoryUser> list()
    {
        return userList;
    }


}