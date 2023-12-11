using ChatDemoAPI.Models;
using OpenAI_API.Embedding;
using OpenAI_API;
using System.IO;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using OpenAI_API.Models;
using Microsoft.AspNetCore.Components.Forms;
using ChatDemoAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ChatDemoAPI.Services
{
    // <summary>
    // Represents a service for reading PDF files and handling webpages.
    // </summary>
    internal class PdfReader
    {
        // <summary>
        // Uploads a PDF file and splits its content into text chunks.
        //
        // Parameters:
        // - file: The PDF file to upload.
        // - wordsPerChunk: The maximum number of words per text chunk.
        // - OpenAI_Key: The OpenAI API key for creating text vectors.
        //
        // Returns:
        // - A list of TextChunk objects representing the split text chunks of the PDF.
        // </summary>
        public async Task<List<TextChunk>> UploadPdf(IFormFile file, int wordsPerChunk, string OpenAI_Key)
        {
            var textChunks = new List<TextChunk>();
            int totalTokens = 0;
            List<string> pageTexts = new List<string>();
            using (var stream = file.OpenReadStream())
            using (PdfDocument document = PdfDocument.Open(stream))
            {
                foreach (Page page in document.GetPages())
                {
                    var chunks = page.Text
                        .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                        .Select((x, i) => new { Index = i, Value = x })
                        .GroupBy(x => x.Index / wordsPerChunk)
                        .Select(x => string.Join(" ", x.Select(y => y.Value)));

                    int i = 0;
                    foreach (var chunk in chunks)
                    {
                        textChunks.Add(new TextChunk
                        {
                            SourceFile = file.FileName,
                            Page = page.Number,
                            OnPageIndex = i++,
                            Text = chunk,
                            TextVectors = await CreateVectors(chunk, OpenAI_Key)
                        });
                    }
                }
            }
            return textChunks;
        }

        // <summary>
        // Handles a webpage by converting it to PDF and splitting its content into text chunks.
        //
        // Parameters:
        // - htmlUrl: The URL of the webpage to handle.
        // - deep: The depth of the conversion process (optional, default is 1).
        // - wordsPerChunk: The maximum number of words per text chunk (optional, default is 600).
        // - OpenAI_Key: The OpenAI API key for creating text vectors (optional, default is an empty string).
        //
        // Returns:
        // - A list of TextChunk objects representing the split text chunks of the webpage.
        // </summary>
        public async Task<List<TextChunk>> HandleWebpage(string htmlUrl, int deep = 1, int wordsPerChunk = 600, string OpenAI_Key = "")
        {
            try
            {
                var textChunks = new List<TextChunk>();
                int totalTokens = 0;
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                var pdfBytes = htmlToPdf.GeneratePdfFromFile(htmlUrl, null);
                var count = pdfBytes.Length;

                using (PdfDocument document = PdfDocument.Open(pdfBytes))
                {
                    StringBuilder builder = new StringBuilder();
                    string contentent = string.Empty;
                    // Iterate through each page in the PDF document.
                    foreach (Page page in document.GetPages())
                    {
                        var pageText = string.Join(" ", page.GetWords());
                        builder.Append(pageText);
                        var chunks = pageText
                         .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                         .Select((x, i) => new { Index = i, Value = x })
                         .GroupBy(x => x.Index / wordsPerChunk)
                         .Select(x => string.Join("", x.Select(y => y.Value)));

                        int i = 0;
                        foreach (var chunk in chunks)
                        {
                            // Create a new TextChunk object for each chunk and add it to the list.
                            textChunks.Add(new TextChunk
                            {
                                SourceFile = htmlUrl.ToString(),
                                Page = page.Number,
                                OnPageIndex = i++,
                                Text = chunk,
                                TextVectors = await CreateVectors(chunk, OpenAI_Key)
                            });
                        }
                    }
                }
                return textChunks;
            }
            catch (Exception ex) //TODO add error handling
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        // <summary>
        // Creates text vectors for the provided text using the OpenAI API.
        //
        // Parameters:
        // - text: The text for which to create text vectors.
        // - OpenAI_Key: The OpenAI API key for creating text vectors.
        //
        // Returns:
        // - An array of floats representing the text vectors.
        // </summary>
        private async Task<float[]> CreateVectors(string text, string OpenAI_Key)
        {
            // Create an instance of the OpenAIAPI class with the API authentication key.
            OpenAIAPI api = new OpenAIAPI(new APIAuthentication(OpenAI_Key));

            // Create an embedding request using the AdaTextEmbedding model and the provided text.
            var result = await api.Embeddings.CreateEmbeddingAsync(
                new EmbeddingRequest(Model.AdaTextEmbedding, text));

            // Return the embedding vectors from the result.
            return result.Data[0].Embedding;
        }
    }
}