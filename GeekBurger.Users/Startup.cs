using AutoMapper;
using GeekBurger.Users.Extensions;
using GeekBurger.Users.Repository;
using GeekBurger.Users.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace GeekBurger.Users
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
   
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UsersDbContext>(o => o.UseInMemoryDatabase("GeekBurger.Users"));
           
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "GeekBurger.Users",
                    Description = "GeekBurguer Users Api"
                });
            });

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IServiceBusService, ServiceBusService>();

            var config = new MapperConfiguration(cfg =>
            {    
                cfg.CreateMap<GeekBurger.Users.Model.User, GeekBurger.Users.Contract.User>();
               
            });


            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            var mvcCoreBuilder = services.AddMvcCore();

            mvcCoreBuilder
            .AddFormatterMappings()            
            .AddCors();
            
        }
  
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UsersDbContext usersDbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           
            app.UseCors();

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint(@"/swagger/v1/swagger.json", "GeekBurguerUsers");
            });

            using (var serviceScope = app
                .ApplicationServices
                .GetService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<UsersDbContext>();
                context.Database.EnsureCreated();
            }

            usersDbContext.Seed();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("GeekBurger.Users running");
            });
        }
    
    }
}
