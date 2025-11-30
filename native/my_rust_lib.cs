using System;
using System.Runtime.InteropServices;

namespace AvaloniaApplication1.native;

[StructLayout((LayoutKind.Sequential))]
public struct ImageResult
{
    public IntPtr DataPtr;
    public int DataLen;
    public int Width;
    public int Height;
}

public static partial class RustLibNativeMethods
{
    private const string LibName = "my_rust_lib";

    [LibraryImport(LibName)]
    public static partial ImageResult resize_image_keep_aspect(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string path,
        uint targetWidth
    );


    [LibraryImport(LibName)]
    public static partial void free_image_result(IntPtr ptr, int len);
}