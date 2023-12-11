using ChatDemoAPI.Models;
using ChatDemoAPI.Models.RequestModels;
using ChatDemoAPI.Models.ViewModels;
using ChatDemoAPI.Services;
using ChatDemoAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ChatDemoAPI.Controllers
{
    // <summary>
    // Controller for managing chat robots and related operations.
    // </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRobotController : ControllerBase
    {
        private readonly ILogger<ChatRobotController> _logger;
        private readonly IChatService _chatService;
        private readonly IConversation _conversationService;

        // <summary>
        // Constructs a new instance of the ChatRobotController class.
        //
        // Parameters:
        // - logger: The logger instance for logging.
        // - chatService: The chat service for managing chat robots.
        // - conversationService: The conversation service for managing conversations.
        // </summary>
        public ChatRobotController(ILogger<ChatRobotController> logger, IChatService chatService, IConversation conversationService)
        {
            _logger = logger;
            _chatService = chatService;
            _conversationService = conversationService;
        }

        // <summary>
        // Adds a new user with the specified user description.
        //
        // Parameters:
        // - userDescription: The description of the user to be added.
        //
        // Returns:
        // - An ActionResult containing a string representing the result of the operation.
        // </summary>
        [HttpPost("AddUser")]
        public async Task<ActionResult<string>> AddUser(string userDescription)
        {
            try
            {
                var result = await _chatService.AddUser(userDescription);
                if (result == true)
                    return Ok(result);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // <summary>
        // Adds a new chat robot with the specified chat robot request.
        //
        // Parameters:
        // - chatRobot: The chat robot request containing the details of the chat robot to be added.
        //
        // Returns:
        // - An ActionResult containing the chat robot request.
        // </summary>
        [HttpPost("AddChatRobot")]
        public async Task<ActionResult<ChatRobotRequest>> AddChatRobot(ChatRobotRequest chatRobot)
        {
            try
            {
                var result = await _chatService.AddChatRobot(chatRobot);
                if (result == true)
                    return Ok(result);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // <summary>
        // Adds a new chat robot description with the specified chat robot description request.
        //
        // Parameters:
        // - chatRobotDescriptionRequest: The chat robot description request containing the details of the chat robot description to be added.
        //
        // Returns:
        // - An ActionResult containing the chat robot description request.
        // </summary>
        [HttpPost("AddChatRobotDescription")]
        public async Task<ActionResult<ChatRobotDescriptionRequest>> AddChatRobotDescription(ChatRobotDescriptionRequest chatRobotDescriptionRequest)
        {
            try
            {
                var result = await _chatService.AddChatRobotDescription(chatRobotDescriptionRequest);
                if (result == true)
                    return Ok(result);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // <summary>
        // Uploads a PDF file and extracts the text chunks from it.
        //
        // Parameters:
        // - file: The PDF file to be uploaded.
        //
        // Returns:
        // - An ActionResult containing a list of text chunks extracted from the PDF file.
        // </summary>
        [HttpPost("upload-pdf")]
        public async Task<ActionResult<List<TextChunk>>> UploadPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            try
            {
                var result = await _chatService.UploadPdf(file);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // <summary>
        // Saves the extracted text chunks from a PDF file to the chat robot with the specified chat robot ID.
        //
        // Parameters:
        // - file: The PDF file to be saved.
        // - chatRobotId: The ID of the chat robot to which the PDF file should be saved.
        //
        // Returns:
        // - An ActionResult representing the result of the operation.
        // </summary>
        [HttpPost("Save-pdf-to-chat-db")]
        public async Task<ActionResult> SavePdfToChatRobot(IFormFile file, Guid chatRobotId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            try
            {
                var result = await _chatService.UploadPdf(file);
                if (result.Count() >= 0)
                {
                    await _chatService.SavePdfToChatRobot(result, chatRobotId);
                    return Ok();
                }
                return BadRequest("No file was uploaded and saved.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // <summary>
        // Saves the content of a webpage to the chat robot with the specified chat robot ID.
        //
        // Parameters:
        // - chatRobotId: The ID of the chat robot to which the webpage content should be saved.
        // - ulr: The URL of the webpage to be saved. Default value is "https://www.anders.com".
        // - deep: The depth of the webpage to be saved. Default value is 1.
        //
        // Returns:
        // - An ActionResult representing the result of the operation.
        // </summary>
        [HttpPost("Save-webpage-to-db")]
        public async Task<ActionResult> SaveWebpageChatRobot(Guid chatRobotId, string ulr = "https://www.anders.com", int deep = 1)
        {
            var result = await _chatService.SaveWebpageToDb(chatRobotId, ulr, deep); ;
            return Ok();
        }

        // <summary>
        // Retrieves a list of users.
        //
        // Returns:
        // - A Task representing the asynchronous operation that returns a list of users.
        // </summary>
        [HttpGet("GetUsers")]
        public async Task<List<User>> GetUsers()
        {
            var result = await _chatService.GetUsers();
            return result;
        }

        // <summary>
        // Retrieves a list of chat robots.
        //
        // Returns:
        // - A Task representing the asynchronous operation that returns a list of chat robots.
        // </summary>
        [HttpGet("GetChatRobots")]
        public async Task<List<ChatRobot>> GetChatRobots()
        {
            var result = await _chatService.GetChatRobots();
            return result;
        }

        // <summary>
        // Retrieves a list of chat robots with the specified chat robot ID.
        //
        // Parameters:
        // - chatRobotId: The ID of the chat robot to retrieve.
        //
        // Returns:
        // - A Task representing the asynchronous operation that returns a list of chat robots.
        // </summary>
        [HttpGet("GetChatRobotById")]
        public async Task<List<ChatRobot>> GetChatRobotById(Guid chatRobotId)
        {
            var result = await _chatService.GetChatRobotById(chatRobotId);
            return result;
        }

        // <summary>
        // Retrieves the top ten vector data for the chat robot with the specified chat robot ID.
        //
        // Parameters:
        // - chatRobotId: The ID of the chat robot to retrieve the vector data for.
        //
        // Returns:
        // - A Task representing the asynchronous operation that returns a list of vector data responses.
        // </summary>
        [HttpGet("GetDocumentVectorData")]
        public async Task<List<VectorDataResponse>> GetChatRobotVectorData(Guid chatRobotId)
        {
            var result = await _chatService.GetChatRobotVectorData(chatRobotId);

            List<VectorDataResponse> objListOrder = result.OrderBy(order => order.VectorValue).ToList();

            var topTen = objListOrder.Take(10).ToList();

            return topTen;
        }

        // <summary>
        // Asks the chat GPT model a question for the chat robot with the specified chat robot ID and user ID.
        //
        // Parameters:
        // - chatRobotId: The ID of the chat robot.
        // - userId: The ID of the user.
        // - question: The question to ask the chat GPT model.
        //
        // Returns:
        // - A Task representing the asynchronous operation that returns a string representing the answer from the chat GPT model.
        // </summary>
        [HttpGet("AskChatGPT")]
        public async Task<string> AskChatGPT(Guid chatRobotId, Guid userId, string question)
        {
            // Call the chat robot service to get the answer from the chat GPT model.
            var result = await _conversationService.AskChatGPT(chatRobotId, userId, question);

            // Return the answer from the chat GPT model.
            return result;
        }

        // <summary>
        // Retrieves a list of similar content documents for the chat robot with the specified chat robot ID and question.
        //
        // Parameters:
        // - chatRobotId: The ID of the chat robot.
        // - question: The question to find similar content for.
        // - count: The number of similar content documents to retrieve.
        //
        // Returns:
        // - A Task representing the asynchronous operation that returns a list of similar content document results.
        // </summary>
        [HttpGet("GetSimilarContent")]
        public async Task<List<SimilarContentDocumentResult>> GetSimilarContent(Guid chatRobotId, string question, int count)
        {
            // Call the chat robot service to retrieve the similar content documents.
            var result = await _chatService.GetSimilarContent(chatRobotId, question, count);

            // Return the similar content documents.
            return result;
        }
    }
}