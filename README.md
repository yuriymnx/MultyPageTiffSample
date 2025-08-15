using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

public static class ListBoxExtensions
{
    public static void ScrollItemToTop(this ListBox listBox, object item)
    {
        if (item == null) return;

        listBox.Dispatcher.InvokeAsync(() =>
        {
            var listBoxItem = listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
            if (listBoxItem == null) return;

            // Находим ScrollViewer
            var scrollViewer = FindVisualChild<ScrollViewer>(listBox);
            if (scrollViewer == null) return;

            // Получаем позицию элемента относительно ListBox
            var transform = listBoxItem.TransformToAncestor(listBox);
            var position = transform.Transform(new System.Windows.Point(0, 0));

            // Смещаем ScrollViewer так, чтобы элемент оказался сверху
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + position.Y);
        }, DispatcherPriority.Loaded);
    }

    private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child is T t) return t;
            var childOfChild = FindVisualChild<T>(child);
            if (childOfChild != null) return childOfChild;
        }
        return null;
    }
}