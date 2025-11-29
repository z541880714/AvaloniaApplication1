using System;
using System.Collections.Generic;
using Avalonia.Controls;
using AvaloniaApplication1.ui.components;
using CommunityToolkit.Mvvm.ComponentModel;
using global::AvaloniaApplication1.global;

namespace AvaloniaApplication1.ui.pages.main;

public partial class SideMenuVm : ObservableObject
{
    [ObservableProperty] private string _selectItemId = "";
}