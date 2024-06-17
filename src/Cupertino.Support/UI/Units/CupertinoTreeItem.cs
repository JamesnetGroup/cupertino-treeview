using Cupertino.Support.Local.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cupertino.Support.UI.Units
{
    public class CupertinoTreeItem : TreeViewItem
    {
        public ICommand SelectionCommand
        {
            get { return (ICommand)GetValue(SelectionCommandProperty); }
            set { SetValue(SelectionCommandProperty, value); }
        }

        public static readonly DependencyProperty SelectionCommandProperty =
            DependencyProperty.Register("SelectionCommand", typeof(ICommand), typeof(CupertinoTreeItem), new PropertyMetadata(null));



        static CupertinoTreeItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CupertinoTreeItem), new FrameworkPropertyMetadata(typeof(CupertinoTreeItem)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CupertinoTreeItem();
        }

        public CupertinoTreeItem()
        {
            MouseLeftButtonUp += CupertinoTreeItem_MouseLeftButtonUp;
        }

        private void CupertinoTreeItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (DataContext is FileItem item)
            {
                SelectionCommand?.Execute(item);
            }
        }
    }
}
