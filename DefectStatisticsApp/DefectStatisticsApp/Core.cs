using System.ComponentModel;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;


namespace DefectStatisticsApp
{
    class Core : INotifyPropertyChanged
    {
        private const double e = Math.E;
        private const double pi = Math.PI;

        private double scale1 = 10000;
        private double ei1 = 0.006;
        private double es1 = 0.055;
        private double xAvg1 = 0.026;
        private double sigma1 = 0.012;
        private Point origin1;
        private Point[] graphArr1 = [];
        private Dictionary<string, Point[]> vertLinesDict1 = new();
        private double graphWidth;
        private double graphHeight;
        private Point offset1 = new Point(-300.0, -200.0);
        private Point[] eiZoneArr = [];
        private Point[] esZoneArr = [];
        private double pEs;
        private double pEi;
        private double pnorm;

        //Граница неисправимого брака
        public double Ei
        {
            get => ei1; set
            {
                if (ei1 != value)
                {
                    try
                    {
                        ei1 = Convert.ToDouble(value);
                    }
                    catch { MessageBox.Show("Введите число!"); }
                    OnPropertyChanged();
                    CalcDefects();
                    DefectsChanged?.Invoke();
                }
            }
        }

        //Граница исправимого брака
        public double Es
        {
            get => es1; set
            {
                if (es1 != value)
                {
                    try
                    {
                        es1 = Convert.ToDouble(value);
                    }
                    catch { MessageBox.Show("Введите число!"); }
                    OnPropertyChanged();
                    CalcDefects();
                    DefectsChanged?.Invoke();
                }
            }
        }

        //Математическое ожидание
        public double XAvg
        {
            get => xAvg1; set
            {
                if (xAvg1 != value)
                {
                    try
                    {
                        xAvg1 = Convert.ToDouble(value);
                    }
                    catch { MessageBox.Show("Введите число!"); }
                    OnPropertyChanged();
                    CalcDefects();
                    DefectsChanged?.Invoke();
                }
            }
        }

        //Стандартное отклонение
        public double Sigma
        {
            get => sigma1; set
            {
                if (sigma1 != value)
                {
                    try
                    {
                        sigma1 = Convert.ToDouble(value);
                    }
                    catch { MessageBox.Show("Введите число!"); }
                    OnPropertyChanged();
                    CalcDefects();
                    DefectsChanged?.Invoke();
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
        public Point Offset
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

        public string TxbOffsetX
        {
            get => Offset.X.ToString(); set
            {
                try
                {
                    Offset = new Point(Convert.ToDouble(value), Offset.Y);
                }
                catch { MessageBox.Show("Введите число!"); }
            }
        }
        public string TxbOffsetY
        {
            get => Offset.Y.ToString(); set
            {
                try
                {
                    Offset = new Point(Offset.X, Convert.ToDouble(value));
                }
                catch { MessageBox.Show("Введите число!"); }
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

        //Массив граничных точек зоны неисправимого брака
        protected Point[] EiZoneArr
        {
            get => eiZoneArr; set
            {
                eiZoneArr = value;
                PointsChanged?.Invoke();
            }
        }

        //Массив граничных точек зоны исправимого брака
        protected Point[] EsZoneArr
        {
            get => esZoneArr; set
            {
                esZoneArr = value;
                PointsChanged?.Invoke();
            }
        }

        //Доля неисправимого брака
        protected double PEi
        {
            get => pEi; set
            {
                pEi = value;
            }
        }

        //Доля исправимого брака
        protected double PEs
        {
            get => pEs; set
            {
                pEs = value;
            }
        }

        //Доля годных деталей
        protected double Pnorm
        {
            get => pnorm; set
            {
                pnorm = value;
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

        protected delegate void DefectChangeHandler();

        //Событие, вызываемое при изменени основных коэффициентов (Es, Ei, sigma, XAvg).
        //Используется для перерисовки процента брака
        protected event DefectChangeHandler? DefectsChanged;

        protected delegate void PointsChangeHandler();

        //Событие, вызываемое при изменени массивов точек графика.
        //Используется для перерисовки графика
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
            //Создаём пустые динамические списки
            List<Point> tempGraphPtsList = new();
            List<Point> tempEi = new();
            List<Point> tempEs = new();

            //Заполняем списки точками графика
            for (double x = -Sigma*3+XAvg; x <= Sigma*3+XAvg; x+=precision)
            {
                Point currPt = new Point(x * Scale + Origin.X + Offset.X,
                                        -CalcPhi(x) * Scale / 1000 + Origin.Y - Offset.Y);
                tempGraphPtsList.Add(currPt);

                if (x <= Ei)
                {
                    tempEi.Add(currPt);
                }

                if (x >= Es)
                {
                    tempEs.Add(currPt);
                }
            }

            //Проецируем на ось Х граничные точки частей графика над зонами брака
            if (tempEi.Any()) 
            {
                tempEi.Add(new Point(tempEi.Last().X, VertLinesDict["ox"][0].Y));
                tempEi.Add(new Point(tempEi.First().X, VertLinesDict["ox"][0].Y));
            }

            if (tempEs.Any())
            {
                tempEs.Add(new Point(tempEs.Last().X, VertLinesDict["ox"][0].Y));
                tempEs.Add(new Point(tempEs.First().X, VertLinesDict["ox"][0].Y));
            }

            //Конвертируем их в массивы и сохраняем
            GraphArr = tempGraphPtsList.ToArray();
            EiZoneArr = tempEi.ToArray();
            EsZoneArr = tempEs.ToArray();
        }

        /// <summary>
        /// Расчёт процента брака
        /// </summary>
        protected void CalcDefects()
        {
            Pnorm = F( (Ei - XAvg)/Sigma, (Es - XAvg)/Sigma );
            PEi = F( -3 ,(Ei - XAvg)/Sigma );
            PEs = F( (Es - XAvg)/Sigma, 3 );
        }

        /// <summary>
        /// Подинтегральная функция
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private double Y(double t)
        {
            return Math.Pow(e, -t*t/2);
        }

        /// <summary>
        /// Вычисляет определённый интеграл по методу Симпсона
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private double F(double a, double b)
        {
            double x, h, s;
            double n = 1000000; //точность вычислений

            h = (b - a) / n;
            s = 0; 
            x = a + h;
            while (x < b)
            {
                s += 4 * Y(x);
                x += h;
                s += 2 * Y(x);
                x += h;
            }
            s = h / 3 * (s + Y(a) - Y(b));

            return s / Math.Sqrt(2 * pi);
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

        /// <summary>
        /// Смещает мат. ожидание до минимального значения, при котором процент неисправимого брака будет равен 0
        /// </summary>
        protected void CalcNoEi()
        {
            XAvg = Math.Round(XAvg - (XAvg - 3 * Sigma - Ei), 3);
        }
    }
}
