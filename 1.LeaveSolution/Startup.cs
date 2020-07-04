using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using LeaveSolution.BAL.ServiceInterface;
using LeaveSolution.BAL.ServiceClass;
using LeaveSolution.Models;
using Microsoft.AspNetCore.Identity;
using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Repository;
using LeaveSolution.DAL.Data;
using LeaveSolution.DAL.SMS_Mail;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using LeaveSolution.CustomFilter;
using Microsoft.AspNetCore.Http.Features;
using LeaveSolution.RESTAPI;

namespace LeaveSolution
{
    public class Startup
    {


        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appSettings.json").Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<MySettingsModel>(Configuration.GetSection("MySettings"));
            //cookies disabled
            //services.AddAuthentication(options => { }).AddCookie(opts => { opts.Cookie.HttpOnly = false; });
            services.AddAuthentication(options => { }).AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            //commenting for localhost
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.Secure = CookieSecurePolicy.Always;
            });
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
            //services.AddAntiforgery(options =>
            //{
            //    // Set Cookie properties using CookieBuilder properties†.
            //    options.FormFieldName = "BCL";
            //    options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
            //    options.SuppressXFrameOptionsHeader = false;
            //});
            Mapper.Initialize(cfg =>
            {

                //login
                cfg.CreateMap<LoginViewModel, LoginViewServiceModel>().ReverseMap();
                cfg.CreateMap<LoginViewServiceModel, tbl_OTPG>().ReverseMap();
                cfg.CreateMap<LoginViewServiceModel, LoginViewModel>().ReverseMap();
                cfg.CreateMap<tbl_OTPG, LoginViewServiceModel>().ReverseMap();
                cfg.CreateMap<EmployeeServicemodel, Zftm0008Response>().ReverseMap();
                cfg.CreateMap<Zftm0008Response, EmployeeServicemodel>().ReverseMap();
                cfg.CreateMap<Zftm0008Tabservice, Zftm0008Response>().ReverseMap();
                cfg.CreateMap<Zftm0008Response, Zftm0008Tabservice>().ReverseMap();
                cfg.CreateMap<LoginViewServiceModel, tbl_Employee>().ReverseMap();
                cfg.CreateMap<tbl_Employee, LoginViewServiceModel>().ReverseMap();

                cfg.CreateMap<LeaveRequestViewModel, LeaveRequestViewtServiceModel>().ReverseMap();
                cfg.CreateMap<DeptLeaveRequestViewModel, DeptLeaveRequestViewtServiceModel>().ReverseMap();
                cfg.CreateMap<DeptLeaveRequestViewtServiceModel, DeptLeaveRequestViewModel>().ReverseMap();
                cfg.CreateMap<DeptLeaveRequestViewtServiceModel, tbl_DeptLeaveRequest>().ReverseMap();
                cfg.CreateMap<tbl_DeptLeaveRequest, DeptLeaveRequestViewtServiceModel>().ReverseMap();
                cfg.CreateMap<LeaveApprovalModel, LeaveApprovalServiceViewModel>().ReverseMap();
                cfg.CreateMap<LeaveApprovalServiceViewModel, tbl_LeaveApproval>().ReverseMap();
                cfg.CreateMap<tbl_LeaveApproval, LeaveApprovalServiceViewModel>().ReverseMap();
                cfg.CreateMap<tbl_LeaveApproval, LeaveApprovalServiceModel>().ReverseMap();
                cfg.CreateMap<LeaveApprovalServiceModel, tbl_LeaveApproval>().ReverseMap();
                cfg.CreateMap<LeaveApprovalServiceViewModel, LeaveApprovalModel>().ReverseMap();
                cfg.CreateMap<LeaveApprovalServiceViewModel, LeaveApprovalModelViewModel>().ReverseMap();
                cfg.CreateMap<LeaveApprovalModelViewModel, LeaveApprovalServiceModel>().ReverseMap();
                cfg.CreateMap<LeaveApprovalModel, LeaveApprovalServiceViewModel>().ReverseMap();
                cfg.CreateMap<LeaveApprovalModel, LeaveApprovalServiceModel>().ReverseMap();
                cfg.CreateMap<LeaveApprovalServiceModel, LeaveApprovalModel>().ReverseMap();
                //For LeaveQuota
                cfg.CreateMap<LeaveQuotaModel, LeaveQuotaServiceViewModel>().ReverseMap();
                cfg.CreateMap<LeaveQuotaServiceViewModel, tbl_LeaveQuota>().ReverseMap();
                cfg.CreateMap<tbl_LeaveQuota, LeaveQuotaServiceViewModel>().ReverseMap();
                cfg.CreateMap<tbl_LeaveQuota, LeaveQuotaServiceModel>().ReverseMap();
                cfg.CreateMap<tbl_LeaveQuotaSAP, QuotaServiceModel>().ReverseMap();
                cfg.CreateMap<QuotaServiceModel, tbl_LeaveQuotaSAP>().ReverseMap();

                cfg.CreateMap<LeaveQuotaServiceViewModel, LeaveQuotaModel>().ReverseMap();
                cfg.CreateMap<LeaveQuotaServiceViewModel, LeaveQuotaModelViewModel>().ReverseMap();
                cfg.CreateMap<LeaveQuotaModel, LeaveQuotaServiceViewModel>().ReverseMap();
                cfg.CreateMap<LeaveQuotaModel, LeaveQuotaServiceModel>().ReverseMap();
                //For Leavehistory

                cfg.CreateMap<LeaveHistoryModelViewModel, LeaveHistoryServiceViewModel>().ReverseMap();
                cfg.CreateMap<LeaveHistoryServiceViewModel, tbl_LeaveHistory>().ReverseMap();
                cfg.CreateMap<tbl_LeaveHistory, LeaveHistoryServiceViewModel>().ReverseMap();
                cfg.CreateMap<tbl_LeaveHistory, LeaveHistoryServiceViewModel>().ReverseMap();
                cfg.CreateMap<LeaveHistoryServiceViewModel, LeaveHistoryModelViewModel>().ReverseMap();
                cfg.CreateMap<LeaveHistoryServiceViewModel, LeaveHistoryModelViewModel>().ReverseMap();
                cfg.CreateMap<LeaveHistoryModelViewModel, LeaveHistoryServiceViewModel>().ReverseMap();
                cfg.CreateMap<LeaveHistoryModelViewModel, LeaveHistoryServiceViewModel>().ReverseMap();
                //menu
                cfg.CreateMap<MenuModel, MenuServiceModel>().ReverseMap();
                cfg.CreateMap<tbl_MenuMaster, MenuServiceModel>().ReverseMap();
                cfg.CreateMap<MenuServiceModel, MenuModel>().ReverseMap();
                cfg.CreateMap<MenuServiceModel, tbl_MenuMaster>().ReverseMap();
                //Leave Dashboard
                cfg.CreateMap<LeaveDashboardModel, LeaveDashboardServiceViewModel>().ReverseMap();
                cfg.CreateMap<LeaveDashboardServiceViewModel, tbl_LeaveDashboard>().ReverseMap();
                cfg.CreateMap<tbl_LeaveDashboard, LeaveDashboardServiceViewModel>().ReverseMap();
                cfg.CreateMap<tbl_LeaveDashboard, LeaveDashboardServiceModel>().ReverseMap();
                cfg.CreateMap<LeaveDashboardServiceViewModel, LeaveDashboardModel>().ReverseMap();
                cfg.CreateMap<LeaveDashboardServiceViewModel, LeaveDashboardModelViewModel>().ReverseMap();
                cfg.CreateMap<LeaveDashboardModel, LeaveDashboardServiceViewModel>().ReverseMap();
                cfg.CreateMap<LeaveDashboardModel, LeaveDashboardServiceModel>().ReverseMap();
                //Leave Withdraw
                cfg.CreateMap<LeaveWithdrawalModel, LeaveWithdrawalServiceViewModel>().ReverseMap();
                cfg.CreateMap<LeaveWithdrawalServiceViewModel, tbl_Leavewithdrawal>().ReverseMap();
                cfg.CreateMap<tbl_Leavewithdrawal, LeaveWithdrawalServiceViewModel>().ReverseMap();
                cfg.CreateMap<tbl_Leavewithdrawal, LeaveWithdrawalServiceModel>().ReverseMap();
                cfg.CreateMap<LeaveWithdrawalServiceModel, tbl_Leavewithdrawal>().ReverseMap();
                cfg.CreateMap<LeaveWithdrawalServiceViewModel, LeaveWithdrawalModel>().ReverseMap();
                cfg.CreateMap<LeaveWithdrawalServiceViewModel, LeaveWithdrawalViewModel>().ReverseMap();
                cfg.CreateMap<LeaveWithdrawalViewModel, LeaveWithdrawalServiceModel>().ReverseMap();
                cfg.CreateMap<LeaveWithdrawalModel, LeaveWithdrawalServiceViewModel>().ReverseMap();
                cfg.CreateMap<LeaveWithdrawalModel, LeaveWithdrawalServiceModel>().ReverseMap();
                cfg.CreateMap<LeaveWithdrawalServiceModel, LeaveWithdrawalModel>().ReverseMap();
                cfg.CreateMap<tbl_LeaveRequest, LeaveRequestViewtServiceModel>().ReverseMap();

            });

            //Dependency Injection

            services.AddScoped<ILoginViewService, GnerateOTPService>();
            services.AddScoped<IGenerateOTPRepositary, OTPGenerateRepositary>();
            services.AddScoped<ISMSUtility, SMSUtilityRepositary>();
            services.AddScoped<ILeaveRequestViewService, LeaveRequestService>();
            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();

            services.AddScoped<IDeptLeaveRequestViewService, DeptLeaveRequestService>();
            services.AddScoped<IDeptLeaveRequestRepository, DeptLeaveRequestRepository>();

            services.AddScoped<IMenuServices, MenuServices>();
            services.AddScoped<IMenumasterRepositay, MenuRepositary>();
            services.AddScoped<ILeaveApprovalService, LeaveApprovalService>();
            services.AddScoped<ILeaveApprovalRepositary, LeaveApprovalRepositary>();

            services.AddScoped<ILeaveQuotaService, LeaveQuotaService>();
            services.AddScoped<ILeaveQuotaRepository, LeaveQuotaRepository>();

            services.AddScoped<ILeaveHistoryService, LeaveHistoryService>();
            services.AddScoped<ILeaveHistoryRepository, LeaveHistoryRepository>();

            services.AddScoped<ILeaveDashboardService, LeaveDashboardService>();
            services.AddScoped<ILeaveDashboardRepositary, LeaveDashboardRepository>();

            services.AddScoped<ILeaveWithdrawalService, LeaveWithdrawalService>();
            services.AddScoped<ILeaveWithdrawalRepository, LeaveWithdrawalRepositary>();


            services.AddScoped<Authentication>();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);//You can set Time   
                options.Cookie.IsEssential = true;
            });
            //for form authentication
            services.AddHttpContextAccessor();

            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("DefaultNoCacheProfile", new CacheProfile
                {
                    NoStore = true,
                    Location = ResponseCacheLocation.None
                });
                options.Filters.Add(new ResponseCacheAttribute
                {
                    CacheProfileName = "DefaultNoCacheProfile"
                });
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddSingleton(_ => Configuration);
            services.AddSingleton<IConfiguration>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
            //}
            //xss protection
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();
            app.UseCookiePolicy();
            //new add
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            //app.UseAuthentication();
            app.UseExceptionHandler("/Home/Error");
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=LoginHome}/{id?}");
            });
            //ConnectionString = Configuration["ConnectionStrings:DefaultConnection"];
        }
    }
}
