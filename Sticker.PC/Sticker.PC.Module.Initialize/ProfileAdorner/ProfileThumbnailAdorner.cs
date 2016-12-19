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

namespace Sticker.PC.Module.Initialize.ProfileAdorner
{
    public class ProfileThumbnailAdorner : Adorner
    {
        private VisualCollection _Visuals;
        private ContentPresenter _ContentPresenter;

        private Grid _selection;
        public Grid Selection
        {
            get
            {
                return _selection;
            }
        }

        private Grid _decision;
        public Grid Decision
        {
            get
            {
                return _decision;
            }
        }

        #region Property
        protected override int VisualChildrenCount
        {
            get
            {
                return _Visuals.Count;
            }
        }

        public object Content
        {
            get
            {
                return _ContentPresenter.Content;
            }
            set
            {
                _ContentPresenter.Content = value;
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ProfileThumbnailAdorner), new PropertyMetadata(new PropertyChangedCallback(IsSelectedChanged)));

        private static void IsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProfileThumbnailAdorner dObj = d as ProfileThumbnailAdorner;
            if ((bool)e.NewValue)
            {
                dObj.Selection.Visibility = Visibility.Visible;
            }
            else
            {
                dObj.Selection.Visibility = Visibility.Hidden;
            }
        }

        public object IsFinalDecision
        {
            get { return (object)GetValue(IsFinalDecisionProperty); }
            set { SetValue(IsFinalDecisionProperty, value); }
        }

        public static readonly DependencyProperty IsFinalDecisionProperty =
            DependencyProperty.Register("IsFinalDecision", typeof(object), typeof(ProfileThumbnailAdorner), new PropertyMetadata(new PropertyChangedCallback(IsFinalDecisionChanged)));

        private static void IsFinalDecisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProfileThumbnailAdorner dObj = d as ProfileThumbnailAdorner;
            if ((string)e.NewValue=="D")
            {
                dObj.Decision.Visibility = Visibility.Visible;
            }
            else
            {
                dObj.Decision.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        public ProfileThumbnailAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _Visuals = new VisualCollection(this);
            _ContentPresenter = new ContentPresenter();
            _Visuals.Add(_ContentPresenter);
        }

        public ProfileThumbnailAdorner(UIElement adornedElement, Uri ProfileUri) : this(adornedElement)
        {
            _selection = new Grid();

            Image selectionBox = new Image();
            selectionBox.Source = LoadSourceFormUri(new Uri(@"/Sticker.PC.Infra;component/Resources/Images/Profile/img_thumbBox_glow.png", UriKind.Relative));
            selectionBox.Width = 292;
            selectionBox.Height = 292;
            selectionBox.Stretch = System.Windows.Media.Stretch.Uniform;

            Image thumbnail = new Image();
            thumbnail.Source = LoadSourceFormUri(new Uri(@ProfileUri.OriginalString.Replace(".png", "_shadow.png"), UriKind.Relative));
            thumbnail.Width = 130;
            thumbnail.Height = 175;
            thumbnail.Stretch = System.Windows.Media.Stretch.Uniform;

            _selection.Children.Add(selectionBox);
            _selection.Children.Add(thumbnail);

            _decision = new Grid();

            Image decisionBack = new Image();
            decisionBack.Source = LoadSourceFormUri(new Uri(@"/Sticker.PC.Infra;component/Resources/Images/Profile/img_thumbBox_select.png", UriKind.Relative));
            decisionBack.Width = 268;
            decisionBack.Height = 268;
            decisionBack.Stretch = System.Windows.Media.Stretch.Uniform;

            Image thumbnail_dropshadow = new Image();
            thumbnail_dropshadow.Source = LoadSourceFormUri(new Uri(@ProfileUri.OriginalString.Replace(".png", "_shadow.png"), UriKind.Relative));
            thumbnail_dropshadow.Width = 130;
            thumbnail_dropshadow.Height = 175;
            thumbnail_dropshadow.Stretch = System.Windows.Media.Stretch.Uniform;

            _decision.Children.Add(decisionBack);
            _decision.Children.Add(thumbnail_dropshadow);

            _selection.Visibility = Visibility.Hidden;
            _decision.Visibility = Visibility.Hidden;

            Grid allContent = new Grid();

            _selection.VerticalAlignment = VerticalAlignment.Center;
            allContent.Children.Add(_selection);
            allContent.Children.Add(_decision);

            Content = allContent;
        }

        private BitmapImage LoadSourceFormUri(Uri uri)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = uri;
            img.EndInit();

            return img;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _ContentPresenter.Measure(constraint);
            return _ContentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _ContentPresenter.Arrange(new Rect(-35, -35,
                 finalSize.Width, finalSize.Height));

            return _ContentPresenter.RenderSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _Visuals[index];
        }
    }
}
