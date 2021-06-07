
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TakiClient
{
    public partial class FrmUser : Form
    {  
        public PictureBox recvpic;

        public Thread tcpThd;//טרנד של הלקוח
        public TcpClient tcpclnt;// מאזין של הלקוח
        public NetworkStream stm; //קורא רצף של מידע

        public int Port { get; set; }
        public string IpAddress { get; set; }
        public string UserName { get; set; }
        public bool ShouldConnect { get; set; }
        public int PlayerNum { get; set; }
        private PictureBox[] PictureBoxArray { get; set; }
        private List<CardImage> Cards { get; set; }
        private string LastCardName { get; set; }
        private string SelectedCardName { get; set; }
        private char CurrentColor { get; set; }
        private bool AllowedChange { get; set; }
        private int CardsToTake { get; set; }
        private int CurrentFirstIndex { get; set; }
        private bool InTaki { get; set; }
        private bool InTakeTwo { get; set; }

        public FrmUser(Form parent)
        {
            InitializeComponent();
            this.ShouldConnect = false;
            AllowedChange = true;
            InitDeck();
            ResetScreen();
        }

        private void ResetScreen() // לאפס את המסך
        {
            InvokeIfRequired(btnLogOut, c => c.Enabled = false);
            InvokeIfRequired(btnLogIn, c => c.Enabled = true);
            InvokeIfRequired(lblPlayer1, c =>
            {
                c.Text = "Player 1";
                c.BackColor = Color.Transparent;
                c.Enabled = true;
            });
            InvokeIfRequired(lblPlayer2, c =>
            {
                c.Text = "Player 2";
                c.BackColor = Color.Transparent;
                c.Enabled = true;
            });
            InvokeIfRequired(this, c => c.Text = "Taki Client");
            AllowDrop = true;
            InvokeIfRequired(btnColor, c => c.Hide());
            InvokeIfRequired(btnCloseTaki, c => c.Hide());
            InvokeIfRequired(lstColor, c => c.Hide());
            InvokeIfRequired(pbP1, c => c.Visible = false);
            InvokeIfRequired(pbP2, c => c.Visible = false);

            ClearCards();
            ChangeKeysStatus(false);
        }

        private void InitDeck() // להתחיל את הקלפים
        {
            PictureBoxArray = new PictureBox[8];
            for (int i = 1; i < 9; i++)
            {
                PictureBoxArray[i - 1] = (PictureBox)Controls.Find("pbCard" + i, true)[0]; // להוסיף את הפיקטור בוקס למערך
                InvokeIfRequired(PictureBoxArray[i - 1], (c) => // ברגע שניתן, לשנות את התמונה בפיקטור בוקס לרקע אחורי ולבטל את הלחיצה
                {
                    ((PictureBox)c).Image = strToImage("back_card");
                    ((PictureBox)c).Tag = string.Empty;
                    c.Enabled = false;
                });
            }
        }

        private void InvokeIfRequired(Control c, Action<Control> action) // אם צריך לעדכן, אז לעדכן את ה ui
        {
            if (c.InvokeRequired)
            {
                c.Invoke(new Action(() => action(c)));
            }
            else
            {
                action(c);
            }
        }
        private void btnLogIn_Click(object sender, EventArgs e)
        { 
            FrmConnect f2 = new FrmConnect(this);
            f2.ShowDialog();
            if (IpAddress != null && Port != 0 && UserName != null)
            {
                if (!ShouldConnect)
                    return;

                this.Text = UserName;//הכותרת של הטקסט תהיה השם של המשתמש
                if (!ConnectServer())
                {
                    return;
                }
                btnLogIn.Enabled = false;
                btnLogOut.Enabled = true;
            }
            else
            {
                if (ShouldConnect)
                    MessageBox.Show("לא הזנת נתוני התחברות", "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            WriteToServer($"Disconnected: {UserName}");
            CloseLogic();
            ResetScreen();
        }

        public bool ConnectServer()
        {
            try
            {
                tcpclnt = new TcpClient();//מאתחל את המאזין
                tcpclnt.Connect(IpAddress, Port);//מחברים אותו לכתובות האייפי והפורט
                stm = tcpclnt.GetStream();//ככה אנחנו מקבלים מידע מן המאזין שלנו
                WriteToServer(UserName);
                tcpThd = new Thread(new ThreadStart(ReadSocket));
                tcpThd.Start();
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("השרת לא מחובר", "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private void WriteToServer(string msg)
        {
            byte[] writeBuffer = Encoding.UTF8.GetBytes(msg);//המרה לבייט
            if (stm != null)
                stm.Write(writeBuffer, 0, writeBuffer.Length);//אם המאזין הופעל אז תשלח את המידע
        }

        public void ReadSocket()
        {
            int ret;
            while (true)// הלולאה אינסופית
            {
                try
                {
                    byte[] readBuffer = new byte[1024];//מערך מטיפוס בייט של מידע שהשרת ישלח לנו ישמר בתוך האובייקט readBuffer
                    ret = stm.Read(readBuffer, 0, readBuffer.Length);//קורא מהשרת ושומר ב readBuffer
                    string msg = Encoding.UTF8.GetString(readBuffer).Replace("\0", string.Empty);
                    string[] commands = msg.Split('|'); // לחלק לפי התו המחלק של הפקודות
                    foreach (string command in commands)
                        handleToken(command.Split(':')); // לחלק את השורה לחלקים ולשלוח לניתוח פקודה
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private void handleToken(string[] tokens)
        {
            string clients = tokens[0]; // החלק הראשון הוא מי צריך לקבל את הפקודה
            if ((clients != "" && clients != UserName) || tokens.Length < 3) // אם המשתמש השני צריך לקבל את הפקודה, לעצור כאן
                return;

            string comKey = tokens[1]; // מה הפקודה

            string comVal = tokens.Length > 2 ? tokens[2] : string.Empty; // מה הערך של הפקודה

            switch (comKey)
            {
                case "FirstPlayer":
                    HandleFirstPlayer(comVal);
                    break;
                case "UserList":
                    HandleUserListCommand(comVal);
                    break;
                case "Disconnect":
                    HandleDisconnectCommand();
                    break;
                case "CardsList":
                    HandleCardList(comVal);
                    break;
                case "DroppedCard":
                    HandleDroppedCard(comVal);
                    break;
                case "FirstCard":
                    HandleFirstCard(comVal);
                    break;
                case "UpdateLast":
                    HandleUpdateLast(comVal);
                    break;
                case "Turn":
                    HandleTurn(comVal);
                    break;
                case "Win":
                    HandleWin();
                    break;
                case "Lost":
                    HandleLost();
                    break;
                case "Color":
                    HandleColor(comVal);
                    break;
                case "TakeCard":
                    HandleTake(comVal);
                    break;
                case "CardsTaken":
                    HandleCardsTaken();
                    break;
                case "Rename":
                    HandleRename(comVal);
                    break;

            }
        }

        private void HandleFirstPlayer(string player) // שהסרבר שולח שחקן ראשון
        {
            HandleTurn(player);
            ChangeKeysStatus(player == UserName); // לשנות את סטטוס הכפתורים
            AllowedChange = true; // לאפשר להחליף תור
            CardsToTake = 1; // מספר הקלפים בלקיחה מהקופה יהיה 1
            CurrentFirstIndex = 0; // ההצגה של הקלפים מתחילה מאינדקס 0
            InTaki = false;
            InTakeTwo = false;
            foreach (PictureBox p in PictureBoxArray) // לאפשר ללחוץ על כל התמונות
            {
                InvokeIfRequired(p, c => c.Enabled = true);
            }
            FillPictureBoxes(); // למלא את התמונוץ
        }

        private void HandleUserListCommand(string userList)
        {
            string[] users = userList.Split(','); // לחלק לפי משתמשים

            InvokeIfRequired(lblPlayer1, c => c.Text = users[0]); // לעדכן את המשתמש הראשון

            if (users.Length > 1)
                InvokeIfRequired(lblPlayer2, c => c.Text = users[1]); // ואת השני אם יש שניים

            if (UserName == lblPlayer1.Text) // לחלק את מספר השחקן
                PlayerNum = 1;
            else
                PlayerNum = 2;

        }

        private void HandleDisconnectCommand() // לעדכן חזרה את צבעי הכפתורים והטקסטים, לאפס את הקלפים
        {
            InvokeIfRequired(PlayerNum == 1 ? lblPlayer2 : lblPlayer1, c => c.Text = $"Player {(PlayerNum == 1 ? 2 : 1)}");
            InvokeIfRequired(pbP1, c => c.Visible = false);
            InvokeIfRequired(pbP2, c => c.Visible = false);
            ClearCards();
        }

        private void HandleFirstCard(string card) // בקלף ראשון נשים אותו בתור הקלף האחרון שהונח
        {
            InvokeIfRequired(lastDropped, c => {
                ((PictureBox)c).Image = strToImage(card);
                c.Show();
            });
            LastCardName = card;
            CurrentColor = LastCardName[0];
        }

        private void HandleCardList(string cards) // ברגע ששולחים רשימה של קלפים, נפצל אותם לפי הפסיק ונוסיף את כולם לרשימה
        {
            string[] toks = cards.Split(',');
            foreach (string card in toks)
            {
                Cards.Add(new CardImage(card, strToImage(card)));
            }
            FillPictureBoxes(); 
        }

        private void HandleDroppedCard(string card) // ברגע שהסרבר שולח שקלף נזרק
        {
            if(card == string.Empty) // אם לא נבחר קלף שגיאה
            {
                MessageBox.Show("לא נבחר קלף", "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
                            
            for(int i = 0; i < 8 && i < Cards.Count; i++) // לחפש את הקלף שאנו רוצים לזרוק לפי הטאג
            {
                string tag = PictureBoxArray[i].Tag.ToString();
                if (tag == string.Empty) // אם הטאג ריק אז לצאת כי הקלף ריק
                    break;
                if(tag == card) // ברגע שמצאנו לאפס את הקלף
                    InvokeIfRequired(PictureBoxArray[i], c =>
                    {
                        ((PictureBox)c).Image = strToImage("back_card"); 
                        c.Tag = string.Empty;
                    });
            }
            
            RemoveCard(SelectedCardName); // להעיף את הקלף מהרשימה
            if (SelectedCardName[0] == 'C') // אם יש שינוי צבע כלשהו
                ChangeColor();
            if (SelectedCardName.EndsWith("taki")) // אם יש מצב של טאקי
                TakiCard();
            if (SelectedCardName.EndsWith("2")) // אם יש מצב של 2 פלוס
                TakeTwo();
            if (Cards.Count == 0) // אם אין קלפים ניצחנו
                WriteToServer($"Win:");
            CurrentColor = SelectedCardName[0]; // נוציא את האות הראשונה מהקלף שהיא הצבע
            SelectedCardName = string.Empty;
            FillPictureBoxes();
        }

        private void FillPictureBoxes()
        {
            ResetPictureBoxes(); // נאפס את התמונות
            RefillPictureBoxes(); // ונמלא אותן
        }

        private void TakiCard() // להודיע על מצב טאקי ולהציג כפתור סיום טאקי
        {
            InTaki = true;
            InvokeIfRequired(btnCloseTaki, c => c.Show());
        }

        private void TakeTwo() // להודיע על מצב פלוס 2
        {
            InTakeTwo = true;
            WriteToServer($"TakeTwo:{(CardsToTake > 1 ? CardsToTake : 2)}");
        }

        private void ChangeColor() // במצב של שינוי צבע, נחכה שהמשתמש יבחר לאיזה צבע לשנות
        {
            InvokeIfRequired(btnColor, c => c.Show());
            InvokeIfRequired(lstColor, c => c.Show());
            InvokeIfRequired(dropBtn, c => c.Enabled = false);
            InvokeIfRequired(this, c => MessageBox.Show("תבחר צבע שאליו תרצה להחליף"));
            AllowedChange = false;
        }

        private void HandleUpdateLast(string card) // ברגע שמעדכנים את הקלף האחרון בקופה נעדכן את התמונה
        {
            LastCardName = card;
            InvokeIfRequired(lastDropped, c => ((PictureBox)c).Image = strToImage(card));
            if (card.EndsWith("2"))
            {
                CardsToTake = CardsToTake == 1 ? 2 : CardsToTake + 2;
            }
        }

        private void EndGame(string msg)
        {
            try
            {
                ResetScreen();
                CloseLogic();
            }
            catch (Exception)
            {
            }
            finally
            {
                InvokeIfRequired(this, c => MessageBox.Show(msg));
            }
        }
        private void HandleWin() // ברגע שניצחנו
        {
            EndGame("ניצחת!");
        }

        private void HandleLost() // ברגע שהפסדנו
        {
            EndGame("הפסדת!");
        }

        private void RemoveCard(string card) // להעיף קלף מהרשימה, נחפש לפי שם ונוציא
        {
            CardImage cardToRemove = null;
            foreach(CardImage c in Cards)
            {
                if (c.CardName == card)
                    cardToRemove = c;
            }
            Cards.Remove(cardToRemove);
        }

        private void ClearCards() // ננקה את הקלפים והתמונות שלהם
        {
            Cards = new List<CardImage>();
            LastCardName = string.Empty;

            InvokeIfRequired(lastDropped, c => {
                ((PictureBox)c).Image = strToImage("back_card");
            });

            ResetPictureBoxes();
            InitDeck();
        }

        private void ResetPictureBoxes() // נאפס את הפיקטור בוקס
        {
            foreach (PictureBox p in PictureBoxArray)
            {
                InvokeIfRequired(p, c =>
                {
                    ((PictureBox)c).Image = strToImage("back_card");
                    c.Tag = string.Empty;
                    c.Enabled = false;
                });
            }
        }

        private void RefillPictureBoxes() // נמלא מחדש בצורה הבאה, נתחיל באינדקס הנוכחי שהוא ניתן לקביעה על ידי לחיצה על הכפתורים נקסט ו-פרביוס 
        {
            for(int i = CurrentFirstIndex; i < 8 + CurrentFirstIndex && i < Cards.Count; i++)
            {
                InvokeIfRequired(PictureBoxArray[i - CurrentFirstIndex], c =>
                {
                    ((PictureBox)c).Image = Cards[i].Image;
                    c.Tag = Cards[i].CardName;
                    c.Enabled = true;
                });
            }
        }


        private Image strToImage(string card) // נמיר סטרינג לתמונה
        {
            Image t = (Image)Properties.Resources.ResourceManager.GetObject(card);
            return t;
        }

        private void FrmUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseLogic();
            Environment.Exit(0);
        }

        private void CloseLogic()
        {
            if (tcpclnt != null)
            {
                tcpclnt.Close();
                tcpclnt = null;
            }
            try
            {
                if (tcpThd != null && tcpThd.IsAlive) tcpThd.Abort();
            }
            catch (Exception e)
            {
                tcpThd = null;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (stm != null)
                {
                    stm.Close();
                    stm = null;
                }
            }


        }

        private void HandleTurn(string player) // ננהל תור
        {
            if (!AllowedChange || InTaki) // אם אסור להחליף או שבטאקי לא להחליף תור
                return;
            if (UserName == player) // לשנות את הצבע של הלייבל ואת סטטוס הכפתורים
            {
                InvokeIfRequired(PlayerNum == 1 ? pbP1 : pbP2, c => c.Visible = true);
                InvokeIfRequired(PlayerNum == 1 ? pbP2 : pbP1, c => c.Visible = false);

                ChangeKeysStatus(true);
            }
            else
            {
                InvokeIfRequired(PlayerNum == 1 ? pbP2 : pbP1, c => c.Visible = true);
                InvokeIfRequired(PlayerNum == 1 ? pbP1 : pbP2, c => c.Visible = false);

                ChangeKeysStatus(false);
            }
        }


        private void HandleColor(string color) // ברגע שהסרבר שולח שהשתנה צבע נעדכן את הצבע ושאפשר להחליף תור
        {
            switch (color)
            {
                case "R":
                    CurrentColor = 'R';
                    break;
                case "B":
                    CurrentColor = 'B';
                    break;
                case "G":
                    CurrentColor = 'G';
                    break;
                case "Y":
                    CurrentColor = 'Y';
                    break;
            }
            AllowedChange = true;
        }

        private void HandleTake(string cards) // ברגע שיש לקיחת קלפים, נוסיף אותם לרשימה ונמלא את הפיקטור בוקס
        {
            string[] tokens = cards.Split(',');
            foreach(string tok in tokens)
            {
                Cards.Add(new CardImage(tok, strToImage(tok)));
            }
            FillPictureBoxes();
        }

        private void HandleCardsTaken()
        {
            CardsToTake = 1;
        }

        private void ChangeKeysStatus(bool isEnabled) // לעדכן את מצב הכפתורים
        {
            InvokeIfRequired(dropBtn, c => c.Enabled = isEnabled);
            InvokeIfRequired(btnTake, c => c.Enabled = isEnabled);
        }

        private bool DropCard() // להודיע לסרבר שאנו זורקים קלף
        {
            if (!allowedToDrop())
            {
                InvokeIfRequired(this, c => MessageBox.Show("לא ניתן לזרוק את הקלף"));
                return false;
            }
            WriteToServer($"DropCard:{SelectedCardName}");
            return true;
        }

        private bool allowedToDrop() // הלוגיקה של מה אפשר לזרוק
        {
            if (SelectedCardName == null || SelectedCardName == string.Empty)
                return false;
            string[] selectedProps = SelectedCardName.Split('_');
            string[] lastProps = LastCardName.Split('_');

            if (InTaki || (selectedProps[0] != lastProps[0] && selectedProps[1] != lastProps[1])) // אם אנחנו בטאקי, או שהקלפים שונים גם בצבע וגם במספר, נזרוק רק בתנאי שהצבע תואם למה שהשתנה אם היה שינוי צבע, או אם אנחנו זורקים משנה צבע
                return selectedProps[0][0] == 'C' || selectedProps[0][0] == CurrentColor;

            if (CardsToTake != 1)
                return selectedProps[1] == "2";
            return true; // אחרת אפשר לזרוק
        }

        private void HandleRename(string newname)
        {
            UserName = newname;
        }

        private void pbCard1_Click(object sender, EventArgs e)
        {
            SelectedCardName = PictureBoxArray[0].Tag.ToString();
        }

        private void pbCard2_Click(object sender, EventArgs e)
        {
            SelectedCardName = PictureBoxArray[1].Tag.ToString();
        }

        private void pbCard3_Click(object sender, EventArgs e)
        {
            SelectedCardName = PictureBoxArray[2].Tag.ToString();
        }

        private void pbCard4_Click(object sender, EventArgs e)
        {
            SelectedCardName = PictureBoxArray[3].Tag.ToString();
        }

        private void pbCard5_Click(object sender, EventArgs e)
        {
            SelectedCardName = PictureBoxArray[4].Tag.ToString();
        }

        private void pbCard6_Click(object sender, EventArgs e)
        {
            SelectedCardName = PictureBoxArray[5].Tag.ToString();
        }

        private void pbCard7_Click(object sender, EventArgs e)
        {
            SelectedCardName = PictureBoxArray[6].Tag.ToString();
        }

        private void pbCard8_Click(object sender, EventArgs e)
        {
            SelectedCardName = PictureBoxArray[7].Tag.ToString();
        }

        private void dropBtn_Click(object sender, EventArgs e)
        {
            DropCard();
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            if (lstColor.SelectedIndex == -1)
                return;
            string color = lstColor.GetItemText(lstColor.SelectedItem); // ניקח את הצבע הנבחר
            btnColor.Hide(); // נחביא את הכפתורים
            lstColor.Hide();
            lstColor.ClearSelected();
            dropBtn.Enabled = true;
            HandleColor(color[0].ToString()); // נשלח שאפשר להחליף צבע
            WriteToServer($"Color:{CurrentColor}"); // ונודיע לסרבר
        }

        private void btnTake_Click(object sender, EventArgs e)
        {
            WriteToServer($"GiveCard:{CardsToTake}");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (CurrentFirstIndex >= Cards.Count - 8) // אם יש פחות משמונה קלפים לא ניתן להזיז לקלפים הבאים
                return;
            CurrentFirstIndex++; // נקדם את האינדקס שממנו נציג קלפים
            FillPictureBoxes();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (CurrentFirstIndex == 0) // אם האינדקס הוא אפס לא נרד לאינדקס שלילי
                return;
            CurrentFirstIndex--;
            FillPictureBoxes();
        }

        private void btnCloseTaki_Click(object sender, EventArgs e) // כאשר נגמר טאקי, נעבור לתור הבא
        {
            if (btnColor.Visible == true)
                return;
            InTaki = false;
            btnCloseTaki.Hide();
            WriteToServer("NextTurn:");
        }
    }
}
