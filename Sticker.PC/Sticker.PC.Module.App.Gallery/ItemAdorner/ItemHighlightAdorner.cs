using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sticker.PC.Module.App.Gallery.ItemAdorner
{
    public class ItemHighlightAdorner : Adorner
    {
        VisualCollection _visualCollection;
        Image _highlightImage;

        public enum AdornerType
        {
            Main,
            Detail
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ItemHighlightAdorner), new PropertyMetadata(new PropertyChangedCallback(IsSelectedChanged)));

        private static void IsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue)
            {
                ItemHighlightAdorner adorner = d as ItemHighlightAdorner;
                adorner.Visibility = Visibility.Visible;
            }
            else
            {
                ItemHighlightAdorner adorner = d as ItemHighlightAdorner;
                adorner.Visibility = Visibility.Collapsed;
            }
        }

        AdornerType _adornerType;

        public ItemHighlightAdorner(UIElement adornedElement, AdornerType type) : base(adornedElement)
        {
            _adornerType = type;

            _highlightImage = new Image();
            _visualCollection = new VisualCollection(this);

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            switch (_adornerType)
            {
                case AdornerType.Main:
                    img.UriSource = new Uri(@"/Sticker.PC.Infra;component/Resources/Images/Gallery/img_over.png", UriKind.Relative);
                    break;
                case AdornerType.Detail:
                    img.UriSource = new Uri(@"/Sticker.PC.Infra;component/Resources/Images/Gallery/img_over_fullscreen.png", UriKind.Relative);
                    break;
                default:
                    break;
            }
            
            img.EndInit();
            _highlightImage.Source = img;

            _highlightImage.Stretch = Stretch.Fill;
            this.Visibility = Visibility.Collapsed;

            _visualCollection.Add(_highlightImage);
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _visualCollection.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visualCollection[index];
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(drawingContext);
            //Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
            //drawingContext.DrawRectangle(null, new Pen(Brushes.Blue, 1), adornedElementRect);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            switch (_adornerType)
            {
                case AdornerType.Main:
                    {
                        _highlightImage.Width = 282;
                        _highlightImage.Height = 282;
                        _highlightImage.Arrange(new Rect(-15, -10, 282, 282));
                    }
                    break;
                case AdornerType.Detail:
                    {
                        _highlightImage.Width = 197;
                        _highlightImage.Height = 197;
                        _highlightImage.Arrange(new Rect(8, 23, 197, 197));
                    }
                    break;
                default:
                    break;
            }
            
            return finalSize;
        }
    }
}

