using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.geometry
{
    class Primitive
    {
        //Fields
        public Point3D basePoint;
        public double height;
        public Color color;
        public List<Face> faces = new List<Face>();
        //Metgods
        public Primitive(Point3D basePoint, double height, Color color)
        {
            this.basePoint = basePoint;
            this.height = height;
            this.color = color;
        }
        public List<Face> getFaces()
        {
            return faces;
        }
    }
}
