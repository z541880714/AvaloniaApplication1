using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.vm;
// 引用这个命名空间

public class TodoItem
{
    // Guid 是 ItemKey 的最佳选择
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; }
}

// 1. 继承 ObservableObject (它帮你实现了 INotifyPropertyChanged)
public partial class MainViewModel : ObservableObject
{
    // 🔥 集合类型改为 TodoItem
    [ObservableProperty] private ObservableCollection<TodoItem> _items = new();

    public MainViewModel()
    {
        // 初始化数据
        foreach (var i in Enumerable.Range(0, 20))
        {
            _items.Add(new TodoItem { Content = i.ToString() });
        }
    }

    // 🔥 接收 TodoItem 对象作为参数
    public void RemoveItem(TodoItem item)
    {
        // ObservableCollection 精准删除这个内存地址的对象，通知 ItemsRepeater
        _items.Remove(item);
    }
}