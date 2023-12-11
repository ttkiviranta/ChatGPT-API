namespace ChatDemoAPI.Models
{
    // <summary>
    // Represents the data for a document vector.
    // </summary>
    public class DocumentVectorData
    {
        // <summary>
        // Gets or sets the unique identifier for the document vector data.
        // </summary>
        public long DocumentVectorDataId { get; set; }

        // <summary>
        // Gets or sets the unique identifier for the document detail associated with the document vector data.
        // </summary>
        public Guid DocumentDetailId { get; set; }

        // <summary>
        // Gets or sets the identifier for the vector value associated with the document vector data.
        // </summary>
        public int VectorValueId { get; set; }

        // <summary>
        // Gets or sets the value of the vector associated with the document vector data.
        // </summary>
        public double VectorValue { get; set; }

        // <summary>
        // Gets or sets the document detail associated with the document vector data.
        // </summary>
        public virtual DocumentDetail DocumentDetail { get; set; }
    }
}
