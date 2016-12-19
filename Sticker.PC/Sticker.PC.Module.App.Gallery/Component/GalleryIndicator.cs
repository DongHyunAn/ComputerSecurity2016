using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Sticker.PC.Module.App.Gallery.Component
{
    public class GalleryIndicator : ListBox
    {
        public GalleryIndicator() : base()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryIndicator), new FrameworkPropertyMetadata(typeof(GalleryIndicator)));
            this.DefaultStyleKey = typeof(GalleryIndicator);
        }
        
        public ListBox ListBoxBinder
        {
            get { return (ListBox)GetValue(ListBoxBinderProperty); }
            set { SetValue(ListBoxBinderProperty, value); }
        }

        public static readonly DependencyProperty ListBoxBinderProperty =
            DependencyProperty.Register("ListBoxBinder", typeof(ListBox), typeof(GalleryIndicator), new PropertyMetadata(null, (depobj, args) =>
            {
                GalleryIndicator fvi = (GalleryIndicator)depobj;
                ListBox fv = (ListBox)args.NewValue;

                // this is a special case where ItemsSource is set in code
                // and the associated FlipView's ItemsSource may not be available yet
                // if it isn't available, let's listen for SelectionChanged 
                fv.SelectionChanged += (s, e) =>
                {
                    fvi.ItemsSource = fv.ItemsSource;
                };

                fvi.ItemsSource = fv.ItemsSource;

                // create the element binding source
                Binding eb = new Binding();
                eb.Mode = BindingMode.TwoWay;
                eb.Source = fv;
                eb.Path = new PropertyPath("SelectedItem");

                // set the element binding to change selection when the FlipView changes
                fvi.SetBinding(GalleryIndicator.SelectedItemProperty, eb);
            }));
    }
}
