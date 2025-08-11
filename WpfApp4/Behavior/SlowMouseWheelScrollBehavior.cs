using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp4.Behavior;

public class SlowMouseWheelScrollBehavior : Behavior<ListBox>
{
    public double ScrollFactor { get; set; } = 0.0099;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
    }

    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var scrollViewer = FindScrollViewer(AssociatedObject);
        if (scrollViewer == null) return;

        // Оригинальный дельта
        double delta = e.Delta * ScrollFactor;

        // Прокрутка на уменьшенный шаг
        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - delta);

        // Отменяем стандартную прокрутку
        e.Handled = true;
    }

    private ScrollViewer? FindScrollViewer(DependencyObject d)
    {
        if (d is ScrollViewer sv) return sv;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
        {
            var child = VisualTreeHelper.GetChild(d, i);
            var result = FindScrollViewer(child);
            if (result != null)
                return result;
        }
        return null;
    }
}
