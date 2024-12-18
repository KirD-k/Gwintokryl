using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;


namespace DefectStatisticsApp
{
    class Core
    {
        private float ei { get; set; }
        private float es { get; set; }
        private float xAvg { get; set; }
        private float stdDeviation { get; set; }

        Canvas canvas { get; set; }
        Point origin { get; set; }

        

        public Core(Canvas graphCanvas) 
        {
            //определяем новый холст и начало координат
            canvas = graphCanvas;
            var a = canvas.Width;
            origin = new Point(x:0, y:0);

            //определяем начальные значения величин
            ei = 0.002f;
            es = 0.035f;
            xAvg = 0.014f;
            stdDeviation = 0.009f;
        }

        public void calcP()
        {

        }

        /// <summary>
        /// Для отрисовки координатных осей 
        /// </summary>
        public void drawAxes()
        {
            canvas.Children.Clear();

            //создаём объекты осей
            Line oX = new Line();
            Line oY = new Line();


            //ось Х
            oX.X1 = 0;
            oX.Y1 = canvas.ActualHeight/2;

            oX.X2 = canvas.ActualWidth;
            oX.Y2 = oX.Y1;


            //ось Y
            oY.X1 = canvas.ActualWidth/2;
            oY.Y1 = 0;

            oY.X2 = oY.X1;
            oY.Y2 = canvas.ActualHeight;

            //Цвет и толщина осей
            oX.Stroke = Brushes.Black;
            oY.Stroke = Brushes.Black;
            oX.StrokeThickness = 1;
            oY.StrokeThickness = 1;

            //отрисовать оси
            canvas.Children.Add(oX);
            canvas.Children.Add(oY);
        }
    }
}
