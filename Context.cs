using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PhotoParse.Models;

namespace PhotoParse
{   
   public class Context : DbContext
  {
    public DbSet<User> User { get; set; }

    public DbSet<Photo> Photo { get; set; }

    public DbSet<Models.ToDoList> ToDoList { get; set; }

    public DbSet<Models.ToDoListItem> ToDoListItem { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlServer("server=localhost;database=Photo;user=root;password=salt1230");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

    //   modelBuilder.Entity<User>(entity =>
    //   {
    //     entity.Property(f =>f.ID).ValueGeneratedOnAdd();
    //     entity.HasKey(e => e.ID);
    //     entity.Property(f => f.Name).IsRequired();
    //   });

      modelBuilder.Entity<Photo>(entity =>
      {
        entity.Property(f =>f.ID).ValueGeneratedOnAdd();
        entity.HasKey(e => e.ID);
        entity.Property(f => f.Location).IsRequired();
      });

    //   modelBuilder.Entity<Models.ToDoList>(entity =>
    //   {
    //     entity.Property(f =>f.ID).ValueGeneratedOnAdd();
    //     entity.HasKey(e => e.ID);
    //     entity.Property(f => f.Name).IsRequired();
    //     entity.HasOne(d => d.User)
    //       .WithMany(p => p.Lists);
    //   });

    //   modelBuilder.Entity<Models.ToDoListItem>(entity =>
    //   {
    //     entity.Property(f =>f.ID).ValueGeneratedOnAdd();
    //     entity.HasKey(e => e.ID);
    //     entity.Property(f => f.Name).IsRequired();
    //     entity.HasOne(d => d.ToDoList)
    //       .WithMany(p => p.Items);
    //   });
    }
  }



}
