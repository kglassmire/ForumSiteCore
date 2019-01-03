# ForumSiteCore
.net core 2.1 attempt at forumsite with redis, postgresql, ef core code-first


This application is ready to roll with asp.net identity 2.0 on a postgresql db (created against a 9.6 instance)

I added in some of the cool tricks with this article to maintain snake_case: https://andrewlock.net/customising-asp-net-core-identity-ef-core-naming-conventions-for-postgresql/

Also altered the Startup, ApplicationDbContext , and the Account, ManageControllers classes so that the aspnetidentity features all utilize bigints as primary keys for 
better postgresql performance

*** OLD INSTRUCTIONS -- STOPPED WORKING BECAUSE MSFT CAN'T GET IT TOGETHER WITH DOTNET CORE TOOLING ***
-Consider updating npgsql dependencies in Nuget Package Manager
-run against a valid postgresql instance with cmd prompt open to web project directory: dotnet ef database update
-begin adding models to ApplicationDbContext as desired.
-run dotnet ef migrations add {MigrationName} as DbSet items are added to ApplicationDbContext class
-run dotnet ef database update as needed.

*** NEW INSTRUCTIONS ***
-cd into DAL directory (ForumSite.DAL)
-ALSO MAKE SURE THE DROPDOWN IS POINTED AT DAL PROJECT (ForumSiteCore.DAL)
-run PMC command for same thing as above (in this case -- Add-Migration {MigrationName})
-run PMC command to update db (Update-Database)