using System;
using AvaloniaApplication1.native;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ui.pages.test;

public class TestVm : ObservableObject
{
    public void TestNativeMethod()
    {
        string imgPath = @"assets\imgs\001.jpg";
        var ret = RustLibNativeMethods.resize_image_keep_aspect(imgPath, 400);
        Console.WriteLine($"img size {ret.Width}x{ret.Height}");
        RustLibNativeMethods.free_image_result(ret.DataPtr, ret.DataLen);
    }
}