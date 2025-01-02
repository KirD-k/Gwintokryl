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

            corePC = new CorePC(graphCanvas);
            myPanel.DataContext = corePC;
            //Перерисовка графика при изменении размеров экрана
            //this.SizeChanged += (s, e) => DrawGraph(core.graphArr, core.vertLinesDict);
            this.SizeChanged += (s, e) => upd();

            //Масштабирование графика колесом мышки
            this.graphCanvas.MouseWheel += (s, e) => corePC.ChangeScaleOnMouseWheel(s, e);

            //Обновление значенией в ядре при нажатии клавиши Enter во время ввода в TextBox
            this.txbEI.KeyDown += (s, e) => corePC.OnKeyEnterDown(s, e);
            this.txbES.KeyDown += (s, e) => corePC.OnKeyEnterDown(s, e);
            this.txbSigma.KeyDown += (s, e) => corePC.OnKeyEnterDown(s, e);
            this.txbXavg.KeyDown += (s, e) => corePC.OnKeyEnterDown(s, e);

            

            //DrawGraph(core.graphArr, core.vertLinesDict);



        }

        public void upd()
        {
            corePC.resize();
            this.lbGraphStatus.Content 
                = $"Статус графика: {((int)graphCanvas.ActualHeight)} x {((int)graphCanvas.ActualWidth)}";
        }




    }
}