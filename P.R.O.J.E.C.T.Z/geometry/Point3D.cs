using P.R.O.J.E.C.T.Z.mathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.geometry
{
    class Point3D
    {
        public double x, y, z;
        public double Length { get { return (float)Math.Sqrt(x * x + y * y + z * z); } }
        public Point3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Point3D(Point3D point)
        {
            this.x = point.x;
            this.y = point.y;
            this.z = point.z;
        }
        public double getX()
        {
            return this.x;
        }
        public double getY()
        {
            return this.y;
        }
        public double getZ()
        {
            return this.z;
        }
        public virtual void setPoint(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public virtual void add(double dx, double dy, double dz)
        {
            this.x += dx;
            this.y += dy;
            this.z += dz;
        }
        
        public Array2D getProjectiveCoordinates()
        {
            double[] result = new double[4];
            result[0] = this.x;
            result[1] = this.y;
            result[2] = this.z;
            result[3] = 1d;
            return new Array2D(result);
        }   
        public Point3D copy()
        {
            return new Point3D(x,y,z);
        }
    }
}
