using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using AvaloniaApplication1.copilot;

namespace AvaloniaApplication1.ui.components;

public partial class ImageData : ObservableObject
{
    [ObservableProperty] private string _path;
    [ObservableProperty] private string _name;


    public override string ToString()
    {
        return Path;
    }
}

public partial class ImagePageVm : ObservableObject
{
    private string _imgDir = Path.Combine(ZEnv.ProjectRoot.FullName, "res", "images");
    [ObservableProperty] ObservableCollection<ImageData> _pathList = new();
    private readonly Channel<bool> _signalChannel = Channel.CreateBounded<bool>(1);

    public ImagePageVm()
    {
        Task.Run(InitializeAsync);
    }

    private async Task InitializeAsync()
    {
        var imgPathList = GetFilePathList("*.jpg");
        // 切回 UI 线程添加数据
        ZAsync.PostToUi(() =>
        {
            foreach (var path in imgPathList)
            {
                PathList.Add(new ImageData { Path = path, Name = Path.GetFileNameWithoutExtension(path) });
            }

            _signalChannel.Writer.TryWrite(true);
        });
        await _signalChannel.Reader.ReadAsync();
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