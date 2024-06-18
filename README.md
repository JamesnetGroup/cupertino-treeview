
This article covers the content of Vicky's sixth WPF tutorial series video, which provides a detailed technical tutorial lasting an hour, available for free on YouTube and Bilibili. Additionally, the source code is shared on [GitHub](https://github.com/vickyqu115/cupertino-treeview). We welcome your support through [Stars](https://github.com/vickyqu115/cupertino-treeview/stargazers), participation via [Forks](https://github.com/vickyqu115/cupertino-treeview/forks), and communication through [Discussions](https://github.com/vickyqu115/cupertino-treeview/discussions).

## Why Customizing TreeView in WPF is Necessary

In WPF, TreeView and TreeViewItem provide default templates like other standard controls. However, TreeView offers diverse ways of representation and a free hierarchical layout without constraints, making the default templates somewhat limited. Therefore, it is crucial to thoroughly understand and utilize the mechanisms and characteristics of this TreeView control, which inherits from ItemsControl.

## TreeView Mechanism: Different from ListBox but Both Inherit ItemsControl

ListBox, one of the typical controls inheriting from ItemsControl, has a clear hierarchical mechanism between parent and child. Thus, while ListBox inherits from ItemsControl, ListBoxItem inherits from ContentControl, making the hierarchical structure intuitive to understand from the inheritance structure alone.

However, in the case of TreeView and TreeViewItem, the top-level parent is TreeView, but the child TreeViewItem can also contain its own children, acting as both parent and child. Hence, ironically, TreeViewItem also inherits from ItemsControl. This is a fundamental concept of the TreeView control.

## Understanding TreeView's Identity through ItemsSource Property and ItemsPresenter Element

As mentioned earlier, both TreeView and TreeViewItem inherit from ItemsControl, thus both controls have the ItemsSource collection property. Therefore, it is essential to specify the area for the ItemsPresenter element within the template for the parent control. Similarly, for recursive structures, the child control must also create a binding for the ItemsSource property and an area for the ItemsPresenter element.

The summarized source code is as follows:

_TreeView Template_
```xaml
<Style TargetType="{x:Type TreeView}">
    <Setter Property="ItemsSource" Value="{Binding Files}"/>
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate>
                <ItemsPresenter/>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

_TreeViewItem Template_
```xaml
<Style TargetType="{x:Type TreeViewItem}">
    <Setter Property="ItemsSource" Value="{Binding Children}"/>
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate>
                <StackPanel>
                    <TextBlock Text="{Binding Name}"/>
                    <ItemsPresenter/>
                </StackPanel>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```
 ---

By comparing the two template source codes of TreeView and TreeViewItem, we can see the structural similarity where both use ItemsSource and ItemsPresenter. These mechanistic elements enable the TreeView control to expand freely and flexibly without any hierarchical constraints.

Therefore, understanding and configuring these features allows for the quick and easy implementation of a complex TreeView structure.

## Designing the Data Model for TreeView Control

First, you need to select data for hierarchical representation in the TreeView control. In this tutorial video, we use a folder/file structure as the basis for our data. This is an easily understandable example if you think of Windows Explorer or Mac Finder. It is also well-suited for representing recursive hierarchical structures.

The model design is as follows:

_FileItem Model_

```csharp
public class FileItem
{
    public string Name { get; set; }
    public string Path { get; set; }
    public string Type { get; set; }
    public string Extension { get; set; }
    public long? Size { get; set; }
    public int Depth { get; set; }
    public List<FileItem> Children { get; set; }
}
```

The model properties are the minimum required to represent Windows Explorer or Finder. The key properties to focus on are Path, Depth, and Children. Let's look at these properties in detail:

- Name: The name of the folder/file, extracted from Path.
- Path: The full path of the folder/file.
- Type: Distinguishes between folder and file.
- Extension: The file extension.
- Size: The size of the file.
- Depth: The depth (level) of the current item.
- Children: A list of child items.

## Creating Demo Data

Although you could load the actual system directories like Windows Explorer or Finder, for this tutorial, we prepare a safe execution by creating the folder/file structure in a common accessible space like My Documents.

Therefore, we need the following logic to create the folder/file structure:

_FileCreator.cs_

```csharp
public class FileCreator
{
    public string BasePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public void Create()
    {
        string textData = "Vicky test file content.";

        string[] tempFiles = 
        {
            @"\Vicky\Microsoft\Visual Studio\solution.txt",
            @"\Vicky\Microsoft\Visual Studio\debug.mp3",
            @"\Vicky\Microsoft\Visual Studio\class.cs",
            @"\Vicky\Microsoft\Sql Management Studio\query.txt",
            @"\Vicky\Apple\iPhone\store.txt",
            @"\Vicky\Apple\iPhone\calculator.mp3",
            @"\Vicky\Apple\iPhone\safari.cs",
        };

        foreach (string file in tempFiles)
        { 
            string fullPath = BasePath + file;
            string dirName = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            File.WriteAllText(fullPath, textData);
        }
    }
}
```

The source code shows the logic to create various folders and files based on the My Documents path. We've included a variety of items to form a hierarchical directory structure with various file extensions. Feel free to create additional files as needed.

This creation logic ensures the folders and files are safely generated within My Documents.

MVVM Pattern
In this tutorial, we will set up the ViewModel for the MVVM pattern for the first time. The MVVM pattern is highly relied upon in WPF and plays a significant role.

However, we didn't cover it in the previous five tutorials because we intended to first provide a thorough understanding of the foundational elements of WPF, such as CustomControl, ControlTemplate, and ContentControl/ItemsControl, before introducing MVVM.

Therefore, if you want to strengthen your WPF foundation, we recommend studying the five WPF tutorial series in order before proceeding.

Creating a List Based on Generated Folders/Files in the ViewModel
This tutorial introduces the core of the MVVM pattern: the ViewModel. We'll create a ViewModel class that uses the physical path defined in the FileCreator.cs class's BasePath. Using the .NET Directory class methods GetDirectories and GetFiles, we'll retrieve all folder/file lists and compose the FileItem data.

First, we create the List property to bind to the TreeView's ItemsSource.

Files Property to Bind to ItemsSource

csharp
코드 복사
public List<FileItem> Files { get; set; }
It is notable that there is no OnPropertyChanged processing for the Files property. This implies that the Files list will be created in the ViewModel's constructor. Therefore, using init instead of set is also a good approach.

Using init Instead

csharp
코드 복사
public List<FileItem> Files { get; init; }
Although a minor rule, such elements contribute to writing good code.

Next, we need to implement a method that recursively traverses folders and creates the folder/file list as FileItem models.

Implementing GetFiles Method

csharp
코드 복사
private void GetFiles(string root, List<FileItem> source, int depth)
{
    string[] dirs = Directory.GetDirectories(root);
    string[] files = Directory.GetFiles(root);

    foreach (string dir in dirs)
    {
        FileItem item = new();
        item.Name = Path.GetFileNameWithoutExtension(dir);
        item.Path = dir;
        item.Size = null;
        item.Type = "Folder";
        item.Depth = depth;
        item.Children = new();

        source.Add(item);

        GetFiles(dir, item.Children, depth + 1);
    }

    foreach (string file in files)
    {
        FileItem item = new();
        item.Name = Path.GetFileNameWithoutExtension(file);
        item.Path = file;
        item.Size = new FileInfo(file).Length;
        item.Type = "File";
        item.Extension = new FileInfo(file).Extension;
        item.Depth = depth;

        source.Add(item);
    }
}
In this method, the target folder/file items are recursively traversed only if they are directories. The recursive calls occur within the foreach loop for dirs, where children are added through the Children property.

The next notable aspect is Depth. This property calculates the depth of the current folder/file item, which is essential for visual hierarchical representation in XAML. The recursive method increases the Depth value with each call, distinguishing the hierarchy. Other elements mainly serve for visual data representation.
