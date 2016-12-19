using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticker.PC.Module.App.OneCard.Class
{
    public class Deck
    {
        ObservableCollection<string> _deck;
        ObservableCollection<string> _grave;

        Random _random = new Random();

        public void MakeDefaultDeck()
        {
            _deck = new ObservableCollection<string>();
            _grave = new ObservableCollection<string>();

            string[] shape = { "s", "c", "d", "h" };
            string[] number = { "a", "2", "3", "4", "5", "6", "7", "8", "9", "x", "j", "q", "k", "a" };
            
            foreach (string shap in shape)
            {
                foreach (string num in number)
                {
                    _deck.Add(shap + num);
                }
            }

            // two joker card
            _deck.Add("j1");
            _deck.Add("j2");
        }

        public Deck()
        {
            MakeDefaultDeck();
        }

        public string getCardFromDeck()
        {
            string card = null;

            if(_deck.Count==0)
            {
                moveCardFromGraveToDeck();
            }

            int randomIndex = _random.Next(0, _deck.Count);
            card = _deck[randomIndex];
            _deck.RemoveAt(randomIndex);

            return card;
        }

        public void addToGrave(string card)
        {
            _grave.Add(card);
        }


        private void moveCardFromGraveToDeck()
        {
            int count = _grave.Count;

            if(_grave.Count==0)
            {
                MakeDefaultDeck();
            }

            for(int i=0;i<count;i++)
            {
                int randomIndex = _random.Next(0, _grave.Count);
                _deck.Add(_grave[randomIndex]);
                _grave.RemoveAt(randomIndex);
            }
        }
    }
}
