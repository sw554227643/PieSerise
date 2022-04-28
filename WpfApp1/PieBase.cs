using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfApp1
{
    public class PieBase: PieSerise
    {
        public double Angle { get; set; }

        public ArcSegment ArcSegment { get; set; }

        public LineSegment LineSegmentStar { get; set; }

        public LineSegment LineSegmentEnd { get; set; }

        public Point StarPoint { get; set; }

        public Point EndPoint { get; set; }
    }
}
