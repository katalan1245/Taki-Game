using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TakiClient
{
    class AllPlayer
    {
        int i = 0, j = 0;
        player[] allPlayer = new player[3];  // מערך בגודל 3 כאשר המקום האפס ריק 
        FrmUser frm;
        deck d;

        internal void setPlayer(int num, string name) // ליצור שחקן
        {
            allPlayer[num] = new player(num, name);
        }

        public AllPlayer(FrmUser frm)
        {
            this.frm = frm;
        }

        internal void setCardsToHand(int playernum, string str) // מוסיף קלף לכל שחקן
        {
            string[] st = str.Split(' ');
            for (i = 0; i <= allPlayer.Length; i++)
            {
                for (j = 0; j < st.Length; j++)
                {
                    allPlayer[i].MyHand.Add(d.GetCard(st[j]));
                }

            }

        }
        int x = 10, y = 10;
        public void picDef(Image cardPic, int playernum, int i, int y, int x)
        {
            allPlayer[playernum].MyHand[i].Getcardpic = new PictureBox();
            allPlayer[playernum].MyHand[i].Getcardpic.Image = cardPic;
            allPlayer[playernum].MyHand[i].Getcardpic.Location = new Point(x, y);
            allPlayer[playernum].MyHand[i].Getcardpic.Size = new Size(100, 100);
            allPlayer[playernum].MyHand[i].Getcardpic.SizeMode =
            PictureBoxSizeMode.StretchImage;
            allPlayer[playernum].MyHand[i].Getcardpic.BorderStyle =
            BorderStyle.Fixed3D;
            allPlayer[playernum].MyHand[i].Getcardpic.Tag = "" ;
            frm.Controls.Add(allPlayer[playernum].MyHand[i].Getcardpic);
        }
    }
}
