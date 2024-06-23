## 在 TreeView 控件中通过Drag Drop移动位置（With Cupertino TreeView)
![image](https://github.com/vickyqu115/cupertino-treeview/assets/101777355/bb6f9186-65d0-48d8-8a1c-fa6edaf12146)

本文将解释在 TreeView 控件中进行Drag Drop操作的过程。内容基于我们的第六个 WPF 教程视频 "Cupertino TreeView" 的源代码，因此建议先查看 GitHub 上公开的 cupertino-treeview 源代码。另外如果你还没有观看教程视频，可以通过 [BiliBili](https://www.bilibili.com/video/BV1xz42187wV) 免费观看。

### WPF 中的Drag Drop与 TreeView
WPF 对所有 UI 元素的拖放设计非常灵活。因此，虽然本文以 TreeView 控件为例，但内容同样适用于其他情况。

TreeView 控件通常涉及将选中的项移动到目标项的子级。因此，除了处理简单的拖放操作外，关键还在于将拖动的项从其父级中移除并添加到新的目标项中。

AllowDrop
首先要设置的是 AllowDrop 属性。这个 Boolean 类型的属性由 UIElement 控件继承提供。因此，所有继承 UIElement 的类都可以提供拖放功能。

我们需要在作为拖放实际目标的 TreeViewItem 类中添加此属性。在本教程中，我们将在 CupertinoTreeItem 的构造函数中添加 AllowDrop 设置。
