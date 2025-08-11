using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Imaging;

namespace WpfApp4;

public partial class TiffPageViewModel(TiffPageLoader loader, int pageIndex) : ObservableObject
{
    public int PageIndex { get; } = pageIndex;

    [ObservableProperty]
    private BitmapImage? _image;

    public async Task LoadAsync()
    {
        Image ??= await loader.GetPageAsync(PageIndex);
    }
}
