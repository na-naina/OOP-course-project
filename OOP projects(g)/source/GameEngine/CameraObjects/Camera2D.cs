using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace OOP_projects.GameEngine
{
    class Camera2D : Camera
    {
        public Vector2 Pos;

        public float ZoomVal;

        public Camera2D(Vector2 focusPos)
        {
            Pos = focusPos;
            ZoomVal = 1.0f;
        }

        public override Matrix4x4 GetViewMatrix()
        {

            Matrix4x4 zoom = Matrix4x4.CreateScale(ZoomVal);

            Vector3 pos = new Vector3(Pos.X, Pos.Y, 1.0f);

            Matrix4x4 view = Matrix4x4.CreateLookAt(pos, pos + new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f));

            return zoom*view;
        }

        public void Zoom(float times)
        {
            ZoomVal *= times;
        }

        public void ProcessMouseScroll(float yoffset)
        {
            ZoomVal += yoffset/8;
            if (ZoomVal < 0.02f)
                ZoomVal = 0.02f;
            if (ZoomVal > 16.0f)
                ZoomVal = 16.0f;
        }
        //NotReallyNeededAsCameraVectors always stays the same;
        protected override void UpdateCameraVectors()
        {
        }
    }
}
