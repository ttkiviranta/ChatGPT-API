using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatDemoAPI.Models
{
    // <summary>
    // Represents a user in the chat system.
    // </summary>
    public class User
    {
        // <summary>
        // Gets or sets the unique identifier for the user.
        // </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }

        // <summary>
        // Gets or sets the description of the user.
        // </summary>
        public string? UserDescription { get; set; }

        // <summary>
        // Gets or sets the collection of chat history associated with the user.
        // </summary>
        public virtual ICollection<ChatHistory> ChatHistory { get; set; } = new List<ChatHistory>();
    }
}