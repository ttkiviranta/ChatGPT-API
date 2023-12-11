using ChatDemoAPI.Models;
using ChatDemoAPI.Models.RequestModels;
using ChatDemoAPI.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API.Moderation;

namespace ChatDemoAPI.Services.Interfaces
{
    /// <summary>
    /// Represents the interface for the Chat Service.
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Adds a new user to the chat.
        /// </summary>
        /// <param name="userDescription">The description of the user.</param>
        /// <returns>A task representing the asynchronous operation. Returns true if the user is added successfully, otherwise false.</returns>
        Task<bool> AddUser(string userDescription);

        /// <summary>
        /// Retrieves a list of all users in the chat.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. Returns a list of User objects.</returns>
        Task<List<User>> GetUsers();

        /// <summary>
        /// Adds a new chat robot to the chat.
        /// </summary>
        /// <param name="chatRobot">The chat robot to be added.</param>
        /// <returns>A task representing the asynchronous operation. Returns true if the chat robot is added successfully, otherwise false.</returns>
        Task<bool> AddChatRobot(ChatRobotRequest chatRobot);

        /// <summary>
        /// Adds a description to a chat robot.
        /// </summary>
        /// <param name="chatRobotDescription">The chat robot description to be added.</param>
        /// <returns>A task representing the asynchronous operation. Returns true if the chat robot description is added successfully, otherwise false.</returns>
        Task<bool> AddChatRobotDescription(ChatRobotDescriptionRequest chatRobotDescription);

        /// <summary>
        /// Saves the PDF data to a chat robot.
        /// </summary>
        /// <param name="data">The PDF data to be saved.</param>
        /// <param name="chatRobotId">The ID of the chat robot.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SavePdfToChatRobot(List<TextChunk> data, Guid chatRobotId);

        /// <summary>
        /// Uploads a PDF file and returns the extracted text chunks.
        /// </summary>
        /// <param name="file">The PDF file to be uploaded.</param>
        /// <returns>A task representing the asynchronous operation. Returns a list of TextChunk objects.</returns>
        Task<List<TextChunk>> UploadPdf(IFormFile file);

        /// <summary>
        /// Saves the data to the database for a chat robot.
        /// </summary>
        /// <param name="data">The data to be saved.</param>
        /// <param name="chatRobotName">The name of the chat robot.</param>
        /// <param name="chatRobotDescription">The description of the chat robot.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveDataToDb(List<TextChunk> data, string chatRobotName, string chatRobotDescription);

        /// <summary>
        /// Saves a webpage to the database for a chat robot.
        /// </summary>
        /// <param name="chatRobotId">The ID of the chat robot.</param>
        /// <param name="url">The URL of the webpage to be saved.</param>
        /// <param name="deep">The depth of the webpage to be saved (default is 1).</param>
        /// <returns>A task representing the asynchronous operation. Returns true if the webpage is saved successfully, otherwise false.</returns>
        Task<bool> SaveWebpageToDb(Guid chatRobotId, string url, int deep = 1);

        /// <summary>
        /// Creates vectors for the given text.
        /// </summary>
        /// <param name="text">The text for which vectors are to be created.</param>
        /// <returns>A task representing the asynchronous operation. Returns an array of floats representing the vectors.</returns>
        Task<float[]> CreateVectors(string text);

        /// <summary>
        /// Adds a chat robot with the specified ID, name, and description.
        /// </summary>
        /// <param name="chatRobotId">The ID of the chat robot.</param>
        /// <param name="chatRobotName">The name of the chat robot.</param>
        /// <param name="chatRobotDescription">The description of the chat robot.</param>
        /// <returns>A task representing the asynchronous operation. Returns true if the chat robot is added successfully, otherwise false.</returns>
        Task<bool> AddChatRobot(Guid chatRobotId, string chatRobotName, string chatRobotDescription);

        /// <summary>
        /// Adds a document with the specified ID, chat robot ID, and name.
        /// </summary>
        /// <param name="documentId">The ID of the document.</param>
        /// <param name="chatRobotId">The ID of the chat robot.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <returns>A task representing the asynchronous operation. Returns true if the document is added successfully, otherwise false.</returns>
        Task<bool> AddDocument(Guid documentId, Guid chatRobotId, string documentName);

        /// <summary>
        /// Adds details to a document.
        /// </summary>
        /// <param name="documentId">The ID of the document.</param>
        /// <param name="data">The data to be added as details.</param>
        /// <returns>A task representing the asynchronous operation. Returns true if the details are added successfully, otherwise false.</returns>
        Task<bool> AddDocumentDetail(Guid documentId, List<TextChunk> data);

        /// <summary>
        /// Adds vector data to a document detail.
        /// </summary>
        /// <param name="documentDetailId">The ID of the document detail.</param>
        /// <param name="data">The vector data to be added.</param>
        /// <returns>A task representing the asynchronous operation. Returns true if the vector data is added successfully, otherwise false.</returns>
        Task<bool> AddDocumentVectorData(Guid documentDetailId, TextChunk data);

        /// <summary>
        /// Retrieves vector data for a chat robot.
        /// </summary>
        /// <param name="chatRobotId">The ID of the chat robot.</param>
        /// <returns>A task representing the asynchronous operation. Returns a list of VectorDataResponse objects.</returns>
        Task<List<VectorDataResponse>> GetChatRobotVectorData(Guid chatRobotId);

        /// <summary>
        /// Retrieves a list of all chat robots.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. Returns a list of ChatRobot objects.</returns>
        Task<List<ChatRobot>> GetChatRobots();

        /// <summary>
        /// Retrieves a list of chat robots with the specified ID.
        /// </summary>
        /// <param name="chatRobotId">The ID of the chat robot.</param>
        /// <returns>A task representing the asynchronous operation. Returns a list of ChatRobot objects.</returns>
        Task<List<ChatRobot>> GetChatRobotById(Guid chatRobotId);

        /// <summary>
        /// Retrieves a list of similar content documents for a chat robot and a given question.
        /// </summary>
        /// <param name="chatRobotId">The ID of the chat robot.</param>
        /// <param name="question">The question for which similar content is to be retrieved.</param>
        /// <param name="count">The number of similar content documents to retrieve.</param>
        /// <returns>A task representing the asynchronous operation. Returns a list of SimilarContentDocumentResult objects.</returns>
        Task<List<SimilarContentDocumentResult>> GetSimilarContent(Guid chatRobotId, string question, int count);
    }
}