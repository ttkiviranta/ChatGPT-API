namespace ChatDemoAPI.Models
{
    // <summary>
    // Represents a chat message with a role and a message content.
    // </summary>
    public class ChatMessage
    {
        // <summary>
        // Gets or sets the role of the chat message.
        // </summary>
        // <value>The role of the chat message.</value>
        public string? Role { get; set; }

        // <summary>
        // Gets or sets the content of the chat message.
        // </summary>
        // <value>The content of the chat message.</value>
        public string? Message { get; set; }
    }
}
