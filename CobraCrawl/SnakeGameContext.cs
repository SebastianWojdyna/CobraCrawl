using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System;
using System.IO;
using System.Reflection;

namespace CobraCrawl
{
    public class SnakeGameContext : DbContext
    {
        public DbSet<HighScore> HighScores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dbPath = Path.Combine(executableLocation, "snakegame.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }

    public class HighScore
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PlayerName { get; set; }

        public int Score { get; set; }

        public DateTime Date { get; set; }
    }
}
