using System;
using System.Collections.Generic;

namespace InstantMessenger.Server
{
    public class ClientEventArgs : EventArgs
    {
        public ISet<ClientContext> Users { get; set; }

        public ClientEventArgs(IEnumerable<ClientContext> users)
        {
            Users = new HashSet<ClientContext>(users);
        }
    }
}
