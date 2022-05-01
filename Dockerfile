FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["bugtracker.csproj", "./"]
RUN dotnet restore "bugtracker.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "bugtracker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bugtracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "bugtracker.dll"]
