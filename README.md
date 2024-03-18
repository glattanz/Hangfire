# Hangfire

ASP .NET Core Web API

Commands used:
	Adding SQLClient to the project: 
		dotnet add package Microsoft.Data.SqlClient
	Adding Hangfire framework (Package Manager Console): 
		NuGet\Install-Package Hangfire.Core -Version 1.8.11
		Install-Package Hangfire.SqlServer -Version 1.8.11
		Install-Package Hangfire.AspNetCore -Version 1.8.11

	**Verify why install each of those

Following this reference: https://docs.hangfire.io/en/latest/getting-started/aspnet-core-applications.html

	Creating the database connection via cmd:
		sqllocaldb create "Hangfire"

	Creating the database via query:
		CREATE DATABASE Hangfire