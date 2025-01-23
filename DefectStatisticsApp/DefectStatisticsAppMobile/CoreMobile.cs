using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefectStatisticsAppMobile
{
    class CoreMobile : Core
    {
        //Переменная для доступа к холсту
        private GraphicsView Canvas { get; set; }
        private Label LbDefects { get; set; }

        public CoreMobile(GraphicsView canv, Label label)
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
            GraphHeight = Canvas.Height;
            GraphWidth = Canvas.Width;
        }

        /// <summary>
        /// Отрисовывает оси координат и график нормального распрделения
        /// </summary>
        public void DrawGraph()
        {
            //Canvas.Invalidate();

            Canvas.Drawable = new GraphDrawable(GraphArr, VertLinesDict);

            //Canvas.Invalidate();
        }

        ///// <summary>
        ///// Синхронизация значения текстового поля со значением внутри ядра
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public void OnKeyEnterDown(object sender, KeyEventArgs e)
        //{
        //    if ((e.Key == Key.Enter) && e.IsDown)
        //    {
        //        if (sender is TextBox tb)
        //        {
        //            BindingExpression binding = tb.GetBindingExpression(TextBox.TextProperty);
        //            if (binding != null)
        //            {
        //                binding.UpdateSource();
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Масштабирование графика жестом
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeScaleOnPinchGesture(object sender, PinchGestureUpdatedEventArgs e)
        {
            //if (e.Status == GestureStatus.Started)
            //{
            //    // Сохраняем текущее значение масштаба
            //    _currentScale = MyImage.Scale;
            //}

            if (e.Status == GestureStatus.Running)
            {
                // Устанавливаем новый масштаб
                ChangeScale(e.Scale * 2.5);
            }
        }

        /// <summary>
        /// Перемещение центра координат нажатием пальца
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeOffsetOnFingerTap(object sender, TappedEventArgs e)
        {
            //var pos = e.GetPosition(Canvas);
            //pos.X -= GraphWidth / 2;
            //pos.Y = -(pos.Y - GraphHeight / 2);
            //Offset = pos;
            //TxbOffsetX = pos.X.ToString();
            //TxbOffsetY = pos.Y.ToString();
        }

        /// <summary>
        /// Обновляет надписи о проценте брака
        /// </summary>
        public void updateDefectText()
        {
            LbDefects.Text
                = $"Процент годных деталей: {Pnorm * 100:F2}%"
                + $"\nПроцент исправимого брака: {PEs * 100:F2}%"
                + $"\nПроцент неисправимого брака: {PEi * 100:F2}%";
        }

        public void CalcNoEiPC()
        {
            CalcNoEi();
        }

        public class GraphDrawable : IDrawable
        {
            private Point[] GraphArr = [];
            private Dictionary<string, Point[]> VertLinesDict = new();

            public GraphDrawable(Point[] graphArr, Dictionary<string, Point[]> vertLinesDict)
            {
                GraphArr = graphArr;
                VertLinesDict = vertLinesDict;
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                // Устанавливаем цвет фона
                canvas.FillColor = Colors.AliceBlue;
                canvas.FillRectangle(dirtyRect);

                // Устанавливаем цвет линии
                canvas.StrokeColor = Colors.Blue;
                canvas.StrokeSize = 2;

                //int hatchThickness = 1;
                //var hatchBrush = new VisualBrush
                //{
                //    TileMode = TileMode.Tile,
                //    Viewport = new Rect(0, 0, 30, 30),
                //    ViewportUnits = BrushMappingMode.Absolute,
                //    Visual = new Path
                //    {
                //        Stroke = Brushes.Black,
                //        StrokeThickness = hatchThickness,
                //        Data = new LineGeometry(new Point(0, 0), new Point(50, 50))
                //    }
                //};

                //Отрисовка областей брака
                //Polygon EiZone = new Polygon
                //{
                //    StrokeThickness = 2,   // Толщина обводки
                //    Fill = hatchBrush // Цвет заливки
                //};
                //EiZone.Points = new PointCollection(EiZoneArr);
                //Canvas.Children.Add(EiZone);

                //Polygon EsZone = new Polygon
                //{
                //    StrokeThickness = 2,   // Толщина обводки
                //    Fill = hatchBrush // Цвет заливки
                //};
                //EsZone.Points = new PointCollection(EsZoneArr);
                //Canvas.Children.Add(EsZone);

                //Отрисовка осей координат
                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 3;
                canvas.DrawLine(VertLinesDict["ox"][0], VertLinesDict["ox"][1]);
                canvas.DrawLine(VertLinesDict["oy"][0], VertLinesDict["oy"][1]);

                //Отрисовка вертикальных линий
                canvas.StrokeColor = Colors.Orange;
                canvas.StrokeSize = 2;
                canvas.DrawLine(VertLinesDict["ei"][0], VertLinesDict["ei"][1]);
                canvas.DrawLine(VertLinesDict["es"][0], VertLinesDict["es"][1]);

                canvas.StrokeColor = Colors.Red;
                canvas.DrawLine(VertLinesDict["xAvg"][0], VertLinesDict["xAvg"][1]);

                canvas.StrokeColor = Colors.Blue;
                canvas.StrokeSize = 1;
                canvas.DrawLine(VertLinesDict["sigma+"][0], VertLinesDict["sigma+"][1]);
                canvas.DrawLine(VertLinesDict["sigma-"][0], VertLinesDict["sigma-"][1]);

                //Отрисовка графика нормального распределения вероятностей
                canvas.StrokeColor = Colors.Green;
                canvas.StrokeSize = 2;

                for (int i = 0; i < GraphArr.Length - 1; i++)
                {
                    canvas.DrawLine(GraphArr[i], GraphArr[i + 1]);
                }

            }
        }

    }
}
