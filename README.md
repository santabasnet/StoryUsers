## StoryUsers
An interview question User CRUD operations with C# and REST API. It has user login feature with JWT token and refresh token generation.

# Instructions
1. Install the docker and docker-compose in your computer.
2. Open Termanal and run "docker compose up -d", to run the MSSQL server.
3. User dotnet setup commands in your machine.
4. Run the migrations :
   a. dotnet ef migrations add InitialCreate
   b. dotnet ef database update
5. Finally use "dotnet run" command to run ther StoryUser server.
