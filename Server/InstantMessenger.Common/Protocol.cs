using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantMessenger.Common
{
    public static class Protocol
    {
        public enum MessageType : byte
        {
            //Request          
            IM_Login,           // Login
            IM_Register,        // Register
            IM_Add,             // Add friend
            
            IM_Find,            // Find user
            IM_Send,            // Send message
            IM_Received,        // Message received
            IM_InitMain,        // Init main window with friends and conversations
            IM_InitConversation,// Init conversation with messages
            IM_GetOldMessages,  // Get next batch of messages
            IM_GetRequests,     // Get friendship requests
            IM_AcceptRequest,   // Accept friend request
            IM_DeleteRequest,   // Delete friendship request

            //Response
            IM_OK,              // OK
            IM_ERROR,           // Error

            //Special
            IM_DONT_SEND        // Special case, when don't want the TransportObject being sent
        }
    }
}
