using Microsoft.Win32;
using NavigationDrawerPopUpMenu2.Classes;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NavigationDrawerPopUpMenu2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            Canvas.Visibility = Visibility.Hidden;


        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
            Canvas.Visibility = Visibility.Collapsed;
            Canvas.Visibility = Visibility.Visible;

        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc = null;
            GridMain.Children.Clear();

            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "Home":
                    WriteGrid.Visibility = Visibility.Collapsed;
                    WriteGrid.Visibility = Visibility.Visible;
                    break;
                case "Save":
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Text Files|*.txt";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        System.IO.File.WriteAllText(saveFileDialog.FileName, textBox.Text);
                    }
                    break;
                case "OpenFile":
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Text Files|*.txt";
                    if (openFileDialog.ShowDialog() == true)
                        textBox.Text = File.ReadAllText(openFileDialog.FileName);
                    break;
                case "About":
                    usc = new UserControlCreate();
                    GridMain.Children.Add(usc);
                    WriteGrid.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }
        private void textBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer)?.Parent as Border;
            var data = Prediction.Models();
            string query = (sender as TextBox)?.Text;
            //            var words = textHandler(query);
            if (query.Length == 0)
            {
                resultStack.Children.Clear();
                border.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                border.Visibility = System.Windows.Visibility.Visible;
            }
            resultStack.Children.Clear();
            foreach (var word in data)
            {
                if (word.ToLower().StartsWith(query.ToLower()))
                {
                    addItem(word);
                    found = true;
                }
            }
        }
        private void addItem(string word)
        {
            TextBlock block = new TextBlock
            {
                Text = word,
                Margin = new Thickness(2, 3, 2, 3),
                Cursor = Cursors.Hand
            };

            block.MouseLeftButtonDown += (sender, e) => { textBox.Text = (sender as TextBlock).Text; };
            block.MouseEnter += (sender, e) =>
            {
                TextBlock blk = sender as TextBlock;
                blk.Background = Brushes.CornflowerBlue;
            };
            block.MouseLeave += (sender, e) =>
            {
                TextBlock blk = sender as TextBlock;
                blk.Background = Brushes.Transparent;
            };
            resultStack.Children.Add(block);
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Parser(object sender, RoutedEventArgs e)
        {
            string str = "Ali wakes up from bed and goes to Television in hole, Ali sits down on chair to eat his food,then Ali changes his clothes from cupboard in bedroom ,at last he go out door. Ali ridding the bus to go to school,Enter class room and sitting on his chair, listening carefully to his teacher while she teaches, then go from school to home. Ali goes to garden to run.";
            var json = new WebClient().DownloadString("http://192.168.137.85:5000/" + str);


            string createText = json + Environment.NewLine;
            File.WriteAllText(@"C:\Users\Bashar\Desktop\ff.txt", createText);
            try
            {
                Rootobject jsonclass = JsonConvert.DeserializeObject<Rootobject>(json);
                DataContext = jsonclass;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void NightMode(object sender, RoutedEventArgs e)
        {
            var bc = new BrushConverter();
            if (Equals(Night.Foreground, Brushes.White))
            {
                GridMain.Background = (Brush)bc.ConvertFrom("#1b2836");
                IconGrid.Background = (Brush)bc.ConvertFrom("#1b2836");
                ButtonOpenMenu_.Foreground = Brushes.White;
                ButtonCloseMenu_.Foreground = Brushes.White;
                ScrollViewer.Foreground = Brushes.White;
                Night.Foreground = Brushes.AliceBlue;
            }
            else
            {
                GridMain.Background = (Brush)bc.ConvertFrom("#FFDEDEDE");
                IconGrid.Background = (Brush)bc.ConvertFrom("#FFDEDEDE");
                ButtonOpenMenu_.Foreground = Brushes.Black;
                ButtonCloseMenu_.Foreground = Brushes.Black;
                ScrollViewer.Foreground = Brushes.Black;
                Night.Foreground = Brushes.White;

            }
        }
    }
}