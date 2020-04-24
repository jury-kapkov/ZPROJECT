using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.geometry
{
    class Box : Primitive
    {
        //Fields
        public string typeObj = "Box";
        public double width;
        public double length;
        public Box(Point3D basePoint, double length, double width, double height, Color color) : base(basePoint, height, color)
        {
            this.length = length;
            this.width = width;
            modifySize();
        }
        public void modifyLength(double newLength)
        {
            this.length = newLength;
            modifySize();
        }
        public void modifyWidth(double newWidth)
        {
            this.width = newWidth;
            modifySize();
        }
        public void modifyHeight(double newHeight)
        {
            this.height = newHeight;
            modifySize();
        }
        public void modifyBasePoint(Point3D newBasePoint)
        {
            this.basePoint = newBasePoint;
            modifySize();
        }
        public void modifySize()
        {
            double x = width / 2;
            double y = height / 2;
            double z = length / 2;

            List<Point3D> points = new List<Point3D>();

            points.Add(new Point3D(-x, -y, -z));
            points.Add(new Point3D(-x, -y, z));
            points.Add(new Point3D(x, -y, z));
            points.Add(new Point3D(x, -y, -z));
            points.Add(new Point3D(-x, y, -z));
            points.Add(new Point3D(-x, y, z));
            points.Add(new Point3D(x, y, z));
            points.Add(new Point3D(x, y, -z));

            faces.Clear();
            faces.Add(new Face(new Point3D[] { points[7], points[4], points[0] }));
            faces.Add(new Face(new Point3D[] { points[0], points[3], points[7] }));
            faces.Add(new Face(new Point3D[] { points[6], points[7], points[3] }));
            faces.Add(new Face(new Point3D[] { points[2], points[6], points[3] }));
            faces.Add(new Face(new Point3D[] { points[5], points[6], points[2] }));
            faces.Add(new Face(new Point3D[] { points[1], points[5], points[2] }));
            faces.Add(new Face(new Point3D[] { points[4], points[5], points[1] }));
            faces.Add(new Face(new Point3D[] { points[0], points[4], points[1] }));
            faces.Add(new Face(new Point3D[] { points[2], points[3], points[0] }));
            faces.Add(new Face(new Point3D[] { points[1], points[2], points[0] }));
            faces.Add(new Face(new Point3D[] { points[6], points[4], points[7] }));
            faces.Add(new Face(new Point3D[] { points[6], points[5], points[4] }));
        }
        public Point3D getBasePoint()
        {
            return basePoint;
        }
    }
}
