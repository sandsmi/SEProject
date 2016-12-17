﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogicCircuits
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool canDraw { get; set; }
        private bool canLink { get; set; }
        private bool canDelete { get; set; }
        private string gateImg { get; set; }
        bool captured { get; set; }
        UIElement source { get; set; }
        double xShape, xCanvas, yShape, yCanvas;
        Point lineStartPoint, lineEndPoint;

        public MainWindow()
        {
            InitializeComponent();
            canDraw = false;
            canLink = false;
            canDelete = false;
            captured = false;
            source = null;
        }

        private void line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!canLink)
                return;
            lineStartPoint = e.GetPosition(Surface);
            int mX = (int)e.GetPosition(Surface).X;
            int mY = (int)e.GetPosition(Surface).Y;
            Ellipse el = new Ellipse();
            el.Width = 6;
            el.Height = 6;
            el.SetValue(Canvas.LeftProperty, (Double)(mX - 3));
            el.SetValue(Canvas.TopProperty, (Double)(mY - 3));
            el.Fill = Brushes.MediumBlue;

            Surface.Children.Add(el);
        }

        private void wire_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canDelete)
                removeElement(e);
        }
        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!canDraw)
                return;
            var rect = new Rectangle
            {
                Width = 40,
                Height = 40,
                Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(gateImg, UriKind.Relative))
                },
                Name = "Gate"
            };
            rect.MouseLeftButtonDown += new MouseButtonEventHandler(gate_MouseLeftButtonDown);
            rect.MouseMove += new MouseEventHandler(gate_MouseMove);
            rect.MouseUp += new MouseButtonEventHandler(gate_MouseLeftButtonUp);

            Surface.Children.Add(rect);
            Canvas.SetLeft(rect, e.GetPosition(Surface).X);
            Canvas.SetTop(rect, e.GetPosition(Surface).Y);
        }
        private void canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                lineEndPoint = e.GetPosition(Surface);
        }

        private void gate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canDelete)
                removeElement(e);
            if (canLink)
                lineStartPoint = e.GetPosition(Surface);
            if (canDraw || canLink)
                return;
            source = (UIElement)sender;
            Mouse.Capture(source);
            captured = true;
            xShape = Canvas.GetLeft(source);
            xCanvas = e.GetPosition(Surface).X;
            yShape = Canvas.GetTop(source);
            yCanvas = e.GetPosition(Surface).Y;
        }

        private void gate_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                double x = e.GetPosition(Surface).X;
                double y = e.GetPosition(Surface).Y;
                xShape += x - xCanvas;
                Canvas.SetLeft(source, xShape);
                xCanvas = x;
                yShape += y - yCanvas;
                Canvas.SetTop(source, yShape);
                yCanvas = y;
            }
        }
        private void gate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (canLink)
            {
                lineEndPoint = e.GetPosition(Surface);
                DrawLine(lineStartPoint, lineEndPoint);
            }
            Mouse.Capture(null);
            captured = false;
        }

        private void newClick(object sender, RoutedEventArgs e)
        {
            var images = Surface.Children.OfType<Rectangle>().ToList();
            foreach (var image in images)
            {
                if (image.Name == "Gate")
                    Surface.Children.Remove(image);
            }
        }
        private void saveAsClick(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)Surface.RenderSize.Width,
            (int)Surface.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(Surface);

            var crop = new CroppedBitmap(rtb, new Int32Rect(65, 87, 692, 417));

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
        private void deleteClick(object sender, RoutedEventArgs e)
        {
            canDelete = true;
        }
        private void moveClick(object sender, RoutedEventArgs e)
        {
            canDraw = false;
            canLink = false;
            canDelete = false;
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
        private void a_BtnClick(object sender, RoutedEventArgs e)
        {
            toggleZeroOne(a_Btn, a_wire);
        }
        private void b_BtnClick(object sender, RoutedEventArgs e)
        {
            toggleZeroOne(b_Btn, b_wire);
        }
        private void c_BtnClick(object sender, RoutedEventArgs e)
        {
            toggleZeroOne(c_Btn, c_wire);
        }
        private void d_BtnClick(object sender, RoutedEventArgs e)
        {
            toggleZeroOne(d_Btn, d_wire);
        }
        private void e_BtnClick(object sender, RoutedEventArgs e)
        {
            toggleZeroOne(e_Btn, e_wire);
        }
        private void f_BtnClick(object sender, RoutedEventArgs e)
        {
            toggleZeroOne(f_Btn, f_wire);
        }
        private void infoClick(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = true;
        }
        private void linkClick(object sender, RoutedEventArgs e)
        {
            canLink = true;
            canDraw = false;
            canDelete = false;
        }
        private void toggleZeroOne(Button button, Rectangle wire)
        {
            if (button.Content.ToString() == "0")
            {
                button.Content = "1";
                wire.Fill = new SolidColorBrush(System.Windows.Media.Colors.Firebrick);
                return;
            }
            button.Content = "0";
            wire.Fill = new SolidColorBrush(System.Windows.Media.Colors.MediumBlue);
        }
        private void setPropertiesOnClick(string imgUri)
        {
            canDraw = true;
            canLink = false;
            canDelete = false;
            gateImg = imgUri;
        }
        void DrawLine(Point spt, Point ept)
        {
            Line link = new Line();
            link.X1 = spt.X;
            link.Y1 = spt.Y;
            link.X2 = ept.X;
            link.Y2 = ept.Y;

            link.Stroke = Brushes.MediumBlue;
            link.StrokeThickness = 2;
            link.MouseLeftButtonDown += new MouseButtonEventHandler(wire_MouseLeftButtonDown);

            Surface.Children.Add(link);
        }
        private void removeElement(RoutedEventArgs e)
        {
            UIElement element = (UIElement)e.OriginalSource;
            Surface.Children.Remove(element);
        }
        private void hidePopup(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = false;
        }
    }
}
