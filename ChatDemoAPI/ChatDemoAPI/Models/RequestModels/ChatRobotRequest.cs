using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatDemoAPI.Models.RequestModels
{
    /// <summary>
    /// Represent a chat robot request
    /// </summary>
    public class ChatRobotRequest
    {
        /// <summary>
        /// ID of the search robot
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ChatRobotId { get; set; }

        /// <summary>
        /// The name of the chat robot.
        /// </summary>
        public string? ChatRobotName { get; set; }

        /// <summary>
        /// The description of the chat robot.
        /// </summary>
        public string? ChatRobotDescription { get; set; }
    }
}
