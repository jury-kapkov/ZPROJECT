using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.geometry
{
    class Tube : Primitive
    {
        private const int BOTTOM = 0;
        private const int TOP = 1;
        private const int INSIDE = 0;
        private const int OUTSIDE = 1;
        public string typeObj = "Tube";
        public double TopRadius { get; set; }
        public double BottomRadius { get; set; }
        public double Height { get; private set; }
        public double Thickness { get; private set; }
        public int SegmentCount { get; private set; }

        public Tube(Point3D basePoint, double topRadius, double bottomRadius, double height, double thickness, int segmentsCount, Color color) : base(basePoint, height, color)
        {
            Height = height;
            TopRadius = topRadius;
            BottomRadius = bottomRadius;
            Thickness = thickness;
            SegmentCount = Math.Max(segmentsCount, 3);
            UpdatePoints();
        }

        private void UpdatePoints()
        {
            double angle = Math.PI * 2 / SegmentCount;
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            Point3D[,,] points = new Point3D[2, 2, SegmentCount + 1];
            points[BOTTOM, INSIDE, 0] = new Point3D(-BottomRadius, 0, 0);
            points[BOTTOM, OUTSIDE, 0] = new Point3D(-(BottomRadius + Thickness), 0, 0);
            points[TOP, INSIDE, 0] = new Point3D(-TopRadius, Height, 0);
            points[TOP, OUTSIDE, 0] = new Point3D(-(TopRadius + Thickness), Height, 0);
            faces.Clear();

            for (int i = 1; i <= SegmentCount; ++i)
            {
                double x = points[BOTTOM, INSIDE, i - 1].getX() * cos - points[BOTTOM, INSIDE, i - 1].getZ() * sin;
                double z = points[BOTTOM, INSIDE, i - 1].getX() * sin + points[BOTTOM, INSIDE, i - 1].getZ() * cos;
                points[BOTTOM, INSIDE, i] = new Point3D(x, 0, z);

                x = points[BOTTOM, OUTSIDE, i - 1].getX() * cos - points[BOTTOM, OUTSIDE, i - 1].getZ() * sin;
                z = points[BOTTOM, OUTSIDE, i - 1].getX() * sin + points[BOTTOM, OUTSIDE, i - 1].getZ() * cos;
                points[BOTTOM, OUTSIDE, i] = new Point3D(x, 0, z);

                x = points[TOP, INSIDE, i - 1].getX() * cos - points[TOP, INSIDE, i - 1].getZ() * sin;
                z = points[TOP, INSIDE, i - 1].getX() * sin + points[TOP, INSIDE, i - 1].getZ() * cos;
                points[TOP, INSIDE, i] = new Point3D(x, Height, z);

                x = points[TOP, OUTSIDE, i - 1].getX() * cos - points[TOP, OUTSIDE, i - 1].getZ() * sin;
                z = points[TOP, OUTSIDE, i - 1].getX() * sin + points[TOP, OUTSIDE, i - 1].getZ() * cos;
                points[TOP, OUTSIDE, i] = new Point3D(x, Height, z);

                if (i > 0)
                {
                    faces.Add(new Face(new Point3D[] {
                        points[TOP, OUTSIDE, i - 1],
                        points[TOP, OUTSIDE, i],
                        points[BOTTOM, OUTSIDE, i - 1]
                    }));
                    faces.Add(new Face(new Point3D[] {
                        points[BOTTOM, OUTSIDE, i - 1],
                        points[BOTTOM, OUTSIDE, i],
                        points[TOP, OUTSIDE, i]
                    }));
                    faces.Add(new Face(new Point3D[] {
                        points[TOP, INSIDE, i - 1],
                        points[TOP, INSIDE, i],
                        points[BOTTOM, INSIDE, i - 1]
                    }));
                    faces.Add(new Face(new Point3D[] {
                        points[BOTTOM, INSIDE, i - 1],
                        points[BOTTOM, INSIDE, i],
                        points[TOP, INSIDE, i]
                    }));

                    faces.Add(new Face(new Point3D[] {
                        points[TOP, INSIDE, i - 1],
                        points[TOP, INSIDE, i],
                        points[TOP, OUTSIDE, i - 1]
                    }));
                    faces.Add(new Face(new Point3D[] {
                        points[TOP, OUTSIDE, i - 1],
                        points[TOP, OUTSIDE, i],
                        points[TOP, INSIDE, i]
                    }));

                    faces.Add(new Face(new Point3D[] {
                        points[BOTTOM, INSIDE, i - 1],
                        points[BOTTOM, INSIDE, i],
                        points[BOTTOM, OUTSIDE, i - 1]
                    }));
                    faces.Add(new Face(new Point3D[] {
                        points[BOTTOM, OUTSIDE, i - 1],
                        points[BOTTOM, OUTSIDE, i],
                        points[BOTTOM, INSIDE, i]
                    }));
                }
            }
        }

        public void ModifyTopRadius(double radius)
        {
            TopRadius = radius;
            UpdatePoints();
        }

        public void ModifyBottomRadius(double radius)
        {
            BottomRadius = radius;
            UpdatePoints();
        }

        public void ModifyThickness(double thickness)
        {
            Thickness = thickness;
            UpdatePoints();
        }

        public void ModifyHeight(double height)
        {
            Height = height;
            UpdatePoints();
        }

        public void ModifySegmentsCount(int segmentsCount)
        {
            SegmentCount = segmentsCount;
            UpdatePoints();
        }

        public void ModifyBasePoint(Point3D basePoint)
        {
            this.basePoint = basePoint;
            UpdatePoints();
        }
        public Point3D GetBasePoint()
        {
            return basePoint;
        }
        }
}
