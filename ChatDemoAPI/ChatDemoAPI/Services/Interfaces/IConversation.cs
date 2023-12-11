using System;
using System.Threading.Tasks;

namespace ChatDemoAPI.Services.Interfaces
{
    // <summary>
    // Represents an interface for a conversation with a chat robot.
    // </summary>
    public interface IConversation
    {
        // <summary>
        // Asks a question to a chat robot and returns the response.
        //
        // Parameters:
        // - chatRobotId: The unique identifier of the chat robot.
        // - userID: The unique identifier of the user.
        // - question: The question to ask the chat robot.
        //
        // Returns:
        // - A task that represents the asynchronous operation. The task result contains the response from the chat robot as a string.
        // </summary>
        Task<string> AskChatGPT(Guid chatRobotId, Guid userID, string question);
    }
}
