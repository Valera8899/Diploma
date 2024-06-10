using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Monitor_2.Models;
using Monitor_2.Models.Currency;
using Monitor_2.Models.Shopping;

namespace Monitor_2.Data
{
    public class Monitor_2Context : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public Monitor_2Context(DbContextOptions<Monitor_2Context> options)
            : base(options)
        {
        }

        public DbSet<CurrencyPair> CurrencyPair { get; set; } = default!;
        public DbSet<CurrentCpValue> CurrentCpValue { get; set; } = default!;
        public DbSet<ExchangeCompany> ExchangeCompany { get; set; } = default!;
        public DbSet<ExCompany_CurrencyPair> ExCompany_CurrencyPair { get; set; } = default!;
        public DbSet<Search> Searches { get; set; } = default!;
        public DbSet<SearchResult> SearchResults { get; set; } = default!;
        public DbSet<Lot> Lots { get; set; } = default!;
        public DbSet<Marketplace> Marketplaces { get; set; } = default!;
        public DbSet<User_Search> User_Searches { get; set; } = default!;
        public DbSet<User_Lot> User_Lots { get; set; } = default!;
        public DbSet<SearchResult_Lot> SearchResult_Lots { get; set; } = default!;
        public DbSet<KeyWord> KeyWords { get; set; } = default!;
        public DbSet<Address> Addresses { get; set; } = default!;
        public DbSet<CurrentPriceValue> CurrentPriceValues { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExCompany_CurrencyPair>()
                .HasKey(ecp => new { ecp.ExchangeCompanyId, ecp.CurrencyPairId });

            modelBuilder.Entity<ExCompany_CurrencyPair>()
                .HasOne(ecp => ecp.ExchangeCompany)
                .WithMany(ec => ec.ExCompany_CurrencyPairs)
                .HasForeignKey(ecp => ecp.ExchangeCompanyId);

            modelBuilder.Entity<ExCompany_CurrencyPair>()
                .HasOne(ecp => ecp.CurrencyPair)
                .WithMany(cp => cp.ExCompany_CurrencyPairs)
                .HasForeignKey(ecp => ecp.CurrencyPairId);

            int precision = 5;

            modelBuilder.Entity<CurrentCpValue>()
                .Property(c => c.BuyRate)
                .HasPrecision(18, precision);

            modelBuilder.Entity<CurrentCpValue>()
                .Property(c => c.SellRate)
                .HasPrecision(18, precision);

            modelBuilder.Entity<User_Search>()
                .HasKey(us => new { us.UserId, us.SearchId });

            modelBuilder.Entity<User_Search>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSearches)
                .HasForeignKey(us => us.UserId);

            modelBuilder.Entity<User_Search>()
                .HasOne(us => us.Search)
                .WithMany(s => s.UserSearches)
                .HasForeignKey(us => us.SearchId);

            modelBuilder.Entity<User_Lot>()
                .HasKey(ul => new { ul.UserId, ul.LotId });

            modelBuilder.Entity<User_Lot>()
                .HasOne(ul => ul.User)
                .WithMany(u => u.UserLots)
                .HasForeignKey(ul => ul.UserId);

            modelBuilder.Entity<User_Lot>()
                .HasOne(ul => ul.Lot)
                .WithMany(l => l.UserLots)
                .HasForeignKey(ul => ul.LotId);

            modelBuilder.Entity<SearchResult_Lot>()
                .HasKey(srl => new { srl.SearchResultId, srl.LotId });

            modelBuilder.Entity<SearchResult_Lot>()
                .HasOne(srl => srl.SearchResult)
                .WithMany(sr => sr.SearchResultLots)
                .HasForeignKey(srl => srl.SearchResultId);

            modelBuilder.Entity<SearchResult_Lot>()
                .HasOne(srl => srl.Lot)
                .WithMany(l => l.SearchResultLots)
                .HasForeignKey(srl => srl.LotId);

            modelBuilder.Entity<Search>()
                .HasMany(s => s.SearchResults)
                .WithOne(sr => sr.Search)
                .HasForeignKey(sr => sr.SearchId);

            modelBuilder.Entity<Marketplace>()
                .HasMany(m => m.Lots)
                .WithOne(l => l.Marketplace)
                .HasForeignKey(l => l.MarketplaceId);

            modelBuilder.Entity<Search>()
                .HasMany(s => s.KeyWords)
                .WithOne(kw => kw.Search)
                .HasForeignKey(kw => kw.SearchId);

            // Marketplace to Address relationship
            modelBuilder.Entity<Marketplace>()
                .HasMany(m => m.Addresses)
                .WithOne(a => a.Marketplace)
                .HasForeignKey(a => a.MarketplaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // ExchangeCompany to Address relationship
            modelBuilder.Entity<ExchangeCompany>()
                .HasMany(e => e.Addresses)
                .WithOne(a => a.ExchangeCompany)
                .HasForeignKey(a => a.ExchangeCompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Lot to CurrentPriceValue relationship
            modelBuilder.Entity<Lot>()
                .HasMany(l => l.CurrentPriceValues)
                .WithOne(cpv => cpv.Lot)
                .HasForeignKey(cpv => cpv.LotId);
        }
    }
}
