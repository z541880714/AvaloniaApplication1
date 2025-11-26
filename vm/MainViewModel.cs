using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AvaloniaApplication1.viewmodel;

using CommunityToolkit.Mvvm.ComponentModel; // 引用这个命名空间

// 1. 继承 ObservableObject (它帮你实现了 INotifyPropertyChanged)
public partial class MainViewModel : ObservableObject
{
    // 2. 只需要声明私有字段，并打上标签
    [ObservableProperty] private string _message = "vmvm";

    [ObservableProperty] private ObservableCollection<string> _items = new();


    public MainViewModel()
    {
        foreach (var i in Enumerable.Range(0, 20))
        {
            _items.Add(i.ToString());
        }
    }
}