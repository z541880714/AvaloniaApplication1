using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ui.components;

/// <summary>
/// 内部状态管理 VM
/// </summary>
internal partial class AsyncImageState : ObservableObject
{
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private Bitmap? _loadedBitmap;
    [ObservableProperty] private string? _errorMessage;
}

public partial class ImageData : ObservableObject
{
    [ObservableProperty] private string _path;
}

/// <summary>
/// 高性能异步图片加载组件
/// </summary>
public class AsyncImageView : TemplatedControl
{
    // 内部状态
    private readonly AsyncImageState _state = new();

    // 用于取消上一次的加载任务
    private CancellationTokenSource? _cts;

    // 静态 HttpClient 复用 (避免端口耗尽)
    private static readonly HttpClient HttpClient = new();

    #region Avalonia Properties (对外公开的属性)

    // 1. 图片源地址 (支持 http://, https://, avares://, 本地路径)
    public static readonly StyledProperty<string?> SourceUriProperty =
        AvaloniaProperty.Register<AsyncImageView, string?>(nameof(SourceUri));

    public string? SourceUri
    {
        get => GetValue(SourceUriProperty);
        set => SetValue(SourceUriProperty, value);
    }

    public AsyncImageView BindSource(ImageData imageData)
    {
        Bind(SourceUriProperty, new Binding(nameof(ImageData.Path)) { Source = imageData });
        return this;
    }

    // 2. 加载时的占位内容 (可选)
    public static readonly StyledProperty<object?> PlaceholderContentProperty =
        AvaloniaProperty.Register<AsyncImageView, object?>(nameof(PlaceholderContent));

    public object? PlaceholderContent
    {
        get => GetValue(PlaceholderContentProperty);
        set => SetValue(PlaceholderContentProperty, value);
    }


    // 3. 加载失败时的占位内容 (可选)
    public static readonly StyledProperty<object?> ErrorContentProperty =
        AvaloniaProperty.Register<AsyncImageView, object?>(nameof(ErrorContent));

    public object? ErrorContent
    {
        get => GetValue(ErrorContentProperty);
        set => SetValue(ErrorContentProperty, value);
    }

    // 4. 图片拉伸模式
    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<AsyncImageView, Stretch>(nameof(Stretch), Stretch.Uniform);

    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    #endregion

    public AsyncImageView()
    {
        Console.WriteLine("new instance  AsyncImageView.....");
        // 监听 SourceUri 属性变化
        this.GetObservable(SourceUriProperty).Subscribe(OnSourceUriChanged);
        Template = new FuncControlTemplate<AsyncImageView>(BuildUi);
    }

    private Control BuildUi(AsyncImageView parent, INameScope scope)
    {
        // 使用 Grid 重叠布局，根据状态显示不同层
        return new Grid()
            .Children(
                // 层1: 成功显示的图片
                new Image()
                    .Stretch(new Binding(nameof(Stretch)))
                    .Source(new Binding(nameof(AsyncImageState.LoadedBitmap)) { Source = _state })
                    // 只有不加载且无错误时才显示
                    .IsVisible(new MultiBinding
                    {
                        Converter = new FuncMultiValueConverter<bool, bool>(ps =>
                        {
                            var arr = ps.ToArray();
                            return arr.All(it => !it);
                        }),
                        Bindings =
                        {
                            new Binding(nameof(AsyncImageState.IsLoading)) { Source = _state },
                            new Binding(nameof(AsyncImageState.HasError)) { Source = _state }
                        }
                    })
                ,

                // 层2: 加载中占位符
                new ContentControl()
                    .IsVisible(new Binding(nameof(AsyncImageState.IsLoading)) { Source = _state })
                    // 🔥 核心修改：把逻辑写在 Content 的绑定里
                    .Content(new Binding(nameof(PlaceholderContent)) // 注意绑定的是属性名，不是 Property 定义
                    {
                        Source = this,
                        // 转换器：如果有值就用值，没值就 new 一个默认的
                        Converter = new FuncValueConverter<object?, object>(val =>
                            val ?? new ProgressBar
                            {
                                IsIndeterminate = true,
                                Width = 30,
                                Height = 30,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            })
                    }),

                // 层3: 错误占位符
                new ContentControl()
                    .IsVisible(new Binding(nameof(AsyncImageState.HasError)) { Source = _state })
                    // 🔥 核心修改：同理
                    .Content(new Binding(nameof(ErrorContent))
                        {
                            Source = this,
                            Converter = new FuncValueConverter<object?, object>(val =>
                            {
                                if (val != null) return val;

                                // 没传自定义内容，就返回默认的报错图标
                                return new StackPanel()
                                    .HorizontalAlignment(HorizontalAlignment.Center)
                                    .VerticalAlignment(VerticalAlignment.Center)
                                    .Spacing(5)
                                    .Children(
                                        new PathIcon()
                                            .Width(24).Height(24)
                                            .Data(StreamGeometry.Parse(
                                                "M12,2C17.53,2 22,6.47 22,12C22,17.53 17.53,22 12,22C6.47,22 2,17.53 2,12C2,6.47 6.47,2 12,2M15.59,7L12,10.59L8.41,7L7,8.41L10.59,12L7,15.59L8.41,17L12,13.41L15.59,17L17,15.59L13.41,12L17,8.41L15.59,7Z"))
                                            .Foreground(Brushes.Red),
                                        new TextBlock()
                                            .Text("加载失败")
                                            .FontSize(12)
                                            .Foreground(Brushes.Gray)
                                    );
                            })
                        }
                    )
            );
    }


    // 在 AsyncImageView 类中添加：
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        // 🔥 打印出来看看，到底是啥？
        var dataType = DataContext?.GetType().Name ?? "NULL";
        var dataValue = DataContext?.ToString() ?? "NULL";

        Console.WriteLine($"[监控] DataContext 变了! dataType: {dataType}, dataValue: {dataValue}");
        if (DataContext is ImageData imageData)
        {
            Console.WriteLine($"url: {imageData.Path}");
        }
    }

    /// <summary>
    /// 当 SourceUri 发生变化时触发
    /// </summary>
    private void OnSourceUriChanged(string? newUri)
    {
        Console.WriteLine($"[AsyncImageView] OnSourceUriChanged: {newUri}");
        if (string.IsNullOrEmpty(newUri)) return;

        // 1. 取消之前的任务
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        // 2. 重置状态
        // _state.LoadedBitmap = null;
        _state.HasError = false;
        _state.ErrorMessage = null;

        if (string.IsNullOrEmpty(newUri))
        {
            _state.IsLoading = false;
            return;
        }

        // 3. 开始新的加载任务
        _state.IsLoading = true;

        Task.Run(async () =>
        {
            Bitmap? bitmap = null; // 定义在外面，方便 finally 或 catch 里处理
            try
            {
                bitmap = await LoadImageAsync(newUri, token);
                Console.WriteLine($"[AsyncImageView] Loaded bitmap token state: {token.IsCancellationRequested}");

                // 如果任务被取消了，就不要更新 UI
                if (token.IsCancellationRequested)
                {
                    bitmap?.Dispose();
                    return;
                }

                // 切回 UI 线程更新成功状态
                Dispatcher.UIThread.Post(() =>
                {
                    if (bitmap == null) return;

                    if (token.IsCancellationRequested)
                    {
                        bitmap?.Dispose();
                        return;
                    }


                    var old = _state.LoadedBitmap;
                    _state.LoadedBitmap = null; // 先摘除
                    old?.Dispose(); // 再销毁

                    _state.LoadedBitmap = bitmap;
                    _state.IsLoading = false;
                });
            }
            catch (OperationCanceledException)
            {
                bitmap?.Dispose();
            }
            catch (Exception ex)
            {
                bitmap?.Dispose();
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Exception: " + ex.Message +
                                      $"token cancelState: ${token.IsCancellationRequested}");
                }

                // 切回 UI 线程更新失败状态
                Dispatcher.UIThread.Post(() =>
                {
                    Console.WriteLine($"[AsyncImageView] Error loading {newUri}: {ex.Message}");
                    _state.ErrorMessage = ex.Message;
                    _state.HasError = true;
                    _state.IsLoading = false;
                });
            }
        }, token);
    }

    /// <summary>
    /// 核心加载逻辑 (支持网络、资源、本地文件)
    /// </summary>
    private async Task<Bitmap?> LoadImageAsync(string uri, CancellationToken token)
    {
        await Task.Delay(500, token);
        if (token.IsCancellationRequested)
        {
            return null;
        }

        // A. 处理网络图片 (http/https)
        if (uri.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            var response = await HttpClient.GetAsync(uri, token);
            response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync(token);
            return new Bitmap(stream);
        }

        // B. 处理内嵌资源 (avares://)
        if (uri.StartsWith("avares://", StringComparison.OrdinalIgnoreCase))
        {
            // 模拟一点点延迟，让 loading 效果显现 (实际使用可去掉)
            // await Task.Delay(200, token);
            using var stream = AssetLoader.Open(new Uri(uri));
            return new Bitmap(stream);
        }

        // C. 处理本地文件 (绝对/相对路径)
        // 模拟一点点延迟
        // await Task.Delay(200, token);
        if (!File.Exists(uri)) throw new FileNotFoundException($"Path not found: {uri}");

        // 使用 File.OpenRead 打开流 (比 ReadAllBytes 省内存，因为不用一次性把文件读进 C# 数组)
        using var fileStream = new FileStream(uri, FileMode.Open, FileAccess.Read, FileShare.Read);
        // 这个方法会立即读取流，生成 Bitmap，然后 fileStream 就会被 using 关闭 (释放文件锁)
        return Bitmap.DecodeToWidth(fileStream, 400);
    }
}