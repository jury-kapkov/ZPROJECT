using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.geometry
{
    class Plate : Group
    {
        SceneObject Desk, HandlFirst, HandlSecond, Nail;
        public Plate(string name) : base(name)
        {
            Desk = new SceneObject(new Box(new Point3D(0, 0, 0), 2, 53, 25, Color.Gray), "Деревяшка");
            HandlFirst = new SceneObject(new Box(new Point3D(-9, 20, 0), 2, 23, 2, Color.Gray), "Ручка левая");
            HandlFirst.angleZ = -45;
            HandlSecond = new SceneObject(new Box(new Point3D(9, 20, 0), 2, 23, 2, Color.Gray), "Ручка правая");
            HandlSecond.angleZ = 45;
            Nail = new SceneObject(new Cylinder(new Point3D(0, 25, 0), 11, Color.SandyBrown, 2, 7), "Гвоздь");
            Nail.angleX = 90;
            groupObjects.Add(Desk);
            groupObjects.Add(HandlFirst);
            groupObjects.Add(HandlSecond);
            groupObjects.Add(Nail);
        }
    }
}
