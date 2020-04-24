using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.geometry
{
    class Sphere : Primitive
    {
        public string typeObj = "Sphere";
        public double Radius { get; set; }
        public int SegmentCount { get; set; }
        public Sphere(Point3D basePoint, Color color, double height, double Radius, int SegmentCount) : base(basePoint, height, color)
        {
            this.Radius = Radius;
            this.SegmentCount = SegmentCount;
            UpdatePoints();
        }
        private void UpdatePoints()
        {
            int quater = Math.Max(SegmentCount / 4 - 1, 1);
            double angle = 360.0 / SegmentCount;
            double sin = Math.Sin(-angle / 180 * Math.PI);
            double cos = Math.Cos(-angle / 180 * Math.PI);

            Point3D[] edges = new Point3D[quater];
            edges[0] = new Point3D(-Radius, 0, 0);
            for (int i = 1; i < quater; ++i)
            {
                double x = edges[i - 1].getX() * cos - edges[i - 1].getY() * sin;
                double y = edges[i - 1].getX() * sin + edges[i - 1].getY() * cos;
                edges[i] = new Point3D(x, y, 0);
            }

            Point3D[,] points = new Point3D[quater, SegmentCount + 1];
            getFaces().Clear();

            for (int i = 0; i <= SegmentCount; ++i)
            {
                for (int j = 0; j < quater; ++j)
                {
                    double x = edges[j].getX() * cos - edges[j].getZ() * sin;
                    double z = edges[j].getX() * sin + edges[j].getZ() * cos;
                    edges[j] = new Point3D(x, edges[j].getY(), z);
                    points[j, i] = edges[j];
                    if (i > 0)
                    {
                        if (j > 0)
                        {
                            faces.Add(new Face(new Point3D[] {
                                new Point3D(points[j - 1, i - 1].getX(), points[j - 1, i - 1].getY(), points[j - 1, i - 1].getZ()),
                                new Point3D(points[j - 1, i].getX(), points[j - 1, i].getY(), points[j - 1, i].getZ()),
                                new Point3D(points[j, i - 1].getX(), points[j, i - 1].getY(), points[j, i - 1].getZ())
                            }));
                            faces.Add(new Face(new Point3D[] {
                                new Point3D(points[j, i - 1].getX(), points[j, i - 1].getY(), points[j, i - 1].getZ()),
                                new Point3D(points[j, i].getX(), points[j, i].getY(), points[j, i].getZ()),
                                new Point3D(points[j - 1, i].getX(), points[j - 1, i].getY(), points[j - 1, i].getZ())
                            }));
                            faces.Add(new Face(new Point3D[] {
                                new Point3D(points[j - 1, i - 1].getX(), -points[j - 1, i - 1].getY(), points[j - 1, i - 1].getZ()),
                                new Point3D(points[j - 1, i].getX(), -points[j - 1, i].getY(), points[j - 1, i].getZ()),
                                new Point3D(points[j, i - 1].getX(), -points[j, i - 1].getY(), points[j, i - 1].getZ())
                            }));
                            faces.Add(new Face(new Point3D[] {
                                new Point3D(points[j, i - 1].getX(), -points[j, i - 1].getY(), points[j, i - 1].getZ()),
                                new Point3D(points[j, i].getX(), -points[j, i].getY(), points[j, i].getZ()),
                                new Point3D(points[j - 1, i].getX(), -points[j - 1, i].getY(), points[j - 1, i].getZ())
                            }));
                        }
                        if (j == quater - 1)
                        {
                           faces.Add(new Face(new Point3D[] {
                                new Point3D(points[j, i - 1].getX(), points[j, i - 1].getY(), points[j, i - 1].getZ()),
                                new Point3D(points[j, i].getX(), points[j, i].getY(), points[j, i].getZ()),
                                new Point3D(0, Radius, 0)
                            }));
                            faces.Add(new Face(new Point3D[] {
                                new Point3D(points[j, i - 1].getX(), -points[j, i - 1].getY(), points[j, i - 1].getZ()),
                                new Point3D(points[j, i].getX(), -points[j, i].getY(), points[j, i].getZ()),
                                new Point3D(0, -Radius, 0)
                            }));
                        }
                    }
                }
            }
        }
        public void ModifyRadius(double radius)
        {
            Radius = radius;
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
