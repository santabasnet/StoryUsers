using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StoryUsers.Data;
using StoryUsers.Model;
using StoryUsers.UserService;
using StoryUsers.Utils;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using StoryUsers.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

// JWT Authentication Configuration
var key = Encoding.ASCII.GetBytes("bf335d5b-9979-4699-a443-2ff42de9dc69");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = JwtHelper.Issuer,
        ValidateAudience = true,
        ValidAudience = JwtHelper.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtHelper.JwtSecret)),
        ValidateLifetime = true
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


/* Ping Server Health Endpoint */
app.MapGet("/", () => "Hello from Server >> " + DateTime.Now.ToString());

/* Get User by Id */
/*app.MapGet("/api/users/{id}", (int id) =>
{
    var storyUser = DemoUser.list().Find(user => user.Id == id);
    if (storyUser == null) return Results.NotFound(ResponseUtils.notFound(id));
    else return Results.Ok(storyUser.ToString());
});*/

// GET: /api/users/{id} (Find user by ID)
app.MapGet("/api/users/{id}", async (AppDbContext db, int id) =>
{
    var user = await db.Users.FindAsync(id);
    if (user == null)
    {
        return Results.NotFound(ResponseUtils.notFound(id));
    }
    return Results.Ok(user);
}).RequireAuthorization();

/* List all Users */
/*app.MapGet("/api/users", () =>
{
    return Results.Ok(JsonSerializer.Serialize(DemoUser.list()));
});*/

// GET: /api/users (List all users)
app.MapGet("/api/users", async (AppDbContext db) =>
{
    var users = await db.Users.ToListAsync();
    return Results.Ok(users);  // Return all users with HTTP 200 OK
});

// POST: /api/users/register
app.MapPost("/api/users/register", async (AppDbContext db, StoryUser user) =>
{
    // Validate the input
    if (user == null)
    {
        return Results.BadRequest("User data is required.");
    }

    // Invalid Password
    if (!ResponseUtils.isValidPassword(user.Password))
    {
        return Results.BadRequest("Password must be at least 8 characters long, contain at least one letter, one number, and one special character.");
    }

    // Check if the email is already taken
    var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
    if (existingUser != null)
    {
        return Results.Conflict("Email is already registered.");
    }

    // Add the new user to the Users table
    db.Users.Add(user);
    await db.SaveChangesAsync();

    // Return a response with the created user (HTTP 201 Created)
    return Results.Created($"/api/users/{user.Id}", user);
});

// POST: /api/auth/login (Login and generate JWT)
app.MapPost("/api/auth/login", async (AppDbContext db, StoryUser loginUser) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email && u.Password == loginUser.Password);
    if (user == null) return Results.Unauthorized();
    var accessToken = JwtHelper.GenerateAccessToken(user, minutes: 15);
    var refreshToken = JwtHelper.GenerateAccessToken(user, minutes: 60 * 24 * 7); // Valid for one week.
    return Results.Ok(new
    {
        AccessToken = accessToken,
        RefreshToken = refreshToken
    });
});

// POST: /api/auth/refresh-token (Refresh expired access token)
// === REFRESH TOKEN API ===
app.MapPost("/api/auth/refresh-token", async (AppDbContext db, HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var refreshToken = await reader.ReadToEndAsync();
    refreshToken = refreshToken.Trim();

    if (string.IsNullOrEmpty(refreshToken)) return Results.BadRequest("Refresh token is required.");

    var principal = JwtHelper.ValidateToken(refreshToken);

    if (principal == null) return Results.Unauthorized();

    var email = principal.FindFirst("nameid")?.Value;
    if (email == null) return Results.Unauthorized();

    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null) return Results.Unauthorized();

    var newAccessToken = JwtHelper.GenerateAccessToken(user, minutes: 15);
    return Results.Ok(new { AccessToken = newAccessToken });
});

/* Update User.*/
app.MapPut("/api/users/{id:int}", async (AppDbContext db, int id, StoryUser updatedUser) =>
{
    var user = await db.Users.FindAsync(id);
    if (user == null)
    {
        return Results.NotFound();
    }

    user.Name = updatedUser.Name;
    await db.SaveChangesAsync();
    return Results.Ok(ResponseUtils.userUpdated(id));
});

/* Delete Story User by Id.*/
app.MapDelete("/api/users/{id:int}", async (AppDbContext db, int id) =>
{
    var user = await db.Users.FindAsync(id);
    if (user == null)
    {
        return Results.NotFound();
    }

    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = $"User with id {id} deleted successfully." });
});


// Configure the HTTP request pipeline.
app.Run();


