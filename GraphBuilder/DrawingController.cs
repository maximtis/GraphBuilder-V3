using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphBuilder
{

    enum States { Relocate = 1, Bridge = 2, Delete = 3 };

    static class DrawingController
    {
        static DrawingController()
        {
            StartVertex = new VertexBag();
            CurrentStorage = new DataStorage();
            MoveStartPoint = new Point();
            BridgeStartPoint = new Point();
            BridgeEndPoint = new Point();
            Mode = States.Relocate;
        }

        public static void ConfigStorage(DataStorage data)
        {
            CurrentStorage = data;
            DrawAreaCanvas.Children.Clear();
            foreach(var x in data.DrawAreaChildrens)
            {
                DrawAreaCanvas.Children.Add(x);
            }
        }
        public static void ConnectDrawingObject(SceneController sc)
        {
            sc.ScaleChanged += SceneScaleTransform_Holder;
        }
        public static Canvas Scene
        {
            get{ return scene; }
            set{
                scene = value; /// !!!!!!!!!!
                scene.MouseMove += Border_MouseMove;
                }
        }        
        public static Canvas DrawAreaCanvas
        {
            get
            {
                return ((Canvas)scene.Children[0]);
            }

        }

        private static VertexBag StartVertex;
        public static object DrawAreaCanvasChildrens
        {
            set
            {
                DrawAreaCanvas.Children.Clear();
                foreach (UIElement x in (List<UIElement>)value)
                    ((Canvas)scene.Children[0]).Children.Add(x);
            }
            get
            {
                return DrawAreaCanvas.Children;
            }
        }

        private static Border CurrentVertex;
        private static Point MoveStartPoint;
        private static Point BridgeStartPoint, BridgeEndPoint;

        private static double offsetLeft, offsetTop;
        private static double scaleX = 1, scaleY = 1;

        public static States Mode { get; set; }

        public static DataStorage CurrentStorage { get; set; }
        private static DataStorage StorageForCopy { get; set; }

        public static event StorageChangeEventHandler StorageChanged;
        public static event EventHandler BridgeCreated;
        public static Canvas scene;

        public static void AddNewVertex()
        {
            try
            {
                // Привязываем графическое представлением вершины и данные для алгоритма
                XmlSerializableVertexBag NewVertexData = VertexBag.GenereteVertex();
                CurrentStorage.BindingVertexWithAlgoVertex.Add(NewVertexData);
                // Добавляем вершину для алгоритма
                CurrentStorage.VertexList.Add(NewVertexData.DataVertex);

                // Добавляем графическое представление
                Border NewVertex = NewVertexData.GraphicVertex;
                NewVertex.MouseEnter += Border_MouseEnter;
                NewVertex.MouseLeave += Border_MouseLeave;
                NewVertex.MouseDown += Border_MouseLeftButtonDown;
                NewVertex.MouseUp += Border_MouseLeftButtonUp;

                Canvas.SetLeft(NewVertex, DrawAreaCanvas.Width / 2);
                Canvas.SetTop(NewVertex, DrawAreaCanvas.Height / 2);
                CurrentStorage.BindingVertexWithLinesStart.Add(NewVertex, new List<Line>());
                CurrentStorage.BindingVertexWithLinesEnd.Add(NewVertex, new List<Line>());
                DrawAreaCanvas.Children.Add(NewVertex);
                if (StorageChanged != null)
                    StorageChanged(new StorageChangeEventArgs(Backup()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void CopyVertex(VertexBag x)
        {
            try
            {
                // Привязываем графическое представлением вершины и данные для алгоритма
                XmlSerializableVertexBag NewVertexData = VertexBag.GenereteVertex(x.DataVertex.Name);
                StorageForCopy.BindingVertexWithAlgoVertex.Add(NewVertexData);
                // Добавляем вершину для алгоритма
                StorageForCopy.VertexList.Add(NewVertexData.DataVertex);

                // Добавляем графическое представление
                Border NewVertex = NewVertexData.GraphicVertex;
                NewVertex.MouseEnter += Border_MouseEnter;
                NewVertex.MouseLeave += Border_MouseLeave;
                NewVertex.MouseDown += Border_MouseLeftButtonDown;
                NewVertex.MouseUp += Border_MouseLeftButtonUp;

                Canvas.SetLeft(NewVertexData.GraphicVertex, Canvas.GetLeft(x.GraphicVertex));
                Canvas.SetTop(NewVertexData.GraphicVertex, Canvas.GetTop(x.GraphicVertex));
                StorageForCopy.BindingVertexWithLinesStart.Add(NewVertex, new List<Line>());
                StorageForCopy.BindingVertexWithLinesEnd.Add(NewVertex, new List<Line>());
                StorageForCopy.DrawAreaChildrens.Add(NewVertex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Работа с вершинами, перемещение

        private static void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (Mode)
            {
                case States.Bridge:
                    if (BridgeStartPoint.X == 0 && BridgeStartPoint.Y == 0)
                        GetStartPointForBrige(sender,false);
                    else
                    {
                        CreateBridge(sender,false);
                    if (BridgeCreated != null)
                        BridgeCreated(new Point(1,1), new EventArgs());
                    }
                    break;
                case States.Delete:
                    DeleteVertex(sender);
                    break;
                default:
                    MoveStartPoint = Mouse.GetPosition(Scene);
                    offsetLeft = Canvas.GetLeft((Border)sender);
                    offsetTop = Canvas.GetTop((Border)sender);
                    break;
            }
        }
        private static void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CurrentVertex.BorderThickness = new Thickness(1);
            CurrentVertex.Background = Brushes.BlueViolet;
            CurrentVertex = new Border();
        }
        private static void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && Mode == States.Relocate)
            {
                Point RunPoint = Mouse.GetPosition(Scene);
                //PosX.Text = ((MoveStartPoint.X - RunPoint.X) / scaleX).ToString();
                //PosY.Text = ((MoveStartPoint.Y - RunPoint.Y) / scaleY).ToString();
                foreach (KeyValuePair<Border, List<Line>> x in CurrentStorage.BindingVertexWithLinesStart)
                    if (x.Key == CurrentVertex)
                        foreach (Line l in x.Value)
                        {
                            l.X1 = offsetLeft - ((MoveStartPoint.X - RunPoint.X) / scaleX) + (CurrentVertex).Width / 2;
                            l.Y1 = offsetTop - ((MoveStartPoint.Y - RunPoint.Y) / scaleY) + (CurrentVertex).Height / 2;
                        }
                foreach (KeyValuePair<Border, List<Line>> x in CurrentStorage.BindingVertexWithLinesEnd)
                    if (x.Key == CurrentVertex)
                        foreach (Line l in x.Value)
                        {
                            l.X2 = offsetLeft - ((MoveStartPoint.X - RunPoint.X) / scaleX) + (CurrentVertex).Width / 2;
                            l.Y2 = offsetTop - ((MoveStartPoint.Y - RunPoint.Y) / scaleY) + (CurrentVertex).Height / 2;
                        }
                Canvas.SetLeft(CurrentVertex, offsetLeft - ((MoveStartPoint.X - RunPoint.X) / scaleX));
                Canvas.SetTop(CurrentVertex, offsetTop - ((MoveStartPoint.Y - RunPoint.Y) / scaleY));
            }
        }
        private static void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if(Mouse.LeftButton == MouseButtonState.Released)
            CurrentVertex = (Border)sender;
            (CurrentVertex).BorderThickness = new Thickness(2);
            (CurrentVertex).Background = Brushes.OrangeRed;
        }
        private static void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            (CurrentVertex).BorderThickness = new Thickness(1);
            (CurrentVertex).Background = Brushes.BlueViolet;
        }


        private static void DeleteVertex(object sender)
        {
            StartVertex = (from vertex in CurrentStorage.BindingVertexWithAlgoVertex
                           where vertex.GraphicVertex == ((Border)sender)
                           select vertex).ToList()[0];

            var EdgesToDelete = (from edge in CurrentStorage.BindingBridgeWithAlgoEdge
                                 where edge.DataEdge.FirstPoint == StartVertex.DataVertex ||
                                 edge.DataEdge.SecondPoint == StartVertex.DataVertex
                                 select edge).ToList();

            // Удаление вершины
            CurrentStorage.VertexList.Remove(StartVertex.DataVertex);

            // Удаление ребер связаных с ней
            foreach (XmlSerializableEdgeBag x in EdgesToDelete)
                CurrentStorage.EdgeList.Remove(x.DataEdge);
            // Удаление графических составляющих

            XmlSerializableEdgeBag RememberEdgeBag = null;
            foreach (KeyValuePair<Border, List<Line>> x in CurrentStorage.BindingVertexWithLinesStart)
                if (x.Key == ((Border)sender))
                {
                    foreach (Line l in x.Value)
                    {
                        DrawAreaCanvas.Children.Remove(l);
                        CurrentStorage.BridgesList.Remove(l);
                        foreach (XmlSerializableEdgeBag eb in CurrentStorage.BindingBridgeWithAlgoEdge)
                        {
                            if (eb.GraphicEdge == l)
                                RememberEdgeBag = eb;
                        }
                        CurrentStorage.BindingBridgeWithAlgoEdge.Remove(RememberEdgeBag);
                    }
                    DrawAreaCanvas.Children.Remove(x.Key);
                }

            CurrentStorage.BindingVertexWithLinesStart.Remove((Border)sender);
            foreach (KeyValuePair<Border, List<Line>> x in CurrentStorage.BindingVertexWithLinesEnd)
                if (x.Key == ((Border)sender))
                {
                    foreach (Line l in x.Value)
                    {
                        DrawAreaCanvas.Children.Remove(l);
                        CurrentStorage.BridgesList.Remove(l);
                        foreach (XmlSerializableEdgeBag eb in CurrentStorage.BindingBridgeWithAlgoEdge)
                        {
                            if (eb.GraphicEdge == l)
                                RememberEdgeBag = eb;
                        }
                        CurrentStorage.BindingBridgeWithAlgoEdge.Remove(RememberEdgeBag);
                    }
                    DrawAreaCanvas.Children.Remove(x.Key);
                }
            CurrentStorage.BindingVertexWithLinesEnd.Remove((Border)sender);
            // Удаление связей в коде
            XmlSerializableVertexBag RememberVertex = null;
            foreach (VertexBag x in CurrentStorage.BindingVertexWithAlgoVertex)
                if (x.GraphicVertex == ((Border)sender))
                    RememberVertex = new XmlSerializableVertexBag(x.GraphicVertex,x.DataVertex);
            CurrentStorage.BindingVertexWithAlgoVertex.Remove(RememberVertex);

            if (StorageChanged != null)
                StorageChanged(new StorageChangeEventArgs(Backup()));
        }
        public static void GetStartPointForBrige(object sender,bool clone)
        {
            BridgeStartPoint = new Point(Canvas.GetLeft((Border)sender) + ((Border)sender).Width / 2, Canvas.GetTop((Border)sender) + ((Border)sender).Height / 2);

            StartVertex = (from vertex in (!clone ? CurrentStorage : StorageForCopy).BindingVertexWithAlgoVertex
                           where vertex.GraphicVertex == ((Border)sender)
                           select vertex).ToList()[0];
        }
        public static void CreateBridge(object sender,bool clone)
        {
            BridgeEndPoint = new Point(Canvas.GetLeft((Border)sender) + ((Border)sender).Width / 2, Canvas.GetTop((Border)sender) + ((Border)sender).Height / 2);

            foreach (VertexBag x in (!clone ? CurrentStorage : StorageForCopy).BindingVertexWithAlgoVertex)
                if (x.GraphicVertex == ((Border)sender))
                    (!clone ? CurrentStorage : StorageForCopy).EdgeList.Add(new Edge(StartVertex.DataVertex, x.DataVertex, 0));

            XmlSerializableEdgeBag NewEdgeBag = XmlSerializableEdgeBag.GenerateBridge(BridgeStartPoint, BridgeEndPoint);
            NewEdgeBag.DataEdge = (!clone ? CurrentStorage : StorageForCopy).EdgeList.Last();
            (!clone ? CurrentStorage : StorageForCopy).BindingBridgeWithAlgoEdge.Add(NewEdgeBag);

            foreach (KeyValuePair<Border, List<Line>> x in (!clone ? CurrentStorage : StorageForCopy).BindingVertexWithLinesStart)
                if (x.Key == StartVertex.GraphicVertex)
                    x.Value.Add(NewEdgeBag.GraphicEdge);
            foreach (KeyValuePair<Border, List<Line>> x in (!clone ? CurrentStorage : StorageForCopy).BindingVertexWithLinesEnd)
                if (x.Key == (Border)sender)
                    x.Value.Add(NewEdgeBag.GraphicEdge);

            if (clone) StorageForCopy.DrawAreaChildrens.Add(NewEdgeBag.GraphicEdge);
            else DrawAreaCanvas.Children.Add(NewEdgeBag.GraphicEdge);

            BridgeStartPoint = new Point();
            if(!clone)
            if (StorageChanged != null)
                StorageChanged(new StorageChangeEventArgs(Backup()));
        }

        #endregion

        public static void SceneScaleTransform_Holder(SceneController sender,SceneScaleTransformEventArgs e)
        {
            scaleY = e.scaleY;
            scaleX = e.scaleX;
        }

        public static DataStorage Backup()
        {
            StorageForCopy = new DataStorage();
            foreach (var x in CurrentStorage.BindingVertexWithAlgoVertex)
                DrawingController.CopyVertex(x);
            foreach (var x in CurrentStorage.BindingBridgeWithAlgoEdge)
            {
                var start = (from s in StorageForCopy.BindingVertexWithAlgoVertex where s.DataVertex.Name == x.DataEdge.FirstPoint.Name select s).ToList()[0];
                var end = (from s in StorageForCopy.BindingVertexWithAlgoVertex where s.DataVertex.Name == x.DataEdge.SecondPoint.Name select s).ToList()[0];
                DrawingController.GetStartPointForBrige(start.GraphicVertex,true);
                DrawingController.CreateBridge(end.GraphicVertex,true);
                DrawingController.StorageForCopy.BindingBridgeWithAlgoEdge.Last().DataEdge.Weight = x.DataEdge.Weight;
            }
            return StorageForCopy;
        }

        internal static AlgorithmPrima AlgorithmPrima
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        internal static DekstraAlgorim DekstraAlgorim
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        internal static DeepSearchAlgorithm DeepSearchAlgorithm
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        internal static FileController FileController
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        internal static SceneController SceneController
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public static DataStorage DataStorage
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }
    }
}
