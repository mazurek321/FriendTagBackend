using Microsoft.EntityFrameworkCore;
using FriendTagBackend.src.Models.User;
using FriendTagBackend.src.Models.Friendship;
using FriendTagBackend.src.Models.Blocked;

namespace FriendTagBackend.src.Data;

public class ApiDbContext : DbContext 
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) 
        : base(options) {}

     public DbSet<User> Users { get; set; }
     public DbSet<Friendship> Friendship { get; set; }
     public DbSet<Blocked> Blocked { get; set; }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasConversion(id => id.Value, value => new UserId(value));

            builder.HasIndex(x=>x.Email).IsUnique();
            builder.Property(x=>x.Email).HasConversion(email => email.Value, value => new Email(value));

            builder.Property(x=>x.Name).HasConversion(name => name.Value, value => new Name(value))
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x=>x.LastName).HasConversion(ln=>ln.Value, value=>new LastName(value))
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x=>x.Password).HasConversion(p => p.Value, value => new Password(value))
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x=>x.Phone).HasConversion(Phone =>Phone.Value, value => new Phone(value));

            builder.Property(x=>x.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Friendship>(builder =>
        {
            builder.HasKey(x=>x.Id);
        
            builder.Property(x => x.User1).HasConversion(x => x.Value, value => new UserId(value));
           
            builder.Property(x => x.User2).HasConversion(x => x.Value, value => new UserId(value));
            
            builder.Property(x => x.RequestSender).HasConversion(x => x.Value, value => new UserId(value));
                        
        });

        modelBuilder.Entity<Blocked>(builder =>
        {
            builder.HasKey(x=>x.Id);
            
            builder.Property(x=>x.Blocker).HasConversion(x=>x.Value, value=>new UserId(value));
            
            builder.Property(x=>x.BlockedPerson).HasConversion(x=>x.Value, value=>new UserId(value));
        });
    }

}