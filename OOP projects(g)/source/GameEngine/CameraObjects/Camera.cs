using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace OOP_projects.GameEngine
{
    abstract class Camera
    {
        abstract public Matrix4x4 GetViewMatrix();

        abstract protected void UpdateCameraVectors();
    }
}
