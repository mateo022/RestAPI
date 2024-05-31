using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using RestAPIBackendWebService.Domain.User.Validators;
using RestAPIBackendWebService.Domain.Auth.Entities;
using RestAPIBackendWebService.Domain.Common.DTOs;
using RestAPIBackendWebService.Domain.Common.Errors;
using RestAPIBackendWebService.Services.Localization.Contract;
using RestAPIBackendWebService.Services.Localization.Logic;
using RestAPIBackendWebService.Services.Security.Contracts;
using RestAPIBackendWebService.Services.Security.Logic;
using System.Net;
using System.Text;
using RestAPIBackendWebService.Identity;
using RestAPIBackendWebService.DataAccess;
using RestAPIBackendWebService.Business.Logger.Logic;
using RestAPIBackendWebService.Business.Logger.Contracts;
using RestAPIBackendWebService.Business.Mailer.Contracts;
using RestAPIBackendWebService.Business.Mailer.Logic;
using RestAPIBackendWebService.Business.Auth.Contracts;
using RestAPIBackendWebService.DataAccess.APIs.Softipal.Auth.Logic;
using RestAPIBackendWebService.DataAccess.APIs.Softipal.Auth.Contracts;
using BostonOrderDeliveriesManagementAPI.Services.Mailing.Logic;
using RestAPIBackendWebService.Services.Logger.Contract;
using RestAPIBackendWebService.Services.Logger.Logic;
using RestAPIBackendWebService.Services.Mailing.Contract;
using O2CBackendWebService.Business.Role.Logic;
using RestAPIBackendWebService.Business.Role.Contracts;
using RestAPIBackendWebService.Business.User.Contracts;
using RestAPIBackendWebService.Business.User.Logic;

namespace RestAPIBackendWebService.Extensions
{
        public static class ServicesExtension
        {

            public const string DEVELOPMENT_CORS = "_developmentCorsPolicy";
            public const string COMMON_CORS = "_commonCorsPolicy";
            public const string STAGING_CORS = "_stageCorsPolicy";
            public static void RegisterDependencies(this IServiceCollection services)
            {
                #region SERVICES LAYER

            services.AddSingleton<IJwtService, JwtService>();
            services.AddSingleton<IPasswordHasherService, PasswordHasherService>();
            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<IMailerService, MailerService>();
            services.AddSingleton<ILanguageProviderStrategy, LanguageProviderStrategy>();

            #endregion

            #region BUSINESS LAYER
            services.AddScoped<IMailerBusiness, MailerBusiness>();
            services.AddScoped<IAuthBusiness, AuthBusiness>();
            services.AddScoped<ILoggerBusiness, LoggerBusiness>();
            services.AddScoped<IRoleBusiness, RoleBusiness>();
            services.AddScoped<IUserBusiness, UserBusiness>();
            services.AddSingleton<IJwtService, JwtService>();

            #endregion

            #region DATA ACCESS LAYER
            services.AddScoped<ITwoAuth, TwoAuth>();

            #endregion

        }
        public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            var userValidatorServiceDescriptor = new ServiceDescriptor(
                typeof(IUserValidator<CustomIdentityUser>),
                typeof(CustomIdentityUserValidator<CustomIdentityUser>),
                ServiceLifetime.Scoped);

            services.Replace(userValidatorServiceDescriptor);

            services.AddIdentityCore<CustomIdentityUser>(options =>
            {
                //Configure allowed characters for username
                options.User.AllowedUserNameCharacters = configuration["IdentityAllowdUserNameCharacters"];

                //Set unique rule for emails
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<CustomIdentityRole>()
            .AddRoleValidator<RoleValidator<CustomIdentityRole>>()
            .AddUserConfirmation<DefaultUserConfirmation<CustomIdentityUser>>()
            .AddSignInManager<SignInManager<CustomIdentityUser>>()
            .AddRoleManager<RoleManager<CustomIdentityRole>>()
            // Added custom error describer for change errors language
            .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
            .AddEntityFrameworkStores<RestAPIDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void ConfigureApiVersion(this IServiceCollection services)
            {
                services.AddApiVersioning(o =>
                {
                    o.AssumeDefaultVersionWhenUnspecified = true;
                    o.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
                    o.ReportApiVersions = true;
                });
            }

            public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
            {
                var jwtSettings = configuration.GetSection("JwtSettings");

                services.AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                }).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["validIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["tokensKey"]))
                    };
                });

                services.AddAuthorization();
            }

            public static void ConfigureCors(this IServiceCollection services)
            {
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });

                    options.AddPolicy(DEVELOPMENT_CORS, policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                });
            }


            public static void ConfigureResponseForInvalidModelsState(this IMvcBuilder apiBuilder)
            {
                apiBuilder.ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = (errorContext) =>
                    {
                        var errors = new RequestFieldsErrorsCollection<string>();

                        foreach (var propertyValue in errorContext.ModelState.Keys)
                        {
                            var valueErrors = new List<string>();

                            foreach (var error in errorContext.ModelState[propertyValue].Errors)
                            {
                                valueErrors.Add(error.ErrorMessage);
                            }

                            errors.AddErrorsForKey(propertyValue, valueErrors);
                        }

                        return new BadRequestObjectResult(new ErrorResponseDTO<object>
                        {
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Message = "Failed validations",
                            Errors = errors.Collection
                        });
                    };
                });
            }
        }
    }