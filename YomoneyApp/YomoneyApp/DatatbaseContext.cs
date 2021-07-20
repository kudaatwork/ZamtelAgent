using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using YomoneyApp.Models.Work;

namespace YomoneyApp
{
    public class DatatbaseContext : DbContext
    {
        public DbSet<YoContact> YoContacts { get; set; }
        public DbSet<ChatMessage> YoMessages { get; set; }
        public DbSet<UserCredentials> UserCredentials { get; set; }

        private readonly string _databasePath;

        public DatatbaseContext(string databasePath)
        {
            _databasePath = databasePath;
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_databasePath}");
        }
    }
}
