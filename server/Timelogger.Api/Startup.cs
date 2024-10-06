using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Timelogger.Entities;
using System;
using System.Collections.Generic;
using Timelogger.Api.Repositories;

namespace Timelogger.Api
{
	public class Startup
	{
		private readonly IWebHostEnvironment _environment;
		public IConfigurationRoot Configuration { get; }

		public Startup(IWebHostEnvironment env)
		{
			_environment = env;

			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("e-conomic interview"));
			services.AddLogging(builder =>
			{
				builder.AddConsole();
				builder.AddDebug();
			});

			services.AddMvc(options => options.EnableEndpointRouting = false);

			if (_environment.IsDevelopment())
			{
				services.AddCors();
			}

            services.AddScoped<IProjectsRepository, ProjectsRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseCors(builder => builder
					.AllowAnyMethod()
					.AllowAnyHeader()
					.SetIsOriginAllowed(origin => true)
					.AllowCredentials());
			}

			app.UseMvc();


			var serviceScopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
			using (var scope = serviceScopeFactory.CreateScope())
			{
				SeedDatabase(scope);
			}
		}

		private static void SeedDatabase(IServiceScope scope)
		{
			var context = scope.ServiceProvider.GetService<ApiContext>();
			var project1 = new Project
			{
				Id = 1,
				Name = "e-conomic Interview",
				Deadline = new DateTime(2024, 10, 8),
                TimeRegistrations = new List<TimeRegistration>
				{
					new TimeRegistration { 
						Id = 1, 
						TimeSpent = TimeDuration.TwoHours,
                        RegistrationDate = new DateTime(2024, 10 , 5)
                    },
					new TimeRegistration { 
						Id = 2,
                        TimeSpent = TimeDuration.ThirtyMinutes,
                        RegistrationDate = new DateTime(2024, 10 , 6)
                    }
                }
            };

            var project2 = new Project
            {
                Id = 2,
                Name = "mom's garage sale",
                Deadline = new DateTime(2023, 8, 10),
                TimeRegistrations = new List<TimeRegistration>
                {
                    new TimeRegistration { 
						Id = 3,
                        TimeSpent = TimeDuration.ThirtyMinutes,
						RegistrationDate = new DateTime(2023, 7 , 10)
					},
                    new TimeRegistration { 
						Id = 4,
                        TimeSpent = TimeDuration.ThreeHours,
                        RegistrationDate = new DateTime(2023, 7 , 28)
                    }
                }
            };

            var project3 = new Project
            {
                Id = 3,
                Name = "dad's garage sale",
                Deadline = new DateTime(2024, 12, 1)
            };

            var project4 = new Project
            {
                Id = 4,
                Name = "new tamagotchi project",
                Deadline = new DateTime(2024, 1, 18),
                TimeRegistrations = new List<TimeRegistration>
                {
                    new TimeRegistration {
                        Id = 5,
                        TimeSpent = TimeDuration.ThirtyMinutes,
                        RegistrationDate = new DateTime(2024, 1 , 17)
                    },
                    new TimeRegistration {
                        Id = 6,
                        TimeSpent = TimeDuration.ThreeHours,
                        RegistrationDate = new DateTime(2024, 1 , 17)
                    },
                    new TimeRegistration {
                        Id = 7,
                        TimeSpent = TimeDuration.ThreeHours,
                        RegistrationDate = new DateTime(2024, 1 , 10)
                    }
                }
            };

            context.Projects.Add(project1);
            context.Projects.Add(project2);
            context.Projects.Add(project3);
            context.Projects.Add(project4);

            context.SaveChanges();
		}
	}
}