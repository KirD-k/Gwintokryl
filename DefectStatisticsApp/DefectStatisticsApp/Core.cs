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

        private double scale1 = 1;
        private double ei1 = 0.002;
        private double es1 = 0.035;
        private double xAvg1 = 0.014;
        private double sigma1 = 0.009;
        private Point origin1;
        private Point[] graphArr1 = [];
        private Dictionary<string, Point[]> vertLinesDict1;
        private double graphWidth;
        private double graphHeight;

        //
        protected double ei
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
        protected double es
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
        protected double xAvg
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
        protected double sigma
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
        protected Point origin
        {
            get => origin1; set
            {
                if (origin1 != value)
                {
                    origin1 = value;
                }
            }
        }

        //Масштаб графика
        public double scale
        {
            get => scale1; private set
            {
                if (value <= 10 & value >= 2 & -calcPhi(xAvg) * GraphHeight / 100 * scale <= GraphHeight)
                {
                    scale1 = value;
                }
            }
        }

        //Массив точек графика 
        public Point[] graphArr
        {
            get => graphArr1; private set
            {
                graphArr1 = value;
            }
        }

        //Словарь точек вертикальных линий
        public Dictionary<string, Point[]> vertLinesDict
        {
            get => vertLinesDict1; private set
            {
                vertLinesDict1 = value;
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
                    origin = new Point((GraphWidth / 2), (GraphHeight / 2));
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
                    origin = new Point((GraphWidth / 2), (GraphHeight / 2));
                    OnPropertyChanged();
                }
            }
        }


        //Обработка события изменения величин
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string v = "") 
        {
            calcAll();
            PropertyChanged?.Invoke(this, new(v));
        }

        protected Core() 
        {
            vertLinesDict = new();

            calcAll();
        }

        /// <summary>
        /// Расчёт числа Фи
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double calcPhi(double x) 
        {
            double phi = Math.Pow(e, -(x - xAvg) * (x - xAvg) / (2 * sigma * sigma));
            phi /= sigma * Math.Sqrt(2 * pi);

            return phi;
        }

        /// <summary>
        /// Изменение масштаба графика
        /// </summary>
        /// <param name="step"></param>
        protected void changeScale(int step) 
        {
            scale -= step * 0.0025;
        }

        /// <summary>
        /// Рассчитывает точки для прямых линий на графике
        /// </summary>
        /// <param name="lineName">Название линии</param>
        /// <param name="lineType">Тип линии (ox или oy)</param>
        /// <param name="a"></param>
        public void calcLines(string lineName, string lineType, double a)
        {
            if (lineType == "ox") //Если это линия, параллельная оси Х
            {
                Point start = new Point(0,
                        (int)(a * GraphHeight) + origin.Y);

                Point end = new Point(GraphWidth,
                                      (int)(a * GraphHeight) + origin.Y);

                vertLinesDict[lineName] = [start, end];
            }
            else if (lineType == "oy") //Если это линия, параллельная оси Y
            {
                Point start = new Point((int)(a * GraphWidth) + origin.X,
                                        0);

                Point end = new Point((int)(a * GraphWidth) + origin.X,
                                      GraphHeight);

                vertLinesDict[lineName] = [start, end];
            }
        }

        /// <summary>
        /// Рассчитывает точки графика
        /// </summary>
        /// <param name="precision">Гладкость графика</param>
        public void calcGraph(double precision=0.001)
        {
            //Создаём пустой динамический список
            List<Point> list = new();

            //Заполняем список точками графика
            for (double x = -sigma*3; x <= sigma*3; x+=precision)
            {
                list.Add(new Point(x + origin.X,
                                   -calcPhi(x) + origin.Y));
            }

            //Конвертируем его в массив и сохраняем
            graphArr = list.ToArray();
        }

        /// <summary>
        /// Рассчитывает все точки
        /// </summary>
        public void calcAll()
        {
            //Считаем и записываем точки, определяющие оси абсцисс и ординат
            calcLines("ox", "ox", 0);
            calcLines("oy", "oy", 0);

            //Считаем и записываем точки, определяющие линии:
            calcLines("sigma+", "oy", sigma * 3); //тройного стандартного отклонения
            calcLines("sigma-", "oy", sigma * -3);
            calcLines("xAvg", "oy", xAvg); //мат. ожидания
            calcLines("ei", "oy", ei); //ei
            calcLines("es", "oy", es); //es

            //Считаем и записываем точки графика нормального распределения
            calcGraph();
        }
    }
}
