using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TakiServer
{
    public class Cards
    {
        private List<string> cardsList = new List<string>();
        public int Length
        {
            get
            {
                return cardsList.Count;
            }
        }
        int nextCard = 0;
        public Cards()
        {
            foreach (char c in "BGRY")
            {
                for (int i = 1; i < 10; i++)
                    cardsList.Add($"{c}_{i}");
                cardsList.Add($"{c}_10_dir");
                cardsList.Add($"{c}_11_plus");
                cardsList.Add($"{c}_12_stop");
                cardsList.Add($"{c}_13_taki");
            }
            cardsList.Add($"C_14_change");
            cardsList.Add($"C_14_taki");

            shuffle();
        }

        private void shuffle()
        {
            Random rnd = new Random();
            int x;
            string s;
            for (int i = 0; i < cardsList.Count; i++)
            {
                x = rnd.Next(0, cardsList.Count);
                s = cardsList[x];
                cardsList[x] = cardsList[i];
                cardsList[i] = s;
            }
        }

        public string TakeCard()
        {
            if (nextCard < cardsList.Count)
            {
                string card = cardsList.First();
                cardsList.Remove(card);
                return card;
            }
            else
            {
                MessageBox.Show("אין יותר קלפים בחפיסה");
                return null;
            }
        }

        public void AddCard(string card)
        {
            cardsList.Add(card);
        }


        public bool isNumber(string card)
        {
            string[] props = card.Split('_');
            if (props.Length > 2)
                return false;
            int num = int.Parse(props[1]);
            return num < 10 && num != 2;
        }
    }
}
