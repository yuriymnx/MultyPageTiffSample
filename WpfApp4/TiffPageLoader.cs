using ImageMagick;
using System.IO;
using System.Windows.Media.Imaging;

namespace WpfApp4;

public class TiffPageLoader
{
    public static async Task<BitmapImage> GetPageFromStreamAsync(Stream stream, int index)
    {
        return await Task.Run(() =>
        {
            using var img = new MagickImage();
            stream.Position = 0;
            img.Read(stream, new MagickReadSettings
            {
                FrameIndex = (uint)index,
                Density = new Density(96, 96)
            });

            using var ms = new MemoryStream();
            img.Format = MagickFormat.Png;
            img.Write(ms);
            ms.Position = 0;

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.StreamSource = ms;
            bmp.EndInit();
            bmp.Freeze();

            return bmp;
        });
    }
}