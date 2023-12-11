namespace ChatDemoAPI.Models
{
    // <summary>
    // Represents the result of a document similarity comparison in a chat demo API.
    // </summary>
    public class SimilarContentDocumentResult
    {
        // <summary>
        // Gets or sets the name of the chat robot associated with the document.
        // </summary>
        public string? ChatRobotName { get; set; }

        // <summary>
        // Gets or sets the name of the document.
        // </summary>
        public string? DocumentName { get; set; }

        // <summary>
        // Gets or sets the content of the document.
        // </summary>
        public string? DocumentContent { get; set; }

        // <summary>
        // Gets or sets the sequence number of the document.
        // </summary>
        public int DocumentSequence { get; set; }

        // <summary>
        // Gets or sets the cosine distance between the document and a reference document.
        // </summary>
        public double? cosine_distance { get; set; }

        // <summary>
        // Gets or sets the unique identifier of the document detail.
        // </summary>
        public Guid DocumentDetailId { get; set; }
    }
}
