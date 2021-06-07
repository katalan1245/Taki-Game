using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TakiClient
{
    class player
    {
        List<Cards> myhand = new List<Cards>();
        int playerNum;
        string playerName;
        int score;
        int numCards;
        Image playerImage;
        int whichRound;

        public player(int num, string name, int score, int numcards, int whichround) // מגדירה תכונות של שחקו
        {
            this.MyHand = MyHand;
            this.PlayerNum = num;
            this.PlayerName = name;
            this.Score = score;
            this.NumCards = numcards;
            this.WhichRound = whichround;
        }

        public void ShowCards(FrmUser frm)
        {
            int x = 600, y = 750;
            for (int i = 0; i < 4; i++)
{
                frm.Controls.Add(this.MyHand[i].Cardpic());
                this.MyHand[i].Cardpic().Location = new Point(x, y);
                x += 170;
            }
        }
        public void ShowOponent(FrmUser frm)
        {
            int x = 600, y = 10;
            PictureBox[] empty = new PictureBox[4];
            for (int i = 0; i < 4; i++)
{
                empty[i] = new PictureBox();
                picdef(empty[i]);
                frm.Controls.Add(empty[i]);
                empty[i].Location = new Point(x, y);
                x += 170;
            }
        }

        private void picdef(PictureBox pictureBox) // קובע תכונות לתמונה של סיטי
        {
            throw new NotImplementedException();
        }

        public int PlayerNum { get => playerNum; set => playerNum = value; } // קביעת פעולות לשחקן
        public string PlayerName { get => playerName; set => playerName = value; }
        public int Score { get => score; set => score = value; }
        public int NumCards { get => numCards; set => numCards = value; }
        public int WhichRound { get => whichRound; set => whichRound = value; }
        internal List<Cards> MyHand { get => MyHand; set => MyHand = value; }

        public player(int num, string name)
        {
            this.playerNum = num;
            this.playerName = name;
        }
    }
}