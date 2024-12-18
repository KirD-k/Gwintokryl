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
        private double ei { get; set; }
        private double es { get; set; }
        private double xAvg { get; set; }
        private double stdDeviation { get; set; }

        private const double e = Math.E;
        private const double pi = Math.PI;

        Canvas canvas { get; set; }
        Point origin { get; set; }

        

        public Core(Canvas graphCanvas) 
        {
            //определяем новый холст и начало координат
            canvas = graphCanvas;
            origin = new Point(x: ((int)canvas.ActualWidth/2), y: ((int)canvas.ActualHeight/2));

            //определяем начальные значения величин
            ei = 0.002f;
            es = 0.035f;
            xAvg = 0.014f;
            stdDeviation = 0.009f;
        }

        public double calcProbDistribution(int x)
        {
            double phi = Math.Pow(e, -(x - xAvg) * (x - xAvg) / (2 * stdDeviation * stdDeviation));
            phi /= stdDeviation * Math.Sqrt(2 * pi);

            return phi;
        }

        public void drawGraph()
        {
            canvas.Children.Clear();
            drawAxes();

            for (int x = ((int)-canvas.ActualWidth / 2); x < canvas.ActualWidth/2; x++)
            {
                Line line = new Line
                {
                    X1 = x,
                    Y1 = - calcProbDistribution(x)*6,
                    X2 = x + 1,
                    Y2 = - calcProbDistribution(x + 1)*6,
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2
                };

                line.X1 += origin.X;
                line.Y1 += origin.Y;

                line.X2 += origin.X;
                line.Y2 += origin.Y;

                canvas.Children.Add(line);
            }
        }

        public void updateOrigin()
        {
            origin = new Point(x: ((int)canvas.ActualWidth / 2), y: ((int)canvas.ActualHeight / 2));
        }

        /// <summary>
        /// Для отрисовки координатных осей 
        /// </summary>
        public void drawAxes()
        {
            updateOrigin();

            try
            {
                canvas.Children.RemoveAt(0);
                canvas.Children.RemoveAt(1);
            }
            catch { }

            //создаём объекты осей
            Line oX = new Line();
            Line oY = new Line();
            

            //ось Х
            oX.X1 = 0;
            oX.Y1 = origin.Y;

            oX.X2 = canvas.ActualWidth;
            oX.Y2 = oX.Y1;


            //ось Y
            oY.X1 = origin.X;
            oY.Y1 = 0;

            oY.X2 = oY.X1;
            oY.Y2 = canvas.ActualHeight;

            //Цвет и толщина осей
            oX.Stroke = Brushes.Black;
            oY.Stroke = Brushes.Black;
            oX.StrokeThickness = 1;
            oY.StrokeThickness = 1;

            //отрисовать оси
            canvas.Children.Insert(0, oX);
            canvas.Children.Insert(1, oY);
        }
    }
}
