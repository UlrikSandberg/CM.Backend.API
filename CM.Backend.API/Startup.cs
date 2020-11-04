using System;
using System.Collections.Generic;
using System.IO;
using CM.Backend.API.Configuration;
using CM.Backend.API.EnumOptions;
using CM.Backend.API.Registry;
using CM.Backend.Persistence.Configuration;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using StructureMap;
using CM.Backend.API.Helpers;
using CM.Backend.API.Middleware;
using Swashbuckle.AspNetCore.Swagger;
using CM.Backend.EventHandlers.Configuration;
using CM.Backend.Eventstore.Persistence.Configuration;
using CM.Backend.Queries.Model;
using CM.Instrumentation;
using CorrelationId;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;

namespace CM.Backend.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;
        
        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            _configuration = config;
            _environment = env;
        }
        
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            SetupConfiguration(services);

            services.AddResponseCaching();
           
            SetupSecurity(services);

            services.AddMemoryCache();
            services.AddMvc();
            services.AddCors();

            SetupSwagger(services);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            var container = new Container(new WebRegistry());
            container.Configure(config => config.Populate(services));
            //container.AssertConfigurationIsValid();
            
            return container.GetInstance<IServiceProvider>();
        }

        private void SetupConfiguration(IServiceCollection services)
        {
            services.Configure<ProjectionsPersistenceConfiguration>(_configuration.GetSection(nameof(ProjectionsPersistenceConfiguration)));
            services.Configure<EventstoreConfiguration>(_configuration.GetSection(nameof(EventstoreConfiguration)));
            services.Configure<IdentityServerConfiguration>(_configuration.GetSection(nameof(IdentityServerConfiguration)));
            services.Configure<NotificationHubConfiguration>(_configuration.GetSection(nameof(NotificationHubConfiguration)));
            services.Configure<InstrumentationConfiguration>(_configuration.GetSection(nameof(InstrumentationConfiguration)));
            services.Configure<ServiceInfoConfiguration>(_configuration.GetSection(nameof(ServiceInfoConfiguration)));
            services.Configure<SendGridConfiguration>(_configuration.GetSection(nameof(SendGridConfiguration)));
            services.Configure<EventHandlerCMBackendUrlConfiguration>(_configuration.GetSection(nameof(AppConfiguration)));
            services.Configure<EventHandlerSendGridConfiguration>(_configuration.GetSection(nameof(SendGridConfiguration)));
            services.Configure<IPStackConfiguration>(_configuration.GetSection(nameof(IPStackConfiguration)));
            services.Configure<AppConfiguration>(_configuration.GetSection(nameof(AppConfiguration)));
            services.Configure<EmailAuthorityConfiguration>(_configuration.GetSection(nameof(EmailAuthorityConfiguration)));
        }
          
        private void SetupSwagger(IServiceCollection services)
        {
            var identityConfig = _configuration.GetSection(nameof(IdentityServerConfiguration)).Get<IdentityServerConfiguration>();
            
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Flow = "implicit", // just get token via browser (suitable for swagger SPA)
                    AuthorizationUrl = $"{identityConfig.ConnectionString}/connect/authorize",
                    TokenUrl = $"{identityConfig.ConnectionString}/connect/token",
                    Scopes = new Dictionary<string, string> {{"Backend.API", "Demo API - full access"}}
                    
                });
                
                c.AddSecurityDefinition("client-id", new ApiKeyScheme
                {
                    Name = "x-client-id",
                    In = "header",
                    Type = "apiKey"
                });
                
                
                c.AddSecurityDefinition("client-secret", new ApiKeyScheme
                {
                    Name = "x-client-secret",
                    In = "header",
                    Type = "apiKey"
                });
                
                c.SwaggerDoc("v1", new Info { Title = "Champagne Moments Cellar", Version = "v1", Description="Champagne Moments API for use with prior agreement" });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "CM.Backend.API.xml"); 
                c.IncludeXmlComments(xmlPath);     

                c.DescribeAllEnumsAsStrings();
                c.DocumentFilter<ApplyDocumentExtensions>();
                c.SchemaFilter<AddSchemaExamples>();
                c.OrderActionsBy(a => a.RelativePath);
                c.OperationFilter<AddFileParamTypesOperationFilter>();
                c.OperationFilter<AddHeaderParameters>();
                c.ApplySecurityDefinitionToMethods();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
            });

        }

        private void SetupSecurity(IServiceCollection services)
        {
            var identityConfig = _configuration.GetSection(nameof(IdentityServerConfiguration)).Get<IdentityServerConfiguration>();
            services
                .AddAuthentication(x =>
                {
                    //x.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                    x.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                    
                })
                .AddIdentityServerAuthentication(x =>
                {
                    x.Authority = identityConfig.ConnectionString; // auth server base endpoint (will use to search for disco doc)
                    x.ApiName = "Backend.API"; // required audience of access tokens
                    x.RequireHttpsMetadata = _environment.IsProduction();
                });
            services.AddAuthorization(x =>
            {
                x.AddPolicy(AuthorizationRoles.CMAdmin.ToString(), policy =>
                    policy.RequireClaim("CMAdminAccessClaim", AuthorizationRoles.CMAdmin.ToString()));
                x.AddPolicy(AuthorizationRoles.CMUser.ToString(), policy =>
                    policy.RequireClaim("CMUserAccessClaim", AuthorizationRoles.CMUser.ToString(), AuthorizationRoles.CMAdmin.ToString()));
            });
        }
        
        public void Configure(IApplicationBuilder app)
        {
            app.UseResponseCaching();
            app.UseDefaultCMLoggingMiddlewares();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("*", "location"));
            
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Champagne Moments Cellar v1");
                c.EnableFilter();
                c.EnableValidator();
                
                c.OAuthAppName("Backend.API - Swagger Auth"); // presentation purposes only
            });

            app.UseStatusCodePages();
            app.UseAuthentication();
            
           
            app.UseMvc();
            
            if(_environment.IsDevelopment() || _environment.IsEnvironment("QA"))
                app.UseDeveloperExceptionPage();
        }
    }
}
