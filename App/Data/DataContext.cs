using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GODP.APIsContinuation.DomainObjects.Supplier;
using Puchase_and_payables.DomainObjects.Supplier;
using Puchase_and_payables.DomainObjects.Approvals;
using Puchase_and_payables.DomainObjects.Purchase;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Puchase_and_payables.DomainObjects.Auth;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using System.Threading.Tasks;
using System.Threading;
using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using Puchase_and_payables.Requests;
using Puchase_and_payables.DomainObjects.Invoice;

namespace Puchase_and_payables.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext() { }
        private readonly IIdentityServerRequest _serverRequest; 
        public DataContext(DbContextOptions<DataContext> options, 
            IIdentityServerRequest serverRequest  ) : base(options) 
        {
            _serverRequest = serverRequest; 
        }

        public DbSet<inv_invoice> inv_invoice { get; set; }
        public DbSet<cor_identification> cor_identification { get; set; }
        public DbSet<purch_invoice> purch_invoice { get; set; }
        public DbSet<cor_paymentterms> cor_paymentterms { get; set; }
        public DbSet<cor_bid_and_tender> cor_bid_and_tender { get; set; }
        public DbSet<cor_financialdetail> cor_financialdetail { get; set; }
        public DbSet<cor_bankaccountdetail>  cor_bankaccountdetail { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<purch_plpo> purch_plpo { get; set; }
        public DbSet<purch_plpodetails> purch_plpodetails { get; set; }
        public DbSet<purch_prndetails> purch_prndetails { get; set; }
        public DbSet<purch_requisitionnote> purch_requisitionnote { get; set; }
        public DbSet<cor_supplier> cor_supplier { get; set; }
        public DbSet<cor_approvaldetail> cor_approvaldetail { get; set; }
        public DbSet<cor_supplierauthorization> cor_supplierauthorization { get; set; }
        public DbSet<cor_supplierbusinessowner> cor_supplierbusinessowner { get; set; }
        public DbSet<cor_supplierdocument> cor_supplierdocument { get; set; }
        public DbSet<cor_suppliertype> cor_suppliertype { get; set; }
        public DbSet<cor_topclient> cor_topclient { get; set; }
        public DbSet<cor_topsupplier> cor_topsupplier { get; set; }
        public DbSet<cor_taxsetup> cor_taxsetup { get; set; }
        public DbSet<cor_serviceterms> cor_serviceterms { get; set; }
        public DbSet<ConfirmEmailCode> ConfirmEmailCodes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            
            IConfigurationRoot config = builder.Build();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var user = _serverRequest.UserDataAsync().Result;
            if (user.StaffId < 0)
            {
                user.UserName = "Clientuser";
            }
            foreach (var entry in ChangeTracker.Entries<GeneralEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Active = true;
                    entry.Entity.Deleted = false;
                    entry.Entity.Active = false;
                    entry.Entity.CreatedBy = user.UserName;
                    entry.Entity.CreatedOn = DateTime.Now;
                }
                else
                {
                    entry.Entity.UpdatedOn = DateTime.Now;
                    entry.Entity.UpdatedBy = user.UserName;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

    }
}