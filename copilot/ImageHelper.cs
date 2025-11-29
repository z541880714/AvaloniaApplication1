using System;
using System.IO;
using Avalonia.Media.Imaging;

namespace AvaloniaApplication1.copilot;

public static class ImageHelper
{
    public static readonly Func<string, Bitmap> BitmapFactory = path =>
    {
        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        // 这个方法会立即读取流，生成 Bitmap，然后 fileStream 就会被 using 关闭 (释放文件锁)
        return Bitmap.DecodeToWidth(fileStream, 400);
    };
}