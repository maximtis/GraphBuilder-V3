using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphBuilder
{
    delegate void ScaleTransformEventHandler(SceneController sender, SceneScaleTransformEventArgs e);
    class SceneScaleTransformEventArgs : EventArgs
    {
        public SceneScaleTransformEventArgs( double x,double y)
        {
            scaleX = x;
            scaleY = y;
        }
        public double scaleX;
        public double scaleY;
    }


    class SceneController
    {
        public SceneController(Canvas scene,Canvas dac)
        {
            Scene = scene;
            DrawAreaCanvas = dac;
            Scene.MouseWheel += Scene_MouseWheel;
            Scene.MouseRightButtonDown += Scene_MouseRightButtonDown;
            Scene.MouseMove += Scene_MouseMove;
        }

        private Canvas DrawAreaCanvas;
        private Canvas Scene;
        private double offsetLeft;
        private double offsetTop;
        private double scaleX;
        private double scaleY;
        public event ScaleTransformEventHandler ScaleChanged;

        public Point MoveStartPoint { get; private set; }
        
        #region Работа с главной сценой, масштаб, перемещение
        private void Scene_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ScaleTransform scale = new ScaleTransform(scaleX += 0.05, scaleY += 0.05);
                DrawAreaCanvas.RenderTransform = scale;
                if (ScaleChanged != null)
                    ScaleChanged(this, new SceneScaleTransformEventArgs(scaleX, scaleY));
            }
            if (e.Delta < 0 && scaleX > 0.05)
            {
                ScaleTransform scale = new ScaleTransform(scaleX -= 0.05, scaleY -= 0.05);
                DrawAreaCanvas.RenderTransform = scale;
                if (ScaleChanged != null)
                    ScaleChanged(this, new SceneScaleTransformEventArgs(scaleX, scaleY));
            }

        }
        private void Scene_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MoveStartPoint = Mouse.GetPosition(Scene);
            offsetLeft = Canvas.GetLeft(DrawAreaCanvas);
            offsetTop = Canvas.GetTop(DrawAreaCanvas);
        }
        private void Scene_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Point RunPoint = Mouse.GetPosition(Scene);
                Canvas.SetLeft(DrawAreaCanvas, offsetLeft - (MoveStartPoint.X - RunPoint.X));
                Canvas.SetTop(DrawAreaCanvas, offsetTop - (MoveStartPoint.Y - RunPoint.Y));
            }
        }
        #endregion
    }
}
