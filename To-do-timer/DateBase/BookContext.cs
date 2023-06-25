using Microsoft.EntityFrameworkCore;
using To_do_timer.Models.Book;

namespace To_do_timer.DateBase;

    public class BookContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Status> Statuses { get; set; }

        public BookContext(DbContextOptions<BookContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=TDT;Username=postgres;Password=Last");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Book)
                .WithMany(b => b.Events)
                .HasForeignKey(e => e.BookId);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Status)
                .WithMany(s => s.Events)
                .HasForeignKey(e => e.StatusId);
        }
    }