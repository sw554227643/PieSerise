using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// PieSeriseControl.xaml 的交互逻辑
    /// </summary>
    public partial class PieSeriseControl : UserControl
    {
        bool flg = false;
        private double radius;
        Point minPoint;

        double offsetX = 0;
        double offsetY = 0;

        double centenrX = 0;
        double centenrY = 0;
        private static List<PieSerise> pies;
        private static List<PieBase> serieList = new List<PieBase>();

        public PieSeriseControl()
        {
            InitializeComponent();
            this.Loaded += PieSeriseControl_Loaded;
            this.SizeChanged += PieSeriseControl_SizeChanged;
        }

        private void PieSeriseControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawPath(pies);
        }

        private void PieSeriseControl_Loaded(object sender, RoutedEventArgs e)
        {
            DrawPath(pies);
        }

        private   void DrawPath(IEnumerable<PieSerise> pieSerises)
        {
            if(pies is null || !pies.Any())
            {
                return;
            }
            mainCanvas.Children.Clear();
            //draw pie 
            var canvasWidth = mainCanvas.ActualWidth;
            var canvasHeight = mainCanvas.ActualHeight;

            var pieWidth = canvasWidth > canvasHeight ? canvasHeight : canvasWidth;
            var pieHeight = canvasWidth > canvasHeight ? canvasHeight : canvasWidth;
            centenrX = pieWidth / 2;
            centenrY = pieHeight / 2;
            radius = pieWidth / 2;
            mainCanvas.Width = pieWidth;
            mainCanvas.Height = pieHeight;


            double angle = 0;
            double prevAngle = 0;

            var sum = pieSerises.Select(ser => ser.Percentage).Sum();

            foreach (var pieSerise in pieSerises)
            {
                //1\初始点的位置 这个点是圆心到的第一个点A1
                var line1X = radius * Math.Cos(angle * Math.PI / 180) + centenrX;
                var line1Y = radius * Math.Sin(angle * Math.PI / 180) + centenrY;

                //2\计算偏转角
                angle = pieSerise.Percentage / sum * 360 + prevAngle;

                //3 根据偏转角计算旋转后新点 的位置
                double arcX = centenrX + Math.Cos(angle * Math.PI / 180) * radius;
                double arcY = (radius * Math.Sin(angle * Math.PI / 180)) + centenrY;

                //4.1 画直线从圆心到第一个点A1 的直线
                var line1Segment = new LineSegment(new Point(line1X, line1Y), false);

                //4.2 判断比例是不是超过了一半
                bool isLargeArc = pieSerise.Percentage / sum > 0.5;

                //4.4 画曲线 就是从起始点到圆弧的终点的圆弧曲线

                var arcWidth = radius;
                var arcHeight = radius;
                //4.2.1 弧线
                var arcSegment = new ArcSegment()
                {
                    Size = new Size(arcWidth, arcHeight),
                    Point = new Point(arcX, arcY),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = isLargeArc
                };



                //4.4 直线 用来封闭弧线
                var line2Segment = new LineSegment(new Point(centenrX, centenrY), false);

                //4.5 设置路径图 PathFigure
                var pathFigure = new PathFigure(new Point(centenrX, centenrY), new List<PathSegment>()
                {
                    line1Segment,
                    arcSegment,
                    line2Segment
                }, true);

                PieBase piebase = new PieBase();
                piebase.Title = pieSerise.Title;
                piebase.Percentage = pieSerise.Percentage;
                piebase.PieColor = pieSerise.PieColor;
                piebase.LineSegmentStar = line1Segment;
                piebase.ArcSegment = arcSegment;
                piebase.LineSegmentEnd = line2Segment;
                piebase.Angle = angle;
                piebase.StarPoint = new Point(line1X, line1Y);
                piebase.EndPoint = new Point(arcX, arcY);
                serieList.Add(piebase);



                //4.6 添加到路径几何中
                var pathFigures = new List<PathFigure>()
                {
                    pathFigure
                };
                var pathGeometry = new PathGeometry(pathFigures);
                var path = new Path() { Fill = pieSerise.PieColor, Data = pathGeometry, DataContext = piebase };
                mainCanvas.Children.Add(path);

                prevAngle = angle;



                path.MouseMove += Path_MouseMove1;
                path.MouseLeave += Path_MouseLeave;


                //4.7 画扇形之间的白线
                var outline1 = new Line()
                {
                    X1 = centenrX,
                    Y1 = centenrY,
                    X2 = line1Segment.Point.X,
                    Y2 = line1Segment.Point.Y,
                    Stroke = Brushes.White,
                    StrokeThickness = 0.8,
                };
                var outline2 = new Line()
                {
                    X1 = centenrX,
                    Y1 = centenrY,
                    X2 = arcSegment.Point.X,
                    Y2 = arcSegment.Point.Y,
                    Stroke = Brushes.White,
                    StrokeThickness = 0.8,
                };

                mainCanvas.Children.Add(outline1);
                mainCanvas.Children.Add(outline2);

            }
        }


        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            var path = sender as Path;
            TranslateTransform ttf = new TranslateTransform();
            ttf.X = 0;
            ttf.Y = 0;
            path.RenderTransform = ttf;

            path.Effect = new DropShadowEffect()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF949494"),
                BlurRadius = 20,
                Opacity = 0,
                ShadowDepth = 0
            };
            //BegionRestoreAnimation(path);
            flg = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseMove1(object sender, MouseEventArgs e)
        {

            if (!flg)
            {
                var path = sender as Path;
                BegionOffsetAnimation(path);
            }
        }

        private void BegionOffsetAnimation(Path path)
        {
            NameScope.SetNameScope(this, new NameScope());
            var pathDataContext = path.DataContext as PieBase;
            var angle = pathDataContext.Angle;

            //计算圆弧的中点坐标
            minPoint = new Point(Math.Round(pathDataContext.StarPoint.X + pathDataContext.EndPoint.X) / 2, Math.Round(pathDataContext.StarPoint.Y + pathDataContext.EndPoint.Y) / 2);


            //计算圆心到圆弧中点的向量
            var v1 = minPoint - new Point(centenrX, centenrY);
            //X轴方向 的向量
            var v2 = new Point(2000, 0) - new Point(0, 0);
            double vAngle = 0;
            //计算两个向量的夹角
            if (180 < angle && angle < 360 && pathDataContext.Percentage / Source.Select(p => p.Percentage).Sum() >= 0.5)
            {
                vAngle = Math.Round(Vector.AngleBetween(v2, -v1));
            }
            else
            {
                vAngle = Math.Round(Vector.AngleBetween(v2, v1));

            }

            //计算X Y 方向的偏移
            offsetX = 10 * Math.Cos(vAngle * Math.PI / 180);
            offsetY = 10 * Math.Sin(vAngle * Math.PI / 180);

            TranslateTransform animatedTranslateTransform =
                new TranslateTransform();
            this.RegisterName("AnimatedTranslateTransform", animatedTranslateTransform);
            path.RenderTransform = animatedTranslateTransform;

            //X方向上的偏移
            DoubleAnimation daX = new DoubleAnimation();
            //Storyboard.SetTargetName(daX, );
            Storyboard.SetTargetProperty(daX, new PropertyPath(TranslateTransform.XProperty));
            daX.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            daX.From = 0;
            daX.To = offsetX;


            //Y方向上的偏移
            DoubleAnimation daY = new DoubleAnimation();
            Storyboard.SetTargetName(daY, nameof(animatedTranslateTransform));
            Storyboard.SetTargetProperty(daY, new PropertyPath(TranslateTransform.YProperty));
            daY.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            daY.From = 0;
            daY.To = offsetY;

            //增加阴影 <DropShadowEffect Color="#FF949494" BlurRadius="33"  Opacity="0.6" ShadowDepth="0"/>
            path.Effect = new DropShadowEffect()
            {
                Color = (Color)ColorConverter.ConvertFromString("#2E2E2E"),
                BlurRadius = 33,
                Opacity = 0.6,
                ShadowDepth = 0
            };

            animatedTranslateTransform.BeginAnimation(TranslateTransform.XProperty, daX);
            animatedTranslateTransform.BeginAnimation(TranslateTransform.YProperty, daY);

            flg = true;
        }

        private void BegionRestoreAnimation(Path path)
        {

            TranslateTransform animatedTranslateTransform =
               new TranslateTransform();
            path.RenderTransform = animatedTranslateTransform;

            //X方向上的偏移
            DoubleAnimation daX = new DoubleAnimation();
            //Storyboard.SetTargetName(daX, );
            Storyboard.SetTargetProperty(daX, new PropertyPath(TranslateTransform.XProperty));
            //daX.Duration = new Duration(TimeSpan.FromSeconds(20));
            daX.To = 0;


            //Y方向上的偏移
            DoubleAnimation daY = new DoubleAnimation();
            // Storyboard.SetTargetName(daY, nameof(animatedTranslateTransform));
            Storyboard.SetTargetProperty(daY, new PropertyPath(TranslateTransform.YProperty));
            //daY.Duration = new Duration(TimeSpan.FromSeconds(20));
            daY.To = 0;

            animatedTranslateTransform.BeginAnimation(TranslateTransform.XProperty, daX);
            animatedTranslateTransform.BeginAnimation(TranslateTransform.YProperty, daY);

        }



        public  IEnumerable<PieSerise> Source
        {
            get { return (SeriseList<PieSerise>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(SeriseList<PieSerise>), typeof(PieSeriseControl), new PropertyMetadata(null,new PropertyChangedCallback(SourceChanged)));

        private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            pies = new List<PieSerise>();
            pies = (List<PieSerise>)e.NewValue;
        }

        private EventRoute SourceChangedEventRoute { get; set; }

    }
}
