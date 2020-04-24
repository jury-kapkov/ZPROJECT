using P.R.O.J.E.C.T.Z.mathLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.geometry
{
    class Face
    {
        //Fiellds
        public List<Point3D> points = new List<Point3D>();
        //Methods
        public Face(Point3D p1, Point3D p2, Point3D p3, Color color)
        {
            points.Add(p1);
            points.Add(p2);
            points.Add(p3);
        }
        public Face(Point3D[] p)
        {
            points.Add(p[0]);
            points.Add(p[1]);
            points.Add(p[2]);
        }
        public List<Point3D> getPoints()
        {
            return points;
        }
        public Vector3D getNormalVector()
        {
            double ux = points[1].getX() - points[0].getX();
            double uy = points[1].getY() - points[0].getY();
            double uz = points[1].getZ() - points[0].getZ();

            double vx = points[2].getX() - points[0].getX();
            double vy = points[2].getY() - points[0].getY();
            double vz = points[2].getZ() - points[0].getZ();

            Vector3D u = new Vector3D(ux, uy, uz);
            Vector3D v = new Vector3D(vx, vy, vz);
            //! Возможно функция работает не так
            return (u ^ v).normalize();
        }

        public Point3D getMediansIntersection()
        {
            Point3D p1 = getPoints()[0];
            Point3D p2 = getPoints()[1];
            Point3D p3 = getPoints()[2];

            double x23 = (p3.getX() + p2.getX()) / 2.0;
            double y23 = (p3.getY() + p2.getY()) / 2.0;
            double z23 = (p3.getZ() + p2.getZ()) / 2.0;

            double xRes = (p1.getX() + x23) * (2.0 / 3.0);
            double yRes = (p1.getY() + y23) * (2.0 / 3.0);
            double zRes = (p1.getZ() + z23) * (2.0 / 3.0);

            return new Point3D(xRes, yRes, zRes);
        }
    }
}
