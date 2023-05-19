# Bugtracker Server

REST API for the [bugtracker app](https://www.bugtracker.tk/).

This app was built using .NET Core 5, and MongoDB. Hosted on Heroku.

## Features

- JWT Authentication
- Role Based Authorization.
- Encrypted sensitive data.
- Project activity logs.
- In Memory Rate limiting.
- CORS.
- More than 20 endpoints.

## How to run locally

### To run this app you'll need the `dotnet CLI` and `docker`.

- Run `dotnet restore` to restore all the dependencies.
- Run `docker pull mongo` to pull the mongodb docker image.
- Run `docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:data/db -e MONGO_INITDB_ROOT_USERNAME=[YOUR_USERNAME_HERE] -e MONGO_INITDB_ROOT_PASSWORD=[YOUR_PASSWORD_HERE] mongo` and **replace the values between `[]`** to run the database instance with docker.
- Create a file called `appsettings.Development.json` in the root folder with the following content and **replace all the values surrounded by `[]` with your own**:

  ```json
  {
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "JwtConfig": {
      "Secret": "[YOUR_32_CHARACTERS_LONG_STRING_HERE]"
    },
    "MongoDbSettings": {
      "Env": "mongodb",
      "Host": "localhost",
      "Username": "[YOUR_USERNAME_HERE]",
      "Password": "[YOUR_PASSWORD_HERE]",
      "Port": ":27017"
    }
  }
  ```

- Update the line `70` in the [Startup.cs file](/Startup.cs) to allow your frontned to comunicate with the API.
- Run `dotnet run` to run the app. Now you can access all the endpoints at this url: [http://localhost:80](http://localhost:80).

## Documentation

You can see all the available documentation [here.](/docs/)

## Planed features

- Real time project updates using SignalIR.
- Add more functionality for project admins (removing members, join queue, assign issues to members).
