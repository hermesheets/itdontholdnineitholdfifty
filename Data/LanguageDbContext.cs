using itdontholdnineitholdfifty.Models;
using Microsoft.EntityFrameworkCore;

namespace itdontholdnineitholdfifty.Data;

public class LanguageDbContext(DbContextOptions<LanguageDbContext> options) : DbContext(options)
{
    public DbSet<VocabularyWord> Words => Set<VocabularyWord>();
    public DbSet<QuizProgress> QuizProgress => Set<QuizProgress>();
    public DbSet<WordQuizStat> WordQuizStats => Set<WordQuizStat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WordQuizStat>()
            .HasKey(stat => stat.VocabularyWordId);

        modelBuilder.Entity<WordQuizStat>()
            .HasOne<VocabularyWord>()
            .WithOne()
            .HasForeignKey<WordQuizStat>(stat => stat.VocabularyWordId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
