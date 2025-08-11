using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp4.Behavior;

public class ScrollToIndexBehavior : Behavior<ListBox>
{
    public int Index
    {
        get => (int)GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }

    public static readonly DependencyProperty IndexProperty =
        DependencyProperty.Register(nameof(Index), typeof(int), typeof(ScrollToIndexBehavior),
            new PropertyMetadata(-1, OnIndexChanged));

    private static void OnIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ScrollToIndexBehavior behavior && behavior.AssociatedObject != null)
        {
            var idx = (int)e.NewValue;
            if (idx >= 0 && idx < behavior.AssociatedObject.Items.Count)
                behavior.AssociatedObject.ScrollIntoView(behavior.AssociatedObject.Items[idx]);
        }
    }
}
