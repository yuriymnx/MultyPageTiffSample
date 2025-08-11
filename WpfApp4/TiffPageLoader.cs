using ImageMagick;
using System.IO;
using System.Runtime.Caching;
using System.Windows.Media.Imaging;

namespace WpfApp4;

public class TiffPageLoader(string filePath)
{
    private readonly MemoryCache _cache = new("TiffPages");
    private readonly int _cacheLimit = 10;

    public async Task<BitmapImage> GetPageAsync(int index)
    {
        var key = index.ToString();

        if (_cache.Contains(key))
            return (BitmapImage)_cache.Get(key);

        return await Task.Run(() =>
        {
            using var img = new MagickImage();
            img.Read(filePath, new MagickReadSettings
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

            // Кэшируем
            _cache.Add(key, bmp, DateTimeOffset.Now.AddMinutes(5));

            // Если кэш раздулся — чистим лишние
            if (_cache.GetCount() > _cacheLimit)
            {
                foreach (var item in _cache)
                {
                    _cache.Remove(item.Key);
                    break; // удаляем по одному
                }
            }

            return bmp;
        });
    }
}