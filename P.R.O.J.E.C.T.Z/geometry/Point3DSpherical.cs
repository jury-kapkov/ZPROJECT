using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.geometry
{
    class Point3DSpherical : Point3D
    {
        public double r;
        public double theta;
        public double phi;

        public Point3DSpherical(double r, double theta, double phi) : base(0, 0, 0)
        {
            this.r = r;
            this.theta = (theta > 0) ? theta : 0;
            this.phi = phi;
            UpdateXYZ();
        }

        public Point3DSpherical(Point3D p) : base(p)
        {
            this.setPoint(p.getX(), p.getY(), p.getZ());
        }
        private void UpdateXYZ(bool updateY = true)
        {
            x = r * Math.Sin(theta) * Math.Cos(phi);
            if (updateY)
            {
                y = r * Math.Cos(theta);
            }
            z = r * Math.Sin(theta) * Math.Sin(phi);
        }

        private void UpdateRTP()
        {
            r = Math.Sqrt(x * x + y * y + z * z);
            theta = Math.Max(Math.Acos(y / r), 0);
            phi = Math.Atan(z / x);
        }
        public override void add(double dx, double dy, double dz)
        {
            base.add(dx, dy, dz);
            UpdateRTP();
        }
        public override void setPoint(double x, double y, double z)
        {
            base.setPoint(x, y, z);
            UpdateRTP();
            if (x < 0)
            {
                phi = Math.PI + phi;
            }
            else if (z < 0)
            {
                phi = 2 * Math.PI + phi;
            }
        }
        public void setTheta(double theta)
        {
            this.theta = Math.Max(theta, 0);
            UpdateXYZ();
        }
        public void setPhi(double phi)
        {
            this.phi = phi;
            UpdateXYZ(false);
        }
        public double getTheta()
        {
            return theta;
        }
        public double getPhi()
        {
            return phi;
        }
    }
}
