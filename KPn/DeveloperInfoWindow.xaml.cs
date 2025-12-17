using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KPn
{
    /// <summary>
    /// Логика взаимодействия для DeveloperInfoWindow.xaml
    /// </summary>
    public partial class DeveloperInfoWindow : Window
    {
        public DeveloperInfoWindow()
        {
            InitializeComponent();
            InitializeGlowAnimations();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void InitializeGlowAnimations()
        {
            Loaded += (s, e) => StartGradientGlowAnimation();
        }

        private void StartGradientGlowAnimation()
        {
            var colorAnimation = new System.Windows.Media.Animation.ColorAnimation
            {
                From = Color.FromRgb(0, 120, 212),
                To = Color.FromRgb(0, 200, 255),
                Duration = TimeSpan.FromSeconds(3),
                AutoReverse = true,
                RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever
            };

            var opacityAnimation = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 0.4,
                To = 0.8,
                Duration = TimeSpan.FromSeconds(2),
                AutoReverse = true,
                RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever
            };

            GlowEffect.BeginAnimation(DropShadowEffect.ColorProperty, colorAnimation);
            GlowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, opacityAnimation);
        }
    }
}

