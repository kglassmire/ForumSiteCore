# ForumSiteCore
## Purpose
The goal of this web project is to create a clone of a fairly data intensive application that I'm familiar with (Reddit / Voat) and duplicate it with an ASP.NET Core stack.

## Reasons
I don't generally get to work with open-source technologies in my day-to-day. 

I come from a SQL Server environment. I'd like to know more about other database platforms and if they could work for me in future personal projects and employment opportunities. Hence why I chose PostgreSQL. 

I wanted to try a front-end technology that doesn't force me into making a single page application. Knockout.js used to be the go-to for this time of implementation but it is fairly outdated. Vue.js seems to fit the bill.

I have very little experience with webpack and I wanted to configure a project with it from scratch so I understood it better.

I want to explore application data caching mechanisms with distributed memory cache with Redis.

## Projects
Inside of this ASP.NET solution there are multiple projects
* **ForumSiteCore.Web** - ASP.NET Core MVC app. Also some API controllers moved over from ForumSiteCore.API for simplicity
* **ForumSiteCore.DAL** - .NET Core class library housing Entity Framework Core
* **ForumSiteCore.Utility** - .NET Core class library -- with, you guessed it, utilities
* **ForumSiteCore.Business** - .NET Core class library w/ business logic 
* ~~**ForumSiteCore.API** - ASP.NET Core MVC / API class library -- was using this for the Angular project but I'm going to just house those controllers in the Web project since they can be combined now.~~

## Technologies
This app is intended to be a high performance app. It also means I'm going to be working with a lot of technologies I don't really encounter in my day to day.

Some of these include:
1. .NET Core
2. ASP.NET Core
3. Vue.js 
4. webpack
5. Serilog (logging library)
6. Entity Framework Core (code-first with migrations)
7. PostgreSQL (using Npgsql wrapper for Entity Framework Core)
8. CacheManager (Abstraction layer for various cache providers)
9. FluentValidation
10. AutoMapper
