using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CGLab2
{
    internal class MyForm : Form
    {
        private static readonly Size PictureBoxSize = new Size(431, 431);
        private static readonly PictureBox PictureBox = new PictureBox { Location = new Point(10, 10), Size = PictureBoxSize };

        public MyForm()
        {
            var button = new Button {Text = "Перерисовать", Size = new Size(100, 25), Location = new Point(180, 460)};
            button.Click += ButtonClick;
            Controls.Add(button);
            PictureBox.Paint += DrawFigure;
            Controls.Add(PictureBox);
        }

        private static void ButtonClick(object sender, EventArgs e)
        {
            PictureBox.Paint += DrawFigure;
            PictureBox.Refresh();
        }

        private static void DrawFigure(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);
            var pixels = GetNet(g);
            var lines = GetLines();
            var rnd = new Random();
            var number = new Point(rnd.Next(-30, 30), rnd.Next(-30, 30));
            lines = lines.Select(line => line + number).ToList();

            foreach (var line in lines)
            {
                var tuple = SectionCut(line);
                var tempLine = tuple.Item1;
                var isLineInPictureBox = tuple.Item2;
                if (isLineInPictureBox)
                    DrawBresenham(g, pixels, tempLine.FirstPoint, tempLine.LastPoint);
            }
        }

        private static Rectangle[,] GetNet(Graphics g)
        {
            var startPoint = new Point(0, 0);
            var xSize = PictureBoxSize.Width - 1;
            var ySize = PictureBoxSize.Width - 1;
            var rects = new Rectangle[xSize, ySize];
            var p = startPoint;
            for (var y = 0; y < ySize; y++)
            {
                p.X = startPoint.X;
                for (var x = 0; x < xSize; x++)
                {
                    rects[x, y] = new Rectangle(p, new Size(5, 5));
                    p.X += 5;
                    g.FillRectangle(Brushes.WhiteSmoke, rects[x, y]);
                    g.DrawRectangle(new Pen(Brushes.Black), rects[x, y]);
                }
                p.Y += 5;
            }
            return rects;
        }

        private static List<Line> GetLines()
        {
            var listOfLines = new List<Line>
            {
                new Line(new Point(43, 7), new Point(69, 50)),
                new Line(new Point(69, 50), new Point(17, 50)),
                new Line(new Point(17, 50), new Point(43, 7)),
                new Line(new Point(43, 69), new Point(17, 26)),
                new Line(new Point(17, 26), new Point(69, 26)),
                new Line(new Point(69, 26), new Point(43, 69)),
            };

            return listOfLines;
        }

        private static Tuple<Line, bool> SectionCut(Line line)
        {
            var newLine = new Line(line.FirstPoint, line.LastPoint);
            var firstPointCode = GetPointCode(newLine.FirstPoint);
            var lastPointCode = GetPointCode(newLine.LastPoint);
            if (firstPointCode != 0 && lastPointCode != 0)
                return Tuple.Create(line, false);

            newLine.FirstPoint = GetCutPoint(new Point(newLine.FirstPoint.X, line.FirstPoint.Y), firstPointCode, newLine);
            newLine.LastPoint = GetCutPoint(new Point(newLine.LastPoint.X, newLine.LastPoint.Y), lastPointCode, newLine);

            return Tuple.Create(newLine, true);
        }

        private static int GetPointCode(Point point)
        {
            var pointCode = 0;
            if (point.X < 0)
                pointCode = pointCode | PlaceCode.Left;
            if (point.X > PictureBoxSize.Width - 2)
                pointCode = pointCode | PlaceCode.Right;
            if (point.Y < 0)
                pointCode = pointCode | PlaceCode.Top;
            if (point.Y > PictureBoxSize.Height - 2)
                pointCode = pointCode | PlaceCode.Bottom;

            return pointCode;
        }

        private static Point GetCutPoint(Point point, int pointCode, Line line)
        {
            while (pointCode != 0)
            {
                if ((pointCode & PlaceCode.Left) == PlaceCode.Left)
                {
                    point.X = 0;
                    point.Y = GetEquationY(line, point.X);
                }

                if ((pointCode & PlaceCode.Top) == PlaceCode.Top)
                {
                    point.Y = 0;
                    point.X = GetEquationX(line, point.Y);
                }

                if ((pointCode & PlaceCode.Right) == PlaceCode.Right)
                {
                    point.X = PictureBoxSize.Width - 2;
                    point.Y = GetEquationY(line, point.X);
                }

                if ((pointCode & PlaceCode.Bottom) == PlaceCode.Bottom)
                {
                    point.Y = PictureBoxSize.Height - 2;
                    point.X = GetEquationX(line, point.Y);
                }

                pointCode = GetPointCode(point);
            }


            return point;
        }

        private static int GetEquationY(Line line, int x)
        {
            var k = (line.LastPoint.Y - line.FirstPoint.Y) * 1.0 / (line.LastPoint.X - line.FirstPoint.X);
            var b = line.FirstPoint.Y - k * line.FirstPoint.X;
            return (int)(k * x + b);
        }

        private static int GetEquationX(Line line, int y)
        {
            var k = (line.LastPoint.Y - line.FirstPoint.Y) * 1.0 / (line.LastPoint.X - line.FirstPoint.X);
            var b = line.FirstPoint.Y - k * line.FirstPoint.X;
            return (int)((y - b) / k);
        }

        private static void DrawBresenham(Graphics g, Rectangle[,] pixels, Point firstPoint, Point lastPoint)
        {
            var a = lastPoint.Y - firstPoint.Y;
            var b = firstPoint.X - lastPoint.X;
            var signA = 1;
            var signB = 1;
            if (a < 0)
                signA = -1;
            if (b < 0)
                signB = -1;
            var f = 0;
            var point = new Point(firstPoint.X, firstPoint.Y);
            g.FillRectangle(Brushes.Blue, pixels[point.X, point.Y]);
            g.DrawRectangle(new Pen(Brushes.Black), pixels[point.X, point.Y]);
            while (point.X != lastPoint.X || point.Y != lastPoint.Y)
            {
                if (Math.Abs(a) < Math.Abs(b))
                {
                    f += a * signA;
                    if (f > 0)
                    {
                        f -= b * signB;
                        point.Y += signA;
                    }

                    point.X -= signB;
                    g.FillRectangle(Brushes.Blue, pixels[point.X, point.Y]);
                    g.DrawRectangle(new Pen(Brushes.Black), pixels[point.X, point.Y]);
                }
                else
                {
                    f += b * signB;
                    if (f > 0)
                    {
                        f -= a * signA;
                        point.X -= signB;
                    }

                    point.Y += signA;
                    g.FillRectangle(Brushes.Blue, pixels[point.X, point.Y]);
                    g.DrawRectangle(new Pen(Brushes.Black), pixels[point.X, point.Y]);
                }
            }
        }
    }
}
