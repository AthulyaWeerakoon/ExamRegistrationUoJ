# Exam Registration System for UoJ

## Introduction

This is an Exam registration system made specifically for the Engineering Faculty of University of Jaffna with the business logic suited and optimized for it.

![UoJ Logo](ExamRegistrationUoJ/wwwroot/favicon.png)

## Requirements

ASP .NET and Blazor .NET with,

- Microsoft.AspNetCore.Authentication.MicrosoftAccount v8.0.4
- MySqlConnector v2.3.7
- Newtonsoft.Json v13.0.3

## Intallation

### Option 1 - Install Visual Studio > 2022

Install Visual Studio v > 2022 and with it .NET MAUI development utilities that contain tools for Blazor development.

If done through Visual Studio the required plugins should be installed through NuGet package manager.

### Option 2 - Install dotNET

[Download](https://dotnet.microsoft.com/en-us/download/dotnet-framework), Install .NET Framework and install the required packages into it / or into an IDE of your choice.

## Configuration

- Configure a MySQL Server and add its information (user, password, schema) into the DBMySQL1.cs file.
- Initialize the MySQL server by running [init.sql](Database/init.sql) script.
- Create an account by adding Microsoft account details into the account table of the MySQL server.
- Make it an admin account by linking that entry to the administrator table.
