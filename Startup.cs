using System.Text;
using System.Text.Json;
using System.Linq;
using System.Net.Mime;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using bugtracker.Config;
using bugtracker.Lib.Jwt;
using Microsoft.Extensions.FileProviders;
using System.IO;
using bugtracker.Repositories;

namespace bugtracker {
	public class Startup{
    public Startup(IConfiguration configuration, IWebHostEnvironment enviroment)
    {
      Configuration = configuration;
      _enviroment = enviroment;
    }

    public IConfiguration Configuration { get; }
    private IWebHostEnvironment _enviroment;

    //CORS POLICY NAME
    private readonly string _policyName = "AllowSpecificOrigins";
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      //Serializers
      BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
      BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

      //Getting the settings from the appsettings file.
      var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
      var jwtConfig = Configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>();

      //Adding a new instance of the MongoClient with the connection string.
      services.AddSingleton<IMongoClient>(servicesProvider => {
        var settings = MongoClientSettings.FromConnectionString(mongoDbSettings.ConnectionString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        return new MongoClient(settings);
      });

      //Repositories
      services.AddSingleton<IUserRepo, UserRepo>();
      services.AddSingleton<IProjectRepo, ProjectRepo>();
      services.AddSingleton<IBugRepo, BugRepo>();
      services.AddSingleton<ITaskRepo, TaskRepo>();
      services.AddSingleton<IAuthRepo, AuthRepo>();
      //Utils
      services.AddSingleton<IJwtUtils, JwtUtils>();

      //CORS
      services.AddCors(options => {
        options.AddPolicy(_policyName, policy => {
					if (_enviroment.IsDevelopment()) {
            policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
            return;
					}
          policy.WithOrigins("https://www.bugtracker.tk")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
      });

      //Authentication
      services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(jwt => {
        var key = Encoding.ASCII.GetBytes(jwtConfig.Secret);
        jwt.RequireHttpsMetadata = false;
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = new TokenValidationParameters {

          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ClockSkew = TimeSpan.Zero,
          ValidateIssuer = false,
          ValidateAudience = false,
        };
      });

      services.AddControllers(options => {
        options.SuppressAsyncSuffixInActionNames = false;
      });

      services.AddSwaggerGen(c =>
      {
          c.SwaggerDoc("v1", new OpenApiInfo { Title = "bugtracker", Version = "v1" });
      });

      services.AddHealthChecks().AddMongoDb(mongoDbSettings.ConnectionString,
        name: "MongoDb",
        timeout: TimeSpan.FromSeconds(3),
        tags: new[] { "ready" });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "bugtracker v1"));
      }

			if (!env.IsDevelopment()) {
        app.UseHttpsRedirection();
      }

      app.UseRouting();

      //MORE CORS
      app.UseCors(_policyName);

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();

        endpoints.MapHealthChecks("/api/v1/health/ready", new HealthCheckOptions {
          Predicate = (check) => check.Tags.Contains("ready"),
          ResponseWriter = async (context, report) => {
            var result = JsonSerializer.Serialize(new {
              status = report.Status.ToString(),
              checks = report.Entries.Select((entry) => new {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
                duration = entry.Value.Duration.ToString()
              })
            });
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(result);
          }
        });

        endpoints.MapHealthChecks("/api/v1/health/live", new HealthCheckOptions { Predicate = (_) => false });
      });
    }
  }
}
