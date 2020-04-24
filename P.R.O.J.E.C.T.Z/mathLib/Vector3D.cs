using P.R.O.J.E.C.T.Z.geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.mathLib
{
    class Vector3D
    {
        //Const
        public static Vector3D ZERO = new Vector3D(0.0D, 0.0D, 0.0D);
        public static Vector3D PLUS_I = new Vector3D(1.0D, 0.0D, 0.0D);
        public static Vector3D MINUS_I = new Vector3D(-1.0D, 0.0D, 0.0D);
        public static Vector3D PLUS_J = new Vector3D(0.0D, 1.0D, 0.0D);
        public static Vector3D MINUS_J = new Vector3D(0.0D, -1.0D, 0.0D);
        public static Vector3D PLUS_K = new Vector3D(0.0D, 0.0D, 1.0D);
        public static Vector3D MINUS_K = new Vector3D(0.0D, 0.0D, -1.0D);
        //Fields
        public double x { get; private set; }
        public double y { get; private set; }
        public double z { get; private set; }
        public double Length { get { return (float)Math.Sqrt(x * x + y * y + z * z); } }
        //Methods
        public Vector3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3D(double[] vector)
        {
            this.x = vector[0];
            this.y = vector[1];
            this.z = vector[2];
        }
        public Vector3D(Point3D point)
        {
            this.x = point.getX();
            this.y = point.getY();
            this.z = point.getZ();
        }
        public Vector3D cross(Vector3D v)
        {
            return new Vector3D(this.y * v.z - this.z * v.y, this.z * v.x - this.x * v.z, this.x * v.y - this.y * v.x);
        }
        public Vector3D cross(Vector3D v, Vector3D u)
        {
            return new Vector3D(u.y * v.z - u.z * v.y, u.z * v.x - u.x * v.z, u.x * v.y - u.y * v.x);
        }
        public Vector3D scalarMultiply(double a)
        {
            return new Vector3D(a * this.x, a * this.y, a * this.z);
        }
        public double getNorm()
        {
            return Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
        }
        public Vector3D normalize()
        {
            double s = this.getNorm();
            try
            {
                return this.scalarMultiply(1.0D / s);
            }
            catch
            {
                return null;
            }
        }
        public double dot(Vector3D v)
        {
            return this.x * v.x + this.y * v.y + this.z * v.z;
        }
        public double angle(Vector3D v1, Vector3D v2)
        {
            double normProduct = v1.getNorm() * v2.getNorm();
            if (normProduct == 0.0D)
            {
                return -1;
            }
            else
            {
                double dot = v1.dot(v2);
                double threshold = normProduct * 0.9999D;
                if (dot >= -threshold && dot <= threshold)
                {
                    return Math.Acos(dot / normProduct);
                }
                else
                {
                    Vector3D v3 = cross(v1, v2);
                    return dot >= 0.0D ? Math.Asin(v3.getNorm() / normProduct) : 3.141592653589793D - Math.Asin(v3.getNorm() / normProduct);
                }
            }
        }

        public static Vector3D operator -(Vector3D first, Vector3D second)
        {
            Vector3D result = new Vector3D(
                first.x - second.x,
                first.y - second.y,
                first.z - second.z
            );
            return result;
        }
        public static Vector3D operator ^(Vector3D u, Vector3D v)
        {
            return new Vector3D(u.y * v.z - u.z * v.y, u.z * v.x - u.x * v.z, u.x * v.y - u.y * v.x);
        }
        public static Vector3D operator +(Vector3D first, Vector3D second)
        {
            Vector3D result = new Vector3D(
                first.x + second.x,
                first.y + second.y,
                first.z + second.z
            );
            return result;
        }
        public static Vector3D operator *(Vector3D Vector3D, double scale)
        {
            return new Vector3D(Vector3D.x * scale, Vector3D.y * scale, Vector3D.z * scale);
        }

        public static Vector3D operator /(Vector3D Vector3D, double scale)
        {
            return new Vector3D(Vector3D.x / scale, Vector3D.y / scale, Vector3D.z / scale);
        }
        public static double operator *(Vector3D first, Vector3D second)
        {
            return first.x * second.x + first.y * second.y + first.z * second.z;
        }
        public static double AngleCos(Vector3D first, Vector3D second)
        {
            return (first * second) / Math.Max((first.Length * second.Length), 1);
        }
    }
}
