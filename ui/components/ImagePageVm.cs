using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using AvaloniaApplication1.copilot;

namespace AvaloniaApplication1.ui.components;

public partial class ImagePageVm : ObservableObject
{
    private string _imgDir = @"D:\__4_scrawl\images";
    [ObservableProperty] ObservableCollection<ImageData> _pathList = new();

    public ImagePageVm()
    {
        Task.Run(async () =>
        {
            await Task.Delay(1000);
            // dir:  
            var imgPathList = GetFilePathList("*.jpg");
            ZAsync.PostToUi(() =>
            {
                foreach (var path in imgPathList)
                {
                    _pathList.Add(new ImageData { Path = path });
                }
            });
        });
    }

    private List<string> GetFilePathList(string searchPattern = "*.*")
    {
        List<string> result = new();
        try
        {
            // 1. "*.*" 是通配符，可以改成 "*.jpg"
            // 2. SearchOption.AllDirectories 表示递归查找所有子目录
            // 3. SearchOption.TopDirectoryOnly 表示只查当前层
            var files = Directory.EnumerateFiles(_imgDir, searchPattern, SearchOption.AllDirectories);
            result.AddRange(files);
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine("没有权限访问: " + e.Message);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine("目录不存在: " + e.Message);
        }

        return result;
    }
}