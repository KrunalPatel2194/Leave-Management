using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Microsoft.Extensions.Hosting;
using System.Collections.Specialized;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using LeaveSolution.BAL.ServiceInterface;
using LeaveSolution.BAL.ServiceClass;
using LeaveSolution.Models;
using AutoMapper;
using LeaveSolution.DAL.Repository;
using LeaveSolution.BAL.ServiceModels;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using LeaveSolution.DAL.Data;

namespace LeaveSolution
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateWebHostBuilder(args).UseKestrel(o =>
                {
                    o.AddServerHeader = false;
                    o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(20);
                }).Build().Run();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
            //CreateWebHostBuilder(args).Build().Run();           
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)        
                .UseStartup<Startup>()
            .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Error);
                    logging.AddFilter("Microsoft", LogLevel.Error);
                    logging.AddFilter("System", LogLevel.Error);
                    logging.AddFilter("Engine", LogLevel.Error);                    
                })
                .UseNLog();  // NLog: setup NLog for Dependency injection
    }
}
