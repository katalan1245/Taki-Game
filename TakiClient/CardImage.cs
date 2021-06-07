using System.Drawing;

namespace TakiClient
{
    class CardImage
    {
        public string CardName { get; private set; }
        public Image Image { get; private set; }
        public CardImage(string cardName, Image image)
        {
            CardName = cardName;
            Image = image;
        }
    }
}
