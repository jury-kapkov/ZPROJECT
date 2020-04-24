using P.R.O.J.E.C.T.Z.geometry;
using P.R.O.J.E.C.T.Z.mathLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P.R.O.J.E.C.T.Z
{
    class Scene
    {
        public Camera camera;
        private int drawMode;
        public Vector3D light;
        public static int cameraID = 0;
        public int uniqueObjectsCount { get; set; } = 0;
        public int uniqueGroupCount { get; set; } = 0;

        public List<Group> groups = new List<Group>();
        public List<Camera> cameras = new List<Camera>();
        public List<SceneObject> objects = new List<SceneObject>();

        PictureBox projectionScreen;
        public static int MAX_OBJECTS = 20;
        public Scene(PictureBox projectionScreen, int drawMode)
        {
            this.drawMode = drawMode;
            this.projectionScreen = projectionScreen;
            camera = new Camera(new Point3D(0, 0, -200), new Point3D(0, 0, 0), 50d, 1000d, projectionScreen.Width, projectionScreen.Height, "camera0");
            cameras.Add(camera);
        }
        public void paintObjects()
        {
            LinearGradientBrush linGrBrush = new LinearGradientBrush(
              new Point(camera.screenWidth / 2, 0),
              new Point(camera.screenWidth / 2, camera.screenHeight),
              Color.FromArgb(255, 14, 14, 14),
              Color.FromArgb(255, 66, 66, 66));
            Bitmap gradBack = new Bitmap(camera.screenWidth, camera.screenHeight);
            Graphics graphics = Graphics.FromImage(gradBack);
            graphics.FillRectangle(linGrBrush, 0, 0, camera.screenWidth, camera.screenHeight);
            FastBitmap bitmap = new FastBitmap(gradBack);

            //FastBitmap bitmap = new FastBitmap(camera.screenWidth, camera.screenHeight, Color.FromArgb(60, 60, 60));

            double[,] buffer = new double[camera.screenWidth, camera.screenHeight];
            for (int i = 0; i < camera.screenWidth; ++i)
                for (int j = 0; j < camera.screenHeight; ++j)
                    buffer[i, j] = double.MaxValue;

            Array2D cameraMatrix = camera.getCameraMatrix();

            for (int i = 0; i < groups.Count; ++i)
            {
                Group sceneObject = groups[i];
                if (IsVisibleForCamera(sceneObject.BasePoint, sceneObject.MaxLength, cameraMatrix))
                {
                    Array2D modificationMatrix = sceneObject.GetModificationMatrix();
                    foreach (SceneObject scenePrimitive in sceneObject.groupObjects)
                    {
                        Array2D primitiveMatrix = scenePrimitive.getModificationMatrix();
                        foreach (Face face in scenePrimitive.primitive.getFaces())
                        {
                            List<Point3D> points = new List<Point3D>();
                            foreach (Point3D point in face.getPoints())
                            {
                                points.Add(point.copy());
                            }

                            if (drawMode == 0 /*|| drawMode == 2 || drawMode == 3*/)
                            {
                                //DrawLines(bitmap, obj.basePoint, points, primitiveMatrix, face.getColor());
                                DrawLines(bitmap, sceneObject.BasePoint, scenePrimitive.primitive.basePoint, points, modificationMatrix, primitiveMatrix, scenePrimitive.primitive.color);
                            }
                            if (drawMode == 1 || drawMode == 2 || drawMode == 3)
                            {
                                ConvertLocalToCamera(sceneObject.BasePoint, scenePrimitive.primitive.basePoint, points, modificationMatrix, primitiveMatrix);
                                Face temp = new Face(new Point3D[] { points[0], points[1], points[2] });
                                Vector3D normal = temp.getNormalVector();
                                Vector3D light1 = new Vector3D(20, 20, 20);
                                double brightness = Math.Abs(Vector3D.AngleCos(normal, light1.normalize()));
                                brightness = Math.Max(brightness, 0.1);
                                //brightness *= Math.Abs(Vector3D.AngleCos(normal, light2.normalize()))
                                if (light1.x == 0 && light1.y == 0 && light1.z == 0)
                                {
                                    brightness = 0.1;
                                }
                                //Таргет не можнт иметь все нули - деление на ноль
                                int alpha = 255;
                                if (drawMode == 3) alpha = 160;
                                Color color = Color.FromArgb(alpha, (int)(scenePrimitive.primitive.color.R * brightness), (int)(scenePrimitive.primitive.color.G * brightness), (int)(scenePrimitive.primitive.color.B * brightness));

                                convertCameraToScreenCoords(points);

                                Triangle(new Vector3D(points[0]), new Vector3D(points[1]), new Vector3D(points[2]), color, bitmap, buffer);
                            }
                        }
                    }
                }
            }
            projectionScreen.Image = bitmap.GetBitmap();
        }
        private bool IsVisibleForCamera(Point3D basePoint, double maxLength, Array2D cameraMatrix)
        {
            bool result = false;

            Point3D point = basePoint.copy();
            Array2D cameraToBase = cameraMatrix.Multiply(point.getProjectiveCoordinates());
            point.setPoint(cameraToBase.GetValue(0, 0), cameraToBase.GetValue(1, 0), cameraToBase.GetValue(2, 0));

            if (point.getZ() + maxLength < camera.farClipZ && point.getZ() - maxLength > camera.nearClipZ)
            {
                if (!camera.getTypeOfProjection())
                {
                    double testZ = camera.screenHeight / 2 * point.getZ() / camera.getFocusDist();

                    if (point.getX() - maxLength < testZ && point.getX() + maxLength > -testZ)
                    {
                        testZ = camera.screenHeight / 2 * point.getZ() / camera.getFocusDist();

                        if (point.getY() - maxLength < testZ && point.getY() + maxLength > -testZ)
                        {
                            result = true;
                        }
                    }
                }
                else
                {
                    if (Math.Abs(point.getX()) + maxLength < camera.screenWidth / 2 && Math.Abs(point.getY()) + maxLength < camera.screenHeight / 2)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }
        private void DrawLines(FastBitmap bitmap, Point3D objectBasePoint, Point3D primitiveBasePoint, List<Point3D> points, Array2D objectMatrix, Array2D primitiveMatrix, Color color)
        {
            ConvertLocalToCamera(objectBasePoint, primitiveBasePoint, points, objectMatrix, primitiveMatrix);
            convertCameraToScreenCoords(points);
            for (int pi = 0; pi < points.Count - 1; ++pi)
            {
                for (int pj = pi + 1; pj < points.Count; ++pj)
                {
                    bitmap.DrawLine((int)points[pi].getX(), (int)points[pi].getY(), (int)points[pj].getX(), (int)points[pj].getY(), color);
                }
            }

        }
        public void SetDrawMode(int value)
        {
            drawMode = value;
        }
        private void Triangle(Vector3D p0, Vector3D p1, Vector3D p2, Color color, FastBitmap bitmap, double[,] buffer)
        {
            if (p0.y != p1.y || p0.y != p2.y)
            {
                if (p0.y > p1.y)
                {
                    Swap(ref p0, ref p1);
                }
                if (p0.y > p2.y)
                {
                    Swap(ref p0, ref p2);
                }
                if (p1.y > p2.y)
                {
                    Swap(ref p1, ref p2);
                }
                int totalHeight = (int)Math.Round(p2.y - p0.y);
                for (int i = 0; i < totalHeight; ++i)
                {
                    bool secondHalf = i > p1.y - p0.y || p1.y == p0.y;
                    int segmentHeight = (int)Math.Round(secondHalf ? p2.y - p1.y : p1.y - p0.y);
                    double alpha = (double)i / totalHeight;
                    double beta = (i - (secondHalf ? p1.y - p0.y : 0.0)) / segmentHeight;
                    Vector3D a = p0 + (p2 - p0) * alpha;
                    Vector3D b = (secondHalf ? p1 + (p2 - p1) * beta : p0 + (p1 - p0) * beta);
                    if (a.x > b.x)
                    {
                        Swap(ref a, ref b);
                    }
                    for (int j = (int)Math.Round(a.x); j <= (int)Math.Round(b.x); ++j)
                    {
                        double scale = (a.x == b.x) ? 1 : (j - a.x) / (b.x - a.x);
                        Vector3D p = a + (b - a) * scale;
                        int x = (int)Math.Round(p.x), y = (int)Math.Round(p.y);
                        if (x > 0 && x < bitmap.Width && y > 0 && y < bitmap.Height && (drawMode == 2 || buffer[x, y] >= p.z))
                        {
                            buffer[x, y] = p.z;
                            bitmap.SetPixel(x, y, color);
                        }
                    }
                }
            }
        }
        static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
        public void AddCamera(Camera camera, string name)
        {
            if (name == "")
            {
                name = "camera" + ++cameraID;
            }
            if (checkNameCamera(name))
            {
                cameras.Add(camera);
            }
            camera.name = name;
        }
        public void ResetCamera()
        {
            camera = new Camera(new Point3D(0, 0, -200), new Point3D(0, 0, 0), 50d, 1000d, projectionScreen.Width, projectionScreen.Height, "");
        }
        private bool checkNameCamera(string name)
        {
            foreach (Camera camera in cameras)
            {
                if (camera.name == name)
                {
                    return false;
                }
            }
            return true;
        }
        private void ConvertLocalToCamera(Point3D objectBasePoint, Point3D primitiveBasePoint, List<Point3D> points, Array2D objectMatrix, Array2D primitiveMatrix)
        {
            Array2D cameraMatrix = camera.getCameraMatrix();
            foreach (Point3D point in points)
            {
                Array2D result = primitiveMatrix.Multiply(point.getProjectiveCoordinates());
                point.setPoint(result.GetValue(0, 0) + primitiveBasePoint.getX(), result.GetValue(1, 0) + primitiveBasePoint.getY(), result.GetValue(2, 0) + primitiveBasePoint.getZ());
                result = objectMatrix.Multiply(point.getProjectiveCoordinates());
                point.setPoint(result.GetValue(0, 0) + objectBasePoint.getX(), result.GetValue(1, 0) + objectBasePoint.getY(), result.GetValue(2, 0) + objectBasePoint.getZ());
                result = cameraMatrix.Multiply(point.getProjectiveCoordinates());
                point.setPoint(result.GetValue(0, 0), result.GetValue(1, 0), result.GetValue(2, 0));
            }
        }
        private void convertCameraToScreenCoords(List<Point3D> points)
        {
            foreach (Point3D point in points)
            {
                double x = point.getX();
                double y = point.getY();
                double z = point.getZ();

                if (camera.getTypeOfProjection() == Camera.PARALLEL_PROJECTION)
                {
                    x = camera.getFocusDist() * x / z;
                    y = camera.getFocusDist() * y / z;
                }

                x += camera.screenWidth / 2;
                y += camera.screenHeight / 2;

                point.setPoint((int)x, (int)y, z);
            }
        }
        public Group GetObjectByName(string name)
        {
            foreach (Group group in groups)
            {
                if (name.Equals(group.name))
                {
                    return group;
                }
            }
            return null;
        }
        public void AddObject(Group group)
        {
            groups.Add(group);
        }
    }
}