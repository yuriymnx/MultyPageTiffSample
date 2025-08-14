using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Imaging;

namespace WpfApp4;

public partial class TiffPageViewModel : ObservableObject
{
    public int PageIndex { get; }

    [ObservableProperty]
    private BitmapImage? _image;

    public TiffPageViewModel(int pageIndex, BitmapImage image)
    {
        PageIndex = pageIndex;
        Image = image;
    }
}
