using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageMagick;
using System.Collections.ObjectModel;

namespace WpfApp4;
public partial class TiffViewerViewModel : ObservableObject
{
    public ObservableCollection<TiffPageViewModel> Pages { get; }

    [ObservableProperty]
    private int _currentPage;

    partial void OnCurrentPageChanged(int oldValue, int newValue)
    {
        OnPropertyChanged(nameof(DisplayCurrentPage));
    }

    public int DisplayCurrentPage
    {
        get => CurrentPage + 1;
        set
        {
            if (value < 1) value = 1;
            if (value > Pages.Count) value = Pages.Count;

            if (CurrentPage != value - 1)
            {
                CurrentPage = value - 1;
                OnPropertyChanged(nameof(DisplayCurrentPage));
            }
        }
    }

    public int TotalPages => Pages.Count;

    private readonly TiffPageLoader _loader;

    public TiffViewerViewModel(string filePath)
    {
        _loader = new TiffPageLoader(filePath);

        using var images = new MagickImageCollection(filePath);
        Pages = [];

        for (int i = 0; i < images.Count; i++)
            Pages.Add(new TiffPageViewModel(_loader, i));
    }

    public async Task LoadVisiblePagesAsync(int first, int last)
    {
        CurrentPage = first;
        for (int i = first; i <= last && i < Pages.Count; i++)
            await Pages[i].LoadAsync();
    }

    [RelayCommand]
    private void NextPage()
    {
        if (CurrentPage < Pages.Count - 1)
            CurrentPage++;
    }

    [RelayCommand]
    private void PrevPage()
    {
        if (CurrentPage > 0)
            CurrentPage--;
    }
}
