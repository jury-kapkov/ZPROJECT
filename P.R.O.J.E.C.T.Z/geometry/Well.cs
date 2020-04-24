using System.Drawing;

namespace P.R.O.J.E.C.T.Z.geometry
{
    class Well : Group
    {
        public int CountNails = 12;
        public SceneObject Body, ColumnLeft, ColumnRight, CapFirst, CapSecond, Balka, Nail1, Nail2, Nail3, Nail4, Nail5, Nail6, 
            Nail7, Nail8, Nail9, Nail10, Nail11, Nail12, HandleBox, HandleCyl, HandleSphere, Rope, BucketBody, BucketBottom, BucketHandleLeft, BucketHandlRight,
             PlateDesk, PlateHandlFirst, PlateHandlSecond, PlateNail;
        Color[] Colors = new Color[5];
        public Well(string name) : base(name)
        {
            //Небесные цвета
            //Colors[0] = ColorTranslator.FromHtml("#4a707a");
            //Colors[1] = ColorTranslator.FromHtml("#7697a0");
            //Colors[2] = ColorTranslator.FromHtml("#94b0b7");
            //Colors[3] = ColorTranslator.FromHtml("#c2c8c5");
            //Colors[4] = ColorTranslator.FromHtml("#ddddda");
            //Циан
            //Colors[4] = ColorTranslator.FromHtml("#b9b6b9");
            //Colors[3] = ColorTranslator.FromHtml("#d4d5d9");
            //Colors[2] = ColorTranslator.FromHtml("#9dbdc4");
            //Colors[1] = ColorTranslator.FromHtml("#71bcc3");
            //Colors[0] = ColorTranslator.FromHtml("#00817d");
            //Лайм
            //Colors[4] = ColorTranslator.FromHtml("#8ab186");
            //Colors[3] = ColorTranslator.FromHtml("#b3c8cd");
            //Colors[2] = ColorTranslator.FromHtml("#f2f7f3");
            //Colors[1] = ColorTranslator.FromHtml("#e6efb9");
            //Colors[0] = ColorTranslator.FromHtml("#96ca00");
            //Кекс
            Colors[0] = ColorTranslator.FromHtml("#907d6f");
            Colors[1] = ColorTranslator.FromHtml("#f2e2cf");
            Colors[2] = ColorTranslator.FromHtml("#ffb456");
            Colors[3] = ColorTranslator.FromHtml("#fa556b");
            Colors[4] = ColorTranslator.FromHtml("#810c13");

            Body = new SceneObject(new Tube(new Point3D(0, -23, 0), 60, 60, 90, 10, 32, Colors[0]), "Труба колодца");
            ColumnLeft = new SceneObject(new Cylinder(new Point3D(85, 87, 0), 210,  Colors[1], 12, 18), "Первая колонна");
            ColumnRight = new SceneObject(new Cylinder(new Point3D(-85, 87, 0), 210,  Colors[1], 12, 18), "Вторая колонна");
            CapFirst = new SceneObject(new Box(new Point3D(0, 180, -43), 100, 200, 5, Colors[3]), "Крышка1");
            CapFirst.angleX = 29;
            CapSecond = new SceneObject(new Box(new Point3D(0, 180, 43), 100, 200, 5, Colors[3]), "Крышка2");
            CapSecond.angleX = -29;
            Balka = new SceneObject(new Cylinder(new Point3D(0, 160, 0), 145,  Colors[0], 5, 18), "Балка");
            Balka.angleX = 90;
            Balka.angleZ = 90;
            Nail1 = new SceneObject(new HalfSphere(new Point3D(80, 197, -12), 5, 5, 14,   Colors[2]), "Гвоздь1");
            Nail1.angleX = 30;
            Nail2 = new SceneObject(new HalfSphere(new Point3D(80, 180, -42), 5, 5, 14,   Colors[2]), "Гвоздь2");
            Nail2.angleX = 30;
            Nail3 = new SceneObject(new HalfSphere(new Point3D(80, 163, -72), 5, 5, 14,   Colors[2]), "Гвоздь3");
            Nail3.angleX = 30;
            Nail4 = new SceneObject(new HalfSphere(new Point3D(-80, 197, -12), 5, 5, 14,   Colors[2]), "Гвоздь4");
            Nail4.angleX = 30;
            Nail5 = new SceneObject(new HalfSphere(new Point3D(-80, 180, -42),5, 5, 14,   Colors[2]), "Гвоздь5");
            Nail5.angleX = 30;
            Nail6 = new SceneObject(new HalfSphere(new Point3D(-80, 163, -72), 5, 5, 14,   Colors[2]), "Гвоздь6");
            Nail6.angleX = 30;
            Nail7 = new SceneObject(new HalfSphere(new Point3D(80, 197, 12), 5, 5, 14,   Colors[2]), "Гвоздь7");
            Nail7.angleX = -30;
            Nail8 = new SceneObject(new HalfSphere(new Point3D(80, 180, 42), 5, 5, 14,   Colors[2]), "Гвоздь8");
            Nail8.angleX = -30;
            Nail9 = new SceneObject(new HalfSphere(new Point3D(80, 163, 72), 5, 5, 14,   Colors[2]), "Гвоздь9");
            Nail9.angleX = -30;
            Nail10 = new SceneObject(new HalfSphere(new Point3D(-80, 197, 12), 5, 5, 14,   Colors[2]), "Гвоздь10");
            Nail10.angleX = -30;
            Nail11 = new SceneObject(new HalfSphere(new Point3D(-80, 180, 42), 5, 5, 14,   Colors[2]), "Гвоздь11");
            Nail11.angleX = -30;
            Nail12 = new SceneObject(new HalfSphere(new Point3D(-80, 163, 72), 5, 5, 14,   Colors[2]), "Гвоздь12");
            Nail12.angleX = -30;
            HandleBox = new SceneObject(new Box(new Point3D(102, 144, 0), 15, 7, 40,  Colors[1]), "Ручка бокс");
            HandleCyl = new SceneObject(new Cylinder(new Point3D(122, 130, 0), 31, Colors[2], 4, 18), "Ручка цилиндр");
            HandleCyl.angleZ = 90;
            HandleSphere = new SceneObject(new Sphere(new Point3D(140, 130, 0),  Colors[0], 7, 7, 20), "Ручка сфера");
            Rope = new SceneObject(new Cylinder(new Point3D(0, 140, 0), 30, Colors[3], 2, 7), "Веревка");

            BucketBody = new SceneObject(new Tube(new Point3D(0, 60, 0), 25, 15, 45, 4, 18,  Colors[1]), "Корпус ведра");
            BucketBottom = new SceneObject(new Cylinder(new Point3D(0, 59, 0), 2,  Colors[0], 19, 18), "Днище");
            BucketHandleLeft = new SceneObject(new Box(new Point3D(-13, 119, 0), 2, 2, 38, Colors[2]), "Левая часть ручки");
            BucketHandleLeft.angleZ = 45;
            BucketHandlRight = new SceneObject(new Box(new Point3D(13, 119, 0), 2, 2, 40, Colors[2]), "Правая часть ручки");
            BucketHandlRight.angleZ = -45;

            PlateDesk = new SceneObject(new Box(new Point3D(-80, 100, -15), 2, 53, 25, Colors[4]), "Деревяшка");
            PlateHandlFirst = new SceneObject(new Box(new Point3D(-89, 120, -15), 2, 23, 2, Colors[3]), "Ручка левая");
            PlateHandlFirst.angleZ = -45;
            PlateHandlSecond = new SceneObject(new Box(new Point3D(-71, 120, -15), 2, 23, 2, Colors[3]), "Ручка правая");
            PlateHandlSecond.angleZ = 45;
            PlateNail = new SceneObject(new Cylinder(new Point3D(-80, 125, -15), 11, Colors[0], 2, 7), "Гвоздь");
            PlateNail.angleX = 90;

            groupObjects.Add(Body);
            groupObjects.Add(ColumnLeft);
            groupObjects.Add(ColumnRight);
            groupObjects.Add(CapFirst);
            groupObjects.Add(CapSecond);
            groupObjects.Add(Balka);
            groupObjects.Add(Nail1);
            groupObjects.Add(Nail2);
            groupObjects.Add(Nail3);
            groupObjects.Add(Nail4);
            groupObjects.Add(Nail5);
            groupObjects.Add(Nail6);
            groupObjects.Add(Nail7);
            groupObjects.Add(Nail8);
            groupObjects.Add(Nail9);
            groupObjects.Add(Nail10);
            groupObjects.Add(Nail11);
            groupObjects.Add(Nail12);
            groupObjects.Add(HandleBox);
            groupObjects.Add(HandleCyl);
            groupObjects.Add(HandleSphere);
            groupObjects.Add(Rope);
            groupObjects.Add(BucketBody);
            groupObjects.Add(BucketBottom);
            groupObjects.Add(BucketHandleLeft);
            groupObjects.Add(BucketHandlRight);
            groupObjects.Add(PlateDesk);
            groupObjects.Add(PlateHandlFirst);
            groupObjects.Add(PlateHandlSecond);
            groupObjects.Add(PlateNail);
        }
    }
}
