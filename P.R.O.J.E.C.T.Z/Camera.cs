using P.R.O.J.E.C.T.Z.geometry;
using P.R.O.J.E.C.T.Z.mathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z
{
    class Camera
    {
        public string name;

        public static bool PARALLEL_PROJECTION = false;
        public static bool CENTRAL_PROJECTION = true;
        private static double MIN_FOV = 60d;
        private static double MAX_FOV = 135d;
        public const double MIN_THETA = 1;
        public const double MAX_THETA = 179;
        private double screenCenterX;
        private double screenCenterY;
        public Point3DSpherical position { get; set; }
        public Point3DSpherical targetPoint { get; set; }
        public Vector3D u { get; private set; }
        public Vector3D v { get; private set; }
        public Vector3D w { get; private set; }
        private bool typeOfProjection = PARALLEL_PROJECTION;
        public int anglePhi = 100;
        public int angleTheta = 90;
        private Array2D camMatrix;
        private double fov = 90d;
        public double nearClipZ { get; set; }
        public double farClipZ { get; set; }
        public int screenWidth { get; set; }
        public int screenHeight { get; set; }
        public double aspectRatio { get; private set; }

        public bool getTypeOfProjection()
        {
            return typeOfProjection;
        }
        public double getFOV()
        {
            return fov;
        }
        public double getFocusDist()
        {
            return (screenWidth - 1) / 2.0 * Math.Tan((fov / 2.0) * Math.PI / 180);
        }
        public void setTypeOfProjection(bool typeOfProjection)
        {
            this.typeOfProjection = typeOfProjection;
        }
        public int getAnglePhi()
        {
            return anglePhi;
        }
        public int getAngleTheta()
        {
            return angleTheta;
        }
        public void setFov(double fov)
        {
            this.fov = fov;
        }
        public Camera()
        {

        }
        public Camera(Point3D position, Point3D targetPoint, double nearClipZ, double farClipZ, int screenWidth, int screenHeight, string name)
        {
            Array2D camMatrix = Matrix.GetIdentityMatrix(4);

            this.position = new Point3DSpherical(position);
            this.targetPoint = new Point3DSpherical(targetPoint);
            this.nearClipZ = nearClipZ;
            this.farClipZ = farClipZ;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.name = name;

            u = Vector3D.PLUS_I;
            v = Vector3D.PLUS_J;
            w = Vector3D.PLUS_K;

            screenCenterX = (this.screenWidth - 1) / 2;
            screenCenterY = (this.screenHeight - 1) / 2;

            aspectRatio = (double)this.screenWidth / this.screenHeight;
        }
        public void setScreenSize(int width, int height)
        {
            screenWidth = width;
            screenHeight = height;

            screenCenterX = (this.screenWidth - 1) / 2;
            screenCenterY = (this.screenHeight - 1) / 2;

            aspectRatio = (double)this.screenWidth / this.screenHeight;
        }
        public Array2D getCameraMatrix()
        {
            Array2D matrixInverted = Matrix.GetMoveMatrix(-position.getX(), -position.getY(), -position.getZ());

            w = new Vector3D(targetPoint.getX() - position.getX(), targetPoint.getY() - position.getY(), targetPoint.getZ() - position.getZ());

            v = Vector3D.PLUS_J;
            u = v ^ w;
            v = u ^ w;

            u = u.normalize();
            v = v.normalize();
            w = w.normalize();

            camMatrix = Matrix.GetUVWMatrix(
                u.x, u.y, u.z,
                v.x, v.y, v.z,
                w.x, w.y, w.z);
            camMatrix = camMatrix.Multiply(matrixInverted);

            return camMatrix;
        }
        public void rotateUpDown(double angle)
        {
            Array2D rotateMatrix = Matrix.GetMoveMatrix(-targetPoint.getX(), -targetPoint.getY(), -targetPoint.getZ());
            rotateMatrix = rotateMatrix.Multiply(position.getProjectiveCoordinates());
            position.setPoint(rotateMatrix.GetValue(0, 0), rotateMatrix.GetValue(1, 0), rotateMatrix.GetValue(2, 0));

            angle += GetDegree(position.getTheta());
            angle = Math.Min(Math.Max(angle, MIN_THETA), MAX_THETA);
            position.setTheta(GetRadians(angle));
            angleTheta = 180 - (int)angle;

            rotateMatrix = Matrix.GetMoveMatrix(targetPoint.getX(), targetPoint.getY(), targetPoint.getZ());
            rotateMatrix = rotateMatrix.Multiply(position.getProjectiveCoordinates());
            position.setPoint(rotateMatrix.GetValue(0, 0), rotateMatrix.GetValue(1, 0), rotateMatrix.GetValue(2, 0));
        }
        public void rotateLeftRight(double angle)
        {
            Array2D rotateMatrix = Matrix.GetMoveMatrix(-targetPoint.getX(), -targetPoint.getY(), -targetPoint.getZ());
            rotateMatrix = rotateMatrix.Multiply(position.getProjectiveCoordinates());
            position.setPoint(rotateMatrix.GetValue(0, 0), rotateMatrix.GetValue(1, 0), rotateMatrix.GetValue(2, 0));

            angle += GetDegree(position.getPhi());
            angle %= 360;
            angle = angle < 0 ? 360 + angle : angle;

            position.setPhi(GetRadians(angle));
            anglePhi = (int)angle;

            rotateMatrix = Matrix.GetMoveMatrix(targetPoint.getX(), targetPoint.getY(), targetPoint.getZ());
            rotateMatrix = rotateMatrix.Multiply(position.getProjectiveCoordinates());
            position.setPoint(rotateMatrix.GetValue(0, 0), rotateMatrix.GetValue(1, 0), rotateMatrix.GetValue(2, 0));
        }
        public void rotateCtrlUpDown(double angle)
        {
            angle = GetDegree(angle);

            Array2D rotateMatrix = Matrix.GetMoveMatrix(-position.getX(), -position.getY(), -position.getZ());
            rotateMatrix = rotateMatrix.Multiply(targetPoint.getProjectiveCoordinates());
            targetPoint.setPoint(rotateMatrix.GetValue(0, 0), rotateMatrix.GetValue(1, 0), rotateMatrix.GetValue(2, 0));

            angle += GetDegree(targetPoint.getTheta());
            angle = Math.Min(Math.Max(angle, MIN_THETA), MAX_THETA);
            position.setTheta(GetRadians(angle));
            angleTheta = 180 - (int)angle;

            rotateMatrix = Matrix.GetMoveMatrix(position.getX(), position.getY(), position.getZ());
            rotateMatrix = rotateMatrix.Multiply(targetPoint.getProjectiveCoordinates());
            targetPoint.setPoint(rotateMatrix.GetValue(0, 0), rotateMatrix.GetValue(1, 0), rotateMatrix.GetValue(2, 0));
        }
        public void rotateCtrlLeftRight(double angle)
        {
            Array2D rotateMatrix = Matrix.GetMoveMatrix(-position.getX(), -position.getY(), -position.getZ());
            rotateMatrix = rotateMatrix.Multiply(targetPoint.getProjectiveCoordinates());
            targetPoint.setPoint(rotateMatrix.GetValue(0, 0), rotateMatrix.GetValue(1, 0), rotateMatrix.GetValue(2, 0));

            angle += GetDegree(targetPoint.phi);
            angle %= 360;
            angle = angle < 0 ? 360 + angle : angle;

            targetPoint.setPhi(GetRadians(angle));
            anglePhi = (int)angle;

            rotateMatrix = Matrix.GetMoveMatrix(position.getX(), position.getY(), position.getZ());
            rotateMatrix = rotateMatrix.Multiply(targetPoint.getProjectiveCoordinates());
            targetPoint.setPoint(rotateMatrix.GetValue(0, 0), rotateMatrix.GetValue(1, 0), rotateMatrix.GetValue(2, 0));
        }
        public void CameraParallelMoveLeftRight(double dx)
        {
            Vector3D vector = new Vector3D(targetPoint) - new Vector3D(position);
            vector = vector.normalize();
            if (Math.Abs((vector ^ Vector3D.PLUS_J).x) < 0.1 && Math.Abs((vector ^ Vector3D.PLUS_J).y) < 0.001 && Math.Abs((vector ^ Vector3D.PLUS_J).z) < 0.001)
            {
                vector = vector ^ Vector3D.PLUS_K;
            }
            else
            {
                vector = vector ^ Vector3D.PLUS_J;
            }
            vector = vector * dx;
            targetPoint.add(vector.x, vector.y, vector.z);
            position.add(vector.x, vector.y, vector.z);
        }
        public void CameraParallelMoveUpDown(double dy)
        {
            Vector3D vector = new Vector3D(targetPoint) - new Vector3D(position);
            vector = vector.normalize();
            //vector = vector ^ Vector3D.PLUS_I;
            if (Math.Abs((vector ^ Vector3D.PLUS_I).x) < 0.001 && Math.Abs((vector ^ Vector3D.PLUS_I).y) < 0.1 && Math.Abs((vector ^ Vector3D.PLUS_I).z) < 0.001)
            {
                vector = vector ^ Vector3D.PLUS_K;
            }
            else
            {
                vector = vector ^ Vector3D.PLUS_I;
            }
            vector = vector * dy;
            targetPoint.add(vector.x, vector.y, vector.z);
            position.add(vector.x, vector.y, vector.z);
        }
        public void changeFOV(double diffFOV)
        {
            fov += diffFOV;
            if (fov < MIN_FOV)
            {
                fov = MIN_FOV;
            }
            if (fov > MAX_FOV)
            {
                fov = MAX_FOV;
            }
        }
        private double GetRadians(double angle)
        {
            return angle * Math.PI / 180;
        }
        private double GetDegree(double angle)
        {
            return angle * 180 / Math.PI;
        }
    }
}
