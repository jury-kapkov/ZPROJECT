using System;

namespace RayTracer
{
    class Face1
    {
        private Vertex A, B, C, D;
        public Face1(Vertex A, Vertex B, Vertex C, Vertex D)
        {
            this.A = A;
            this.B = B;
            this.C = C;
            this.D = D;
        }
        //Плоащь треугольника, заданного двумя векторами
        public float areaFromVectors(Vertex a, Vertex b)
        {
            Vertex c = new Vertex();
            c.X = a.Y * b.Z - a.Z * b.Y;
            c.Y = a.X * b.Z - a.Z * b.X;
            c.Z = a.X * b.Y - a.Y * b.X;
            return (float)Math.Sqrt(c.X * c.X + c.Y * c.Y + c.Z * c.Z) / 2;
        }
        // Принадлежит ли точка полигону
        public bool isPointInclude(Vertex E)
        {
            Vertex EA, EB, EC, ED;
            EA = A - E;
            EB = B - E;
            EC = C - E;
            ED = D - E;
            //Площади треугольников
            float areaAEB = areaFromVectors(EA, EB);
            float areaBEC = areaFromVectors(EB, EC);
            float areaCED = areaFromVectors(EC, ED);
            float areaDEA = areaFromVectors(ED, EA);
            //Длины сторон четырехугольника
            float lenAB = (A - B).getLength();
            float lenBC = (B - C).getLength();
            float lenCD = (C - D).getLength();
            float lenDA = (D - A).getLength();
            //Площадь четырехугольника
            float p = ( lenAB + lenBC + lenCD + lenDA ) / 2;
            float areaABCD = (float)Math.Sqrt((p - lenAB) * (p - lenBC) * (p - lenCD) * (p - lenDA));
            float difference = areaABCD - ( areaAEB + areaBEC + areaCED + areaDEA );
            float exp = 0.0001f;
            if (Math.Abs(difference) <= exp )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // Принадлежит ли точка треугольнику
        private bool isPointIncludeTriangle(Vertex E)
        {

            bool result = false;
            //Считаем первое уравнение системы
            float a = A.Y - B.Y;
            float b = B.X - A.X;
            float c = A.X * B.Y - B.X * A.Y;
            float valueAB_X = a * E.X + b * E.Y + c;
            float d = A.Z - B.Z;
            float e = A.X * B.Z - B.X * A.Z;
            float valueAB_Z = d * E.X + b * E.Z + e;

            a = B.Y - C.Y;
            b = C.X - B.X;
            c = B.X * C.Y - C.X * B.Y;
            float valueBC_X = a * E.X + b * E.Y + c;
            d = B.Z - C.Z;
            e = B.X * C.Z - C.X * B.Z;
            float valueBC_Z = d * E.X + b * E.Z + e;

            a = C.Y - A.Y;
            b = A.X - C.X;
            c = C.X * A.Y - A.X * C.Y;
            float valueCA_X = a * E.X + b * E.Y + c;
            d = C.Z - A.Z;
            e = C.X * A.Z - A.X * C.Z;
            float valueCA_Z = d * E.X + b * E.Z + e;


            if ((0 >= valueAB_X && 0 >= valueBC_X && 0 >= valueCA_X ) &&
                (0 >= valueAB_Z && 0 >= valueBC_Z && 0 >= valueCA_Z))
            {
                result = true;
            }
            return result;
        }
        //Получение нормали полигона
        private Vertex getFaceNoraml(ref float D)
        {
            float A, B, C;
            float[,] det = new float[3, 3];
            det[0, 0] = -this.A.X;
            det[1, 0] = -this.A.Y;
            det[2, 0] = -this.A.Z;

            det[0, 1] = this.B.X - this.A.X;
            det[1, 1] = this.B.Y - this.A.Y;
            det[2, 1] = this.B.Z - this.A.Z;

            det[0, 2] = this.C.X - this.A.X;
            det[1, 2] = this.C.Y - this.A.Y;
            det[2, 2] = this.C.Z - this.A.Z;
            //Начало магии
            A = det[1, 1] * det[2, 2] - det[1, 2] * det[2, 1];
            B = -(det[0, 1] * det[2, 2] - det[0, 2] * det[2, 1]);
            C = det[0, 1] * det[1, 2] - det[0, 2] * det[1, 1];
            D = det[0, 0] * A + det[1, 0] * B + det[2, 0] * C;
            //Возвращает еще свободные коэффициент D по ссылке
            return new Vertex(A, B, C);
        }
        //Получение точки пересечения полигона и прямой
        private Vertex getIntersectPoint(Vertex origin, Vertex direction, Vertex normalFace, float D)
        {
            Vertex Q = new Vertex();
            float t0 = -(normalFace.X * origin.X + normalFace.Y * origin.Y + normalFace.Z * origin.Z + D) /
                        (normalFace.X * direction.X + normalFace.Y * direction.Y + normalFace.Z * direction.Z);
            Q.X = direction.X * t0 + origin.X;
            Q.Y = direction.Y * t0 + origin.Y;
            Q.Z = direction.Z * t0 + origin.Z;
            return Q;
        }
        //есть ли пересечение (добавил дистанцию на будущее)
        public bool isIntersect(Vertex origin, Vertex direction, ref float distance)
        {
            float D = 0;
            Vertex normalFace = getFaceNoraml(ref D);
            //normalFace = new Vertex(0, 0, -25);
            Vertex Q = getIntersectPoint(origin, direction, normalFace, D);
            distance = (float)Math.Sqrt((origin.X - Q.X) * (origin.X - Q.X) + (origin.Y - Q.Y) * (origin.Y - Q.Y) + (origin.Z - Q.Z) * (origin.Z - Q.Z));
            //Q = new Vertex(1, 1, 0);
            return isPointInclude(Q);
        }
        //лежат ли 4 точки в 1 плоскости
        private bool isPlanar()
        {
            float d = 0;
            Vertex normal = getFaceNoraml(ref d);
            return normal.X * D.X + normal.Y * D.Y + normal.Z * D.Z + d == 0;
        }
    }
}
