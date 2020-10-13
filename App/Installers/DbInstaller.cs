using AutoMapper;
using Puchase_and_payables.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Puchase_and_payables.DomainObjects.Auth;
using GODP.APIsContinuation.Repository.Inplimentation;
using GODP.APIsContinuation.Repository.Interface;
using Puchase_and_payables.Requests;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Repository.Supplier;
using Puchase_and_payables.Repository.Details;
using Puchase_and_payables.Repository.Invoice;
using GOSLibraries; 
using Support.SDK;

namespace Puchase_and_payables.Installers
{
    public class DbInstaller : IInstaller
    { 
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
                   options.UseSqlServer(
                       configuration.GetConnectionString("DefaultConnection")));
             
            services.AddScoped<IWorkflowDetailService, WorkflowDetailService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IPurchaseService, PurchaseService>(); 
            services.AddScoped<IOnWebSupplierService, OnWebSupplierService>();
            services.AddScoped<IIdentityServerRequest, IdentityServerRequest>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IFinanceServerRequest, FinanceServerRequest>();
            services.AddDefaultIdentity<ApplicationUser>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireNonAlphanumeric = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DataContext>();

            services.AddAutoMapper(typeof(Startup)); 
            services.AddMediatR(typeof(Startup));
            services.AddMvc();  

            

        }
    }
}
