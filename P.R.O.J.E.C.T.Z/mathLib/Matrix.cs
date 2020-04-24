using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.mathLib
{
    class Matrix
    {
        //Fields
        private static Array2D moveMatrix = Matrix.GetIdentityMatrix(4);
        private static Array2D scaleMatrix = Matrix.GetIdentityMatrix(4);
        private static Array2D rotateXMatrix = Matrix.GetIdentityMatrix(4);
        private static Array2D rotateYMatrix = Matrix.GetIdentityMatrix(4);
        private static Array2D rotateZMatrix = Matrix.GetIdentityMatrix(4);
        private static Array2D uvwMatrix = Matrix.GetIdentityMatrix(4);
        //Methods
        public static Array2D GetMoveMatrix(double dx, double dy, double dz)
        {
            moveMatrix.SetValue(0, 3, dx);
            moveMatrix.SetValue(1, 3, dy);
            moveMatrix.SetValue(2, 3, dz);

            return moveMatrix;
        }
        public static Array2D GetIdentityMatrix(int n)
        {
            double[,] result = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        result[i, j] = 1d;
                    }
                    else
                    {
                        result[i, j] = 0d;
                    }
                }
            }
            return new Array2D(result);
        }
        public static Array2D GetScaleMatrix(double sx, double sy, double sz)
        {
            scaleMatrix.SetValue(0, 0, sx);
            scaleMatrix.SetValue(1, 1, sy);
            scaleMatrix.SetValue(2, 2, sz);

            return scaleMatrix;
        }
        public static Array2D GetScaleMatrix(double coeff)
        {
            return GetScaleMatrix(coeff, coeff, coeff);
        }
        public static Array2D GetRotateXMatrix(double phi)
        {

            double cosPhi = Math.Cos(phi);
            double sinPhi = Math.Sin(phi);

            rotateXMatrix.SetValue(1, 1, cosPhi);
            rotateXMatrix.SetValue(2, 1, -sinPhi);
            rotateXMatrix.SetValue(1, 2, sinPhi);
            rotateXMatrix.SetValue(2, 2, cosPhi);

            return rotateXMatrix;
        }
        public static Array2D GetRotateYMatrix(double phi)
        {

            double cosPhi = Math.Cos(phi);
            double sinPhi = Math.Sin(phi);

            rotateYMatrix.SetValue(0, 0, cosPhi);
            rotateYMatrix.SetValue(0, 2, -sinPhi);
            rotateYMatrix.SetValue(2, 0, sinPhi);
            rotateYMatrix.SetValue(2, 2, cosPhi);

            return rotateYMatrix;
        }
        public static Array2D GetRotateZMatrix(double phi)
        {

            double cosPhi = Math.Cos(phi);
            double sinPhi = Math.Sin(phi);

            rotateZMatrix.SetValue(0, 0, cosPhi);
            rotateZMatrix.SetValue(1, 0, -sinPhi);
            rotateZMatrix.SetValue(0, 1, sinPhi);
            rotateZMatrix.SetValue(1, 1, cosPhi);

            return rotateZMatrix;
        }
        public static Array2D GetUVWMatrix(double ux, double uy, double uz, double vx, double vy, double vz, double wx, double wy, double wz)
        {
            uvwMatrix.SetValue(0, 0, ux);
            uvwMatrix.SetValue(1, 0, vx);
            uvwMatrix.SetValue(2, 0, wx);

            uvwMatrix.SetValue(0, 1, uy);
            uvwMatrix.SetValue(1, 1, vy);
            uvwMatrix.SetValue(2, 1, wy);

            uvwMatrix.SetValue(0, 2, uz);
            uvwMatrix.SetValue(1, 2, vz);
            uvwMatrix.SetValue(2, 2, wz);

            return uvwMatrix;
        }
    }
}
