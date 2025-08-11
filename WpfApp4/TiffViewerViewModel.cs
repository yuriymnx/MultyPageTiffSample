using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageMagick;
using System;
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

    [ObservableProperty]
    private double _zoom = 1.0;

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

    [RelayCommand]
    private void ZoomIn()
    {
        const double zoomStepFactor = 1.25; // +25%
        const double maxZoom = 10.0;
        Zoom = Math.Min(Zoom * zoomStepFactor, maxZoom);
    }

    [RelayCommand]
    private void ZoomOut()
    {
        const double zoomStepFactor = 1.25; // -20% (inverse of +25%)
        const double minZoom = 0.05;
        Zoom = Math.Max(Zoom / zoomStepFactor, minZoom);
    }

    [RelayCommand]
    private void ResetZoom()
    {
        Zoom = 1.0;
    }

    [RelayCommand]
    private void FitToWidth(double viewportWidth)
    {
        if (Pages.Count == 0) return;

        var current = Pages[CurrentPage];
        var bmp = current.Image;
        if (bmp == null) return; // page not yet loaded

        var availableWidth = Math.Max(0, viewportWidth - 30);
        if (availableWidth <= 0 || bmp.PixelWidth <= 0) return;

        Zoom = availableWidth / bmp.PixelWidth;
    }
}
