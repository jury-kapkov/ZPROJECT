using P.R.O.J.E.C.T.Z.geometry;
using P.R.O.J.E.C.T.Z.mathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P.R.O.J.E.C.T.Z
{
    class Group
    {
        
        public string name;
        public Point3D BasePoint { get; set; }
        protected Point3D dirI = new Point3D(1, 0, 0);
        protected Point3D dirJ = new Point3D(0, 1, 0);
        protected Point3D dirK = new Point3D(0, 0, 1);
        public List<SceneObject> groupObjects = new List<SceneObject>();
        
        public double Scale { get; private set; } = 1.0;
        public double angleX { get; set; } = 0;
        public double angleY { get; set; } = 0;
        public double angleZ { get; set; } = 0;
        private double maxLength = 0;
        public double MaxLength
        {
            get
            {
                if (maxLength == 0)
                {
                    double result = 0;
                    foreach (SceneObject primitive in groupObjects)
                    {
                        result = Math.Max(primitive.getMaxRadius(), result);
                    }
                    maxLength = result * Scale;
                }
                return maxLength;
            }
        }
        public Group(string name)
        {
            this.name = name;
        }
        public bool AddObject(SceneObject obj)
        {
            if (CheckName(obj.name) == true)
            {
                groupObjects.Add(obj);
                return true;
            }
            else
            {
                MessageBox.Show("Объект по имени {0} уже существует, поэтому он добавлен в группу не будет");
                return false;
            }
        }
        private bool CheckName(string name)
        {
            foreach (SceneObject obj in groupObjects)
            {
                if (obj.name == name)
                {
                    return false;
                }
            }
            return true;
        }
        public void SetScale(double scale)
        {
            Scale = scale;
            maxLength = 0;
        }
        public Array2D GetModificationMatrix()
        {
            Array2D matrix = Matrix.GetIdentityMatrix(4);
            matrix = matrix.Multiply(Matrix.GetScaleMatrix(Scale));
            matrix = matrix.Multiply(Matrix.GetRotateXMatrix(angleX * Math.PI / 180));
            matrix = matrix.Multiply(Matrix.GetRotateYMatrix(angleY * Math.PI / 180));
            matrix = matrix.Multiply(Matrix.GetRotateZMatrix(angleZ * Math.PI / 180));
            return matrix;
        }
        public SceneObject GetScenePrimitiveByName(string name)
        {
            foreach (SceneObject scenePrimitive in groupObjects)
            {
                if (name.Equals(scenePrimitive.name))
                {
                    return scenePrimitive;
                }
            }
            return null;
        }
    }
}
