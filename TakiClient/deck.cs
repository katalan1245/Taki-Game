using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakiClient
{
    class deck
    {
        Cards[] allcards = new Cards[55];
        int k = 0;
        FrmUser frm;
        public deck(FrmUser frm) // פעולה שמכניסה את הקלפים למערכים לפי סוגים
        {
            this.frm = frm;
            for(int i=1; i<13;i++)
            {
                allcards[k] = new Cards(i, "green", (Image) Properties.Resources.ResourceManager.GetObject("green" + i), k, 10 * i, 10);
                k++;
            }
            for (int i = 1; i < 13; i++)
            {
                allcards[k] = new Cards(i, "blue", (Image)Properties.Resources.ResourceManager.GetObject("blue" + i), k, 10 * i, 10);
                k++;
            }
            for (int i = 1; i < 13; i++)
            {
                allcards[k] = new Cards(i, "red", (Image)Properties.Resources.ResourceManager.GetObject("red" + i), k, 10 * i, 10);
                k++;
            }
            for (int i = 1; i < 13; i++)
            {
                allcards[k] = new Cards(i, "yellow", (Image)Properties.Resources.ResourceManager.GetObject("yellow" + i), k, 10 * i, 10);
                k++;
            }
            showcardonform();
        }
        public void showcardonform() // מראה את הקלף על המסך
        {
            for(int i=0; i<allcards.Length; i++)
            {
                if (allcards[i] != null)
                    frm.Controls.Add(allcards[i].Getcardpic);
            }
                    
        }
        public void TurnNull(int i) //  פעולה שמקבלת מיקום במערך ושמה בו
        {
            allcards[i] = null;
        }

        public Cards GetCard(string CardName) // 
        {
            Cards c = null;
            for(int i=0; i<allcards.Length;i++)
            {
                if (allcards[i].Letternum.Equals(CardName))
                    c = allcards[i];
            }
            return c;
        }
    }
}
