using System;

namespace ChatDemoAPI.Models
{
    // <summary>
    // Represents a document in the chat demo API.
    // </summary>
    public class Document
    {
        // <summary>
        // Gets or sets the unique identifier for the document.
        // </summary>
        public Guid DocumentId { get; set; }

        // <summary>
        // Gets or sets the unique identifier for the chat robot associated with the document.
        // </summary>
        public Guid ChatRobotId { get; set; }

        // <summary>
        // Gets or sets the name of the document.
        // </summary>
        public string? DocumentName { get; set; }

        // <summary>
        // Gets or sets the chat robot associated with the document.
        // </summary>
        public virtual ChatRobot ChatRobot { get; set; }

        // <summary>
        // Gets or sets the collection of document details associated with the document.
        // </summary>
        public virtual ICollection<DocumentDetail> DocumentDetail { get; set; } = new List<DocumentDetail>();
    }
}