namespace ChatDemoAPI.Models.RequestModels
{
    // <summary>
    // Represents a chat robot description in the chat demo API.
    // </summary>
    public class ChatRobotDescriptionRequest
    {
        // <summary>
        // Gets or sets the unique identifier for the chat robot description.
        // </summary>
        public Guid ChatRobotDescriptionId { get; set; }

        // <summary>
        // Gets or sets the unique identifier for the chat robot associated with the chat robot description.
        // </summary>
        public Guid ChatRobotId { get; set; }

        // <summary>
        // Gets or sets the description of the chat robot.
        // </summary>
        public string? Description { get; set; }

        // <summary>
        // Gets or sets the default of the chat robot description.
        // </summary>yh
        public bool? IsDefault { get; set; }

    }
}
