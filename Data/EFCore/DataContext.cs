using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.EFCore;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<Comment> Comment { get; set; }
    public DbSet<Favorite> Favorite { get; set; }
    public DbSet<Ingredient> Ingredient { get; set; }
    public DbSet<Media> Media { get; set; }
    public DbSet<ModeratorApplication> ModeratorApplication { get; set; }
    public DbSet<Notification> Notification { get; set; }
    public DbSet<Post> Post { get; set; }
    public DbSet<PostIngredient> PostIngredient { get; set; }
    public DbSet<PostTopic> PostTopic { get; set; }
    public DbSet<Quiz> Quiz { get; set; }
    public DbSet<QuizAnswer> QuizAnswer { get; set; }
    public DbSet<QuizDetail> QuizDetail { get; set; }
    public DbSet<QuizQuestion> QuizQuestion { get; set; }
    public DbSet<QuizRangeScore> QuizRangeScore { get; set; }
    public DbSet<QuizResult> QuizResult { get; set; }
    public DbSet<Reaction> Reaction { get; set; }
    public DbSet<Topic> Topic { get; set; }
    public DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the relationship
        modelBuilder.Entity<ModeratorApplication>()
            .HasOne(a => a.Registrant)
            .WithMany(u => u.RequestRegister)
            .HasForeignKey(a => a.RegisterById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ModeratorApplication>()
            .HasOne(a => a.Confirmer)
            .WithMany(u => u.Comfirmed)
            .HasForeignKey(a => a.ConfirmedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Post>()
            .HasOne(a => a.PostBy)
            .WithMany(u => u.Posts)
            .HasForeignKey(a => a.PostById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Post>()
            .HasOne(a => a.ConfirmBy)
            .WithMany(u => u.ApprovedPosts)
            .HasForeignKey(a => a.ComfirmById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuizResult>()
            .HasOne(a => a.User)
            .WithMany(u => u.QuizMade)
            .HasForeignKey(a => a.QuizMadeById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuizResult>()
            .HasOne(a => a.Evaluator)
            .WithMany(u => u.QuizEvaluate)
            .HasForeignKey(a => a.EvaluateById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
