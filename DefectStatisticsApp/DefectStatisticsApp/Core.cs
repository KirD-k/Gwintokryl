using System.ComponentModel;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace DefectStatisticsApp
{
    class Core : INotifyPropertyChanged
    {
        private const double e = Math.E;
        private const double pi = Math.PI;

        private double scale1 = 10000;
        private double ei1 = 0.002;
        private double es1 = 0.035;
        private double xAvg1 = 0.014;
        private double sigma1 = 0.009;
        private Point origin1;
        private Point[] graphArr1 = [];
        private Dictionary<string, Point[]> vertLinesDict1 = new();
        private double graphWidth;
        private double graphHeight;
        private Point offset1 = new Point(-300.0, -200.0);

        //
        protected double Ei
        {
            get => ei1; set
            {
                if (ei1 != value)
                {
                    ei1 = value;
                    OnPropertyChanged();
                }
            }
        }

        //
        protected double Es
        {
            get => es1; set
            {
                if (es1 != value)
                {
                    es1 = value;
                    OnPropertyChanged();
                }
            }
        }

        //Математическое ожидание
        protected double XAvg
        {
            get => xAvg1; set
            {
                if (xAvg1 != value)
                {
                    xAvg1 = value;
                    OnPropertyChanged();
                }
            }
        }

        //Стандартное отклонение
        protected double Sigma
        {
            get => sigma1; set
            {
                if (sigma1 != value)
                {
                    sigma1 = value;
                    OnPropertyChanged();
                }
            }
        }

        //Начало координат
        protected Point Origin
        {
            get => origin1; set
            {
                if (origin1 != value)
                {
                    origin1 = value;
                    OnPropertyChanged();
                }
            }
        }

        //Смещение начала координат
        protected Point Offset
        {
            get => offset1; set
            {
                if (offset1 != value)
                {
                    offset1 = value;
                    OnPropertyChanged();
                }
            }
        }

        //Масштаб графика
        private double Scale
        {
            get => scale1; set
            {
                //if (value <= 10 & value >= 2 & -calcPhi(xAvg) * GraphHeight / 100 * scale <= GraphHeight)
                if (value >= 1)
                {
                    scale1 = value;
                    OnPropertyChanged();
                }
            }
        }

        //Массив точек графика 
        protected Point[] GraphArr
        {
            get => graphArr1; private set
            {
                graphArr1 = value;
                PointsChanged?.Invoke();
            }
        }

        //Словарь точек вертикальных линий
        protected Dictionary<string, Point[]> VertLinesDict
        {
            get => vertLinesDict1; private set
            {
                vertLinesDict1 = value;
                PointsChanged?.Invoke();
            }
        }

        //Ширина графика
        protected double GraphWidth
        {
            get => graphWidth; set
            {
                if (graphWidth != value)
                {
                    graphWidth = value;
                    Origin = new Point((GraphWidth / 2), (GraphHeight / 2));
                    OnPropertyChanged();
                }
            }
        }

        //Высота графика
        protected double GraphHeight
        {
            get => graphHeight; set
            {
                if (graphHeight != value)
                {
                    graphHeight = value;
                    Origin = new Point((GraphWidth / 2), (GraphHeight / 2));
                    OnPropertyChanged();
                }
            }
        }

        //Обработка события изменения величин
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string v = "") 
        {
            PropertyChanged?.Invoke(this, new(v));
            CalcAll();
        }

        protected delegate void PointsChangeHandler();

        protected event PointsChangeHandler? PointsChanged;

        /// <summary>
        /// Расчёт числа Фи
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double CalcPhi(double x) 
        {
            double phi = Math.Pow(e, -(x - XAvg) * (x - XAvg) / (2 * Sigma * Sigma));
            phi /= Sigma * Math.Sqrt(2 * pi);

            return phi;
        }

        /// <summary>
        /// Изменение масштаба графика
        /// </summary>
        /// <param name="step"></param>
        protected void ChangeScale(double step) 
        {
            Scale -= step;
        }

        /// <summary>
        /// Рассчитывает точки для прямых линий на графике
        /// </summary>
        /// <param name="lineName">Название линии</param>
        /// <param name="lineType">Тип линии (ox или oy)</param>
        /// <param name="a"></param>
        private void CalcLines(string lineName, string lineType, double a)
        {
            if (lineType == "ox") //Если это линия, параллельная оси Х
            {
                Point start = new Point(0,
                        (int)(a * Scale) + Origin.Y - Offset.Y);

                Point end = new Point(GraphWidth,
                                      (int)(a * Scale) + Origin.Y - Offset.Y);

                VertLinesDict[lineName] = [start, end];
            }
            else if (lineType == "oy") //Если это линия, параллельная оси Y
            {
                Point start = new Point((int)(a * Scale) + Origin.X + Offset.X,
                                        0);

                Point end = new Point((int)(a * Scale) + Origin.X + Offset.X,
                                      GraphHeight);

                VertLinesDict[lineName] = [start, end];
            }
        }

        /// <summary>
        /// Рассчитывает точки графика
        /// </summary>
        /// <param name="precision">Гладкость графика</param>
        private void CalcGraph(double precision=0.0001)
        {
            //Создаём пустой динамический список
            List<Point> list = new();

            //Заполняем список точками графика
            for (double x = -Sigma*3+XAvg; x <= Sigma*3+XAvg; x+=precision)
            {
                list.Add(new Point(x * Scale + Origin.X + Offset.X,
                                   -CalcPhi(x) * Scale/1000 + Origin.Y - Offset.Y));
            }

            //Конвертируем его в массив и сохраняем
            GraphArr = list.ToArray();
        }

        /// <summary>
        /// Рассчитывает все точки
        /// </summary>
        private void CalcAll()
        {
            //Считаем и записываем точки, определяющие оси абсцисс и ординат
            CalcLines("ox", "ox", 0);
            CalcLines("oy", "oy", 0);

            //Считаем и записываем точки, определяющие линии:
            CalcLines("sigma+", "oy", (Sigma * 3+ XAvg)); //тройного стандартного отклонения
            CalcLines("sigma-", "oy", (Sigma * -3+ XAvg));
            CalcLines("xAvg", "oy", XAvg); //мат. ожидания
            CalcLines("ei", "oy", (Ei)); //ei
            CalcLines("es", "oy", (Es)); //es

            //Считаем и записываем точки графика нормального распределения
            CalcGraph();
        }
    }
}
