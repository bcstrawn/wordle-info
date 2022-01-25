using System;
using Microsoft.EntityFrameworkCore;

namespace Wordle.Models;

public class WordleContext : DbContext
{
    public DbSet<Guess> Guesses { get; set; }
    public DbSet<SecondGuess> SecondGuesses { get; set; }
    public DbSet<Vocab> Vocab { get; set; }

    public string DbPath { get; }

    public WordleContext()
    {
        // var folder = Environment.SpecialFolder.LocalApplicationData;
        // var path = Environment.GetFolderPath(folder);
        // DbPath = System.IO.Path.Join(path, "wordle.db");
        DbPath = "wordle.db";
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}