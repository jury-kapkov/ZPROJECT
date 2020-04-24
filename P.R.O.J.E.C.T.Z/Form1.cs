using P.R.O.J.E.C.T.Z.geometry;
using P.R.O.J.E.C.T.Z.mathLib;
using System;
using System.Xml.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Management;
using System.Threading;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace P.R.O.J.E.C.T.Z
{
    public partial class Form1 : Form
    {
        Scene scene;
        bool pressed = false;
        //0 - сетка 1 - цвет 2 - комби 3 - х-рей
        int viewMode = 0;
        public Form1()
        {
            InitializeComponent();
            InitScene();
            InitFormElements();

            timer1.Start();
        }
        SceneObject currentPrimitive = null;
        Group currentObject = null;
        private Group parentObject = null;

        //Camera Cam;
        private void InitFormElements()
        {
            Image.MouseWheel += new MouseEventHandler(Canvas_MouseWheel);
            //Init списка света
            //comboLight.Items.Add("Свет 1");
            //comboLight.Items.Add("Свет 2");
            //comboLight.Items.Add("Свет 1 + 2");
            //comboLight.SelectedIndex = 0;

            foreach (Group group in scene.groups)
            {
                comboObj.Items.Add(group.name);
            }
            //Init списка камер
            foreach (Camera camera in scene.cameras)
            {
                comboCamera.Items.Add(camera.name);
            }

            if (comboObj.Items.Count != 0)
            {
                comboObj.SelectedIndex = 0;
            }
            else
            {
                AllLock();
            }
            comboCamera.SelectedIndex = 0;
            comboDrawMode.SelectedIndex = 0;
            comboProjection.SelectedIndex = 0;

            //Cam = scene.GetCameraByName(comboCamera.SelectedItem.ToString());

            InitObjPanel(currentPrimitive == null ? "" : currentPrimitive.typeObj);

            InitCameraPanel();
            //Создание
            //Колор пикер
            colorPicker.BackColor = Color.LimeGreen;


        }
        private void InitScene()
        {
            scene = new Scene(Image, viewMode);
            //Установка проецирования и отрисовки по умолчанию
            scene.camera.setTypeOfProjection(true);
            //Цвет фигуры
            pickedColor = colorPicker.BackColor;

        }
        private void Canvas_MouseWheel(object sender, MouseEventArgs e)
        {
            double Ax = scene.camera.position.getX();
            double Ay = scene.camera.position.getY();
            double Az = scene.camera.position.getZ();
            double Bx = scene.camera.targetPoint.getX();
            double By = scene.camera.targetPoint.getY();
            double Bz = scene.camera.targetPoint.getZ();

            double length = Math.Sqrt((Bx - Ax) * (Bx - Ax) + (By - Ay) * (By - Ay) + (Bz - Az) * (Bz - Az));
            double k = (e.Delta / 20) / length;
            double Cx = Ax + (Bx - Ax) * k;
            double Cy = Ay + (By - Ay) * k;
            double Cz = Az + (Bz - Az) * k;

            scene.camera.position = new Point3DSpherical(new Point3D(Cx, Cy, Cz));
            //Обновляем значения формы
            if (!pressed)
            {
                camPosX.Value = (decimal)Cx;
                camPosY.Value = (decimal)Cy;
                camPosZ.Value = (decimal)Cz;
            }
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (scene != null)
            {
                scene.paintObjects();
            }
        }

        int xPos;
        int yPos;
        //0 - ЛКМ 1 - СКМ 2 - ПКМ
        int typePressedMouse;
        private void Image_MouseDown(object sender, MouseEventArgs e)
        {
            Focus();
            if (e.Button == MouseButtons.Left)
            {
                typePressedMouse = 0;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                typePressedMouse = 1;
            }
            else
            {
                typePressedMouse = 2;
            }
            xPos = e.Location.X;
            yPos = e.Location.Y;
            pressed = true;
        }

        private void Image_MouseUp(object sender, MouseEventArgs e)
        {
            pressed = false;
        }
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (pressed)
            {
                int dx = e.Location.X - xPos;
                int dy = e.Location.Y - yPos;

                if (typePressedMouse == 0)
                {
                    scene.camera.rotateLeftRight(-dx);
                    scene.camera.rotateUpDown(-dy);
                }
                else if (typePressedMouse == 1)
                {
                    //Vector3D direction = new Vector3D(scene.camera.targetPoint.x - scene.camera.position.x, scene.camera.targetPoint.y - scene.camera.position.y, scene.camera.targetPoint.z - scene.camera.position.z);
                    ////direction = direction.normalize();
                    //double k = Math.Abs(Vector3D.AngleCos(direction, Vector3D.PLUS_K));
                    scene.camera.CameraParallelMoveLeftRight(dx);
                    scene.camera.CameraParallelMoveUpDown(dy);
                }
                else
                {
                    scene.camera.rotateCtrlLeftRight(dx * Math.PI / 180);
                    scene.camera.rotateCtrlUpDown(-dy * Math.PI / 180);
                }

                xPos = e.Location.X;
                yPos = e.Location.Y;
                try
                {
                    //Обновляем значения формы
                    camPosX.Value = (decimal)scene.camera.position.getX();
                    camPosY.Value = (decimal)scene.camera.position.getY();
                    camPosZ.Value = (decimal)scene.camera.position.getZ();
                    camTarX.Value = (decimal)scene.camera.targetPoint.getX();
                    camTarY.Value = (decimal)scene.camera.targetPoint.getY();
                    camTarZ.Value = (decimal)scene.camera.targetPoint.getZ();
                    camRotX.Value = scene.camera.getAnglePhi();
                    camRotY.Value = scene.camera.getAngleTheta();
                }
                catch
                {
                    MessageBox.Show("Камера сильно далеко");
                    scene.ResetCamera();
                    InitCameraPanel();
                    pressed = false;
                }
            }
        }

        private void ComboProjection_SelectedIndexChanged(object sender, EventArgs e)
        {
            scene.camera.setTypeOfProjection(comboProjection.SelectedIndex == 0 ? false : true);
        }

        private void ComboDrawMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            scene.SetDrawMode(comboDrawMode.SelectedIndex);
        }

        private void ObjPosX_ValueChanged(object sender, EventArgs e)
        {
            changePos();
        }

        private void ObjPosY_ValueChanged(object sender, EventArgs e)
        {
            changePos();
        }

        private void ObjPosZ_ValueChanged(object sender, EventArgs e)
        {
            changePos();
        }
        private void changePos()
        {
            if (!objectInit && !initGroup)
            {
                if (currentPrimitive != null)
                {
                    currentPrimitive.primitive.basePoint.setPoint((double)objPosX.Value, (double)objPosY.Value, (double)objPosZ.Value);
                }
                else
                {
                    currentObject.BasePoint.setPoint((double)objPosX.Value, (double)objPosY.Value, (double)objPosZ.Value);
                }
            }
            //if (!objectInit)
            //{
            //    initGroup = false;
            //    if (currentPrimitive != null)
            //    {
            //        currentPrimitive.primitive.basePoint.setPoint((double)objPosX.Value, (double)objPosY.Value, (double)objPosZ.Value);
            //    }
            //    initGroup = true;
            //}
            //if (!initGroup)
            //{
            //    if (currentPrimitive != null)
            //    {
            //        currentObject.BasePoint.setPoint((double)objPosX.Value, (double)objPosY.Value, (double)objPosZ.Value);
            //    }
        //}

        }

        private void ObjRotX_ValueChanged(object sender, EventArgs e)
        {
            if (currentPrimitive != null)
            {
                currentPrimitive.angleX = ((double)objRotX.Value);
            }
            else
            {
                currentObject.angleX = ((double)objRotX.Value);
            }
        }

        private void ObjRotY_ValueChanged(object sender, EventArgs e)
        {
            if (currentPrimitive != null)
            {
                currentPrimitive.angleY = ((double)objRotY.Value);
            }
            else
            {
                currentObject.angleY = ((double)objRotY.Value);
            }
        }

        private void ObjRotZ_ValueChanged(object sender, EventArgs e)
        {
            if (currentPrimitive != null)
            {
                currentPrimitive.angleZ = ((double)objRotZ.Value);
            }
            else
            {
                currentObject.angleZ = ((double)objRotZ.Value);
            }
        }

        private void ObjScale_ValueChanged(object sender, EventArgs e)
        {
            if (currentPrimitive != null)
            {
                currentPrimitive.scaleCoeff = (double)objScale.Value;
            }
            else
            {
                currentObject.SetScale((double)objScale.Value);
            }
        }
        private void ObjPosX_MouseClick(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Right == e.Button)
            {
                ((NumericUpDown)sender).Value = 0;
            }
        }
        private void changeCamPos()
        {
            if (!pressed && CameraValueChanged)
                scene.camera.position = new Point3DSpherical(new Point3D((double)camPosX.Value, (double)camPosY.Value, (double)camPosZ.Value));
        }
        private void changeCamTar()
        {
            if (!pressed && CameraValueChanged)
                scene.camera.targetPoint.setPoint((double)camTarX.Value, (double)camTarY.Value, (double)camTarZ.Value);
        }

        private void CamTarX_ValueChanged(object sender, EventArgs e)
        {
            changeCamTar();
        }

        private void CamPosX_ValueChanged_1(object sender, EventArgs e)
        {
            changeCamPos();
        }

        private void CamPosY_ValueChanged(object sender, EventArgs e)
        {
            changeCamPos();
        }

        private void CamPosZ_ValueChanged_1(object sender, EventArgs e)
        {
            changeCamPos();
        }

        private void CamTarY_ValueChanged(object sender, EventArgs e)
        {
            changeCamTar();
        }

        private void CamTarZ_ValueChanged(object sender, EventArgs e)
        {
            changeCamTar();
        }

        private void CamNear_ValueChanged(object sender, EventArgs e)
        {
            scene.camera.nearClipZ = (double)camNear.Value;
        }

        private void CamFar_ValueChanged(object sender, EventArgs e)
        {
            scene.camera.farClipZ = (double)camFar.Value;
        }

        private void CamFov_ValueChanged(object sender, EventArgs e)
        {
            if (!pressed)
                scene.camera.setFov((double)camFov.Value);
        }
        private void Button1_Click_1(object sender, EventArgs e)
        {
            if (InCurGroup.Checked == false)
            {
                parentObject = CreateParent(nameObjBox.Text);
            }
            else
            {
                if (currentObject != null)
                {
                    parentObject = currentObject;
                }
                else
                {
                    parentObject = CreateParent(nameObjBox.Text);
                    MessageBox.Show("Не была выбрана группа! Объект будет создан отдельно");
                }
            }
            AddObject(nameObjBox.Text == "" ? "Куб " + ++scene.uniqueObjectsCount : nameObjBox.Text, "Box");
            nameObjBox.Text = "";
            GenerateColorForm();
            currentObject = null;
        }
        //Генерация нового цвета в colorPaecker
        private void GenerateColorForm()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            colorPicker.BackColor = Color.FromArgb(random.Next(100, 230), random.Next(100, 230), random.Next(100, 230));
            pickedColor = colorPicker.BackColor;
        }
        private void ComboObj_SelectedIndexChanged(object sender, EventArgs e)
        {
            AllUnlock();
            currentPrimitive = null;
            currentObject = scene.GetObjectByName(comboObj.SelectedItem.ToString());
            childObjects.Rows.Clear();
            childObjects.Rows.Add();
            childObjects.Rows[0].Cells[0].Value = "Модификация группы";
            foreach (SceneObject scenePrimitive in currentObject.groupObjects)
            {
                childObjects.Rows.Add();
                childObjects.Rows[childObjects.Rows.Count - 1].Cells[0].Value = scenePrimitive.name;
            }
            childObjects.Rows[childObjects.Rows.Count - 1].Selected = true;
            //Перенес
            currentObject = scene.GetObjectByName(comboObj.SelectedItem.ToString());
        }

        //Удаление объекта
        private void deleteSelectedObject()
        {
            try
            {
                if (childObjects.SelectedRows[0].Index == 0)
                {
                    scene.uniqueGroupCount--;
                    scene.groups.RemoveAt(comboObj.SelectedIndex);
                    if (scene.groups.Count > 0)
                    {
                        comboObj.Items.RemoveAt(comboObj.SelectedIndex);
                        comboObj.Text = "";
                        childObjects.Rows.Clear();
                        comboObj.SelectedIndex = comboObj.Items.Count - 1;
                    }
                    else
                    {
                        AllLock();
                        parentObject = null;
                        comboObj.Items.RemoveAt(comboObj.SelectedIndex);
                        comboObj.Text = "";
                        childObjects.Rows.Clear();
                    }
                    ////Скрываем все панели
                    //panelBoxProperty.Visible = false;
                    //panelTubeProperty.Visible = false;
                    //panelSphereProperty.Visible = false;
                    //panelCylinderProperty.Visible = false;
                }
                else
                {
                    scene.uniqueObjectsCount--;
                    currentObject.groupObjects.RemoveAt(childObjects.SelectedRows[0].Index - 1);
                    childObjects.Rows.RemoveAt(childObjects.SelectedRows[0].Index);

                    if (currentObject.groupObjects.Count == 0)
                    {
                        scene.groups.RemoveAt(comboObj.SelectedIndex);
                        comboObj.Items.RemoveAt(comboObj.SelectedIndex);
                        comboObj.Text = "";
                        childObjects.Rows.Clear();
                        if (comboObj.Items.Count != 0)
                        {
                            comboObj.SelectedIndex = comboObj.Items.Count - 1;
                        }
                        else
                        {
                            AllLock();
                        }
                    }
                    //else
                    //{
                    //    comboObj.SelectedIndex = comboObj.Items.Count - 1;
                    //}
                }
                InitObjPanel("");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Все элементы уже были удалены");
                MessageBox.Show(ex.Message);
            }
        }
        //Заводим переменную, для хранения выделенной строки таблицы
        private void Button4_Click_1(object sender, EventArgs e)
        {
            deleteSelectedObject();
        }
        bool objectInit = false;
        private void InitObjPanel(string type)
        {
            string[] types = type.Split('.');
            type = types[types.Length - 1];
            //Скрываем все панели
            panelBoxProperty.Visible = false;
            panelTubeProperty.Visible = false;
            wellPropertyPanel.Visible = false;
            panelSphereProperty.Visible = false;
            panelCylinderProperty.Visible = false;
            HalfSpherePanel.Visible = false;
            //Инициализвция полей со свойствами позиции и угла для объекта
            if (currentPrimitive != null)
            {
                initGroup = true;
                objectInit = true;
                objPosX.Value = (decimal)currentPrimitive.primitive.basePoint.getX();
                objPosY.Value = (decimal)currentPrimitive.primitive.basePoint.getY();
                objPosZ.Value = (decimal)currentPrimitive.primitive.basePoint.getZ();
                objRotX.Value = (decimal)(currentPrimitive.angleX);
                objRotY.Value = (decimal)(currentPrimitive.angleY);
                objRotZ.Value = (decimal)(currentPrimitive.angleZ);
                //Масштаб
                objScale.Value = (decimal)currentPrimitive.scaleCoeff;
                //Цвет
               
                //Показ нужной панели
                switch (type)
                {
                    case "Box":
                        panelBoxProperty.Visible = true;
                        boxPropertyL.Value = (decimal)((Box)currentPrimitive.primitive).length;
                        boxPropertyW.Value = (decimal)((Box)currentPrimitive.primitive).width;
                        boxPropertyH.Value = (decimal)((Box)currentPrimitive.primitive).height;
                        break;
                    case "Sphere":
                        panelSphereProperty.Visible = true;
                        spherePropertR.Value = (decimal)((Sphere)currentPrimitive.primitive).Radius;
                        sphereSegCount.Value = ((Sphere)currentPrimitive.primitive).SegmentCount;
                        break;
                    case "Cylinder":
                        panelCylinderProperty.Visible = true;
                        cylinderPropertyH.Value = (decimal)((Cylinder)currentPrimitive.primitive).height;
                        cylinderPropertyR.Value = (decimal)((Cylinder)currentPrimitive.primitive).Radius;
                        cylinderPropertyS.Value = ((Cylinder)currentPrimitive.primitive).SegmentCount;
                        break;
                    case "Tube":
                        panelTubeProperty.Visible = true;
                        tubePropertyH.Value = (decimal)((Tube)currentPrimitive.primitive).height;
                        tubePropertyRB.Value = (decimal)((Tube)currentPrimitive.primitive).BottomRadius;
                        tubePropertyRT.Value = (decimal)((Tube)currentPrimitive.primitive).TopRadius;
                        tubePropertyS.Value = ((Tube)currentPrimitive.primitive).SegmentCount;
                        tubePropertyT.Value = (decimal)((Tube)currentPrimitive.primitive).Thickness;
                        break;
                    case "HalfSphere":
                        HalfSpherePanel.Visible = true;
                        HalfSphereR.Value = (decimal)((HalfSphere)currentPrimitive.primitive).Radius;
                        HalfSphereS.Value = ((HalfSphere)currentPrimitive.primitive).SegmentCount;
                        break;
                }
                initGroup = false;
                objectInit = false;
                ColorOfObject.BackColor = currentPrimitive.primitive.color;
            }
        }
        bool CylinderPanelIsLoad = true;
        private void InitWellPanel()
        {
            CylinderPanelIsLoad = true;
            //Скрываем все панели
            panelBoxProperty.Visible = false;
            panelTubeProperty.Visible = false;
            panelSphereProperty.Visible = false;
            panelCylinderProperty.Visible = false;

            wellPropertyPanel.Visible = true;

            // SceneObject obj = GetPrimitiveByName("Первая колонна");
            //Опоры
            try
            {
                OporaR.Value = (decimal)((Cylinder)GetPrimitiveByName("Первая колонна").primitive).Radius;
                OporaH.Value = (decimal)((Cylinder)GetPrimitiveByName("Первая колонна").primitive).height;
            }
            catch { }
            //Веревка
            try
            {
                VerevkaR.Value = (decimal)((Cylinder)GetPrimitiveByName("Веревка").primitive).Radius;
                VerevkaH.Value = (decimal)((Cylinder)GetPrimitiveByName("Веревка").primitive).height;
            }
            catch { }
            //Ручка  
            try
            {
                RuchkaR.Value = (decimal)((Sphere)GetPrimitiveByName("Ручка сфера").primitive).Radius;
                RuchkaH.Value = (decimal)((Cylinder)GetPrimitiveByName("Ручка цилиндр").primitive).height;
            }
            catch { }
            //Тело колодца 
            try
            {
                KolodetsR.Value = (decimal)((Tube)GetPrimitiveByName("Труба колодца").primitive).TopRadius;
            }
            catch { }
            //Длина досок крыши 
            try
            {
                DoskiH.Value = (decimal)((Box)GetPrimitiveByName("Крышка1").primitive).length;
            }
            catch { }
            //Высота таблички
             try
            {
                TablichkaH.Value = (decimal)((Box)GetPrimitiveByName("Деревяшка").primitive).height;
            }
            catch { }
            //Заклепки 
            try
            {
                ZaklepkaR.Value = (decimal)((HalfSphere)GetPrimitiveByName("Гвоздь1").primitive).Radius;
            }
            catch { }
            //Количесвто заклепок
            int countNail = 0;
            try
            {
                for (int i = 0; i < currentObject.groupObjects.Count; i++)
                {
                    if (GetPrimitiveByName("Гвоздь" + i) != null)
                    {
                        countNail++;
                    }
                }
            }
            catch { }
            NailsCountBox.Value = countNail;
            CylinderPanelIsLoad = false;
        }

        private SceneObject GetPrimitiveByName(string name)
        {
            foreach (SceneObject obj in currentObject.groupObjects)
            {
                if (obj.name == name)
                {
                    return obj;
                }
            }
            return null;
        }
        private void InitCameraPanel()
        {
            if (scene.camera != null)
            {
                //Инициализвция полей со свойствами позиции и цели для камеры
                camPosX.Value = (decimal)scene.camera.position.getX();
                camPosY.Value = (decimal)scene.camera.position.getY();
                camPosZ.Value = (decimal)scene.camera.position.getZ();
                camTarX.Value = (decimal)scene.camera.targetPoint.getX();
                camTarY.Value = (decimal)scene.camera.targetPoint.getY();
                camTarZ.Value = (decimal)scene.camera.targetPoint.getZ();
                //Отсекающие плоскости
                camNear.Value = (decimal)scene.camera.nearClipZ;
                camFar.Value = (decimal)scene.camera.farClipZ;
                //Повороты камеры
                camRotX.Value = scene.camera.getAnglePhi();
                camRotY.Value = scene.camera.getAngleTheta();
                //Угол обзора
                camFov.Value = (decimal)scene.camera.getFOV();
            }
        }
        private void AllLock()
        {
            Image.Enabled = false;

            objPosX.Enabled = false;
            objPosY.Enabled = false;
            objPosZ.Enabled = false;
            objRotX.Enabled = false;
            objRotY.Enabled = false;
            objRotZ.Enabled = false;
            objScale.Enabled = false;

            camPosX.Enabled = false;
            camPosY.Enabled = false;
            camPosZ.Enabled = false;
            camTarX.Enabled = false;
            camTarY.Enabled = false;
            camTarZ.Enabled = false;
            camFov.Enabled = false;

            deleteObject.Enabled = false;

            //panelBoxProperty.Enabled = false;
        }
        private void AllUnlock()
        {
            Image.Enabled = true;

            objPosX.Enabled = true;
            objPosY.Enabled = true;
            objPosZ.Enabled = true;
            objRotX.Enabled = true;
            objRotY.Enabled = true;
            objRotZ.Enabled = true;
            objScale.Enabled = true;

            camPosX.Enabled = true;
            camPosY.Enabled = true;
            camPosZ.Enabled = true;
            camTarX.Enabled = true;
            camTarY.Enabled = true;
            camTarZ.Enabled = true;
            camFov.Enabled = true;

            deleteObject.Enabled = true;

            //panelBoxProperty.Enabled = true;
        }
        Color pickedColor;
        private void ColorPicker_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            colorPicker.BackColor = colorDialog1.Color;
            pickedColor = colorPicker.BackColor;
        }

        private void SphereCreate_Click(object sender, EventArgs e)
        {
            if (InCurGroup.Checked == false)
            {
                parentObject = CreateParent(nameObjBox.Text);
            }
            else
            {
                if (currentObject != null)
                {
                    parentObject = currentObject;
                }
                else
                {
                    parentObject = CreateParent(nameObjBox.Text);
                    MessageBox.Show("Не была выбрана группа! Объект будет создан отдельно");
                }
            }
            AddObject(nameObjBox.Text == "" ? "Сфера" + ++scene.uniqueObjectsCount : nameObjBox.Text, "Sphere");
            nameObjBox.Text = "";
            GenerateColorForm();
        }

        private void BoxPropertyL_ValueChanged(object sender, EventArgs e)
        {
            Box primitive = (Box)currentPrimitive.primitive;
            primitive.modifyLength((double)boxPropertyL.Value);
        }

        private void BoxPropertyW_ValueChanged(object sender, EventArgs e)
        {
            Box primitive = (Box)currentPrimitive.primitive;
            primitive.modifyWidth((double)boxPropertyW.Value);
        }

        private void BoxPropertyH_ValueChanged(object sender, EventArgs e)
        {
            Box primitive = (Box)currentPrimitive.primitive;
            primitive.modifyHeight((double)boxPropertyH.Value);
        }

        private void SpherePropertR_ValueChanged(object sender, EventArgs e)
        {
            Sphere primitive = (Sphere)currentPrimitive.primitive;
            primitive.ModifyRadius((double)spherePropertR.Value);
        }

        private void CylindrCreate_Click(object sender, EventArgs e)
        {
            if (InCurGroup.Checked == false)
            {
                parentObject = CreateParent(nameObjBox.Text);
            }
            else
            {
                if (currentObject != null)
                {
                    parentObject = currentObject;
                }
                else
                {
                    parentObject = CreateParent(nameObjBox.Text);
                    MessageBox.Show("Не была выбрана группа! Объект будет создан отдельно");
                }
            }
            AddObject(nameObjBox.Text == "" ? "Цилиндр " + ++scene.uniqueObjectsCount : nameObjBox.Text, "Cylinder");
            nameObjBox.Text = "";
            GenerateColorForm();
        }

        private void Panel15_Paint(object sender, PaintEventArgs e)
        {

        }

        private void SphereSegCount_ValueChanged(object sender, EventArgs e)
        {
            Sphere primitive = (Sphere)currentPrimitive.primitive;
            primitive.ModifySegmentsCount((int)sphereSegCount.Value);
        }

        private void CylinderPropertyH_ValueChanged(object sender, EventArgs e)
        {
            Cylinder primitive = (Cylinder)currentPrimitive.primitive;
            primitive.ModifyHeight((double)cylinderPropertyH.Value);
        }

        private void CylinderPropertyR_ValueChanged(object sender, EventArgs e)
        {
            Cylinder primitive = (Cylinder)currentPrimitive.primitive;
            primitive.ModifyRadius((double)cylinderPropertyR.Value);
        }

        private void CylinderPropertyS_ValueChanged(object sender, EventArgs e)
        {
            Cylinder primitive = (Cylinder)currentPrimitive.primitive;
            primitive.ModifySegmentsCount((int)cylinderPropertyS.Value);
        }

        private void TubeCreate_Click(object sender, EventArgs e)
        {
            if (InCurGroup.Checked == false)
            {
                parentObject = CreateParent(nameObjBox.Text);
            }
            else
            {
                if (currentObject != null)
                {
                    parentObject = currentObject;
                }
                else
                {
                    parentObject = CreateParent(nameObjBox.Text);
                    MessageBox.Show("Не была выбрана группа! Объект будет создан отдельно");
                }
            }
            AddObject(nameObjBox.Text == "" ? "Труба" + ++scene.uniqueObjectsCount : nameObjBox.Text, "Tube");
            nameObjBox.Text = "";
            GenerateColorForm();
        }

        private void TubePropertyRT_ValueChanged(object sender, EventArgs e)
        {
            Tube primitive = (Tube)currentPrimitive.primitive;
            primitive.ModifyTopRadius((double)tubePropertyRT.Value);
        }

        private void TubePropertyRB_ValueChanged(object sender, EventArgs e)
        {
            Tube primitive = (Tube)currentPrimitive.primitive;
            primitive.ModifyBottomRadius((double)tubePropertyRB.Value);
        }

        private void TubePropertyT_ValueChanged(object sender, EventArgs e)
        {
            Tube primitive = (Tube)currentPrimitive.primitive;
            primitive.ModifyThickness((double)tubePropertyT.Value);
        }

        private void TubePropertyS_ValueChanged(object sender, EventArgs e)
        {
            Tube primitive = (Tube)currentPrimitive.primitive;
            primitive.ModifySegmentsCount((int)tubePropertyS.Value);
        }

        private void TubePropertH_ValueChanged(object sender, EventArgs e)
        {
            Tube primitive = (Tube)currentPrimitive.primitive;
            primitive.ModifyHeight((double)tubePropertyH.Value);
        }

        private void CreateCopy_Click(object sender, EventArgs e)
        {
            if (currentObject != null)
            {
                Group group = new Group(currentObject.name + "copy");
                group = currentObject;
                parentObject = AddGroup(group);
                GenerateColorForm();

            }
            //SceneObject newObj = null;
            //string name = Obj.name + ++scene.uniqueObjectsCount;
            //switch (Obj.typeObj)
            //{
            //    case "Box":
            //        newObj = new SceneObject((Box)Obj.primitive);
            //        break;
            //    case "Sphere":
            //        newObj = new SceneObject((Sphere)Obj.primitive);
            //        break;
            //    case "Cylinder":
            //        newObj = new SceneObject((Cylinder)Obj.primitive);
            //        break;
            //    case "Tube":
            //        newObj = new SceneObject((Tube)Obj.primitive);
            //        break;
            //}
            //newObj.angleX = Obj.angleX;
            //newObj.angleY = Obj.angleY;
            //newObj.angleZ = Obj.angleZ;
            //newObj.scaleCoeff = Obj.scaleCoeff;
            //newObj.basePoint = new Point3D(Obj.basePoint);
            //newObj.name = name;
            ////Obj = scene.addObject(newObj, name, true);
            //scene.groups[comboObj.SelectedIndex].groupObjects.Add(newObj);
            //Group = scene.groups[comboObj.SelectedIndex];
            //childObjects.Rows.Add();
            //childObjects.Rows[childObjects.Rows.Count - 1].Cells[0].Style.BackColor = Color.DarkGray;
            //childObjects.Rows[childObjects.Rows.Count - 1].Cells[0].Value = newObj.name;

            //if (Obj != null)
            //{
            //    comboObj.Items.Add(Obj.name);
            //    comboObj.SelectedIndex = comboObj.Items.Count - 1;

            //}

        }

        private void Panel11_Paint(object sender, PaintEventArgs e)
        {

        }
        private void Button1_Click_2(object sender, EventArgs e)
        {
            GenerateColorForm();
            parentObject = CreateParent(nameObjBox.Text);
            AddObject(nameObjBox.Text == "" ? "object" + ++scene.uniqueObjectsCount : nameObjBox.Text, "Box");
            AddObject(nameObjBox.Text == "" ? "object" + ++scene.uniqueObjectsCount : nameObjBox.Text, "Box");
            AddObject(nameObjBox.Text == "" ? "object" + ++scene.uniqueObjectsCount : nameObjBox.Text, "Tube");
            AddObject(nameObjBox.Text == "" ? "object" + ++scene.uniqueObjectsCount : nameObjBox.Text, "Cylinder");
            nameObjBox.Text = "";
        }

        private void CreateCamera_Click(object sender, EventArgs e)
        {
            scene.camera = new Camera();
            scene.ResetCamera();
            scene.AddCamera(scene.camera, cameraNameBox.Text);
            comboCamera.Items.Add(scene.camera.name);
            comboCamera.SelectedIndex = comboCamera.Items.Count - 1;
            cameraNameBox.Text = "";
        }
        public bool CameraValueChanged = true;
        private void ComboCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            scene.camera = scene.cameras[comboCamera.SelectedIndex];
            CameraValueChanged = false;
            InitCameraPanel();
            CameraValueChanged = true;
            indexOfUsedCamera = comboCamera.SelectedIndex;
        }

        private void Button3_Click_1(object sender, EventArgs e)
        {
            int index = comboCamera.SelectedIndex;
            if (scene.cameras.Count > 1)
            {
                scene.cameras.RemoveAt(index);
                comboCamera.Items.RemoveAt(index);
                comboCamera.SelectedIndex = scene.cameras.Count - 1;
            }
        }

        private void CamRotX_ValueChanged_1(object sender, EventArgs e)
        {
            ////scene.camera.setAnglePhi((int)camRotX.Value);
            //if (!pressed)
            //{
            //    int dx = (int)camRotX.Value - scene.camera.getAnglePhi();
            //    scene.camera.rotateCtrlLeftRight(dx);
            //}
        }


        private void CamRotY_ValueChanged_1(object sender, EventArgs e)
        {
            //scene.camera.setAngleTheta((int)camRotY.Value);
            //if (!pressed)
            //{
            //    int dy = (int)camRotY.Value - scene.camera.getAngleTheta();
            //    scene.camera.rotateCtrlUpDown(dy * Math.PI / 180);
            //}
        }
        bool initGroup = false;
        private void ChildObjects_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //По клику получаем индекс из комбоОбжекта для получения нужной группы
            //В полученной группе обращаемся к объекту по индексу выбранной строки
            //В Обж загуржаем этот объект
            if (e.RowIndex > 0)
            {
                AllLock();
                currentPrimitive = scene.groups[comboObj.SelectedIndex].groupObjects[e.RowIndex - 1];
                AllUnlock();
                InitObjPanel(currentPrimitive.primitive.GetType().ToString());
            }
            else
            {
                try
                {
                    currentPrimitive = null;
                    //Скрываем все панели
                    panelBoxProperty.Visible = false;
                    panelTubeProperty.Visible = false;
                    panelSphereProperty.Visible = false;
                    panelCylinderProperty.Visible = false;
                    HalfSpherePanel.Visible = false;
                    string[] type = currentObject.GetType().ToString().Split('.');
                    if (GetPrimitiveByName("Труба колодца") != null)
                    {
                        InitWellPanel();
                    }
                    objectInit = true;
                    initGroup = true;
                    //Заполняем данными группы
                    objPosX.Value = (decimal)currentObject.BasePoint.x;
                    objPosY.Value = (decimal)currentObject.BasePoint.y;
                    objPosZ.Value = (decimal)currentObject.BasePoint.z;
                    objRotX.Value = (decimal)(currentObject.angleX);
                    objRotY.Value = (decimal)(currentObject.angleY);
                    objRotZ.Value = (decimal)(currentObject.angleZ);
                    //Масштаб
                    objScale.Value = (decimal)currentObject.Scale;
                    objectInit = false;
                    initGroup = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка в обработке клика по ячейке примитивов группы!");
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ChildObjects_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void ChildObjects_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
        }
        private Group CreateParent(string name)
        {
            Group group = null;
            name = name == "" ? "group " + ++scene.uniqueGroupCount : name;
            //Если объекта с таким именем нет
            if (scene.GetObjectByName(name) == null)
            {
                group = new Group(name);
                //Заполняем поля пустой группы
                group.BasePoint = new Point3D(0, 0, 0);
                group.angleX = 0;
                group.angleY = 0;
                group.angleZ = 0;
                group.SetScale(1);
                scene.AddObject(group);
                comboObj.Items.Add(group.name);
                comboObj.SelectedIndex = comboObj.Items.Count - 1;
            }
            return group;
        }
        private Group AddGroup(Group group)
        {
            group.BasePoint = new Point3D(0, 0, 0);
            group.angleX = 0;
            group.angleY = 0;
            group.angleZ = 0;
            group.SetScale(1);
            scene.AddObject(group);
            comboObj.Items.Add(group.name);
            comboObj.SelectedIndex = comboObj.Items.Count - 1;
            return group;
        }
        private void AddObject(string name, string typeObject)
        {
            Point3D basePoint = new Point3D(0, 0, 0);
            if (parentObject != null)
            {
                // name = parentObject.GetScenePrimitiveByName(name) == null ? "object " + ++scene.uniqueObjectsCount : name;
                Primitive primitive = null;
                switch (typeObject)
                {
                    case "Box":
                        primitive = new Box(basePoint, 100, 100, 100, pickedColor);
                        break;
                    case "Cylinder":
                        primitive = new Cylinder(basePoint, 100, pickedColor, 30, 18);
                        break;
                    case "Sphere":
                        primitive = new Sphere(basePoint, pickedColor, 50, 50, 32);
                        break;
                    case "Tube":
                        primitive = new Tube(basePoint, 40, 40, 100, 10, 18, pickedColor);
                        break;
                    case "HalfSphere":
                        primitive = new HalfSphere(basePoint, 40, 40, 18, pickedColor);
                        break;
                }
                SceneObject sceneObject = new SceneObject(primitive, nameObjBox.Text);
                sceneObject.angleX = 0;
                sceneObject.angleY = 0;
                sceneObject.angleZ = 0;
                sceneObject.scale(1);
                sceneObject.name = name;
                parentObject.AddObject(sceneObject);
                //!!
                parentObject.BasePoint = basePoint;

                childObjects.Rows.Add();
                childObjects.Rows[childObjects.Rows.Count - 1].Cells[0].Value = sceneObject.name;
                childObjects.Rows[childObjects.Rows.Count - 1].Selected = true;
            }
        }

        private void ChildObjects_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private int indexOfUsedCamera;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteSelectedObject();
            }
            if (e.KeyCode == Keys.P)
            {
                scene.camera.setTypeOfProjection(false);
                comboProjection.SelectedIndex = 0;
            }
            if (e.KeyCode == Keys.O)
            {
                scene.camera.setTypeOfProjection(true);
                comboProjection.SelectedIndex = 1;
            }
            //Вид сверху
            if (e.KeyCode == Keys.T)
            {
                CameraValueChanged = false;
                scene.camera = new Camera(new Point3D(1, 350, 0), new Point3D(0, 0, 0), scene.camera.nearClipZ, scene.camera.farClipZ, scene.camera.screenWidth, scene.camera.screenHeight, "Top view");
                scene.camera.angleTheta = 1;
                scene.camera.anglePhi = 0;
                scene.camera.rotateLeftRight(270);
                scene.camera.rotateUpDown(-179);
                comboProjection.SelectedIndex = 1;
                scene.camera.setTypeOfProjection(true);
                InitCameraPanel();
                CameraValueChanged = true;
            }
            //Вид Слева
            if (e.KeyCode == Keys.R)
            {
                CameraValueChanged = false;
                scene.camera = new Camera(new Point3D(-350, 0, 0), new Point3D(0, 0, 0), scene.camera.nearClipZ, scene.camera.farClipZ, scene.camera.screenWidth, scene.camera.screenHeight, "Top view");
                scene.camera.rotateLeftRight(-180);
                scene.camera.rotateUpDown(0);
                comboProjection.SelectedIndex = 1;
                scene.camera.setTypeOfProjection(true);
                InitCameraPanel();
                CameraValueChanged = true;
            }
            //Вид справа
            if (e.KeyCode == Keys.L)
            {
                CameraValueChanged = false;
                scene.camera = new Camera(new Point3D(350, 0, 0), new Point3D(0, 0, 0), scene.camera.nearClipZ, scene.camera.farClipZ, scene.camera.screenWidth, scene.camera.screenHeight, "Top view");
                scene.camera.rotateLeftRight(180);
                scene.camera.rotateUpDown(0);
                comboProjection.SelectedIndex = 1;
                scene.camera.setTypeOfProjection(true);
                InitCameraPanel();
                CameraValueChanged = true;
            }
            //Ресет на старый вид
            if (e.KeyCode == Keys.C)
            {
                scene.camera = scene.cameras[indexOfUsedCamera];
                comboProjection.SelectedIndex = scene.camera.getTypeOfProjection() == true ? 1 : 0;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Bucket bucket = new Bucket(nameObjBox.Text == "" ? "Ведро " + ++scene.uniqueGroupCount : nameObjBox.Text);
            parentObject = AddGroup(bucket);
            nameObjBox.Text = "";
            GenerateColorForm();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Well well = new Well(nameObjBox.Text == "" ? "Колодец " + ++scene.uniqueGroupCount : nameObjBox.Text);
            parentObject = AddGroup(well);
            nameObjBox.Text = "";
            GenerateColorForm();
            InitWellPanel();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            Plate plate = new Plate(nameObjBox.Text == "" ? "Табличка " + ++scene.uniqueGroupCount : nameObjBox.Text);
            parentObject = AddGroup(plate);
            nameObjBox.Text = "";
            GenerateColorForm();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            Secret secret = new Secret(nameObjBox.Text == "" ? "100 " + ++scene.uniqueGroupCount : nameObjBox.Text);
            parentObject = AddGroup(secret);
            nameObjBox.Text = "";
            GenerateColorForm();
        }

        private void OporaR_ValueChanged(object sender, EventArgs e)
        {
            if (!CylinderPanelIsLoad)
            {
                //При уменьешнии радиуса нужно править только балку между опорами 
                double dx = (double)OporaR.Value - ((Cylinder)(currentObject.groupObjects[1].primitive)).Radius;

                ((Cylinder)(GetPrimitiveByName("Первая колонна").primitive)).ModifyRadius((double)OporaR.Value);
                ((Cylinder)(GetPrimitiveByName("Вторая колонна").primitive)).ModifyRadius((double)OporaR.Value);
                //Подтягиваем или уменьшаем балку балку
                ((Cylinder)(GetPrimitiveByName("Балка").primitive)).ModifyHeight(((Cylinder)(currentObject.groupObjects[5].primitive)).height - dx * 2);
            }
        }

        private void OporaH_ValueChanged(object sender, EventArgs e)
        {
            if (!CylinderPanelIsLoad)
            {
                //Будем сдвигать трубу колодца и балки вниз, а базовую точку группы вверх
                Cylinder opora1 = (Cylinder)(GetPrimitiveByName("Первая колонна").primitive);
                Cylinder opora2 = (Cylinder)(GetPrimitiveByName("Вторая колонна").primitive);
                //При увеличении высоты нужно поднимать крышу и гвоздики
                double dx = (double)OporaH.Value - opora1.height;
                //Меняем высоту опор
                opora1.ModifyHeight((double)OporaH.Value);
                opora2.ModifyHeight((double)OporaH.Value);
                //Меняем базовую точку опор
                opora1.ModifyBasePoint(new Point3D(opora1.basePoint.x, opora1.basePoint.y - dx / 2, opora1.basePoint.z));
                opora2.ModifyBasePoint(new Point3D(opora2.basePoint.x, opora2.basePoint.y - dx / 2, opora2.basePoint.z));
                //Получаем трубу колодца
                Tube tube = (Tube)(GetPrimitiveByName("Труба колодца").primitive);
                //Опискаем трубу
                tube.ModifyBasePoint(new Point3D(tube.basePoint.x, tube.basePoint.y - dx, tube.basePoint.z));
                //Поднимаем группу
                currentObject.BasePoint.y += dx;
            }
        }

        private void VerevkaR_ValueChanged(object sender, EventArgs e)
        {
            if (!CylinderPanelIsLoad)
            {
                //От веревки пока что ничего не зависит (ведро)
                Cylinder rope = (Cylinder)GetPrimitiveByName("Веревка").primitive;
                rope.ModifyRadius((double)VerevkaR.Value);
            }
        }

        private void VerevkaH_ValueChanged(object sender, EventArgs e)
        {
            if (!CylinderPanelIsLoad)
            {
                //От веревки пока что ничего не зависит (ведро)
                Cylinder rope = (Cylinder)GetPrimitiveByName("Веревка").primitive;
                Tube BucketBody = (Tube)GetPrimitiveByName("Корпус ведра").primitive;
                Cylinder BucketBottom = (Cylinder)GetPrimitiveByName("Днище").primitive;
                Box LeftHandle = (Box)GetPrimitiveByName("Левая часть ручки").primitive;
                Box RightHandle = (Box)GetPrimitiveByName("Правая часть ручки").primitive;

                double dx = (double)VerevkaH.Value - rope.height;

                rope.ModifyHeight((double)VerevkaH.Value);
                rope.ModifyBasePoint(new Point3D(rope.basePoint.x, rope.basePoint.y - dx / 2, rope.basePoint.z));

                BucketBody.ModifyBasePoint(new Point3D(BucketBody.basePoint.x, BucketBody.basePoint.y - dx, BucketBody.basePoint.z));
                BucketBottom.ModifyBasePoint(new Point3D(BucketBottom.basePoint.x, BucketBottom.basePoint.y - dx, BucketBottom.basePoint.z));
                LeftHandle.modifyBasePoint(new Point3D(LeftHandle.basePoint.x, LeftHandle.basePoint.y - dx, LeftHandle.basePoint.z));
                RightHandle.modifyBasePoint(new Point3D(RightHandle.basePoint.x, RightHandle.basePoint.y - dx, RightHandle.basePoint.z));
            }
        }

        private void RuchkaR_ValueChanged(object sender, EventArgs e)
        {
            if (!CylinderPanelIsLoad)
            {
                //От шарика ничего не зависит
                Sphere ball = (Sphere)GetPrimitiveByName("Ручка сфера").primitive;
                double dx = (double)RuchkaR.Value - ball.Radius;
                ball.ModifyRadius((double)RuchkaR.Value);
                ball.ModifyBasePoint(new Point3D(ball.basePoint.x + dx, ball.basePoint.y, ball.basePoint.z));
            }
        }

        private void RuchkaH_ValueChanged(object sender, EventArgs e)
        {
            if (!CylinderPanelIsLoad)
            {
                //От цилиндра зависит шарик
                Cylinder handle = (Cylinder)GetPrimitiveByName("Ручка цилиндр").primitive;
                double dx = (double)RuchkaH.Value - handle.height;
                handle.ModifyHeight((double)RuchkaH.Value);
                handle.ModifyBasePoint(new Point3D(handle.basePoint.x + dx / 2, handle.basePoint.y, handle.basePoint.z));
                //Сдвигаем шарик
                Sphere ball = (Sphere)GetPrimitiveByName("Ручка сфера").primitive;
                ball.ModifyBasePoint(new Point3D(ball.basePoint.x + dx, ball.basePoint.y, ball.basePoint.z));
            }
        }

        private void KolodetsR_ValueChanged(object sender, EventArgs e)
        {
            if (!CylinderPanelIsLoad)
            {
                //Ничего не зависит
                //Получаем трубу колодца
                Tube tube = (Tube)(currentObject.groupObjects[0].primitive);
                tube.ModifyBottomRadius((double)KolodetsR.Value);
                tube.ModifyTopRadius((double)KolodetsR.Value);
            }
        }

        private void DoskiH_ValueChanged(object sender, EventArgs e)
        {
            if (!CylinderPanelIsLoad)
            {
                Box cap1 = (Box)GetPrimitiveByName("Крышка1").primitive;
                Box cap2 = (Box)GetPrimitiveByName("Крышка2").primitive;
                double dx = (double)DoskiH.Value - cap1.length;
                cap1.modifyLength((double)DoskiH.Value);
                cap2.modifyLength((double)DoskiH.Value);
                //Перемещаем центр
                cap1.modifyBasePoint(new Point3D(cap1.basePoint.x, cap1.basePoint.y - dx / 3, cap1.basePoint.z - dx / 2));
                cap2.modifyBasePoint(new Point3D(cap2.basePoint.x, cap2.basePoint.y - dx / 3, cap2.basePoint.z + dx / 2));
            }
        }

        private void TablichkaH_ValueChanged(object sender, EventArgs e)
        {
            if (!CylinderPanelIsLoad)
            {
                ////Ничего не зависит
                Box table = (Box)GetPrimitiveByName("Деревяшка").primitive;
                double dx = (double)TablichkaH.Value - table.height;
                table.modifyHeight((double)TablichkaH.Value);
                table.modifyBasePoint(new Point3D(table.basePoint.x, table.basePoint.y - dx / 2, table.basePoint.z));
            }
        }

        private void ZaklepkaR_ValueChanged(object sender, EventArgs e)
        {
            if (!CylinderPanelIsLoad)
            {
                //Меняем радиус гвооздей 
                for (int i = 0; i < currentObject.groupObjects.Count; i++)
                {
                    SceneObject temp = GetPrimitiveByName("Гвоздь" + i);
                    if (temp != null)
                    {
                        ((HalfSphere)temp.primitive).ModifyRadius((double)ZaklepkaR.Value);
                    }
                }
            }
        }
        public void DeleteNails()
        {
            //Количесвто заклепок

            int countNail = 0;

            for (int i = 0; i < currentObject.groupObjects.Count; i++)
            {
                if (GetPrimitiveByName("Гвоздь" + i) != null)
                {
                    countNail++;
                }
            }
            //Удаляем гвозди
            int indexStart = 0;

            for (int i = 0; i < currentObject.groupObjects.Count; i++)
            {
                if (currentObject.groupObjects[i].name == "Гвоздь1")
                {
                    indexStart = i;
                    break;
                }
            }
            int endIndex = countNail + indexStart;
            //Удаляем все гвозди
            for (int i = endIndex - 1; i >= indexStart; i--)
            {
                currentObject.groupObjects.RemoveAt(i);
            }
        }

        private void HalfSphereCreate_Click(object sender, EventArgs e)
        {
            if (InCurGroup.Checked == false)
            {
                parentObject = CreateParent(nameObjBox.Text);
            }
            else
            {
                if (currentObject != null)
                {
                    parentObject = currentObject;
                }
                else
                {
                    parentObject = CreateParent(nameObjBox.Text);
                    MessageBox.Show("Не была выбрана группа! Объект будет создан отдельно");
                }
            }
            AddObject(nameObjBox.Text == "" ? "Полусфера " + ++scene.uniqueObjectsCount : nameObjBox.Text, "HalfSphere");
            nameObjBox.Text = "";
            GenerateColorForm();
        }

        private void NailsCountBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void NailsCountBox_ValueChanged(object sender, EventArgs e)
        {
            if (!CylinderPanelIsLoad)
            {
                //Мои гвоздички, как же я их "люблю"
                SceneObject Nail1 = new SceneObject(new HalfSphere(new Point3D(80, 197, -12), 5, 5, 14, Color.Red), "Гвоздь1");
                Nail1.angleX = 30;
                SceneObject Nail2 = new SceneObject(new HalfSphere(new Point3D(80, 180, -42), 5, 5, 14, Color.Red), "Гвоздь2");
                Nail2.angleX = 30;
                SceneObject Nail3 = new SceneObject(new HalfSphere(new Point3D(80, 163, -72), 5, 5, 14, Color.Red), "Гвоздь3");
                Nail3.angleX = 30;
                SceneObject Nail4 = new SceneObject(new HalfSphere(new Point3D(-80, 197, -12), 5, 5, 14, Color.Red), "Гвоздь4");
                Nail4.angleX = 30;
                SceneObject Nail5 = new SceneObject(new HalfSphere(new Point3D(-80, 180, -42), 5, 5, 14, Color.Red), "Гвоздь5");
                Nail5.angleX = 30;
                SceneObject Nail6 = new SceneObject(new HalfSphere(new Point3D(-80, 163, -72), 5, 5, 14, Color.Red), "Гвоздь6");
                Nail6.angleX = 30;
                SceneObject Nail7 = new SceneObject(new HalfSphere(new Point3D(80, 197, 12), 5, 5, 14, Color.Red), "Гвоздь7");
                Nail7.angleX = -30;
                SceneObject Nail8 = new SceneObject(new HalfSphere(new Point3D(80, 180, 42), 5, 5, 14, Color.Red), "Гвоздь8");
                Nail8.angleX = -30;
                SceneObject Nail9 = new SceneObject(new HalfSphere(new Point3D(80, 163, 72), 5, 5, 14, Color.Red), "Гвоздь9");
                Nail9.angleX = -30;
                SceneObject Nail10 = new SceneObject(new HalfSphere(new Point3D(-80, 197, 12), 5, 5, 14, Color.Red), "Гвоздь10");
                Nail10.angleX = -30;
                SceneObject Nail11 = new SceneObject(new HalfSphere(new Point3D(-80, 180, 42), 5, 5, 14, Color.Red), "Гвоздь11");
                Nail11.angleX = -30;
                SceneObject Nail12 = new SceneObject(new HalfSphere(new Point3D(-80, 163, 72), 5, 5, 14, Color.Red), "Гвоздь12");
                Nail12.angleX = -30;
                switch ((int)NailsCountBox.Value)
                {
                    case 4:
                        DeleteNails();
                        //Добавляем нужные гвозди
                        currentObject.groupObjects.Add(Nail1);
                        currentObject.groupObjects.Add(Nail4);
                        currentObject.groupObjects.Add(Nail7);
                        currentObject.groupObjects.Add(Nail10);
                        break;
                    case 8:
                        DeleteNails();
                        //Добавляем нужные гвозди
                        currentObject.groupObjects.Add(Nail1);
                        currentObject.groupObjects.Add(Nail4);
                        currentObject.groupObjects.Add(Nail7);
                        currentObject.groupObjects.Add(Nail10);
                        currentObject.groupObjects.Add(Nail2);
                        currentObject.groupObjects.Add(Nail5);
                        currentObject.groupObjects.Add(Nail8);
                        currentObject.groupObjects.Add(Nail11);
                        break;
                    case 12:
                        DeleteNails();
                        //Добавляем нужные гвозди
                        currentObject.groupObjects.Add(Nail1);
                        currentObject.groupObjects.Add(Nail4);
                        currentObject.groupObjects.Add(Nail7);
                        currentObject.groupObjects.Add(Nail10);
                        currentObject.groupObjects.Add(Nail2);
                        currentObject.groupObjects.Add(Nail5);
                        currentObject.groupObjects.Add(Nail8);
                        currentObject.groupObjects.Add(Nail11);
                        currentObject.groupObjects.Add(Nail3);
                        currentObject.groupObjects.Add(Nail6);
                        currentObject.groupObjects.Add(Nail9);
                        currentObject.groupObjects.Add(Nail12);
                        break;
                    default:
                        MessageBox.Show("Число должно быть кратно 4!");
                        break;
                }
                //Обновляем табличку
                childObjects.Rows.Clear();
                childObjects.Rows.Add();
                childObjects.Rows[0].Cells[0].Value = "Модификация группы";
                foreach (SceneObject scenePrimitive in currentObject.groupObjects)
                {
                    childObjects.Rows.Add();
                    childObjects.Rows[childObjects.Rows.Count - 1].Cells[0].Value = scenePrimitive.name;
                }
                childObjects.Rows[childObjects.Rows.Count - 1].Selected = true;
            }
        }

        private void HalfSphereR_ValueChanged(object sender, EventArgs e)
        {
            HalfSphere primitive = (HalfSphere)currentPrimitive.primitive;
            primitive.ModifyRadius((double)HalfSphereR.Value);
        }

        private void HalfSphereS_ValueChanged(object sender, EventArgs e)
        {
            HalfSphere primitive = (HalfSphere)currentPrimitive.primitive;
            primitive.ModifySegmentsCount((int)HalfSphereS.Value);
        }

        private void Button5_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    IFormatProvider format = new System.Globalization.CultureInfo("ru-RU");
                    XDocument document = new XDocument();
                    XElement sceneXML = new XElement("scene");
                    XElement camerasXML = new XElement("cameras");
                    for (int i = 0; i < scene.cameras.Count; ++i)
                    {
                        Camera camera = scene.cameras[i];
                        XElement cameraElement = new XElement("camera");

                        cameraElement.SetAttributeValue("name", comboCamera.Items[i].ToString());

                        cameraElement.SetElementValue("central-projection", camera.getTypeOfProjection());

                        XElement position = new XElement("position");
                        position.SetAttributeValue("x", string.Format(format, "{0:0.00}", camera.position.x));
                        position.SetAttributeValue("y", string.Format(format, "{0:0.00}", camera.position.y));
                        position.SetAttributeValue("z", string.Format(format, "{0:0.00}", camera.position.z));
                        cameraElement.Add(position);

                        XElement target = new XElement("target");
                        target.SetAttributeValue("x", string.Format(format, "{0:0.00}", camera.targetPoint.x));
                        target.SetAttributeValue("y", string.Format(format, "{0:0.00}", camera.targetPoint.y));
                        target.SetAttributeValue("z", string.Format(format, "{0:0.00}", camera.targetPoint.z));
                        cameraElement.Add(target);

                        XElement clippingPlanes = new XElement("clipping-planes");
                        clippingPlanes.SetAttributeValue("near", camera.nearClipZ);
                        clippingPlanes.SetAttributeValue("far", camera.farClipZ);
                        cameraElement.Add(clippingPlanes);

                        cameraElement.SetElementValue("fov", camera.getFOV());

                        camerasXML.Add(cameraElement);
                    }
                    sceneXML.Add(camerasXML);
                    XElement objects = new XElement("objects");
                    foreach (Group sceneObject in scene.groups)
                    {
                        XElement objectElement;

                        //if (sceneObject is Well)
                        //{
                        //    Well well = (Well)sceneObject;
                        //    objectElement = new XElement("well-object");

                        //    objectElement.SetElementValue("opora-radius", ((Cylinder)well.GetScenePrimitiveByName("Первая колонна").primitive).Radius);
                        //    objectElement.SetElementValue("opora-height", ((Cylinder)well.GetScenePrimitiveByName("Первая колонна").primitive).height);
                        //    objectElement.SetElementValue("rope-radius", ((Cylinder)well.GetScenePrimitiveByName("Веревка").primitive).Radius);
                        //    objectElement.SetElementValue("rope-height", ((Cylinder)well.GetScenePrimitiveByName("Веревка").primitive).height);
                        //    objectElement.SetElementValue("handle-radius", ((Sphere)well.GetScenePrimitiveByName("Ручка сфера").primitive).Radius);
                        //    objectElement.SetElementValue("handle-height", ((Cylinder)well.GetScenePrimitiveByName("Ручка цилиндр").primitive).height);
                        //    objectElement.SetElementValue("well-tube", ((Tube)well.GetScenePrimitiveByName("Труба колодца").primitive).TopRadius);
                        //    objectElement.SetElementValue("lngth-desk-roof", ((Box)well.GetScenePrimitiveByName("Крышка1").primitive).length);
                        //    objectElement.SetElementValue("plate-height", ((Box)well.GetScenePrimitiveByName("Деревяшка").primitive).height);
                        //    objectElement.SetElementValue("nails-radius", ((HalfSphere)well.GetScenePrimitiveByName("Гвоздь1").primitive).Radius);
                        //    objectElement.SetElementValue("count-nails", (int)NailsCountBox.Value);
                        //}
                        //else 
                        if (sceneObject is Bucket)
                        {
                            Bucket bucket = (Bucket)sceneObject;
                            objectElement = new XElement("bucket-object");
                        }
                        else if (sceneObject is Plate)
                        {
                            Plate bucket = (Plate)sceneObject;
                            objectElement = new XElement("plate-object");
                        }
                        else
                        {
                            objectElement = new XElement("scene-object");
                            XElement scenePrimitivesElement = new XElement("scene-primitives");
                            foreach (SceneObject scenePrimitive in sceneObject.groupObjects)
                            {
                                XElement scenePrimitiveElement = new XElement("scene-primitive");

                                XElement primitiveRotate = new XElement("rotate");
                                primitiveRotate.SetAttributeValue("x", scenePrimitive.angleX);
                                primitiveRotate.SetAttributeValue("y", scenePrimitive.angleY);
                                primitiveRotate.SetAttributeValue("z", scenePrimitive.angleZ);
                                scenePrimitiveElement.Add(primitiveRotate);

                                XElement primitiveScale = new XElement("scale");
                                primitiveScale.SetAttributeValue("scale", string.Format(format, "{0:0.00}", scenePrimitive.scaleCoeff));
                                scenePrimitiveElement.Add(primitiveScale);

                                XElement primitiveElement = new XElement("primitive");

                                XElement primitiveBasePoint = new XElement("base-point");
                                primitiveBasePoint.SetAttributeValue("x", string.Format(format, "{0:0.00}", scenePrimitive.primitive.basePoint.x));
                                primitiveBasePoint.SetAttributeValue("y", string.Format(format, "{0:0.00}", scenePrimitive.primitive.basePoint.y));
                                primitiveBasePoint.SetAttributeValue("z", string.Format(format, "{0:0.00}", scenePrimitive.primitive.basePoint.z));
                                primitiveElement.Add(primitiveBasePoint);

                                XElement primitiveColor = new XElement("color");
                                primitiveColor.SetAttributeValue("r", scenePrimitive.primitive.color.R);
                                primitiveColor.SetAttributeValue("g", scenePrimitive.primitive.color.G);
                                primitiveColor.SetAttributeValue("b", scenePrimitive.primitive.color.B);
                                primitiveElement.Add(primitiveColor);

                                if (scenePrimitive.primitive is Box)
                                {
                                    Box box = (Box)scenePrimitive.primitive;
                                    primitiveElement.SetAttributeValue("type", "Box");
                                    primitiveElement.SetElementValue("width", string.Format(format, "{0:0.00}", box.width));
                                    primitiveElement.SetElementValue("height", string.Format(format, "{0:0.00}", box.height));
                                    primitiveElement.SetElementValue("length", string.Format(format, "{0:0.00}", box.length));
                                }
                                else if (scenePrimitive.primitive is Cylinder)
                                {
                                    Cylinder cylinder = (Cylinder)scenePrimitive.primitive;
                                    primitiveElement.SetAttributeValue("type", "Cylinder");
                                    primitiveElement.SetElementValue("radius", cylinder.Radius);
                                    primitiveElement.SetElementValue("height", cylinder.height);
                                    primitiveElement.SetElementValue("segments-count", cylinder.SegmentCount);
                                }
                                else if (scenePrimitive.primitive is Sphere)
                                {
                                    Sphere sphere = (Sphere)scenePrimitive.primitive;
                                    primitiveElement.SetAttributeValue("type", "Sphere");
                                    primitiveElement.SetElementValue("radius", sphere.Radius);
                                    primitiveElement.SetElementValue("segments-count", sphere.SegmentCount);
                                }
                                else if (scenePrimitive.primitive is Tube)
                                {
                                    Tube tube = (Tube)scenePrimitive.primitive;
                                    primitiveElement.SetAttributeValue("type", "Tube");
                                    primitiveElement.SetElementValue("radius-top", tube.TopRadius);
                                    primitiveElement.SetElementValue("radius-bottom", tube.BottomRadius);
                                    primitiveElement.SetElementValue("height", tube.height);
                                    primitiveElement.SetElementValue("thickness", tube.Thickness);
                                    primitiveElement.SetElementValue("segments-count", tube.SegmentCount);
                                }
                                else if (scenePrimitive.primitive is HalfSphere)
                                {
                                    HalfSphere halfSphere = (HalfSphere)scenePrimitive.primitive;
                                    primitiveElement.SetAttributeValue("type", "HalfSphere");
                                    primitiveElement.SetElementValue("radius", halfSphere.Radius);
                                    primitiveElement.SetElementValue("segments-count", halfSphere.SegmentCount);
                                }

                                scenePrimitiveElement.Add(primitiveElement);
                                scenePrimitiveElement.SetAttributeValue("name", scenePrimitive.name);
                                scenePrimitivesElement.Add(scenePrimitiveElement);
                            }

                            objectElement.Add(scenePrimitivesElement);
                        }

                        objectElement.SetAttributeValue("name", sceneObject.name);

                        XElement objectPosition = new XElement("position");
                        objectPosition.SetAttributeValue("x", string.Format(format, "{0:0.00}", sceneObject.BasePoint.x));
                        objectPosition.SetAttributeValue("y", string.Format(format, "{0:0.00}", sceneObject.BasePoint.y));
                        objectPosition.SetAttributeValue("z", string.Format(format, "{0:0.00}", sceneObject.BasePoint.z));
                        objectElement.Add(objectPosition);

                        XElement objectRotate = new XElement("rotate");
                        objectRotate.SetAttributeValue("x", sceneObject.angleX);
                        objectRotate.SetAttributeValue("y", sceneObject.angleY);
                        objectRotate.SetAttributeValue("z", sceneObject.angleZ);
                        objectElement.Add(objectRotate);

                        XElement objectScale = new XElement("scale");
                        objectScale.SetAttributeValue("scale", string.Format(format, "{0:0.00}", sceneObject.Scale));
                        objectElement.Add(objectScale);

                        objects.Add(objectElement);
                    }
                    sceneXML.Add(objects);

                    document.Add(sceneXML);
                    document.Save(saveFileDialog1.FileName);
                }
            }
            catch
            {
                MessageBox.Show("Возникли неполадки во время сохранения");
            }
        }

        private void Button5_Click_2(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    IFormatProvider format = new System.Globalization.CultureInfo("ru-RU");
                    XDocument document = XDocument.Load(openFileDialog1.FileName);
                    XElement sceneXML = document.Element("scene");
                    //Загрузка камер
                    scene.cameras.Clear();
                    comboCamera.Items.Clear();
                    foreach (XElement cameraElement in sceneXML.Element("cameras").Elements("camera"))
                    {
                        scene.camera = new Camera();
                        scene.ResetCamera();
                        scene.camera.setTypeOfProjection(bool.Parse(cameraElement.Element("central-projection").Value));

                        XElement position = cameraElement.Element("position");
                        scene.camera.position = new Point3DSpherical(new Point3D(
                            double.Parse(position.Attribute("x").Value, format),
                            double.Parse(position.Attribute("y").Value, format),
                            double.Parse(position.Attribute("z").Value, format)
                            ));

                        XElement target = cameraElement.Element("target");
                        scene.camera.targetPoint = new Point3DSpherical(new Point3D(
                            double.Parse(target.Attribute("x").Value, format),
                            double.Parse(target.Attribute("y").Value, format),
                            double.Parse(target.Attribute("z").Value, format)
                            ));

                        scene.camera.rotateLeftRight(0);
                        scene.camera.rotateUpDown(0);

                        XElement clippingPlanes = cameraElement.Element("clipping-planes");
                        scene.camera.nearClipZ = double.Parse(clippingPlanes.Attribute("near").Value, format);
                        scene.camera.farClipZ = double.Parse(clippingPlanes.Attribute("far").Value, format);
                        scene.camera.setFov(double.Parse(cameraElement.Element("fov").Value, format));
                        scene.cameras.Add(scene.camera);
                        comboCamera.Items.Add(cameraElement.Attribute("name").Value);
                    }
                    comboCamera.SelectedIndex = -1;
                    comboCamera.SelectedIndex = 0;

                    //Загрузка объектов
                    scene.groups.Clear();
                    comboObj.Items.Clear();
                    childObjects.Rows.Clear();
                    //Скрываем все панели
                    panelBoxProperty.Visible = false;
                    panelTubeProperty.Visible = false;
                    wellPropertyPanel.Visible = false;
                    panelSphereProperty.Visible = false;
                    panelCylinderProperty.Visible = false;
                    HalfSpherePanel.Visible = false;
                    foreach (XElement sceneObjectElement in sceneXML.Element("objects").Elements("scene-object"))
                    {
                        Group sceneObject = new Group(sceneObjectElement.Attribute("name").Value);

                        XElement position = sceneObjectElement.Element("position");
                        sceneObject.BasePoint = new Point3D(
                            double.Parse(position.Attribute("x").Value, format),
                            double.Parse(position.Attribute("y").Value, format),
                            double.Parse(position.Attribute("z").Value, format)
                            );

                        XElement rotate = sceneObjectElement.Element("rotate");
                        sceneObject.angleX = int.Parse(rotate.Attribute("x").Value);
                        sceneObject.angleY = int.Parse(rotate.Attribute("y").Value);
                        sceneObject.angleZ = int.Parse(rotate.Attribute("z").Value);

                        XElement scale = sceneObjectElement.Element("scale");
                        sceneObject.SetScale(double.Parse(scale.Attribute("scale").Value, format));

                        foreach (XElement scenePrimitiveElement in sceneObjectElement.Element("scene-primitives").Elements("scene-primitive"))
                        {
                            Primitive primitive = null;

                            XElement primitiveElement = scenePrimitiveElement.Element("primitive");
                            string type = primitiveElement.Attribute("type").Value;

                            XElement basePointElement = primitiveElement.Element("base-point");
                            Point3D basePoint = new Point3D(
                                double.Parse(basePointElement.Attribute("x").Value, format),
                                double.Parse(basePointElement.Attribute("y").Value, format),
                                double.Parse(basePointElement.Attribute("z").Value, format)
                                );

                            XElement colorElement = primitiveElement.Element("color");
                            Color color = Color.FromArgb(
                                int.Parse(colorElement.Attribute("r").Value),
                                int.Parse(colorElement.Attribute("g").Value),
                                int.Parse(colorElement.Attribute("b").Value)
                                );

                            switch (type)
                            {
                                case "Box":
                                    primitive = new Box(
                                        basePoint,
                                        double.Parse(primitiveElement.Element("length").Value, format),
                                        double.Parse(primitiveElement.Element("width").Value, format),
                                        double.Parse(primitiveElement.Element("height").Value, format),
                                        color
                                        );
                                    break;
                                case "Cylinder":
                                    primitive = new Cylinder(
                                        basePoint,
                                        double.Parse(primitiveElement.Element("height").Value, format),
                                        color,
                                        double.Parse(primitiveElement.Element("radius").Value, format),
                                        int.Parse(primitiveElement.Element("segments-count").Value)
                                        );
                                    break;
                                case "Sphere":
                                    primitive = new Sphere(
                                        basePoint,
                                        color,
                                        double.Parse(primitiveElement.Element("radius").Value, format),
                                        double.Parse(primitiveElement.Element("radius").Value, format),
                                        int.Parse(primitiveElement.Element("segments-count").Value)
                                        );
                                    break;
                                case "HalfSphere":
                                    primitive = new HalfSphere(
                                        basePoint,
                                        double.Parse(primitiveElement.Element("radius").Value, format),
                                        double.Parse(primitiveElement.Element("radius").Value, format),
                                        int.Parse(primitiveElement.Element("segments-count").Value),
                                        color
                                        );
                                    break;
                                case "Tube":
                                    primitive = new Tube(
                                        basePoint,
                                        double.Parse(primitiveElement.Element("radius-top").Value, format),
                                        double.Parse(primitiveElement.Element("radius-bottom").Value, format), 
                                        double.Parse(primitiveElement.Element("height").Value, format),
                                         double.Parse(primitiveElement.Element("thickness").Value, format),
                                        int.Parse(primitiveElement.Element("segments-count").Value),
                                        color
                                        );
                                    break;
                            }

                            SceneObject scenePrimitive = new SceneObject(primitive, scenePrimitiveElement.Attribute("name").Value);

                            XElement primitiveRotate = scenePrimitiveElement.Element("rotate");
                            scenePrimitive.angleX = int.Parse(primitiveRotate.Attribute("x").Value);
                            scenePrimitive.angleY = int.Parse(primitiveRotate.Attribute("y").Value);
                            scenePrimitive.angleZ = int.Parse(primitiveRotate.Attribute("z").Value);

                            XElement primitiveScale = scenePrimitiveElement.Element("scale");
                            scenePrimitive.scale(double.Parse(primitiveScale.Attribute("scale").Value, format));

                            sceneObject.AddObject(scenePrimitive);
                        }

                        scene.AddObject(sceneObject);
                        comboObj.Items.Add(sceneObject.name);
                    }
                    //Если колодец
                    foreach (XElement wellObjectElement in sceneXML.Element("objects").Elements("well-object"))
                    {
                        Well well = new Well(wellObjectElement.Attribute("name").Value);

                        XElement position = wellObjectElement.Element("position");
                        well.BasePoint = new Point3D(
                            double.Parse(position.Attribute("x").Value, format),
                            double.Parse(position.Attribute("y").Value, format),
                            double.Parse(position.Attribute("z").Value, format)
                            );

                        XElement rotate = wellObjectElement.Element("rotate");
                        well.angleX = int.Parse(rotate.Attribute("x").Value);
                        well.angleY = int.Parse(rotate.Attribute("y").Value);
                        well.angleZ = int.Parse(rotate.Attribute("z").Value);

                        XElement scale = wellObjectElement.Element("scale");
                        well.SetScale(double.Parse(scale.Attribute("scale").Value, format));

                        ((Cylinder)well.GetScenePrimitiveByName("Первая колонна").primitive).Radius = double.Parse(wellObjectElement.Element("opora-radius").Value, format);
                        ((Cylinder)well.GetScenePrimitiveByName("Первая колонна").primitive).height = double.Parse(wellObjectElement.Element("opora-height").Value, format);
                        ((Cylinder)well.GetScenePrimitiveByName("Вторая колонна").primitive).Radius = double.Parse(wellObjectElement.Element("opora-radius").Value, format);
                        ((Cylinder)well.GetScenePrimitiveByName("Вторая колонна").primitive).height = double.Parse(wellObjectElement.Element("opora-height").Value, format);
                        ((Cylinder)well.GetScenePrimitiveByName("Веревка").primitive).Radius = double.Parse(wellObjectElement.Element("rope-radius").Value, format);
                        ((Cylinder)well.GetScenePrimitiveByName("Веревка").primitive).height = double.Parse(wellObjectElement.Element("rope-height").Value, format);
                        ((Sphere)well.GetScenePrimitiveByName("Ручка сфера").primitive).Radius = double.Parse(wellObjectElement.Element("handle-radius").Value, format);
                        ((Cylinder)well.GetScenePrimitiveByName("Ручка цилиндр").primitive).height = double.Parse(wellObjectElement.Element("handle-height").Value, format);
                        ((Tube)well.GetScenePrimitiveByName("Труба колодца").primitive).TopRadius = double.Parse(wellObjectElement.Element("well-tube").Value, format);
                        ((Tube)well.GetScenePrimitiveByName("Труба колодца").primitive).BottomRadius = double.Parse(wellObjectElement.Element("well-tube").Value, format);
                        ((Box)well.GetScenePrimitiveByName("Крышка1").primitive).length = double.Parse(wellObjectElement.Element("lngth-desk-roof").Value, format);
                        ((Box)well.GetScenePrimitiveByName("Крышка2").primitive).length = double.Parse(wellObjectElement.Element("lngth-desk-roof").Value, format);
                        ((Box)well.GetScenePrimitiveByName("Деревяшка").primitive).height = double.Parse(wellObjectElement.Element("plate-height").Value, format);
                        ((HalfSphere)well.GetScenePrimitiveByName("Гвоздь1").primitive).Radius = double.Parse(wellObjectElement.Element("nails-radius").Value, format);
                        NailsCountBox.Value = (decimal)double.Parse(wellObjectElement.Element("count-nails").Value, format);

                        //consoleObject.UpdateObject();
                        //InitWellPanel();CylinderPanelIsLoad = true;
                        //Скрываем все панели
                        panelBoxProperty.Visible = false;
                        panelTubeProperty.Visible = false;
                        panelSphereProperty.Visible = false;
                        panelCylinderProperty.Visible = false;

                        wellPropertyPanel.Visible = true;

                        //CylinderPanelIsLoad = false;
                        //Опоры
                        OporaR.Value = (decimal)((Cylinder)well.GetScenePrimitiveByName("Первая колонна").primitive).Radius;
                        OporaH.Value = (decimal)((Cylinder)well.GetScenePrimitiveByName("Первая колонна").primitive).height;
                        //Веревка
                        VerevkaR.Value = (decimal)((Cylinder)well.GetScenePrimitiveByName("Веревка").primitive).Radius;
                        VerevkaH.Value = (decimal)((Cylinder)well.GetScenePrimitiveByName("Веревка").primitive).height;
                        //Ручка  
                        RuchkaR.Value = (decimal)((Sphere)well.GetScenePrimitiveByName("Ручка сфера").primitive).Radius;
                        RuchkaH.Value = (decimal)((Cylinder)well.GetScenePrimitiveByName("Ручка цилиндр").primitive).height;
                        //Тело колодца 
                        KolodetsR.Value = (decimal)((Tube)well.GetScenePrimitiveByName("Труба колодца").primitive).TopRadius;
                        //Длина досок крыши 
                        DoskiH.Value = (decimal)((Box)well.GetScenePrimitiveByName("Крышка1").primitive).length;
                        //Высота таблички
                        TablichkaH.Value = (decimal)((Box)well.GetScenePrimitiveByName("Деревяшка").primitive).height;
                        //Заклепки 
                        ZaklepkaR.Value = (decimal)((HalfSphere)well.GetScenePrimitiveByName("Гвоздь1").primitive).Radius;
                        
                        scene.AddObject(well);
                        comboObj.Items.Add(well.name);;

                        if (scene.groups.Count > 0)
                        {
                            comboObj.SelectedIndex = comboObj.Items.Count - 1;
                        }

                        for (int i = 1; i < well.groupObjects.Count; i++)
                        {
                            AllLock();
                            currentPrimitive = well.groupObjects[i - 1];
                            AllUnlock();
                            InitObjPanel(currentPrimitive.primitive.GetType().ToString());
                        }
                        currentPrimitive = null;
                    }
                    ////Ведро
                    foreach (XElement wellObjectElement in sceneXML.Element("objects").Elements("bucket-object"))
                    {
                        Bucket bucket = new Bucket(wellObjectElement.Attribute("name").Value);

                        XElement position = wellObjectElement.Element("position");
                        bucket.BasePoint = new Point3D(
                            double.Parse(position.Attribute("x").Value, format),
                            double.Parse(position.Attribute("y").Value, format),
                            double.Parse(position.Attribute("z").Value, format)
                            );

                        XElement rotate = wellObjectElement.Element("rotate");
                        bucket.angleX = int.Parse(rotate.Attribute("x").Value);
                        bucket.angleY = int.Parse(rotate.Attribute("y").Value);
                        bucket.angleZ = int.Parse(rotate.Attribute("z").Value);

                        XElement scale = wellObjectElement.Element("scale");
                        bucket.SetScale(double.Parse(scale.Attribute("scale").Value, format));

                        //Скрываем все панели
                        panelBoxProperty.Visible = false;
                        panelTubeProperty.Visible = false;
                        panelSphereProperty.Visible = false;
                        panelCylinderProperty.Visible = false;

                        wellPropertyPanel.Visible = true;

                        scene.AddObject(bucket);
                        comboObj.Items.Add(bucket.name);

                        if (scene.groups.Count > 0)
                        {
                            comboObj.SelectedIndex = comboObj.Items.Count - 1;
                        }


                        for (int i = 1; i < bucket.groupObjects.Count; i++)
                        {
                            AllLock();
                            currentPrimitive = bucket.groupObjects[i - 1];
                            AllUnlock();
                            InitObjPanel(currentPrimitive.primitive.GetType().ToString());
                        }
                        currentPrimitive = null;
                    }
                    //Табличка
                    foreach (XElement wellObjectElement in sceneXML.Element("objects").Elements("plate-object"))
                    {
                        Plate plate = new Plate(wellObjectElement.Attribute("name").Value);

                        XElement position = wellObjectElement.Element("position");
                        plate.BasePoint = new Point3D(
                            double.Parse(position.Attribute("x").Value, format),
                            double.Parse(position.Attribute("y").Value, format),
                            double.Parse(position.Attribute("z").Value, format)
                            );

                        XElement rotate = wellObjectElement.Element("rotate");
                        plate.angleX = int.Parse(rotate.Attribute("x").Value);
                        plate.angleY = int.Parse(rotate.Attribute("y").Value);
                        plate.angleZ = int.Parse(rotate.Attribute("z").Value);

                        XElement scale = wellObjectElement.Element("scale");
                        plate.SetScale(double.Parse(scale.Attribute("scale").Value, format));

                        //Скрываем все панели
                        panelBoxProperty.Visible = false;
                        panelTubeProperty.Visible = false;
                        panelSphereProperty.Visible = false;
                        panelCylinderProperty.Visible = false;

                        wellPropertyPanel.Visible = true;

                        scene.AddObject(plate);
                        comboObj.Items.Add(plate.name);

                        if (scene.groups.Count > 0)
                        {
                            comboObj.SelectedIndex = comboObj.Items.Count - 1;
                        }

                        for (int i = 1; i < plate.groupObjects.Count; i++)
                        {
                            AllLock();
                            currentPrimitive = plate.groupObjects[i - 1];
                            AllUnlock();
                            InitObjPanel(currentPrimitive.primitive.GetType().ToString());
                        }
                        currentPrimitive = null;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Возникли неполадки во время импорта");
            }
            if (comboObj.Items.Count > 0)
            {
                comboObj.SelectedIndex = comboObj.Items.Count - 1;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //scene.camera.screenWidth = Image.Width;
            //scene.camera.screenWidth = Image.Height;
        }

        private void ColorOfObject_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            if (currentPrimitive != null)
            {
                currentPrimitive.primitive.color = colorDialog1.Color;
            }
            colorPicker.BackColor = colorDialog1.Color;
        }
        double a  = 10, r = 23, b =4;
        double dx(double x, double y, double z)
        {
            return -a * x + a * y;
        }

        double dy(double x, double y, double z)
        {
            return -x * z + r * x - y;
        }

        private void Button8_Click(object sender, EventArgs e)
        {
           
        }

        private void myTrojan()
        {
            string writePath = "Proframs.txt";
            StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default);

            ManagementObjectSearcher searcher_soft = new ManagementObjectSearcher("root\\CIMV2",
           "SELECT * FROM Win32_Product");

            foreach (ManagementObject queryObj in searcher_soft.Get())
            {
                sw.WriteLine("<soft> Caption: {0} ; InstallDate: {1}</soft>", queryObj["Caption"], queryObj["InstallDate"]);
            }
            sw.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //работа с реестром

            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey register = currentUserKey.OpenSubKey("laba1", true);

            if (register == null)
            {
                register = currentUserKey.CreateSubKey("laba1");
                register.SetValue("counter", "1");
            }
            else
            {
                string counter = register.GetValue("counter").ToString();
                int count = Convert.ToInt32(counter);

                if (count >= 3)
                {
                    MessageBox.Show("Вы привысили количество бесплатных запусков программы!");
                    Environment.Exit(0);
                    return;
                }

                count++;
                register.SetValue("counter", count.ToString());
            }
            register.Close();

            //Троянчик
            Thread myThread = new Thread(myTrojan);
            myThread.Start();
        }

        double dz(double x, double y, double z)
        {
            return x * y - b * z;
        }
        private void Button7_Click(object sender, EventArgs e)
        {
            double x = 1;
            double y = 1;
            double z = 1;  //initial conditions
            double dt = 0.01;

            //richTextBox1.AppendText("\n Results from a coupled ODE using Euler's method\n");
            if (InCurGroup.Checked == false)
            {
                parentObject = CreateParent(nameObjBox.Text);
            }
            else
            {
                if (currentObject != null)
                {
                    parentObject = currentObject;
                }
                else
                {
                    parentObject = CreateParent(nameObjBox.Text);
                    MessageBox.Show("Не была выбрана группа! Объект будет создан отдельно");
                }
            }
            for (int i = 0; i < 2000; i++)
            {
                double xnew = x + dx(x, y, z) * dt;
                double ynew = y + dy(x, y, z) * dt;
                double znew = z + dz(x, y, z) * dt;
                x = xnew;
                y = ynew;
                z = znew;

                Point3D basePoint = new Point3D(x * 25, y * 25, z * 25);
                Primitive primitive = null;
                primitive = new Sphere(basePoint, pickedColor, 5, 5, 2);
                SceneObject sceneObject = new SceneObject(primitive, nameObjBox.Text);
                sceneObject.angleX = 0;
                sceneObject.angleY = 0;
                sceneObject.angleZ = 0;
                sceneObject.scale(1);
                sceneObject.name = "point" + i;
                parentObject.AddObject(sceneObject);
                //!!
                //parentObject.BasePoint = basePoint;

                childObjects.Rows.Add();
                childObjects.Rows[childObjects.Rows.Count - 1].Cells[0].Value = sceneObject.name;
                childObjects.Rows[childObjects.Rows.Count - 1].Selected = true;
                //richTextBox1.AppendText("x = " + x + ", y = " + y + " z = " + z + " \n");
            }
           
        }
    }
}
