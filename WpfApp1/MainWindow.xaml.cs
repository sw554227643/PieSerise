using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool flg = false;
        private double radius;
        Point minPoint;
        List<PieSerise> piebases = new List<PieSerise>();

        double offsetX = 0;
        double offsetY = 0;

        double centenrX = 0;
        double centenrY = 0;

       // SeriseList<PieSerise> pieSerises = new SeriseList<PieSerise>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            ////生成30°角的圆弧
            //Matrix matrix = new Matrix();
            //Point p = new Point(50, 200);
            //matrix.RotateAt(30, 200, 200);
            //matrix.Translate(0, 0);
            //arc.Point = matrix.Transform(p);
            //path1.MouseMove += Path1_MouseMove;

            //Point StartPoint = arc.Point;
            //matrix = new Matrix();
            //matrix.RotateAt(150, 200, 200);
            //Point EndPoint = matrix.Transform(StartPoint);
            //bluePathFigure.Segments.Add(new LineSegment(StartPoint, true));
            //bluePathFigure.Segments.Add(new ArcSegment(EndPoint, new Size(150, 150), 0, false, SweepDirection.Clockwise, true));
            //path1.MouseMove += Path1_MouseMove;


            //-----------------------------------


        }


       

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    pieSerises.Add(new PieBase
        //    {
        //        Title = "Category#03",
        //        Percentage = 30,
        //        PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC000")),
        //    });
        //}

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    pieSerises.RemoveRange(new Random().Next(0,pieSerises.MCount-1),1);
           
        //}
    }

    //public class MyList<T> : List<T>
    //{
    //    public delegate void ListChangedEventHandler<T>(object sender);

    //    private int _count;

    //    public String Action { get; set; }




    //    public  int MCount
    //    {
    //        get { return _count; }
    //        set
    //        {
    //            if (_count != value)
    //            {
    //                _count =value;
    //                DoListChangedEvent();
    //            }
    //        }
    //    }

    //    public event ListChangedEventHandler<T> ListChanged;

    //    private void DoListChangedEvent()
    //    {
    //        if (this.ListChanged != null)
    //            this.ListChanged(this);
    //    }

    //    public new void Add(T t)
    //    {
    //        base.Add(t);
    //        this.Action = "Add";
    //        MCount++;
    //    }

    //    public new void Remove(T t)
    //    {
    //        base.Remove(t);
    //        this.Action = "Remove";
    //        MCount--;
    //    }

    //    public new void RemoveRange(int index,int count)
    //    {
    //        base.RemoveRange(index,count);
    //        this.Action = "RemoveRange";
    //        MCount--;
    //    }
    //}



    //public class PieSBase
    //{
    //    public string Title { get; set; }

    //    public Brush PieColor { get; set; }

    //    public double Percentage { get; set; }


    //}

    //public class PieSerise : PieSBase
    //{
    //    public double Angle { get; set; }

    //    public ArcSegment ArcSegment { get; set; }

    //    public LineSegment LineSegmentStar { get; set; }

    //    public LineSegment LineSegmentEnd { get; set; }

    //    public Point StarPoint { get; set; }

    //    public Point EndPoint { get; set; }

    //}
}
