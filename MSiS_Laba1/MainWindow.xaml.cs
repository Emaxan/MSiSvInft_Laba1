using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace MSiS_Laba1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Pascal/Delphi code (*.pas, *.dpr)|*.pas;*.dpr|All files (*.*)|*.*",
                FilterIndex = 1,
                InitialDirectory = @"E:\Programs\VS\_MSiSvInfT\Laba1\",
                Title = "Open code"
            };

            if (ofd.ShowDialog(WMain) != true) return;

            TbFilePath.Text = ofd.FileNames[0];

            var sourceStream = new StreamReader(ofd.OpenFile(),Encoding.Default);

            TbFileText.Text = sourceStream.ReadToEnd();
            sourceStream.Close();
        }

        private void CloseApp_Click(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Analize_Click(object sender, RoutedEventArgs e)
        {
            TbResult.Text = "";
            if (TbFileText.Text.Trim() == "")
            {
                TbResult.Text = "Error. You dont choose or write code to analize.";
                return;
            }
                                                                                                                                                                                                                //Point startPoint;
            var analiz = new Analizator(TbFileText.Text);
            TbResult.Text = "MakkeybNumber = " + (analiz.MakkeybNumber());                                                                                                                                  //TbResult.Text = analiz.Work(out startPoint) == 0 ? analiz.AnalizeResult(startPoint) : "Sorry. I find some error in code. Check your code and try again.";
        }
    }
}

