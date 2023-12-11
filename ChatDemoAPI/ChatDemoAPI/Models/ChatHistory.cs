using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatDemoAPI.Models
{
    // <summary>
    // Represents the chat history entity.
    // </summary>
    public class ChatHistory
    {
        // <summary>
        // Gets or sets the unique identifier for the chat history.
        // </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // <summary>
        // Gets or sets the user ID associated with the chat history.
        // </summary>
        public Guid UserId { get; set; }

        // <summary>
        // Gets or sets the chat robot ID associated with the chat history.
        // </summary>
        public Guid ChatRobotId { get; set; }

        // <summary>
        // Gets or sets the role associated with the chat history.
        // </summary>
        public string? Role { get; set; }

        // <summary>
        // Gets or sets the content of the chat history.
        // </summary>
        public string ChatHistoryContent { get; set; } = "";

        // <summary>
        // Gets or sets the message time of the chat history.
        // </summary>
        public DateTimeOffset MessageTime { get; set; }

        // <summary>
        // Gets or sets the list of referenced memory IDs associated with the chat history.
        // </summary>
        public string ReferencedDocumentDetailsIds { get; set; } 

        // <summary>
        // Gets or sets the user entity associated with the chat history.
        // </summary>
        public virtual User User { get; set; }
    }
}
