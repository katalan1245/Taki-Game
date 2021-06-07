using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TakiServer
{
    public partial class Form1 : Form
    {
        private TcpListener tcpLsn; // מאזין להתחברות של סוקט לשרת 
        private Thread tcpThd; // הגדרת אובייקט של תהליך
        private Dictionary<string,ClientData> ClientDict;
        private string NameOfUsers;
        private Cards Cards;
        private string LastCard;
        private string CurrentPlayer;
        private bool GameOn;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ClientDict = new Dictionary<string, ClientData>();

            tcpLsn = new TcpListener(IPAddress.Parse("127.0.0.1"), 8002);//  יצירת מאזין- 
            tcpLsn.Start();// מפעילה את המאזין
            tcpThd = new Thread(new ThreadStart(ListenerThread));// יצירת תהליך שפועל ברקע   
            tcpThd.Start();
            this.Text = tcpLsn.LocalEndpoint.ToString();
            
            writeLog("Server started, waiting for clients...");
        }

        public void ListenerThread()
        {
            while (true)
            {
                try
                {
                    Socket socket = tcpLsn.AcceptSocket();
                    if (ClientDict.Count == 2) // כל עוד יש שני אנשים נמשיך הלאה, לא אמורים להתחבר
                        continue;
                    lock(this)
                    {
                        
                        Byte[] receive = new Byte[100];// ברשת נתונים עוברים רק בביטים
                        int ret = socket.Receive(receive, receive.Length, 0);// מחזיק את הכמות הבייטים שיתקבלו
                        string message = Encoding.UTF8.GetString(receive);// שורה זו ממירה את המערך שהוא מטיפוס בייט למחרוזת שאותה הוא שומר ב strmess
                        string name = message.Substring(0, ret);// שורה זו מחלצת את מה שכתוב עד לרווח הראשון
                        Thread clientThread = new Thread(() => ReadSocket(name));
                        var client = new ClientData(name, socket, clientThread);
                        bool nameExist = false;
                        if (ClientDict.ContainsKey(name))
                        {
                            nameExist = true;
                            name = $"{name}_copy";
                        }
                            

                        ClientDict.Add(name,client);// מעדכנות את הטבלה ומוסיפות לתקסטבוקס את שעת ההיתחברות
                        writeLog("Connected As> " + name);//
                        client.ClientThread.Start();
                        if (nameExist)
                            SendMessageToClient(client, $":Rename:{name}");
                     }
                    //בונה את רשימת המשתמשים
                    NameOfUsers = ":UserList:";
                    foreach (ClientData c in ClientDict.Values)
                    {
                        if (c.ClientSocket.Connected)
                        {
                            NameOfUsers += $"{c.Name},";
                        }
                    }
                    NameOfUsers = NameOfUsers.Substring(0, NameOfUsers.Length - 1);
                    foreach (ClientData c in ClientDict.Values)
                        SendMessageToClient(c, NameOfUsers);

                    // נתחיל את המשחק אם יש שני אנשים
                    if(ClientDict.Count == 2)
                    {
                        StartGame();
                    }
                }
                catch (Exception e)
                {
                    break;
                }
            }
        }

        private void StartGame() // נתחיל את המשחק
        {
            Cards = new Cards();
            foreach (ClientData c in ClientDict.Values)
                SendCardsToPlayers(c, 8);
            SendFirstTurn();
            SendFirstCard();
            GameOn = true;
        }

        /********************
         הסרבר עובד בפורמט הבא, מי אמור לקבל, נקודותיים, מה הפקודה, נקודותיים, ומה הערכים. הסימן | מפריד בין הודעות במצ של שרשור. אם אין משהו לפני הנוקדותיים הראשונים סימן שכולם מקבלים את ההודעה
         User1:Message:Values|:AnotherMessage:values
         ********************/

        private void SendFirstTurn() // נשלח את התור הראשון
        {
            Random rnd = new Random();
            int first = rnd.Next(0, 2);
            foreach (ClientData c in ClientDict.Values)
            {
                CurrentPlayer = ClientDict.Values.ToArray()[first].Name;
                string str = $":FirstPlayer:{CurrentPlayer}"; // נשלח לכולם מי השחקן הראשון
                
                SendMessageToClient(c, str);
            }
        }

        private void SendCardsToPlayers(ClientData client, int numOfCards) // נשלח לכולם קלפים
        {
            string msg = ":CardsList:";
            for (int i = 0; i<numOfCards; i++)
            {
                msg += $"{Cards.TakeCard()},";
            }
            msg = msg.Substring(0, msg.Length - 1);
            SendMessageToClient(client, msg);
        }

        private void SendFirstCard() // נשלח לכולם את הקלף הראשון לקופה
        {
            string card = Cards.TakeCard();
            while(!Cards.isNumber(card)) // אם הקלף לא מספר ניקח אחד חדש
            {
                Cards.AddCard(card);
                card = Cards.TakeCard();
            }
            string msg = $":FirstCard:{card}";
            LastCard = card;
            foreach(ClientData c in ClientDict.Values)
            {
                SendMessageToClient(c, msg);
            }
        }

        private void SendMessageToClient(ClientData client, string msg) // נשלח את ההודעה ללקוח
        {
            if (client.ClientSocket.Connected)
            {
                msg += "|"; // נוסיף הפרדה בשביל שרשור פקודות
                byte[] writeBuffer = Encoding.UTF8.GetBytes(msg); // ממיר את השמות של המשתתפים לבייט
                client.ClientSocket.Send(writeBuffer, writeBuffer.Length, SocketFlags.None);
            }
        }

        private void writeLog(string displayString)//אם תוך כדי תהליכים נצטרך לעדכן תיבת טקסט או כפתורים על הטופס נעשה אינבואוק
        {
            if (txtData.InvokeRequired)
                txtData.Invoke(new MethodInvoker(() => txtData.AppendText($"{DateTime.Now.ToString()}: {displayString}\r\n")));
            else
                txtData.AppendText($"{DateTime.Now.ToString()}: {displayString}\r\n");
        }

        private void ReadSocket(string name)// תהליך של כול לקוח
        {
            Socket s = ClientDict[name].ClientSocket;
            int ret; // This object will contain the number of characters that are passed in the message
            while (true) // מנהל המשחק: כרגע מקבלת ממל מי כול לקוח ומוסרת אותו לכול הלקוחות
            {
                try
                {
                    if (s.Connected)
                    {
                        byte[] receive = new byte[1024];
                        ret = s.Receive(receive, receive.Length, 0);//s.Receive is a command that gets the info from the client and put it in the array receive
                        if (ret > 0) // If a message is rececived
                        {
                            string msg = Encoding.UTF8.GetString(receive).Replace("\0", string.Empty);
                            receive = HandleMessage(msg, name);
                            foreach (ClientData c in ClientDict.Values)
                            {
                                if (c.ClientSocket.Connected)
                                    c.ClientSocket.Send(receive, receive.Length, SocketFlags.None);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }// If a message was received and its' characters length is 0 we get out of the loop
                }
                catch (Exception e) // If an error occured we want to stop the thread
                {
                    writeLog(e.ToString()); // Show on the screen in the txtbox the error that occured
                    if (!s.Connected) break;// If the client is not connected we get out of the loop
                }
            }
            CloseTheThread(name);// We'll get to this line only if an error occured, because we only get out of the loop if there was an error
                                   //that's why this function will only be summaned if an error occured
        }

        private byte[] HandleMessage(string msg, string player) // ננתח הודעה
        {
            string[] tokens = msg.Split(':'); // נשבור לפי פרמטרים, פקודה ופרמטרים
            string comKey = tokens[0];
            string comVal = tokens[1];
            string msgToRet = null;
            switch (comKey)
            {
                case "DropCard":
                    msgToRet = DropCard(comVal, player);
                    break;
                case "Taki":
                    break;
                case "Disconnected":
                    msgToRet = Disconnect(comVal, player);
                    break;
                case "Win":
                    msgToRet = Win(player);
                    break;
                case "Color":
                    msgToRet = ColorChanged(comVal, player);
                    break;
                case "GiveCard":
                    msgToRet = GiveCard(comVal, player);
                    break;
                case "NextTurn":
                    msgToRet = HandleNextTurn(player);
                    break;
            }
            return Encoding.UTF8.GetBytes(msgToRet); // נחזיר מערך של בתים
        }

        private string DropCard(string card, string playerName) // נשלח לשחקן את הקלף שהוא רוצה לזרוק, ונעדכן את כולם על הקלף האחרון שיזרק
        {
            if(LastCard != null)
                Cards.AddCard(LastCard);
            LastCard = card;
            if(!IsSpecial(card)) // אם הקלף לא מיוחד וניתן אז נחליף תור לשחק השני
                CurrentPlayer = GetNextPlayer(playerName);
           
            
            return $"{playerName}:DroppedCard:{card}|:UpdateLast:{card}|:Turn:{CurrentPlayer}";
        }

        private bool IsSpecial(string card) // התור נשאר רק במקרה של שינוי צבע, טאקי או עצור
        {
            return card[0] == 'C' || card.EndsWith("stop") || card.EndsWith("plus");
        }

        private string Disconnect(string client, string playerName) // במקרה של ניתוק נודיע לשחקן השני שהמשחק נותק
        {
            string clientName = GetNextPlayer(playerName);
            string msg = $"{clientName}:Disconnect:{client}";

            writeLog($"Client {client} disconnected...");
            CloseClient(client);
            return msg;
        }

        private string Win(string player) // במקרה של ניצחון, נשלח למנצח הודעת ניצחון ולמפסיד הודעת הפסד
        {
            GameOn = false;
            string lost = GetNextPlayer(player);
            string msg = $"{player}:Win:|{lost}:Lost:";
            return msg;
        }

        private string ColorChanged(string color, string player) // אם הצבע השתנה, נעביר תור ונשלח את הצבע החדש ושל מי התור
        {
            string turn = GetNextPlayer(player);
            return $":Color:{color}:|:Turn:{turn}";
        }

        private string GiveCard(string numOfCards, string player) // הלקוח מבקש קלפים מהשרת
        {
            int cards = int.Parse(numOfCards);
            string msg = $"{player}:TakeCard:"; // נוסיף קלפים לשלוח ללקוח שביקש
            for(int i = 0; i < cards; i++)
                msg += $"{Cards.TakeCard()},";
            msg = msg.Substring(0, msg.Length - 1);
            msg += $"|:Turn:{GetNextPlayer(player)}"; // נעביר את התור לשחקן הבא
            msg += $"|:CardsTaken:{numOfCards}"; // נשלח שנלקחו קלפים
            return msg;
        }

        private string HandleNextTurn(string player) // במצב של מעבר תור, נעביר תור
        {
            return $":Turn:{GetNextPlayer(player)}";
        }

        private string GetNextPlayer(string player) // כדי למצוא את השחקן השני נעבור על רשימת השחקנים, ונמצא את מי שלא שלח את ההודעה הזאת לסרבר
        {
            foreach (string s in ClientDict.Keys)
                if (s != player)
                    return s;
            return string.Empty; // לא אמורים להגיע לפה, שגיאה
        }

        private void CloseClient(string name) // לסגור את הקליינט
        {
            foreach (ClientData cd in ClientDict.Values)
            {
                if (cd.Name == name)
                {
                    if (cd.ClientSocket.Connected) cd.ClientSocket.Close();
                    if (cd.ClientThread.IsAlive) cd.ClientThread.Abort();
                    ClientDict.Remove(name);
                }
            }
        }

        private void CloseTheThread(string name)
        {
            //This function closes the thread - the process of the client that caused the error
            try
            {
                ClientDict[name].ClientThread.Abort();
            }

            catch (Exception e)
            {
                ClientDict.Remove(name);
                writeLog("Disconnected> " + name);
            }
        }

        private void OnClosing() // זוהי פונקציה שמחסלת תהליכים היא סוגרת את הthread ואת המאזין
        {
            if (tcpLsn != null)
            { tcpLsn.Stop(); }

            foreach (ClientData cd in ClientDict.Values)
            {
                if (cd.ClientSocket.Connected) cd.ClientSocket.Close();
                if (cd.ClientThread.IsAlive) cd.ClientThread.Abort();
            }
            ClientDict.Clear();

            if (tcpThd.IsAlive) tcpThd.Abort();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            OnClosing();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            OnClosing();
        }

    }
}

