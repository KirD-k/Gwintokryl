using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DefectStatisticsApp
{
    class CorePC : Core
    {
        //Переменная для доступа к холсту
        private Canvas Canvas { get; set; }
        private Label LbDefects { get; set; }

        public CorePC(Canvas canv, Label label)
        {
            Canvas = canv;
            LbDefects = label;  
            PointsChanged += DrawGraph;
            DefectsChanged += updateDefectText;
            CalcDefects();
            updateDefectText();
        }

        /// <summary>
        /// Обновление ширины и высоты рисунка
        /// </summary>
        public void UpdateSize()
        {
            GraphHeight = Canvas.ActualHeight;
            GraphWidth = Canvas.ActualWidth;
        }

        /// <summary>
        /// Отрисовывает линию, соединяющую две заданные точки, заданного цвета и толщины
        /// </summary>
        /// <param name="points"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        private void DrawLines(Point[] points, SolidColorBrush color, int thickness)
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

            Canvas.Children.Add(line);
        }

        /// <summary>
        /// Отрисовывает оси координат и график нормального распрделения
        /// </summary>
        public void DrawGraph()
        {
            //Очищаем холст
            Canvas.Children.Clear();

            int graphThickness = 2;
            int hatchThickness = 1;
            var graphColor = Brushes.Green;
            var hatchBrush = new VisualBrush
            {
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, 30, 30),
                ViewportUnits = BrushMappingMode.Absolute,
                Visual = new Path
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = hatchThickness,
                    Data = new LineGeometry(new Point(0, 0), new Point(50, 50))
                }
            };

            //Отрисовка областей брака
            Polygon EiZone = new Polygon
            {
                StrokeThickness = 2,   // Толщина обводки
                Fill = hatchBrush // Цвет заливки
            };
            EiZone.Points = new PointCollection(EiZoneArr);
            Canvas.Children.Add(EiZone);

            Polygon EsZone = new Polygon
            {
                StrokeThickness = 2,   // Толщина обводки
                Fill = hatchBrush // Цвет заливки
            };
            EsZone.Points = new PointCollection(EsZoneArr);
            Canvas.Children.Add(EsZone);

            //Отрисовка осей координат
            DrawLines(VertLinesDict["ox"], Brushes.Black, 3);
            DrawLines(VertLinesDict["oy"], Brushes.Black, 3);

            //Отрисовка вертикальных линий
            DrawLines(VertLinesDict["ei"], Brushes.Orange, 2);
            DrawLines(VertLinesDict["es"], Brushes.Orange, 2);
            DrawLines(VertLinesDict["xAvg"], Brushes.Red, 3);
            DrawLines(VertLinesDict["sigma+"], Brushes.Blue, 1);
            DrawLines(VertLinesDict["sigma-"], Brushes.Blue, 1);

            //Отрисовка графика нормального распределения вероятностей
            for (int i = 0; i < GraphArr.Length - 1; i++)
            {
                Line line = new Line
                {
                    X1 = GraphArr[i].X,
                    Y1 = GraphArr[i].Y,
                    X2 = GraphArr[i + 1].X,
                    Y2 = GraphArr[i + 1].Y,
                    Stroke = graphColor,
                    StrokeThickness = graphThickness
                };
                Canvas.Children.Add(line);
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
                if (sender is TextBox tb)
                {
                    BindingExpression binding = tb.GetBindingExpression(TextBox.TextProperty);
                    if (binding != null)
                    {
                        binding.UpdateSource();
                    }
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
            ChangeScale(e.Delta * 2.5);
        }

        /// <summary>
        /// Перемещение центра координат щелчком ПКМ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeOffsetOnMouseButtonRight(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(Canvas);
            pos.X -= GraphWidth/2; 
            pos.Y = -(pos.Y - GraphHeight / 2);
            Offset = pos;
            TxbOffsetX = pos.X.ToString();
            TxbOffsetY = pos.Y.ToString();

        }

        /// <summary>
        /// Обновляет надписи о проценте брака
        /// </summary>
        public void updateDefectText()
        {
            LbDefects.Content
                = $"Статус графика: Процент годных деталей: {Pnorm * 100:F2}%"
                + $"\nПроцент исправимого брака: {PEs * 100:F2}%"
                + $"\nПроцент неисправимого брака: {PEi * 100:F2}%";
        }

        public void CalcNoEiPC()
        {
            CalcNoEi();
        }
    }
}