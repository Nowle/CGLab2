using System.Drawing;

namespace CGLab2
{
    internal class Line
    {
        public Point FirstPoint { get; set; }
        public Point LastPoint { get; set; }

        public Line(Point firstPoint, Point lastPoint)
        {
            FirstPoint = new Point(firstPoint.X, firstPoint.Y);
            LastPoint = new Point(lastPoint.X, lastPoint.Y);
        }

        public static Line operator +(Line line, Point point) =>
            new Line(new Point(line.FirstPoint.X + point.X, line.FirstPoint.Y + point.Y), 
                new Point(line.LastPoint.X + point.X, line.LastPoint.Y + point.Y));
    }
}
