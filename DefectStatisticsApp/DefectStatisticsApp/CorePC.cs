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

namespace DefectStatisticsApp
{
    class CorePC : Core
    {
        public string txbES { get => es.ToString(); set => es = Convert.ToDouble(value); }
        public string txbEI { get => ei.ToString(); set => ei = Convert.ToDouble(value); }
        public string txbXAvg { get => xAvg.ToString(); set => xAvg = Convert.ToDouble(value); }
        public string txbSigma { get => sigma.ToString(); set => sigma = Convert.ToDouble(value); }
        private Canvas canvas { get; set; }

        public CorePC(Canvas canv)
        {
            canvas = canv;
        }

        public void resize()
        {
            GraphHeight = canvas.ActualHeight;
            GraphWidth = canvas.ActualWidth;
            origin = new Point(canvas.ActualHeight/2, canvas.ActualWidth/2);
            DrawGraph();
        }

        /// <summary>
        /// Отрисовывает линию, соединяющую две заданные точки, заданного цвета и толщины
        /// </summary>
        /// <param name="points"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        private void drawLines(Point[] points, SolidColorBrush color, int thickness)
        {
            Line line = new Line
            {
                X1 = points[0].X,
                Y1 = points[0].Y,
                X2 = points[1].X,
                Y2 = points[1].Y,
                Stroke = color,
                StrokeThickness = thickness
            };

            canvas.Children.Add(line);
        }

        /// <summary>
        /// Отрисовывает оси координат и график нормального распрделения
        /// </summary>
        public void DrawGraph()
        {
            //Очищаем холст
            canvas.Children.Clear();

            //Отрисовка осей координат
            drawLines(vertLinesDict["ox"], Brushes.Black, 1);
            drawLines(vertLinesDict["oy"], Brushes.Black, 1);

            //Отрисовка вертикальных линий
            drawLines(vertLinesDict["ei"], Brushes.Orange, 2);
            drawLines(vertLinesDict["es"], Brushes.Orange, 2);
            drawLines(vertLinesDict["xAvg"], Brushes.Red, 3);
            drawLines(vertLinesDict["sigma+"], Brushes.Gray, 1);
            drawLines(vertLinesDict["sigma-"], Brushes.Gray, 1);


            var graphColor = Brushes.Green;
            int graphThickness = 2;

            //Отрисовка графика нормального распределения вероятностей
            for (int i = 0; i < graphArr.Length-1; i++)
            {
                Line line = new Line
                {
                    X1 = graphArr[i].X,
                    Y1 = graphArr[i].Y,
                    X2 = graphArr[i + 1].X,
                    Y2 = graphArr[i + 1].Y,
                    Stroke = graphColor,
                    StrokeThickness = graphThickness
                };

                canvas.Children.Add(line);
            }
        }

        /// <summary>
        /// Синхронизация значения текстового поля со значением внутри ядра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnKeyEnterDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter) && e.IsDown)
            {
                if (sender is TextBox)
                {
                    MessageBox.Show("Hellou");
                }
            }
        }

        /// <summary>
        /// Масштабирование графика колесом мышки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeScaleOnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            changeScale(e.Delta);
        }
    }
}
