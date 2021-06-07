using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TakiServer
{
    class ClientData
    {        
        public string Name { get; private set; }  //שם הלקוח שיתחבר לשרת 
        public Socket ClientSocket { get; private set; } // הסוקט שדרכו הלקוח התחבר לשרת 
        public Thread ClientThread { get; private set; }//התהליך של הלקוח שדרכו תתנהל כל ההתקשרות של השרת מול הלקוח

        public ClientData(string name, Socket socket, Thread thread)
        {
            Name = name;
            ClientSocket = socket;
            ClientThread = thread;
        }
    }

}
