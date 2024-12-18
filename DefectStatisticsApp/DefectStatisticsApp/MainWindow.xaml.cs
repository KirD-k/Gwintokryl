using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DefectStatisticsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Core core;
        public MainWindow()
        {
            InitializeComponent();
            core = new Core(graphCanvas);

            //this.SizeChanged += (s, e) => DrawGraph();
            this.SizeChanged += (s, e) => core.drawAxes();
            //DrawGraph();
            core.drawAxes();
        }

        

        private void DrawGraph()
        {
            graphCanvas.Children.Clear();

            double amplitude = this.Height / 5; // Амплитуда синуса
            double frequency = 0.1; // Волнистость синуса
            double offsetX = 50; // Смещение по оси X
            double offsetY = 200; // Смещение по оси Y

            for (double x = 0; x < 500; x += 3)
            {
                double y = amplitude * Math.Sin(frequency * x);
                Line line = new Line
                {
                    X1 = x + offsetX,
                    Y1 = offsetY - y,
                    X2 = x + 1 + offsetX,
                    Y2 = offsetY - amplitude * Math.Sin(frequency * (x + 1)),
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2
                };
                graphCanvas.Children.Add(line);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            core.drawAxes();
        }
    }
}