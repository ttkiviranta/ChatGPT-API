using System;
using System.Collections.Generic;

namespace ChatDemoAPI.Models.ViewModels
{
    /// <summary>
    /// Represents a response from a chat robot.
    /// </summary>
    public class ChatRobotResponse
    {
        /// <summary>
        /// Gets or sets the ID of the chat robot.
        /// </summary>
        public Guid ChatRobotId { get; set; }

        /// <summary>
        /// Gets or sets the name of the chat robot.
        /// </summary>
        public string? ChatRobotName { get; set; }

        /// <summary>
        /// Gets or sets the description of the chat robot.
        /// </summary>
        public string? ChatRobotDescription { get; set; }

        /// <summary>
        /// Gets or sets the collection of chat robot descriptions.
        /// </summary>
        public virtual ICollection<ChatRobotDescription> ChatRobotDescriptions { get; set; } = new List<ChatRobotDescription>();
    }
}