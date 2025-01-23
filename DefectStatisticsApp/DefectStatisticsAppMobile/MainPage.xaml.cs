namespace DefectStatisticsAppMobile
{
    public partial class MainPage : ContentPage
    {
        CoreMobile core;

        public MainPage()
        {
            InitializeComponent();
            core = new CoreMobile(graphCanvas, lbGraphStatus);
            this.myPanel.BindingContext = core;

            Application.Current.UserAppTheme = AppTheme.Light;

            this.SizeChanged += (s, e) => core.UpdateSize();

            //this.graphCanvas. += (s, e) => core.ChangeScaleOnPinchGesture(s, e);

            //this.graphCanvas. += (s, e) => core.ChangeOffsetOnFingerTap(s, e);

            this.btnCalcNoEi.Clicked += (s, e) => core.CalcNoEiPC();
        }

        private void OnGraphTapped(object sender, TappedEventArgs e)
        {
            core.ChangeOffsetOnFingerTap(sender, e);
        }

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            core.ChangeScaleOnPinchGesture(sender, e);
        }
    }

}
