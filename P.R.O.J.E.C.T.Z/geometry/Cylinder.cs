using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.geometry
{
    class Cylinder : Primitive
    {
        public double Radius;
        public int SegmentCount;
        public string typeObj = "Cylinder";
        public Cylinder(Point3D basePoint, double height, Color color, double radius, int SegmentCount) : base(basePoint, height, color)
        {
            this.SegmentCount = SegmentCount;
            Radius = radius;
            UpdatePoints();
        }
        private void UpdatePoints()
        {
            double angle = Math.PI * 2 / SegmentCount;
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            //Point3D edge = new Point3D(-Radius, 0, 0);
            Point3D[] points = new Point3D[SegmentCount + 1];
            points[0] = new Point3D(-Radius, 0, 0);
            faces.Clear();

            for (int i = 1; i <= SegmentCount; ++i)
            {
                double x = points[i - 1].getX() * cos - points[i - 1].getZ() * sin;
                double z = points[i - 1].getX() * sin + points[i - 1].getZ() * cos;
                points[i] = new Point3D(x, 0, z);
                //points[i] = edge;
                if (i > 0)
                {
                    faces.Add(new Face(new Point3D[] {
                        new Point3D(points[i - 1].getX(), -height / 2, points[i - 1].getZ()),
                        new Point3D(points[i].getX(), -height / 2, points[i].getZ()),
                        new Point3D(0, -height / 2, 0)
                    }));
                    faces.Add(new Face(new Point3D[] {
                        new Point3D(points[i - 1].getX(), height / 2, points[i - 1].getZ()),
                        new Point3D(points[i].getX(), height / 2, points[i].getZ()),
                        new Point3D(0, height / 2, 0)
                    }));
                    faces.Add(new Face(new Point3D[] {
                        new Point3D(points[i - 1].getX(), height / 2, points[i - 1].getZ()),
                        new Point3D(points[i].getX(), height / 2, points[i].getZ()),
                        new Point3D(points[i - 1].getX(), -height / 2, points[i - 1].getZ())
                    }));
                    faces.Add(new Face(new Point3D[] {
                        new Point3D(points[i - 1].getX(), -height / 2, points[i - 1].getZ()),
                        new Point3D(points[i].getX(), -height / 2, points[i].getZ()),
                        new Point3D(points[i].getX(), height / 2, points[i].getZ())
                    }));
                }
            }
        }
        public Point3D GetBasePoint()
        {
            return basePoint;
        }
        public void ModifyRadius(double radius)
        {
            Radius = radius;
            UpdatePoints();
        }

        public void ModifyHeight(double height)
        {
            this.height = height;
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
    }
}
