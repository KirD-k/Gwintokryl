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
        CorePC corePC;
        public MainWindow()
        {
            InitializeComponent();

            corePC = new CorePC(graphCanvas, lbGraphStatus);
            myPanel.DataContext = corePC;
            //Перерисовка графика при изменении размеров экрана
            this.SizeChanged += (s, e) => corePC.UpdateSize();

            //Масштабирование графика колесом мышки
            this.graphCanvas.MouseWheel += (s, e) => corePC.ChangeScaleOnMouseWheel(s, e);

            //Перемещение центра координат кликом ПКМ
            this.graphCanvas.MouseRightButtonDown += (s, e) => corePC.ChangeOffsetOnMouseButtonRight(s, e);
            
            //Обновление значенией в ядре при нажатии клавиши Enter во время ввода в TextBox
            this.txbEI.KeyDown += (s, e) => corePC.OnKeyEnterDown(s, e);
            this.txbES.KeyDown += (s, e) => corePC.OnKeyEnterDown(s, e);
            this.txbSigma.KeyDown += (s, e) => corePC.OnKeyEnterDown(s, e);
            this.txbXavg.KeyDown += (s, e) => corePC.OnKeyEnterDown(s, e);
            this.txbOffsetX.KeyDown += (s, e) => corePC.OnKeyEnterDown(s, e);
            this.txbOffsetY.KeyDown += (s, e) => corePC.OnKeyEnterDown(s, e);

            this.btnCalcNoEi.Click += (s, e) => corePC.CalcNoEiPC();
        }

    }
}