using P.R.O.J.E.C.T.Z.geometry;
using P.R.O.J.E.C.T.Z.mathLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z
{
    class SceneObject
    {
        //Fields
        public string name { get; set; }
        public Primitive primitive { get; set; }

        public string typeObj = "";
        
        public Point3D basePoint { get; set; }
        public double scaleCoeff = 1d;
        public double angleX { get; set; } = 0d;
        public double angleY { get; set; } = 0d;
        public double angleZ { get; set; } = 0d;
        public double maxRadius = 0;
        public double MaxLength
        {
            get
            {
                if (maxRadius == 0)
                {
                    double result = 0;
                    foreach (Face face in primitive.faces)
                    {
                        foreach (Point3D point in face.getPoints())
                        {
                            double length = point.Length;
                            result = Math.Max(length, result);
                        }
                    }
                    maxRadius = result * scaleCoeff;
                }
                return maxRadius;
            }
        }
        //--
        protected List<Face> faces = new List<Face>();
        //Methods
        public SceneObject(Primitive primitive, string name)
        {
            this.primitive = primitive;
            this.name = name;
        }
        public void scale(double coeff)
        {
            scaleCoeff = coeff;
            maxRadius = 0;
        }
        //--
        public List<Face> getFaces()
        {
            return faces;
        }
        public Array2D getModificationMatrix()
        {
            Array2D matrix = Matrix.GetIdentityMatrix(4);
            matrix = matrix.Multiply(Matrix.GetScaleMatrix(scaleCoeff));
           // matrix = matrix.Multiply(Matrix.GetMoveMatrix(basePoint.x, basePoint.y, basePoint.z));
            matrix = matrix.Multiply(Matrix.GetRotateXMatrix(angleX * Math.PI / 180));
            matrix = matrix.Multiply(Matrix.GetRotateYMatrix(angleY * Math.PI / 180));
            matrix = matrix.Multiply(Matrix.GetRotateZMatrix(angleZ * Math.PI / 180));
            return matrix;
        }
        //--
        private void calculateMaxRadius()
        {
            double maxRad = 0, curR;
            foreach (Face face in faces)
            {
                foreach (Point3D point in face.getPoints())
                {
                    curR = point.getX() * point.getX() + point.getY() * point.getY() + point.getZ() * point.getZ();
                    maxRad = (curR > maxRad) ? curR : maxRad;
                }
            }
            maxRadius = Math.Sqrt(maxRad) * scaleCoeff;
        }
        //--
        public double getMaxRadius()
        {
            if (maxRadius == 0)
            {
                calculateMaxRadius();
            }
            return maxRadius;
        }
    }
}
