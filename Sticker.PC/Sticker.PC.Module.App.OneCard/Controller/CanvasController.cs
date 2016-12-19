using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Sticker.PC.Module.App.OneCard.Controller
{
    public class CanvasController
    {
        public enum Direction
        {
            North,
            South,
            East,
            West
        }

        Canvas _canvas;

        #region Const

        const double Duration = 0.6;

        const int Deck_TOP = 389;
        const int Deck_Left = 732;

        const int SendDeck_TOP = 398;
        const int SendDeck_LEFT = 985;

        const int Screen_Width = 1920;
        const int Screen_Height = 1080;

        const int Card_Width = 204;
        const int Card_Height = 285;

        #endregion
        
        public CanvasController(Canvas canvas)
        {
            InitListBox(canvas);
        }

        public CanvasController(ContentControl contentControl)
        {
            if (contentControl.Content is Panel)
            {
                FindListBox(contentControl.Content as Panel);
            }
        }

        private void FindListBox(Panel panel)
        {
            foreach (FrameworkElement item in panel.Children)
            {
                if (item is Canvas)
                {
                    InitListBox(item as Canvas);
                    return;
                }

                if (item is Panel)
                {
                    FindListBox(item as Panel);
                }
            }
        }

        private void InitListBox(Canvas canvas)
        {
            _canvas = canvas;

            graveTopCard = null;
        }

        public Image ImageMaker(String cardName)
        {
            Image img = new Image();
            img.Width = Card_Width;
            img.Height = Card_Height;

            BitmapImage source = new BitmapImage(new Uri("/Sticker.PC.Infra;component/Resources/Images/OneCard/Cards/card_" + cardName + ".png", UriKind.Relative));
            img.Source = source;

            return img;
        }

        public void GetCard(Direction direction)
        {
            Image img = ImageMaker("bc");

            _canvas.Children.Add(img);
            Canvas.SetTop(img, Deck_TOP);
            Canvas.SetLeft(img, Deck_Left);

            TranslateTransform trans = new TranslateTransform();
            img.RenderTransform = trans;

            switch (direction)
            {
                case Direction.East:
                    {
                        DoubleAnimation anim = new DoubleAnimation(Screen_Width - Deck_Left, TimeSpan.FromSeconds(Duration));
                        trans.BeginAnimation(TranslateTransform.XProperty, anim);
                    }
                    break;
                case Direction.West:
                    {
                        DoubleAnimation anim = new DoubleAnimation(-(Deck_Left + Card_Width), TimeSpan.FromSeconds(Duration));
                        trans.BeginAnimation(TranslateTransform.XProperty, anim);
                    }
                    break;
                case Direction.South:
                    {
                        DoubleAnimation anim = new DoubleAnimation(Screen_Height - Deck_TOP, TimeSpan.FromSeconds(Duration));
                        trans.BeginAnimation(TranslateTransform.YProperty, anim);
                    }
                    break;
                case Direction.North:
                    {
                        DoubleAnimation anim = new DoubleAnimation(-(Deck_TOP + Card_Height), TimeSpan.FromSeconds(Duration));
                        trans.BeginAnimation(TranslateTransform.YProperty, anim);
                    }
                    break;
                default:
                    return;
            }

            ThreadPool.QueueUserWorkItem(cardObjectDestroyer, img);
        }

        private void cardObjectDestroyer(object state)
        {
            Thread.Sleep(TimeSpan.FromSeconds(Duration));

            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                if (state is Image && state != null)
                {
                    Image img = state as Image;

                    if (_canvas.Children.Contains(img))
                    {
                        img.Visibility = Visibility.Hidden;
                        _canvas.Children.Remove(img);
                    }
                }
            }));
        }

        public void SendCard(Direction direction, String cardName, bool isReturn = false)
        {
            int baseTop;
            int baseLeft;

            if (isReturn)
            {
                baseTop = Deck_TOP;
                baseLeft = Deck_Left;
            }
            else
            {
                baseTop = SendDeck_TOP;
                baseLeft = SendDeck_LEFT;
            }

            Image img = ImageMaker(cardName);

            _canvas.Children.Add(img);

            TranslateTransform trans = new TranslateTransform();
            img.RenderTransform = trans;

            switch (direction)
            {
                case Direction.East:
                    {
                        Canvas.SetTop(img, baseTop);
                        Canvas.SetLeft(img, Screen_Width);
                        DoubleAnimation anim = new DoubleAnimation(-(Screen_Width - baseLeft), TimeSpan.FromSeconds(Duration));
                        trans.BeginAnimation(TranslateTransform.XProperty, anim);
                    }
                    break;
                case Direction.West:
                    {
                        Canvas.SetTop(img, baseTop);
                        Canvas.SetLeft(img, -Card_Width);
                        DoubleAnimation anim = new DoubleAnimation(Card_Width + baseLeft, TimeSpan.FromSeconds(Duration));
                        trans.BeginAnimation(TranslateTransform.XProperty, anim);
                    }
                    break;
                case Direction.South:
                    {
                        Canvas.SetTop(img, Screen_Height);
                        Canvas.SetLeft(img, baseLeft);
                        DoubleAnimation anim = new DoubleAnimation(-(Screen_Height - baseTop), TimeSpan.FromSeconds(Duration));
                        trans.BeginAnimation(TranslateTransform.YProperty, anim);
                    }
                    break;
                case Direction.North:
                    {
                        Canvas.SetTop(img, -Card_Height);
                        Canvas.SetLeft(img, baseLeft);
                        DoubleAnimation anim = new DoubleAnimation(Card_Height + baseTop, TimeSpan.FromSeconds(Duration));
                        trans.BeginAnimation(TranslateTransform.YProperty, anim);
                    }
                    break;
                default:
                    return;
            }

            if(!isReturn)
            {
                Image temp = graveTopCard;
                graveTopCard = img;
                img = temp;
            }

            ThreadPool.QueueUserWorkItem(cardObjectDestroyer, img);
        }

        public void FirstCardOpen(String cardName)
        {
            Image img = ImageMaker(cardName);

            _canvas.Children.Add(img);

            TranslateTransform trans = new TranslateTransform();
            img.RenderTransform = trans;

            Canvas.SetTop(img, Deck_TOP);
            Canvas.SetLeft(img, Deck_Left);
            DoubleAnimation xanim = new DoubleAnimation(SendDeck_LEFT - Deck_Left, TimeSpan.FromSeconds(Duration/2));
            trans.BeginAnimation(TranslateTransform.XProperty, xanim);
            DoubleAnimation yanim = new DoubleAnimation(SendDeck_TOP - Deck_TOP, TimeSpan.FromSeconds(Duration/2));
            trans.BeginAnimation(TranslateTransform.YProperty, yanim);
            
            graveTopCard = img;
        }

        public Image graveTopCard = null;

        public void finish()
        {
            ThreadPool.QueueUserWorkItem(cardObjectDestroyer, graveTopCard);
        }
    }
}
