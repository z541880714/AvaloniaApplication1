using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ui.pages.main;
// 引用这个命名空间

public partial class TodoItem : ObservableObject
{
    // 3. 把 Content 变成受监控的属性
    [ObservableProperty] private int _content;
}

// 1. 继承 ObservableObject (它帮你实现了 INotifyPropertyChanged)
public partial class GridListComponentVm : ObservableObject
{
    // 🔥 集合类型改为 TodoItem
    [ObservableProperty] private ObservableCollection<TodoItem> _items = new();

    public GridListComponentVm()
    {
        // 初始化数据
        foreach (var i in Enumerable.Range(0, 20))
        {
            var item = new TodoItem { Content = i };
            Items.Add(item);
        }
    }

    // 🔥 接收 TodoItem 对象作为参数
    public void RemoveItem(TodoItem item)
    {
        // ObservableCollection 精准删除这个内存地址的对象，通知 ItemsRepeater
        Items.Remove(item);
    }

    public void UpdateItem(TodoItem item)
    {
        item.Content++;
    }
}