using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace Apliu.Tools.PlatformTargetView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<FilePlatformTarget> _fileTargetItemsSource;
        private readonly CollectionView _fileTargetViewSource;
        private bool _scrollIsEnd;

        public MainWindow()
        {
            InitializeComponent();
            this._fileTargetItemsSource = new ObservableCollection<FilePlatformTarget>();
            this._fileTargetViewSource = new ListCollectionView(this._fileTargetItemsSource);
            this._fileTargetViewSource.Filter = this.OnFilter;
            this.sourceDataGrid.ItemsSource = this._fileTargetViewSource;
            this.pathTextBox.Text = "";// Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private bool OnFilter(object obj)
        {
            var file = obj as FilePlatformTarget;
            if (file != null)
            {
                if (!string.IsNullOrEmpty(file.FileName) && !file.FileName.ToUpper().Contains(searchFileName.Text.ToUpper()))
                {
                    return false;
                }

                if (searchTargets.SelectedIndex > 0 && file.PlatformTarget.ToString() != (searchTargets.SelectedItem as ComboBoxItem)?.Content.ToString())
                {
                    return false;
                }
            }

            return true;
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            var path = this.pathTextBox.Text;
            if (!Directory.Exists(this.pathTextBox.Text))
            {
                MessageBox.Show("当前目录不存在, 请检查后重试");
                return;
            }

            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles("*.*", this.searchOption.IsChecked == true ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            var programFiles = files.Where(u => ".dll".Equals(u.Extension, StringComparison.InvariantCultureIgnoreCase) || ".exe".Equals(u.Extension, StringComparison.InvariantCultureIgnoreCase)).ToArray();
            if (programFiles.Length == 0)
            {
                MessageBox.Show("当前目录没有任何程序文件");
            }

            this._fileTargetItemsSource.Clear();
            this.checkFileNum.Text = "0";

            Task.Factory.StartNew(new Action(() =>
            {
                int i = 0;
                foreach (var file in programFiles)
                {
                    var fileTarget = PlatformTargetHelper.GetPlatformTarget(file.FullName);
                    fileTarget.FileName = file.FullName.Replace(path + "\\", "");

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this._fileTargetItemsSource.Add(fileTarget);
                        this.checkFileNum.Text = (++i).ToString();

                        if (this._scrollIsEnd)
                        {
                            this.sourceDataGrid.ScrollIntoView(fileTarget);
                        }
                    }));
                }
            }));
        }

        private void Search_FileNameTextChanged(object sender, TextChangedEventArgs e)
        {
            this._fileTargetViewSource.Refresh();
        }

        private void Search_TargetsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._fileTargetViewSource.Refresh();
        }

        private void vsDumpbinExpander_Expanded(object sender, RoutedEventArgs e)
        {
            if (vsDumpbinExpander.IsExpanded)
            {
                vsDumpbinStackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                vsDumpbinStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void vsDumpbinTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlatformTargetHelper.InitVsDumpbinPath(vsDumpbinTextBox.Text);
        }

        private void sourceDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double dVer = e.VerticalOffset;
            double dViewport = e.ViewportHeight;
            double dExtent = e.ExtentHeight;
            if (dVer + dViewport == dExtent)
            {
                this._scrollIsEnd = true;
            }
            else
            {
                this._scrollIsEnd = false;
            }
        }
    }
}
