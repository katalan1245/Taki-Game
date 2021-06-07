using System;
using System.Drawing;
using System.Windows.Forms;

namespace TakiClient
{
    class Cards
    {
        int num;
        string color;
        string letternum;
        string cardname;
        string info;

        PictureBox cardpic;
        Image cardimg;

        int i;
        int x;
        int y;

        public int Num
        {
            get
            {
                return num;
            }

            set
            {
                num = value;
            }
        }
        public string Color
        {
            get // מחזיר צבע הקלף
            {
                return color;
            }

            set // מגדיר צבע הקלף
            {
                color = value;
            }
        }
        public string Letternum
        {
            get
            {
                return letternum;
            }

            set
            {
                letternum = value;
            }
        }

        public object GetCardpic { get; internal set; }
        public PictureBox Getcardpic { get; internal set; }

        internal Control Cardpic()
        {
            throw new NotImplementedException();
        }
        /*
        public PictureBox GetCardpic()
        {
            return cardpic;
        }*/

        public void SetCardpic(PictureBox value)
        {
            cardpic = value;
        }
        public Cards(int num, string color, Image cardimg, int i, int x, int y) // פעולה המגדירה את המספר והצבע של כל קלף, ובנוסף מגדירה את הקלפים המיוחדים למספרים
        {
            this.Num = num;
            this.Color = color;
            if (num == 10) letternum = "directionchanger";
            if (num == 11) letternum = "plus";
            if (num == 12) letternum = "stop";
            if (num == 13) letternum = "taki";
            if (num == 14) letternum = "colorchanger";
            if (num == 15) letternum = "supertaki";
            if (num == 16) letternum = "king";
            cardname = color + num + "";
            this.cardimg = cardimg;
            this.i = i;
            this.x = x;
            this.y = y;
            picdef();
        }
        public void picdef() // פונקציה הקובעת את תכונות הקלף
        {
            cardpic = new PictureBox();
            cardpic.Image = cardimg;
            cardpic.Location = new System.Drawing.Point(x, y);
            cardpic.Size = new System.Drawing.Size(10, 20);
            cardpic.BorderStyle = BorderStyle.FixedSingle;
            cardpic.Tag = cardimg;
        }
    }
}