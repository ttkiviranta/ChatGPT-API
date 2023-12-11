namespace ChatDemoAPI.Models
{
    // <summary>
    // Represents a chunk of text extracted from a source file.
    // </summary>
    public class TextChunk
    {
        // <summary>
        // Gets or sets the source file from which the text chunk was extracted.
        // </summary>
        public string SourceFile { get; set; } = null!;

        // <summary>
        // Gets or sets the page number of the source file where the text chunk is located.
        // </summary>
        public int Page { get; set; }

        // <summary>
        // Gets or sets the index of the text chunk on the page.
        // </summary>
        public int OnPageIndex { get; set; }

        // <summary>
        // Gets or sets the actual text content of the chunk.
        // </summary>
        public string Text { get; set; } = null!;

        // <summary>
        // Gets or sets the vector representation of the text chunk.
        // </summary>
        public float[]? TextVectors { get; set; }
    }
}
