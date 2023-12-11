namespace ChatDemoAPI.Models
{
    // <summary>
    // Represents the details of a document.
    // </summary>
    public class DocumentDetail
    {
        // <summary>
        // Gets or sets the unique identifier of the document detail.
        // </summary>
        public Guid DocumentDetailId { get; set; }

        // <summary>
        // Gets or sets the unique identifier of the document.
        // </summary>
        public Guid DocumentId { get; set; }

        // <summary>
        // Gets or sets the sequence number of the document.
        // </summary>
        public int DocumentSequence { get; set; }

        // <summary>
        // Gets or sets the content of the document.
        // </summary>
        public string? DocumentContent { get; set; }

        // <summary>
        // Gets or sets the type of the document.
        // </summary>
        public DocumentType DocumentType { get; set; }

        // <summary>
        // Gets or sets the associated document.
        // </summary>
        public virtual Document Document { get; set; }

        // <summary>
        // Gets or sets the collection of document vector data associated with the document detail.
        // </summary>
        public virtual ICollection<DocumentVectorData> DocumentVectorData { get; set; } = new List<DocumentVectorData>();
    }

    // <summary>
    // Represents the type of a document.
    // </summary>
    public enum DocumentType
    {
        // The document is in PDF format.
        Pdf,

        // The document is in plain text format.
        Text,

        // The document is a web page.
        WebPage
    }
}