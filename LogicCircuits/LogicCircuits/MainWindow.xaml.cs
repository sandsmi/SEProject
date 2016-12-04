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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace LogicCircuits
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool canDraw { get; set; }
        private string gateImg { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            canDraw = false;
        }
        private void setPropertiesOnClick(string imgUri)
        {
            canDraw = true;
            gateImg = imgUri;
        }
        private void andClick(object sender, RoutedEventArgs e)
        {
            setPropertiesOnClick("Resources/and.png");
        }
        private void orClick(object sender, RoutedEventArgs e)
        {
            setPropertiesOnClick("Resources/or.png");
        }
        private void notClick(object sender, RoutedEventArgs e)
        {
            setPropertiesOnClick("Resources/not.png");
        }
        private void nandClick(object sender, RoutedEventArgs e)
        {
            setPropertiesOnClick("Resources/nand.png");
        }
        private void norClick(object sender, RoutedEventArgs e)
        {
            setPropertiesOnClick("Resources/nor.png");
        }
        private void xorClick(object sender, RoutedEventArgs e)
        {
            setPropertiesOnClick("Resources/xor.png");
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!canDraw)
                return;
            var rect = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(gateImg, UriKind.Relative))
                }
            };
            Surface.Children.Add(rect);
            Canvas.SetLeft(rect, e.GetPosition(Surface).X);
            Canvas.SetTop(rect, e.GetPosition(Surface).Y);
        }

        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!canDraw)
                return;
            
            var ellipse = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(gateImg, UriKind.Relative))
                }
            };
            System.Windows.Point position = e.GetPosition(this);
            double pX = position.X;
            double pY = position.Y;


            Canvas.SetLeft(ellipse, pX);
            Canvas.SetTop(ellipse, pY);
        }

        private void saveAsClick(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)Surface.RenderSize.Width,
            (int)Surface.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(Surface);

            var crop = new CroppedBitmap(rtb, new Int32Rect(50, 50, 692, 417));

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Screenshot";
            dlg.DefaultExt = ".png";
            dlg.Filter = "Image Files (*.bmp, *.png, *.jpg)|*.bmp;*.png;*.jpg";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(crop));
                using (var fs = System.IO.File.OpenWrite(dlg.FileName))
                {
                    pngEncoder.Save(fs);
                }
            }
        }
    }
}
