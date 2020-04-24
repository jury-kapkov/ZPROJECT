using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.geometry
{
    class Secret : Group
    {
        public SceneObject One, ZLeft, ZTop, ZBottom, ZRight;
        public Secret(string name) : base(name)
        {
            One = new SceneObject(new Box(new Point3D(0, 0, -1100), 80, 30, 300, Color.Gray), "Единица");
            ZLeft = new SceneObject(new Box(new Point3D(120, 0, -1100), 80, 30, 300, Color.Gray), "Ноль1 слева");
            ZRight = new SceneObject(new Box(new Point3D(280, 0, -1100), 80, 30, 300, Color.Gray), "Ноль1 справа");
            ZTop = new SceneObject(new Box(new Point3D(240, 240, -1100), 80, 30, 80, Color.Gray), "Ноль1 сверху");
            ZBottom = new SceneObject(new Box(new Point3D(240, 40, -1100), 80, 30, 300, Color.Gray), "Ноль1 снизу");

            groupObjects.Add(One);
            groupObjects.Add(ZLeft);
            groupObjects.Add(ZTop);
            groupObjects.Add(ZBottom);
        }
    }
}
