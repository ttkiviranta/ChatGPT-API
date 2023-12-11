using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatDemoAPI.Models
{
    // <summary>
    // Represents a chat robot entity.
    // </summary>
    public class ChatRobot
    {
        // <summary>
        // Gets or sets the unique identifier for the chat robot.
        // </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ChatRobotId { get; set; }

        // <summary>
        // Gets or sets the name of the chat robot.
        // </summary>
        public string? ChatRobotName { get; set; }

        // <summary>
        // Gets or sets the description of the chat robot.
        // </summary>
        public string? ChatRobotDescription { get; set; }

        // <summary>
        // Gets or sets the collection of documents associated with the chat robot.
        // </summary>
        public virtual ICollection<Document> Document { get; set; } = new List<Document>();

        // <summary>
        // Gets or sets the collection of chat robot descriptions.
        // </summary>
        public virtual ICollection<ChatRobotDescription> ChatRobotDescriptions { get; set; } = new List<ChatRobotDescription>();
    }
}
