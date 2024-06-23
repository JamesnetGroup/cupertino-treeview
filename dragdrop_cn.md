## 在 TreeView 控件中通过Drag Drop移动位置（With Cupertino TreeView)
![image](https://github.com/vickyqu115/cupertino-treeview/assets/101777355/bb6f9186-65d0-48d8-8a1c-fa6edaf12146)

本文将解释在 TreeView 控件中进行Drag Drop操作的过程。内容基于我们的第六个 WPF 教程视频 "Cupertino TreeView" 的源代码，因此建议先查看 GitHub 上公开的 cupertino-treeview 源代码。另外如果你还没有观看教程视频，可以通过 [BiliBili](https://www.bilibili.com/video/BV1xz42187wV) 免费观看。

### WPF 中的Drag Drop与 TreeView
WPF 对所有 UI 元素的Drag Drop设计非常灵活。虽然本文以 TreeView 控件为例，但内容同样适用于其他情况。

TreeView 控件通常涉及将选中的项移动到目标项的子级。因此，除了处理简单的拖放操作外，将拖动的项从其父级中移除并添加到新的目标项中也是关键所在。

### AllowDrop
首先要设置的是 AllowDrop 属性。这个 Boolean 类型的属性由 UIElement 控件继承提供。因此，所有继承 UIElement 的类都可以提供拖放功能。

我们需要在作为拖放实际目标的 TreeViewItem 类中添加这个属性。在本教程中，我们将在 CupertinoTreeItem 的构造函数中添加 AllowDrop 设置。

_CupertinoTreeItem.cs, Constructor_

```csharp
public CupertinoTreeItem()
{
    ...
    AllowDrop = true;
}
```
如果所有处理都进行了，但是仍然无法实现Drag Drop，建议可以查看这个属性的设置是否正确。

### MouseMove 事件（定义拖动目标）

在 MouseMove 事件中，我们将实现“检测拖动目标对象的拖动并指定拖动数据”的逻辑。

这个事件会在鼠标光标进入该对象内部时触发，因此只有当 ‘e.LeftButton’ 状态为 ‘Pressed’ 的时候，才认为是正在拖动该对象，并通过 ‘DragDrop.DoDragDrop’ 方法定义拖动目标对象的信息。

_CupertinoTreeItem.cs, Constructor_

```
public CupertinoTreeItem()
{
    ... 
    MouseMove += CupertinoTreeItem_MouseMove;
}
```

_CupertinoTreeItem.cs_

```csharp
private void CupertinoTreeItem_MouseMove(object sender, MouseEventArgs e)
{
    if (e.LeftButton == MouseButtonState.Pressed)
    {
        var draggedItem = sender as CupertinoTreeItem;
        if (draggedItem != null)
        {
            DragDrop.DoDragDrop(draggedItem, 
                new DataObject(typeof(CupertinoTreeItem), draggedItem), DragDropEffects.Move);

            e.Handled = true; 
        }
    }
}
```
定义 DataObject 时，比起只指定所需的数据，将自身（this）作为数据传递更为有效。这是因为在拖放过程中需要找到拖动目标的父级，并且实现从父级中移除相应 TreeViewItem的功能。所以在定义 ‘DataObject’ 的时候，与其定义绑定数据，不如传递 ‘sender’ 或者 ‘this’ 即 ‘CupertinoTreeItem’，因为这样可以通过 ‘DataContext’ 找到绑定的数据，这种方式既高效又实用。

### 阻止事件冒泡
由于 WPF 的事件特性，‘MouseMove’ 事件会传递到上层的 ‘TreeViewItem’，最终会执行最上层 ’TreeViewItem‘ 对象的 ’MouseMove‘ 事件。因此，在拖动事件正常处理后，需要通过设置 ‘e.Handled = true’; 来立即停止事件的冒泡。

在 CupertinoTreeItem.cs 中的 MouseMove 方法中添加：

```csharp
e.Handled = true;
```
如果忽略了这一点，不论拖动哪个对象，最终都会选择最上层的 CupertinoTreeItem。当然在了解事件冒泡事件的特性后，你就能很轻松地察觉到这个问题。

## Files 类型变更：ObservableCollection

CupertinoTreeView 控件绑定 `List<TreeItem>` 类型的 Files collection到 ItemsSource。因此，即使数据位置发生变化，UI 也不会实时更新。为了解决这个问题，我们需要将 CupertinoTreeView 控件的 ItemsSource 绑定的 Files 类型从 ‘List’ 更改为 ‘ObservableCollection’，来确保项目变更时 UI 也能实时更新。

在 `CupertinoWindowViewModel.cs` 中更改类型：

```csharp
public partial class CupertinoWindowViewModel : ObservableBase
{
    public ObservableCollection<FileItem> Files { get; set; }
    ...
}
```

由于 Files 类型的更改，所有引用和定义这个类型的部分都会出现错误，这些部分我们也需要更改为 ObservableCollection 类型。

## Children 类型变更：ObservableCollection

接下来，在 FileItem 模型中，`List<FileItem>` 类型的 ‘Children’ 属性也需要像 Files 一样更改为 ‘ObservableCollection’。

```csharp
public class FileItem
{
    public string Name { get; set; }
    public string Path { get; set; }
    public string Type { get; set; }
    public string Extension { get; set; }
    public long? Size { get; set; }
    public int Depth { get; set; }
    public ObservableCollection<FileItem> Children { get; set; }
}
```

由于 Children 属性也绑定到 CupertinoTreeItem 的 ItemsSource，所以在递归的 TreeView 控件中这一步是必不可少的。如果忽略这一点，就只有TreeView 控件的最上层 ItemsPresenter中元素才会实现 UI 实时更新，其余下层的元素则无法正确响应 ObservableCollection 的更改。
_CupertinoTreeItem.cs, FindParentItem_

```chsarp
private FileItem FindParentItem(CupertinoTreeItem item)
{
    var parent = VisualTreeHelper.GetParent(item);

    while (parent != null)
    {
        if (parent is CupertinoTreeItem parentTreeItem 
            && parentTreeItem.DataContext is FileItem parentData)
        {
            if (parentData.Children.Contains(item.DataContext as FileItem))
            {
                return parentData;
            }
        }

        parent = VisualTreeHelper.GetParent(parent);
    }

    return null;
}
```
