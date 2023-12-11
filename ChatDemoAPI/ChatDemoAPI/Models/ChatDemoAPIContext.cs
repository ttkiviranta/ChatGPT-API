using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace ChatDemoAPI.Models
{
    // <summary>
    // Represents the database context for the ChatDemoAPI application.
    // Inherits from the DbContext class provided by Entity Framework Core.
    // </summary>
    public class ChatDemoAPIContext : DbContext
    {
        // <summary>
        // Constructs a new instance of the ChatDemoAPIContext class.
        //
        // Parameters:
        // - options: The DbContextOptions used to configure the context.
        // </summary>
        public ChatDemoAPIContext(DbContextOptions<ChatDemoAPIContext> options) : base(options)
        {

        }

        // <summary>
        // Represents a collection of User entities in the database.
        // </summary>
        public virtual DbSet<User> Users { get; set; }

        // <summary>
        // Represents a collection of ChatHistory entities in the database.
        // </summary>
        public virtual DbSet<ChatHistory> ChatHistory { get; set; }

        // <summary>
        // Represents a collection of ChatRobot entities in the database.
        // </summary>
        public virtual DbSet<ChatRobot> ChatRobots { get; set; }

        // <summary>
        // Represents a collection of ChatRobotDescription entities in the database.
        // </summary>
        public virtual DbSet<ChatRobotDescription> ChatRobotsDescription { get; set; }

        // <summary>
        // Represents a collection of Document entities in the database.
        // </summary>
        public virtual DbSet<Document> Document { get; set; }

        // <summary>
        // Represents a collection of DocumentDetail entities in the database.
        // </summary>
        public virtual DbSet<DocumentDetail> DocumentDetail { get; set; }

        // <summary>
        // Represents a collection of DocumentVectorData entities in the database.
        // </summary>
        public virtual DbSet<DocumentVectorData> DocumentVectorData { get; set; }

        // <summary>
        // Configures the model of the database context.
        //
        // Parameters:
        // - modelBuilder: The ModelBuilder used to build the model.
        // </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the User entity.
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserDescription)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            // Configure the ChatHistory entity.
            modelBuilder.Entity<ChatHistory>(entity =>
            {
                entity.HasOne(e => e.User).WithMany(d => d.ChatHistory)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ChatHistory_User");
            });

            // Configure the ChatRobot entity.
            modelBuilder.Entity<ChatRobot>(entity =>
            {
                entity.Property(e => e.ChatRobotName)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            // Configure the Document entity.
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasOne(e => e.ChatRobot).WithMany(d => d.Document)
                    .HasForeignKey(d => d.ChatRobotId)
                    .HasConstraintName("FK_Document_ChatRobot");
            });

            // Configure the ChatRobotDescription entity.
            modelBuilder.Entity<ChatRobotDescription>(entity =>
            {
                entity.HasOne(e => e.ChatRobot).WithMany(d => d.ChatRobotDescriptions)
                    .HasForeignKey(d => d.ChatRobotId)
                    .HasConstraintName("FK_ChatRobotDescription_ChatRobot");
            });

            // Configure the DocumentDetail entity.
            modelBuilder.Entity<DocumentDetail>(entity =>
            {
                entity.HasOne(d => d.Document).WithMany(p => p.DocumentDetail)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("FK_DocumentDetail_Document");
            });

            // Configure the DocumentVectorData entity.
            modelBuilder.Entity<DocumentVectorData>(entity =>
            {
                entity.Property(e => e.VectorValue).HasColumnName("vector_value");
                entity.Property(e => e.VectorValueId).HasColumnName("vector_value_id");

                entity.HasOne(d => d.DocumentDetail).WithMany(p => p.DocumentVectorData)
                    .HasForeignKey(d => d.DocumentDetailId)
                    .HasConstraintName("FK_DocumentVectorData_DocumentDetail");
            });
        }
    }
}