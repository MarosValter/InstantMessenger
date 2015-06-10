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
            IM_Accept,          // Accept friend request
            IM_Find,            // Find user
            IM_Exists,          // Already exists
            IM_IsAvailable,     // Is user available?
            IM_Available,       // User is available or not
            IM_Send,            // Send message
            IM_Received,        // Message received
            IM_FriendsRequests, // Get friends and requests after login
            IM_GetRequests,     // Get friendship requests
            IM_DeleteRequest,   // Delete friendship request

            //Response
            IM_OK,              // OK
            IM_ERROR,           // Error
        }
    }
}
