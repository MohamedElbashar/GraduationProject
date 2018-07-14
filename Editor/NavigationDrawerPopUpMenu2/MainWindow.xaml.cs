using Microsoft.Win32;
using NavigationDrawerPopUpMenu2.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            var Enviroments = ListOfEnvironmentsAnditsObjects.EnvAndObjects();

            string str = textBox.Text;
            var json = new WebClient().DownloadString("http://192.168.137.85:5000/" + str);


            Dictionary<string, string> refenceMap = new Dictionary<string, string>();

            string createText = json + Environment.NewLine;
            List<action> LAction = new List<action>();
            try
            {



                Rootobject jsonclass = JsonConvert.DeserializeObject<Rootobject>(json);
                foreach (var t1 in jsonclass.coref)
                {
                    foreach (var t2 in t1)
                    {
                        string a = t2[0][0].ToString();
                        string b = t2[1][0].ToString();
                        refenceMap[a] = b;

                    }

                }

                foreach (var sentence in jsonclass.sentences)
                {
                    foreach (var word in sentence.words)
                    {
                        var propertyOfWord = JsonConvert.DeserializeObject<PropertyOfWord>(word[1].ToString());
                        if (propertyOfWord.PartOfSpeech == "VBZ" || propertyOfWord.PartOfSpeech == "VBG" ||
                            propertyOfWord.PartOfSpeech == "VB")
                        {
                            LAction.Add(new action()
                            {
                                Name = word[0].ToString()
                            });
                        }

                    }
                }

                int cnt_subject = 0, cnt = 0;
                int ftwofrom = 0;
                foreach (var sentence in jsonclass.sentences)
                {
                    foreach (var dep in sentence.dependencies)
                    {
                        if (dep[0] == "nsubj")
                        {
                            LAction[cnt_subject].ThingDOThis = (refenceMap.ContainsKey(dep[2]) == true) ? refenceMap[dep[2]] : dep[2];
                            cnt_subject++;
                        }

                        if (dep[0] == "prep_on" || dep[0] == "prep_to")
                        {
                            if (dep[1] != LAction[cnt].Name)
                            {
                                cnt++;
                            }
                            LAction[cnt].TO = dep[2];
                            cnt++;
                            ftwofrom = 0;
                        }
                        else if (dep[0] == "prep_from" && LAction[cnt].TO == null)
                        {
                            if (ftwofrom == 1)
                            {
                                cnt++;
                            }
                            LAction[cnt].From = dep[2];
                            ftwofrom = 1;
                        }
                    }
                }
                foreach (var act in LAction)
                {
                    if (act.From == null)
                    {
                        act.From = act.TO;
                    }

                    if (act.TO == null)
                    {
                        act.TO = act.From;
                    }

                    if ((act.TO == "home" && act.From == "school"
                        ) || (act.From == "school" && act.TO == "home"))
                    {
                        act.Eniv = "street";
                    }
                }

                foreach (var sentence in jsonclass.sentences)
                {
                    foreach (var dep in sentence.dependencies)
                    {
                        if (dep[0] == "prep_in")
                        {
                            foreach (var act in LAction)
                            {
                                if (act.Eniv == null)
                                    if (act.Name == dep[1] || (act.From != null && act.From == dep[1]) || (act.TO != null && act.TO == dep[1]))
                                    {
                                        act.Eniv = dep[2];
                                    }
                            }
                        }
                    }
                }

                List<ThingsInEnvironments> lenv = ListOfEnvironmentsAnditsObjects.EnvAndObjects();
                foreach (var act in LAction)
                {
                    if (act.Eniv == null)
                    {

                        string s = act.From;
                        int f = 0;
                        foreach (var thingsInEnvironmentse in lenv)
                        {
                            foreach (var o in thingsInEnvironmentse.Objects)
                            {
                                if (o == s)
                                {
                                    act.Eniv = thingsInEnvironmentse.NameofEnvironment;
                                    f = 1;
                                    break;
                                }

                                if (f == 1)
                                    break;
                            }
                        }
                        f = 0;
                    }
                }

                string temp = "";
                foreach (var action in LAction)
                {
                    if (action.Eniv != null)
                    {
                        temp = action.Eniv;
                    }
                    else
                    {
                        action.Eniv = temp;
                    }
                }



                #region MyRegion

                //int cnt = 0, cnt_in = 0, cnt_on = 0, cnt_from = 0, cnt_to = 0;
                //foreach (var sentence in jsonclass.sentences)
                //{
                //    foreach (var dep in sentence.dependencies)
                //    {

                //        if (dep[0] == "nsubj")
                //        {
                //            LAction[cnt].ThingOfDo = (refenceMap.ContainsKey(dep[2]) == true) ? refenceMap[dep[2]] : dep[2];
                //            cnt++;
                //        }


                //        if (dep[0] == "prep_from")
                //        {
                //            for (int i = cnt_from; i < LAction.Count; i++)
                //            {
                //                if (dep[1] == LAction[i].Name)
                //                {
                //                    cnt_from = i;
                //                    cnt_from++;
                //                    LAction[i].From = dep[2];
                //                }
                //            }
                //        }

                //        if (dep[0] == "prep_to")
                //        {
                //            for (int i = cnt_to; i < LAction.Count; i++)
                //            {
                //                if (dep[1] == LAction[i].Name)
                //                {
                //                    cnt_to = i;
                //                    cnt_to++;
                //                    LAction[i].TO = dep[2];
                //                }
                //            }

                //        }

                //        if (dep[0] == "prep_in")
                //        {
                //            for (int i = cnt_in; i < LAction.Count; i++)
                //            {
                //                if (dep[1] == LAction[i].Name)
                //                {
                //                    cnt_in = i;
                //                    cnt_in++;
                //                    LAction[i].From = dep[2];
                //                }
                //            }
                //        }

                //        if (dep[0] == "prep_on")
                //        {
                //            for (int i = cnt_to; i < LAction.Count; i++)
                //            {
                //                if (dep[1] == LAction[i].Name)
                //                {
                //                    cnt_to = i;
                //                    cnt_to++;
                //                    LAction[i].TO = dep[2];
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            string printJson = JsonConvert.SerializeObject(LAction);
            System.IO.File.WriteAllText(@"C:\Users\Bashar\Desktop\printed.json", printJson);
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