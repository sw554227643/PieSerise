using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Line = System.Windows.Shapes.Line;
using Path = System.Windows.Shapes.Path;

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

        double fontsize = 12;
        double offsetX = 0;
        double offsetY = 0;

        double centenrX = 0;
        double centenrY = 0;
        private static List<PieBase> serieList = new List<PieBase>();

        public PieSeriseControl()
        {
            InitializeComponent();
            this.Loaded += PieSeriseControl_Loaded;
            this.SizeChanged += PieSeriseControl_SizeChanged;
            action = DrawPath;

        }

        private void Source_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DrawPath(Source);
        }


        private void PieSeriseControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawPath(Source);
        }

        private void PieSeriseControl_Loaded(object sender, RoutedEventArgs e)
        {

            Source.CollectionChanged += Source_CollectionChanged;
            DrawPath(Source);

        }

        /// <summary>
        /// 画扇形
        /// </summary>
        /// <param name="pieSerises"></param>
        private void DrawPath(IEnumerable<PieSerise> pieSerises)
        {
            if (Source is null || !Source.Any())
            {
                return;
            }
            mainCanvas.Children.Clear();
            //draw pie 
            var canvasWidth = this.ActualWidth;
            var canvasHeight = this.ActualHeight;

            var pieWidth = canvasWidth > canvasHeight ? canvasHeight : canvasWidth;
            var pieHeight = canvasWidth > canvasHeight ? canvasHeight : canvasWidth;
            centenrX = pieWidth / 2;
            centenrY = pieHeight / 2;
            radius = canvasWidth > canvasHeight ? canvasHeight/2 : canvasWidth/2;
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
                //注意 当只有一个的时候 那么画出来的就是一个圆，但是arcSegment 无法画重合的圆，所以最大角度弄成359度
                double arcX = 0;
                double arcY = 0;

                if (pieSerises.Count() == 1 && angle == 360)
                {
                    arcX = centenrX + Math.Cos(359.99999 * Math.PI / 180) * radius;
                    arcY = (radius * Math.Sin(359.99999 * Math.PI / 180)) + centenrY;
                }
                else
                {
                    arcX = centenrX + Math.Cos(angle * Math.PI / 180) * radius;
                    arcY = (radius * Math.Sin(angle * Math.PI / 180)) + centenrY;
                }


                //4.1 画直线从圆心到第一个点A1 的直线
                var line1Segment = new LineSegment(new Point(line1X, line1Y), false);

                //4.2 判断比例是不是超过了一半
                bool isLargeArc = pieSerise.Percentage / sum > 0.5;

                //4.4 画曲线 就是从起始点到圆弧的终点的圆弧曲线

                var arcWidth = radius;
                var arcHeight = radius;
                //4.2.1 弧线
                var arcSegment = new ArcSegment();


                arcSegment.Size = new Size(arcWidth, arcHeight);
                arcSegment.Point = new Point(arcX, arcY);
                arcSegment.SweepDirection = SweepDirection.Clockwise;
                arcSegment.IsLargeArc = isLargeArc;



                //4.4 直线 用来封闭弧线
                var line2Segment = new LineSegment(new Point(centenrX, centenrY), false);


                PieBase piebase = new PieBase();
                piebase.Title = pieSerise.Title;
                piebase.Percentage = pieSerise.Percentage;
                piebase.PieColor = pieSerise.PieColor;
                piebase.LineSegmentStar = line1Segment;
                piebase.ArcSegment = arcSegment;
                piebase.LineSegmentEnd = line2Segment;
                piebase.Angle = pieSerise.Percentage / sum * 360;
                piebase.StarPoint = new Point(line1X, line1Y);
                piebase.EndPoint = new Point(arcX, arcY);


                //4.5 设置路径图 PathFigure
                var pathFigure = new PathFigure(new Point(centenrX, centenrY), new List<PathSegment>()
                {
                    line1Segment,
                    arcSegment,
                   line2Segment,
                }, true);



                //4.6 添加到路径几何中
                var pathFigures = new List<PathFigure>()
                {
                    pathFigure,

                };
                var pathGeometry = new PathGeometry(pathFigures);
                var path = new Path() { Fill = pieSerise.PieColor, Data = pathGeometry, DataContext = piebase };
                mainCanvas.Children.Add(path);

                prevAngle = angle;

                var line3 = DrawLine(path);
                if (line3 != null)
                {
                    piebase.Line = line3;

                }

                var textPathGeo = DrawText(path);
                var textpath = new Path() {Fill= pieSerise.PieColor, Data = textPathGeo };
                piebase.TextPath = textpath;

                mainCanvas.Children.Add(textpath);


                path.MouseMove += Path_MouseMove1;
                path.MouseLeave += Path_MouseLeave;

                serieList.Add(piebase);

                //4.7 画扇形之间的白线
                if (pieSerises.Count() == 1 && angle == 360)
                {
                    mainCanvas.Children.Add(line3);
                }
                else
                {
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
                    mainCanvas.Children.Add(line3);
                }


            }
        }

        /// <summary>
        /// 画指示线
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Polyline DrawLine(Path path)
        {
            NameScope.SetNameScope(this, new NameScope());
            var pathDataContext = path.DataContext as PieBase;
            var angle = pathDataContext.Angle;
            pathDataContext.Line = null;
            //计算圆弧的中点坐标
            minPoint = new Point(Math.Round(pathDataContext.StarPoint.X + pathDataContext.EndPoint.X) / 2, Math.Round(pathDataContext.StarPoint.Y + pathDataContext.EndPoint.Y) / 2);

            Vector v1;
            //计算圆心到圆弧中点的向量
            if (angle > 180 && angle < 360)
            {
                v1 = new Point(centenrX, centenrY) - minPoint;
            }
            else if (angle == 180 || angle == 360) //当圆被两等分或者参数只有一个的时候 圆心到中点的向量为空的 
            {
                //可以通过 圆弧起始点的X坐标和终点的X坐标 判断是被横轴等分还是数轴等分
                //X的坐标不同 那么就是被横轴等分
                //X的坐标相同 那么就是被竖轴等分
                if (Math.Round(pathDataContext.StarPoint.X) == Math.Round(pathDataContext.EndPoint.X)) //被竖轴等分
                {
                    v1 = new Point(radius * 2, radius) - new Point(centenrX, centenrY); //取圆最右边的点为中点 求圆心到中点的向量

                }
                else                                                           //被横轴等分
                {
                    //判断圆弧的顺序 以此来判断被横轴等分的 两个半圆是朝上还是朝下的 这里是通过起始点的坐标来判断的 
                    //因为是顺时针画的 所以 起始点X坐标比终点X坐标大的 朝下，反之
                    if (Math.Round(pathDataContext.StarPoint.X) - Math.Round(pathDataContext.EndPoint.X) == 2 * radius)
                    {
                        v1 = new Point(radius, 2 * radius) - new Point(centenrX, centenrY);
                    }
                    else
                    {
                        v1 = new Point(radius, 0) - new Point(centenrX, centenrY);
                    }
                }
            }
            else
            {
                v1 = minPoint - new Point(centenrX, centenrY);
            }


            //向量取模
            v1.Normalize();

            //求圆心到圆心与中点向量在圆弧上的向量
            var Vmin = v1 * radius;

            //求圆上交点的坐标
            var RadiusToNodal = Vmin + new Point(centenrX, centenrY);



            //X轴方向 的向量
            var v2 = new Point(2000, 0) - new Point(0, 0);
            double vAngle = 0;
            //计算两个向量的夹角 角度在180°<β<360° 并且 占比大于等于50%

            vAngle = Math.Round(Vector.AngleBetween(v2, v1));


            //计算X Y 方向的偏移
            offsetX = 10 * Math.Cos(vAngle * Math.PI / 180);
            offsetY = 10 * Math.Sin(vAngle * Math.PI / 180);

            //求延长线上一点的坐标
            var prolongPoint = new Point(RadiusToNodal.X + offsetX * 1, RadiusToNodal.Y + offsetY * 1);

            if (RadiusToNodal.X == double.NaN || RadiusToNodal.Y == double.NaN || prolongPoint.X == double.NaN || prolongPoint.Y == double.NaN)
                return null;


            var point1 = RadiusToNodal;
            var point2 = prolongPoint;
            Point point3;
            //判断中点是 X刻度  150（半径） 的位置
            //在圆的右半部分 那么线需要向右边延伸 新点X需要增大
            if (prolongPoint.X >= radius)
            {
                point3 = new Point(prolongPoint.X + 10, prolongPoint.Y);
            }
            else
            {
                point3 = new Point(prolongPoint.X - 10, prolongPoint.Y);
            }
            //画折线
            PointCollection polygonPoints = new PointCollection();
            polygonPoints.Add(point1);
            polygonPoints.Add(point2);
            polygonPoints.Add(point3);
            var line3 = new Polyline();
            line3.Points = polygonPoints;
            line3.Stroke = pathDataContext.PieColor;
            pathDataContext.PolylineEndPoint = point3;

            return line3;
        }


        private PathGeometry DrawText(Path path)
        {
            NameScope.SetNameScope(this, new NameScope());
            var pathDataContext = path.DataContext as PieBase;

            Typeface typeface = new Typeface
                (new FontFamily("Microsoft YaHei"),
                FontStyles.Normal,
                FontWeights.Normal, FontStretches.Normal);

            FormattedText text = new FormattedText(
                pathDataContext.Title,
                new System.Globalization.CultureInfo("zh-cn"),
                FlowDirection.LeftToRight, typeface, fontsize, Brushes.RosyBrown
                );

            var textWidth = text.Width;

            Geometry geo = null;
            //判断是圆形的左边还是右边
            if (pathDataContext.PolylineEndPoint.X > radius)
            {
                geo = text.BuildGeometry(new Point(pathDataContext.PolylineEndPoint.X + 4, pathDataContext.PolylineEndPoint.Y - fontsize/1.8));

            }
            else
            {
                geo = text.BuildGeometry(new Point(pathDataContext.PolylineEndPoint.X - textWidth -4, pathDataContext.PolylineEndPoint.Y - fontsize / 1.8));
            }

            PathGeometry pathGeometry = geo.GetFlattenedPathGeometry();
            return pathGeometry;

        }





        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            pop1.IsOpen = false;
            var path = sender as Path;
            var dt = path.DataContext as PieBase;

            TranslateTransform ttf = new TranslateTransform();
            ttf.X = 0;
            ttf.Y = 0;
            path.RenderTransform = ttf;
            dt.Line.RenderTransform = new TranslateTransform()
            {
                X = 0,
                Y = 0,
            };

            dt.TextPath.RenderTransform = new TranslateTransform()
            {
                X = 0,
                Y = 0,
            };

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
        /// 显示详细框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseMove1(object sender, MouseEventArgs e)
        {
            Path path = sender as Path;
            //动画
            if (!flg)
            {

                BegionOffsetAnimation(path);
            }
            ShowMousePopup(path, e);


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
            //计算两个向量的夹角 角度在180°<β<360° 并且 占比大于等于50%
            if (180 < angle && angle <= 360 && pathDataContext.Percentage / Source.Select(p => p.Percentage).Sum() >= 0.5)
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

            var line3 = pathDataContext.Line;
            var textPath = pathDataContext.TextPath;

            TranslateTransform LineAnimatedTranslateTransform =
                new TranslateTransform();
            this.RegisterName("LineAnimatedTranslateTransform", LineAnimatedTranslateTransform);
            line3.RenderTransform = LineAnimatedTranslateTransform;


            TranslateTransform animatedTranslateTransform =
                new TranslateTransform();
            this.RegisterName("AnimatedTranslateTransform", animatedTranslateTransform);
            path.RenderTransform = animatedTranslateTransform;

            TranslateTransform TextAnimatedTranslateTransform =
               new TranslateTransform();
            this.RegisterName("TextAnimatedTranslateTransform", animatedTranslateTransform);
            textPath.RenderTransform = animatedTranslateTransform;


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
            LineAnimatedTranslateTransform.BeginAnimation(TranslateTransform.XProperty, daX);
            LineAnimatedTranslateTransform.BeginAnimation(TranslateTransform.YProperty, daY);
            TextAnimatedTranslateTransform.BeginAnimation(TranslateTransform.XProperty, daX);
            TextAnimatedTranslateTransform.BeginAnimation(TranslateTransform.YProperty, daY);




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

        /// <summary>
        /// 显示详情窗口
        /// </summary>
        /// <param name="path"></param>
        private void ShowMousePopup(Path path, MouseEventArgs e)
        {
            var data = path.DataContext as PieBase;
            if (!this.pop1.IsOpen)
                this.pop1.IsOpen = true;

            var mousePosition = e.GetPosition((UIElement)mainCanvas.Parent);

            this.pop1.HorizontalOffset = mousePosition.X + 20;
            this.pop1.VerticalOffset = mousePosition.Y + 20;

            txt.Text = (data.Title + " : " + data.Percentage);//显示鼠标当前坐标点
            ract.Fill = data.PieColor;
        }



        public ObservableCollection<PieSerise> Source
        {
            get { return (ObservableCollection<PieSerise>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ObservableCollection<PieSerise>), typeof(PieSeriseControl), new PropertyMetadata(null, new PropertyChangedCallback(SourceChanged)));

        private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var list = e.NewValue;
            action((ObservableCollection<PieSerise>)e.NewValue);

        }

        private static Action<IEnumerable<PieSerise>> action;

    }
}
