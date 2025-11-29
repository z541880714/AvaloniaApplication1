using System;
using System.Collections.Generic;
using Avalonia.Controls;
using AvaloniaApplication1.ui.pages.main;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.global;

using MenuAction = Func<Control>;

public partial class NavNode : ObservableObject
{
    public readonly string Id = Guid.NewGuid().ToString();
    public readonly string Name;
    public readonly MenuAction? Action;
    public readonly bool IsMain;
    public List<NavNode> Children;
    public bool HasStretchIcon => Children.Count > 0;

    [ObservableProperty] private bool _stretchState = false;

    public NavNode(string name,
        bool isMain = false,
        MenuAction? action = null,
        List<NavNode>? children = null)
    {
        Name = name;
        Action = action;
        Children = children ?? [];
        IsMain = isMain;
    }
}

public partial class ZNavigator : ObservableObject
{
    public static ZNavigator Instance { get; } = new();

    [ObservableProperty] private Control _currentView;

    // 1. 历史栈 (依然保留，用于 Push/Pop 的详情页后退功能)
    private readonly Stack<Control> _history = new();

    // 2. 【核心新增】页面缓存池 (Keep-Alive)
    // Key: 页面类型 (Type) -> Value: 页面实例 (Control)
    private readonly Dictionary<string, Control> _viewCache = new();


    private ZNavigator()
    {
        CurrentView = Router.FindMainControl();
    }

    // --- 场景 A & B: 详情页堆栈 (保持不变) ---
    // 详情页通常不需要缓存，因为每次点开的数据可能不一样
    public void Push(Control newView)
    {
        if (CurrentView != null) _history.Push(CurrentView);
        CurrentView = newView;
    }

    public void Pop()
    {
        if (_history.Count > 0) CurrentView = _history.Pop();
    }

    // --- 场景 C: 侧边栏切换 (带缓存功能的 SwitchTo) ---

    // 用法: SwitchTo<HomeComponent>(() => new HomeComponent());
    // 泛型 T: 你要跳转的页面类型
    // factory: 如果缓存里没有，用来创建新页面的工厂函数
    public void SwitchTo<T>(Func<T> factory) where T : Control
    {
        var key = typeof(T).Name;
        SwitchTo(factory, key);
    }

    public void SwitchTo(Func<Control> factory, string navKey)
    {
        // 1. 切 Tab 时，通常要清空详情页的历史栈
        _history.Clear();


        // 2. 【核心逻辑】检查缓存
        if (!_viewCache.TryGetValue(navKey, out var targetView))
        {
            // 没找到：创建新的，并存入缓存
            System.Diagnostics.Debug.WriteLine($"[Cache] 创建新实例: {navKey}");
            targetView = factory();
            _viewCache[navKey] = targetView;
        }

        // 3. 切换显示
        CurrentView = targetView;
    }


    // 可选：如果你想强制刷新某个页面 (清除缓存)
    public void ClearCache<T>()
    {
        _viewCache.Clear();
    }
}