using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace InstantMessenger.Client.LoginScreen
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class CircularProgressBar : UserControl
    {
        #region Data

        private readonly DispatcherTimer _animationTimer;

        #endregion

        public CircularProgressBar()
        {
            InitializeComponent();
            _animationTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, Dispatcher)
                {
                    Interval = new TimeSpan(0, 0, 0, 0, 75)
                };
        }

        #region Private Methods

        private void Start()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            _animationTimer.Tick += HandleAnimationTick;
            _animationTimer.Start();
        }

        private void Stop()
        {
            _animationTimer.Stop();
            Mouse.OverrideCursor = Cursors.Arrow;
            _animationTimer.Tick -= HandleAnimationTick;
        }

        private void HandleAnimationTick(object sender, EventArgs e)
        {
            SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
        }

        private void OnCanvasLoaded(object sender, RoutedEventArgs e)
        {
            const double offset = Math.PI;
            const double step = Math.PI * 2 / 10.0;

            SetPosition(Circle0, offset, 0.0, step);
            SetPosition(Circle1, offset, 1.0, step);
            SetPosition(Circle2, offset, 2.0, step);
            SetPosition(Circle3, offset, 3.0, step);
            SetPosition(Circle4, offset, 4.0, step);
            SetPosition(Circle5, offset, 5.0, step);
            SetPosition(Circle6, offset, 6.0, step);
            SetPosition(Circle7, offset, 7.0, step);
            SetPosition(Circle8, offset, 8.0, step);
        }

        private void SetPosition(Ellipse ellipse, double offset, double posOffSet, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, 50.0
                + Math.Sin(offset + posOffSet * step) * 50.0);

            ellipse.SetValue(Canvas.TopProperty, 50
                + Math.Cos(offset + posOffSet * step) * 50.0);
        }

        private void OnCanvasUnloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void HandleVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var isVisible = (bool)e.NewValue;

            if (isVisible)
                Start();
            else
                Stop();
        }

        #endregion
    }
}
