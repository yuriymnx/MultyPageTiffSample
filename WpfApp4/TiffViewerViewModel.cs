using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageMagick;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;

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

    [ObservableProperty]
    private double _zoom = 1.0;

    public TiffViewerViewModel(Stream tiffStream)
    {
        Pages = new ObservableCollection<TiffPageViewModel>();
        LoadPagesFromStream(tiffStream);
    }

    private void LoadPagesFromStream(Stream tiffStream)
    {
        try
        {
            using var images = new MagickImageCollection(tiffStream);
            
            for (int i = 0; i < images.Count; i++)
            {
                var image = images[i];
                using var ms = new MemoryStream();
                image.Format = MagickFormat.Png;
                image.Write(ms);
                ms.Position = 0;

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = ms;
                bmp.EndInit();
                bmp.Freeze();

                Pages.Add(new TiffPageViewModel(i, bmp));
            }
        }
        catch (Exception ex)
        {
            // В реальном приложении здесь должна быть обработка ошибок
            System.Diagnostics.Debug.WriteLine($"Error loading TIFF: {ex.Message}");
        }
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
        if (bmp == null) return;

        var availableWidth = Math.Max(0, viewportWidth - 30);
        if (availableWidth <= 0 || bmp.PixelWidth <= 0) return;

        Zoom = availableWidth / bmp.PixelWidth;
    }
}
