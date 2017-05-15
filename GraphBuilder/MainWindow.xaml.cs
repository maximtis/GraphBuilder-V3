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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace GraphBuilder
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DrawingController.CurrentStorage = new DataStorage();
            DrawingController.Scene = Scene;
            DrawingController.BridgeCreated += drawingWertex_BridgeCreated;

            sceneController = new SceneController(Scene, DrawAreaCanvas);
            DrawingController.ConnectDrawingObject(sceneController);
            fileController = new FileController(DrawingController.Backup());
            fileController.ConnectToHistoryJournal(this);
            OpacityAnimationOut.Completed += OpacityAnimationOut_Completed;
            IsOriented.Checked += IsOriented_Checked;
            IsOriented.Unchecked += IsOriented_Unchecked;

        }
        public static bool IsOrientedGraph = false;
        private void IsOriented_Unchecked(object sender, RoutedEventArgs e)
        {
            IsOrientedGraph = (bool)IsOriented.IsChecked;
        }

        private void IsOriented_Checked(object sender, RoutedEventArgs e)
        {
            IsOrientedGraph = (bool)IsOriented.IsChecked;
        }

        void drawingWertex_BridgeCreated(object sender, EventArgs e)
        {
            Hider.Visibility = Visibility.Visible;
            ModalWeight.Visibility = Visibility.Visible;
        }

        public void Undo_Holder(object sender, EventArgs e)
        {
            DrawingController.ConfigStorage(((StorageChangeEventArgs)e).Data);

        }
        // ------------------------------------------------------   Animations   ----------------------------------------------------

        // For Left-Side-Menu
        ThicknessAnimation LeftSideInAnimation = new ThicknessAnimation(new Thickness(0, 40, 0, 0), TimeSpan.FromSeconds(0.3));
        ThicknessAnimation LeftSideOutAnimation = new ThicknessAnimation(new Thickness(-320, 40, 0, 0), TimeSpan.FromSeconds(0.3));

        DoubleAnimation OpacityAnimationIn = new DoubleAnimation(0.3, TimeSpan.FromSeconds(0.3));
        DoubleAnimation OpacityAnimationOut = new DoubleAnimation(0, TimeSpan.FromSeconds(0.3));

        //---------------------------------------------------------------------------------------------------------------------------


        SceneController sceneController;
        FileController fileController;

        #region Навигационное меню

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            DrawingController.Mode = States.Bridge;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            DrawingController.Mode = States.Relocate;
        }
        private void AddVertexButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingController.AddNewVertex();
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).BorderThickness = new Thickness(2);
            ((Border)sender).Background = Brushes.OrangeRed;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).BorderThickness = new Thickness(1);
            ((Border)sender).Background = Brushes.BlueViolet;
        }
        
        #endregion

        private void Algoritms_Click(object sender, RoutedEventArgs e)
        {
            Hider.Visibility = Visibility.Visible;
            ModalAlgoritms.Visibility = Visibility.Visible;
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OK_ModalAlgoritms_Click(object sender, RoutedEventArgs e)
        {
            Hider.Visibility = Visibility.Hidden;
            ModalAlgoritms.Visibility = Visibility.Hidden;
            
            if (radio_deikstra.IsChecked == true)
            {
                DeffaultColorBridge();
                ConsoleDeikstra.Text = "Result of Algorithm Deikstra";
                DrawingController.CurrentStorage.VertexList[0].PathLength = 0;
                DekstraAlgorim da = new DekstraAlgorim(DrawingController.CurrentStorage.VertexList, DrawingController.CurrentStorage.EdgeList);
                da.AlgoritmRun(DrawingController.CurrentStorage.VertexList[0]);
                List<string> b = PrintGrath.PrintAllMinPaths(da);
                for (int i = 0; i < b.Count; i++)
                    ConsoleDeikstra.Text += b[i] + Environment.NewLine;

            }
            if(radio_deep.IsChecked == true)
            {
                DeffaultColorBridge();
                // Выполняем алгоримт поиска в глубину
                ConsoleDeikstra.Text = string.Empty;
                DeepSearchAlgorithm da = new DeepSearchAlgorithm(DrawingController.CurrentStorage.VertexList, DrawingController.CurrentStorage.EdgeList);
                ConsoleDeikstra.Text = "Result of Algorithm Deep Search \n"+ da.Begin_Search();
                // Окрашиваем пройденные вершины
                foreach (EdgeBag l in DrawingController.CurrentStorage.BindingBridgeWithAlgoEdge)
                    foreach (Edge ed in da.ResultListEdges)
                        if (IsOrientedGraph)
                        {
                            if ((l.DataEdge.FirstPoint == ed.FirstPoint && l.DataEdge.SecondPoint == ed.SecondPoint) || (l.DataEdge.FirstPoint == ed.SecondPoint && l.DataEdge.SecondPoint == ed.FirstPoint))
                                foreach (var x in DrawAreaCanvas.Children)
                                    if (l.GraphicEdge == x)
                                        ((Line)x).Stroke = Brushes.Blue;
                        }
                        else
                        {
                            if (l.DataEdge.FirstPoint == ed.FirstPoint && l.DataEdge.SecondPoint == ed.SecondPoint)
                                foreach (var x in DrawAreaCanvas.Children)
                                    if (l.GraphicEdge == x)
                                        ((Line)x).Stroke = Brushes.Blue;
                        }
            }
            if(radio_prim.IsChecked == true)
            {
                DeffaultColorBridge();
                AlgorithmPrima ap = new AlgorithmPrima(DrawingController.CurrentStorage.VertexList, DrawingController.CurrentStorage.EdgeList, DrawingController.CurrentStorage.VertexList[0]);
                ap.algorithmByPrim();
                foreach (EdgeBag l in DrawingController.CurrentStorage.BindingBridgeWithAlgoEdge)
                    foreach (Edge edge in ap.MSTRezult)
                        if (l.DataEdge.FirstPoint == edge.FirstPoint && l.DataEdge.SecondPoint == edge.SecondPoint)
                            foreach (var x in DrawAreaCanvas.Children)
                                if (l.GraphicEdge == x)
                                    ((Line)x).Stroke = Brushes.DarkSeaGreen;
                ConsoleDeikstra.Text = "Result of Algorithm Prima" + Environment.NewLine;
                foreach (Edge x in ap.MSTRezult)
                    ConsoleDeikstra.Text += "From " + x.FirstPoint.Name + " To " + x.SecondPoint.Name + Environment.NewLine;
            }
        }

        private void DeffaultColorBridge()
        {
            foreach (var x in DrawAreaCanvas.Children)
                if (x is Line)
                    ((Line)x).Stroke = Brushes.OrangeRed;
        }

        private void DeleteVertexButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingController.Mode = States.Delete;
        }

        private void MenuArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SideHider.Visibility = Visibility.Visible;
            LeftSidePanel.BeginAnimation(MarginProperty, LeftSideInAnimation);
            SideHider.BeginAnimation(OpacityProperty, OpacityAnimationIn);
        }

        private void SideHider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LeftSidePanel.BeginAnimation(MarginProperty, LeftSideOutAnimation);
            SideHider.BeginAnimation(OpacityProperty, OpacityAnimationOut);
        }

        private void OpacityAnimationOut_Completed(object sender, EventArgs e)
        {
            SideHider.Visibility = Visibility.Hidden;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            int Value;

            if (int.TryParse(WeightSelector.Text, out Value) && Value<9999)
            {
                DrawingController.CurrentStorage.EdgeList.Last().Weight = Value;
                //drawingController.Storage.
                //ModalResult = true;
                Hider.Visibility = Visibility.Hidden;
                ModalWeight.Visibility = Visibility.Hidden;
            }
            else
            {
                WeightSelector.Text = string.Empty;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "out data (*.xml)|*.xml|All files (*.*)|*.*";
            sfd.FileName = "GraphData";
            sfd.DefaultExt = "xml";
            if ((bool)sfd.ShowDialog())
                fileController.SaveAs(sfd.FileName);

            
        }

        bool CTRL = false;
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (CTRL && e.Key == Key.Z)
                fileController.UndoCommand();
            else if(e.Key == Key.LeftCtrl)
                CTRL = true;
            
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                CTRL = false;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void RollButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void LoadAsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "out data (*.xml)|*.xml|All files (*.*)|*.*";
            ofd.FileName = "GraphData";
            ofd.DefaultExt = "xml";
            if ((bool)ofd.ShowDialog())
                MessageBox.Show("Load was Success!");
        }

        private void button2_Copy1_Click(object sender, RoutedEventArgs e)
        {
            DrawingController.ConfigStorage(fileController.History.Last());
        }
    }
}
