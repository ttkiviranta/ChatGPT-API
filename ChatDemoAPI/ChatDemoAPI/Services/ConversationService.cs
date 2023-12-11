using AI.Dev.OpenAI.GPT;
using ChatDemoAPI.Models;
using ChatDemoAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OpenAI_API.Moderation;
using System.Text;
using System.Text.RegularExpressions;

namespace ChatDemoAPI.Services
{
    /// <summary>
    /// Represents a service for handling conversations and interacting with the Chat GPT model.
    /// Implements the IConversation interface.
    /// </summary>
    public class ConversationService : IConversation
    {
        public IServiceProvider Services { get; }
        private readonly ChatDemoAPIContext dbcontext;
        private ILogger<ConversationService> _logger;
        private readonly IConfiguration _conf;

        /// <summary>
        /// Constructs a new instance of the ConversationService class.
        /// </summary>
        /// <param name="conf">The IConfiguration instance.</param>
        /// <param name="services">The IServiceProvider instance.</param>
        /// <param name="dbcontext">The ChatDemoAPIContext instance.</param>
        /// <param name="logger">The ILogger instance.</param>
        public ConversationService(IConfiguration conf, IServiceProvider services, ChatDemoAPIContext dbcontext, ILogger<ConversationService> logger)
        {
            Services = services;
            this.dbcontext = dbcontext;
            _logger = logger;
            _conf = conf;
        }

        /// <summary>
        /// Asks the Chat GPT model a question and returns the generated response.
        /// </summary>
        /// <param name="chatRobotId">The ID of the chat robot.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="question">The question to ask the chat robot.</param>
        /// <returns>The generated response from the Chat GPT model.</returns>
        public async Task<string> AskChatGPT(Guid chatRobotId, Guid userId, string question)
        {
            string txtAnswer = "";
            string similarContent = "";
            string chatRobotDiscription = "";
            string referensedDodumentDetailId = "";
            try
            {
                var chatMessages = await GetChatMessages(chatRobotId, userId, 10);
                chatRobotDiscription = await GetChatRobotDiscription(chatRobotId);
                List<int> tokens = GPT3Tokenizer.Encode(chatRobotDiscription);
                int chatRobotDiscriptionToken = tokens.Count;
                var scope = Services.CreateScope();
                var scopedChatService = scope.ServiceProvider.GetRequiredService<IChatService>();
                var similarContentDocumentResults = await scopedChatService.GetSimilarContent(chatRobotId, question, 3);
                if (similarContentDocumentResults != null)
                {
                    foreach (var content in similarContentDocumentResults)
                    {
                        similarContent = similarContent + content.DocumentContent + " ";
                        referensedDodumentDetailId = referensedDodumentDetailId + content.DocumentDetailId + ";";
                    }
                }
                tokens = GPT3Tokenizer.Encode(similarContent);
                int similarContentTokens = tokens.Count;
                string messageContent = chatRobotDiscription.ToString() + " " + similarContent.ToString();
                var messages = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "role" , "system"},
                        { "content" , similarContent }
                    },
                    new Dictionary<string, string>
                    {
                        { "role" , "user"},
                        { "content" ,  chatRobotDiscription}
                    }
                };

                //for (int i = chatMessages.Count - 1; i >= 0; i--)
                //{
                //    string messageRole = "assistant";
                //    if (chatMessages[i].Role == "user")
                //    { messageRole = "user"; }
                //    messages.Add(new Dictionary<string, string>
                //    {
                //        { "role" ,  messageRole},
                //        { "content" ,  chatMessages[i].Message }
                //    });

                //}

                messages.Add(new Dictionary<string, string>
                {
                    { "role" , "user"},
                    { "content" ,  question}
                });

                var response = await SendChatRequest(messages);
                txtAnswer = response["choices"][0]["message"]["content"].ToString();
                await AddChatHistoryContent(userId, chatRobotId, "user", question, DateTimeOffset.Now, referensedDodumentDetailId);
                await AddChatHistoryContent(userId, chatRobotId, "assistant", txtAnswer, DateTimeOffset.Now, referensedDodumentDetailId);

            }
            catch (Exception ex)
            {
                txtAnswer = "Voi rähmä... Jokin meni pieleen.";
                _logger.LogError("AskChatGPT" + ex.Message);
            }

            return txtAnswer;
        }

        /// <summary>
        /// Sends a chat request to the OpenAI API and returns the response.
        /// </summary>
        /// <param name="messages">The list of messages in the conversation.</param>
        /// <returns>The parsed response from the OpenAI API.</returns>
        private async Task<Newtonsoft.Json.Linq.JObject> SendChatRequest(List<Dictionary<string, string>> messages)
        {
            string OpenAI_Key = _conf["OpenAI:OpenAI_Key"];
            string OpenAI_Url = _conf["OpenAI:OpenAI_Url"];
            using (var client = new HttpClient())
            {
                // Set the request headers
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {OpenAI_Key}");
                // client.DefaultRequestHeaders.Add("Content-Type", "application/json");

                // Set the request body
                var requestBody = new
                {
                    model = "gpt-3.5-turbo-16k-0613",
                    messages = messages,
                    temperature = 0.3,
                    max_tokens = 1024,
                    n = 1
                };
                var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                // Send the request
                var response = await client.PostAsync(OpenAI_Url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));
                var responseBody = await response.Content.ReadAsStringAsync();

                // Parse the response
                var parsedResponse = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
                return parsedResponse;
            }
        }

        /// <summary>
        /// Adds the chat history content to the database.
        /// </summary>
        /// <param name="userID">The ID of the user.</param>
        /// <param name="chatRobotId">The ID of the chat robot.</param>
        /// <param name="role">The role of the chat message.</param>
        /// <param name="chatHistoryContent">The content of the chat history.</param>
        /// <param name="MessageTime">The timestamp of the chat message.</param>
        /// <param name="ReferencedDocumentDetailsIds">The IDs of the referenced document details.</param>
        private async Task AddChatHistoryContent(Guid userID, Guid chatRobotId, string role, string chatHistoryContent, DateTimeOffset MessageTime, string ReferencedDocumentDetailsIds)
        {
            ChatHistory chatHistory = new ChatHistory()
            {
                UserId = userID,
                ChatRobotId = chatRobotId,
                Role = role,
                ChatHistoryContent = chatHistoryContent,
                MessageTime = MessageTime,
                ReferencedDocumentDetailsIds = ReferencedDocumentDetailsIds
            };

            using var transaction = await dbcontext.Database.BeginTransactionAsync();

            try
            {
                var res = await dbcontext.ChatHistory.AddAsync(chatHistory);
                await dbcontext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Failed to add chat history content.");
            }
        }

        /// <summary>
        /// Retrieves the chat robot description from the database.
        /// </summary>
        /// <param name="chatRobotId">The ID of the chat robot.</param>
        /// <returns>The chat robot description.</returns>
        private async Task<string> GetChatRobotDiscription(Guid chatRobotId)
        {
            string result = "";
            var chatRobotResult = await dbcontext.ChatRobotsDescription.Where(a => a.ChatRobotId == chatRobotId && a.IsDefault == true).Select(x => new { x.Description }).FirstOrDefaultAsync();
            if (chatRobotResult != null)
            {
                result = chatRobotResult.Description.ToString();
            }
            return result;
        }

        /// <summary>
        /// Retrieves the chat messages from the database.
        /// </summary>
        /// <param name="chatRobotId">The ID of the chat robot.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="count">The number of chat messages to retrieve.</param>
        /// <returns>A list of chat messages.</returns>
        private async Task<List<ChatMessage>> GetChatMessages(Guid chatRobotId, Guid userId, int count)
        {
            string replacement = " ";
            var result = new List<ChatMessage>();
            var chatMessagestResult = await dbcontext.ChatHistory.Where(a => a.ChatRobotId == chatRobotId && a.UserId == userId && a.Role == "user").OrderByDescending(x => x.Id).Take(count).ToListAsync();
            if (chatMessagestResult != null)
            {
                foreach (var chatMessage in chatMessagestResult)
                {
                    var message = new ChatMessage();
                    message.Message = chatMessage.ChatHistoryContent.Replace("\r\n", replacement)
                        .Replace("\r", replacement)
                        .Replace("\n", replacement); ;
                    message.Role = chatMessage.Role;
                    result.Add(message);
                }
            }
            return result;
        }
    }
}