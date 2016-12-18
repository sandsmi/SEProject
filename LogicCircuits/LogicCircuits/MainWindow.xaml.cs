using System;
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


//our logic code 
using Model.DataContainer;
using Model.DataModels;
using Model.ElementInformator;
using Model.Elements;
using Model.Utilities;

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
        private bool drawFromWireToGate { get; set; }
        private bool drawFromGateToGate { get; set; }
        private string gateImg { get; set; }
        bool captured { get; set; }
        UIElement source { get; set; }
        double xShape, xCanvas, yShape, yCanvas;
        Point lineStartPoint, lineEndPoint;
        ElementContainer elem;

        ElementContainer startingGate;
        ElementContainer endingGate;
        public MainWindow()
        {
            InitializeComponent();
            canDraw = false;
            canLink = false;
            canDelete = false;
            captured = false;
            source = null;
            drawFromWireToGate = false;
            drawFromGateToGate = false;
        }

        private void line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            //how to get the wire value?
            if (!canLink)
            {
                drawFromWireToGate = false;
                return;
            }
            drawFromWireToGate = true;
            drawFromGateToGate = false;
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

            DataContainer.CreateNewGate(gateImg, rect.Uid);
           
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
            {
                lineStartPoint = e.GetPosition(Surface);
                //setting information that drawing from gate to gate is initialized, an
                drawFromWireToGate = false;
                drawFromGateToGate = true;

                source = (UIElement)sender;
                //i assume source is the gate currently selected, so we can get its id by it
                startingGate = new ElementContainer();
                DataContainer.AssingObjectByID(source.Uid, startingGate);
            }
            else
            {
                drawFromWireToGate = false;
                drawFromGateToGate = false;

            }
            if (canDraw || canLink)
                return;
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

                //assuming source is the gate over which we are hovering
                //setting the end point of connection to the gate 
                source = (UIElement)sender;
                DataContainer.AssingObjectByID(source.Uid, endingGate);
                lineEndPoint = e.GetPosition(Surface);
                DrawLine(lineStartPoint, lineEndPoint);

                // here set the stuff to the gate to make it connected in logic

            if(drawFromWireToGate)
                {

                }
            else if(drawFromGateToGate)
                {

                }
            }
            Mouse.Capture(null);
            captured = false;
        }

        private void newClick(object sender, RoutedEventArgs e)
        {
            var images = Surface.Children.OfType<Rectangle>().ToList();
            var wires = Surface.Children.OfType<Line>().ToList();
            var points = Surface.Children.OfType<Ellipse>().ToList();
            foreach (var image in images)
            {
                if (image.Name == "Gate")
                    Surface.Children.Remove(image);
            }
            foreach (var wire in wires)
            {
                Surface.Children.Remove(wire);
            }
            foreach (var point in points)
            {
                Surface.Children.Remove(point);
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
            setPropertiesOnClick("Resources/small_and.png");
        }
        private void orClick(object sender, RoutedEventArgs e)
        {
            setPropertiesOnClick("Resources/small_or.png");
        }
        private void notClick(object sender, RoutedEventArgs e)
        {
            setPropertiesOnClick("Resources/small_not.png");
        }
        private void nandClick(object sender, RoutedEventArgs e)
        {
            setPropertiesOnClick("Resources/small_nand.png");
        }
        private void norClick(object sender, RoutedEventArgs e)
        {
            setPropertiesOnClick("Resources/small_nor.png");
        }
        private void xorClick(object sender, RoutedEventArgs e)
        {
            setPropertiesOnClick("Resources/small_xor.png");
        }
        private void a_BtnClick(object sender, RoutedEventArgs e)
        {
            DataContainer.aWire = !DataContainer.aWire;
            toggleZeroOne(a_Btn, a_wire);
        }
        private void b_BtnClick(object sender, RoutedEventArgs e)
        {

            DataContainer.bWire = !DataContainer.bWire;
            toggleZeroOne(b_Btn, b_wire);
        }
        private void c_BtnClick(object sender, RoutedEventArgs e)
        {
            DataContainer.cWire = !DataContainer.cWire;
            toggleZeroOne(c_Btn, c_wire);
        }
        private void d_BtnClick(object sender, RoutedEventArgs e)
        {
            DataContainer.dWire = !DataContainer.dWire;
            toggleZeroOne(d_Btn, d_wire);
        }
        private void e_BtnClick(object sender, RoutedEventArgs e)
        {
            DataContainer.eWire = !DataContainer.eWire;
            toggleZeroOne(e_Btn, e_wire);
        }
        private void f_BtnClick(object sender, RoutedEventArgs e)
        {
            DataContainer.fWire = !DataContainer.fWire;
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
