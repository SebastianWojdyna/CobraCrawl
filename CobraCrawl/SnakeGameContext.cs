using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System;

namespace CobraCrawl
{
    public class SnakeGameContext : DbContext
    {
        public DbSet<HighScore> HighScores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=snakegame.db");
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
