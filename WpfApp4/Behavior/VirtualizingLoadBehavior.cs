using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp4.Behavior;

public class VirtualizingScrollPaginationBehavior : Behavior<ListBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var scrollViewer = FindScrollViewer(AssociatedObject);
        if (scrollViewer != null)
        {
            scrollViewer.ScrollChanged += async (_, __) => await OnScrollChangedAsync();
        }
    }

    private async Task OnScrollChangedAsync()
    {
        if (AssociatedObject.DataContext is not TiffViewerViewModel vm)
            return;

        var scrollViewer = FindScrollViewer(AssociatedObject);
        if (scrollViewer?.Content is ItemsPresenter presenter)
        {
            presenter.ApplyTemplate();

            if (VisualTreeHelper.GetChild(presenter, 0) is VirtualizingStackPanel panel)
            {
                int firstIndex = (int)panel.VerticalOffset;
                int visibleCount = (int)panel.ViewportHeight + 2; // запас
                int lastIndex = firstIndex + visibleCount;

                await vm.LoadVisiblePagesAsync(firstIndex, lastIndex);
            }
        }
    }

    private static ScrollViewer? FindScrollViewer(DependencyObject d)
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

