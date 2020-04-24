using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.geometry
{
    class Bucket : Group
    {
        public SceneObject Body, Bottom, HandleLeft, HandlRight;
        public Bucket(string name) : base(name)
        {
            Body = new SceneObject(new Tube(new Point3D(0, 0, 0), 25, 15, 45, 4, 18, Color.Brown), "Корпус ведра");
            Bottom = new SceneObject(new Cylinder(new Point3D(0, -1, 0), 2, Color.SandyBrown, 19, 18), "Днище");
            HandleLeft = new SceneObject(new Box(new Point3D(-13, 59, 0), 2, 2, 38, Color.Gray), "Левая часть ручки");
            HandleLeft.angleZ = 45;
            HandlRight = new SceneObject(new Box(new Point3D(13, 59, 0), 2, 2, 40, Color.Gray), "Правая часть ручки");
            HandlRight.angleZ = -45;
            groupObjects.Add(Body);
            groupObjects.Add(Bottom);
            groupObjects.Add(HandleLeft);
            groupObjects.Add(HandlRight);
        }
    }
}
