﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static OOP_projects.GameEngine.CustomMath;

namespace OOP_projects.GameEngine
{
    class Camera3D : Camera
    {
        // Default camera values
        private const float YAW = -90.0f;
        private const float PITCH = 0.0f;
        private const float SPEED = 2.5f;
        private const float SENSITIVITY = 0.1f;
        private const float ZOOM = 45.0f;

        public Vector3 Position;
        public Vector3 Front;
        public Vector3 Up;
        public Vector3 Right;
        public Vector3 WorldUp;

        // euler Angles
        public float Yaw;
        public float Pitch;
        // camera options
        public float MovementSpeed;
        public float MouseSensitivity;
        public float Zoom;

        // constructor with vectors
        public Camera3D(Vector3 position, Vector3 up, float yaw = YAW, float pitch = PITCH)
        {
            

            Position = position;
            WorldUp = up;
            Yaw = yaw;
            Pitch = pitch;
            Front = new Vector3(0.0f, 0.0f, -1.0f);
            MovementSpeed = SPEED;
            MouseSensitivity = SENSITIVITY;
            Zoom = ZOOM;
            UpdateCameraVectors();
        }
        public Camera3D(float yaw = YAW, float pitch = PITCH)
        {
            Position = new Vector3(0f);
            WorldUp = new Vector3(0f, 1f, 0f);
            Yaw = yaw;
            Pitch = pitch;
            Front = new Vector3(0.0f, 0.0f, -1.0f);
            MovementSpeed = SPEED;
            MouseSensitivity = SENSITIVITY;
            Zoom = ZOOM;
            UpdateCameraVectors();
        }

        public Camera3D(float posX, float posY, float posZ, float upX, float upY, float upZ, float yaw, float pitch)

        {

            Yaw = yaw;
            Pitch = pitch;
            Front = new Vector3(0.0f, 0.0f, -1.0f);

            Position = new Vector3(posX, posY, posZ);
            WorldUp = new Vector3(upX, upY, upZ);
            Yaw = yaw;
            Pitch = pitch;
            UpdateCameraVectors();
        }

        public override Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        }

        protected override void UpdateCameraVectors()
        {

            Vector3 front;
            front.X = (float)Math.Cos(ToRadians(Yaw)) * (float)Math.Cos(ToRadians(Pitch));

            front.Y = (float)Math.Sin(ToRadians(Pitch));
            front.Z = (float)Math.Sin(ToRadians(Yaw)) * (float)Math.Cos(ToRadians(Pitch));
            Front = Vector3.Normalize(front);
            Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }

        public enum Camera_Movement
        {
            FORWARD,
            BACKWARD,
            LEFT,
            RIGHT,
            UP,
            DOWN
        };




    




        public void ProcessKeyboard(Camera_Movement direction, float deltaTime)
        {
            float velocity = MovementSpeed * deltaTime;
            if (direction == Camera_Movement.FORWARD)
                Position += Front * velocity;
            if (direction == Camera_Movement.BACKWARD)
                Position -= Front * velocity;
            if (direction == Camera_Movement.LEFT)
                Position -= Right * velocity;
            if (direction == Camera_Movement.RIGHT)
                Position += Right * velocity;
            if (direction == Camera_Movement.UP)
                Position += Up * velocity;
            if (direction == Camera_Movement.DOWN)
                Position -= Up * velocity;
        }

        public void ProcessMouseMovement(float xoffset, float yoffset, bool constrainPitch = true)
        {
            xoffset *= MouseSensitivity;
            yoffset *= MouseSensitivity;

            Yaw += xoffset;
            Pitch += yoffset;

            if (constrainPitch)
            {
                if (Pitch > 89.0f)
                    Pitch = 89.0f;
                if (Pitch < -89.0f)
                    Pitch = -89.0f;
            }

            UpdateCameraVectors();
        }

        public void ProcessMouseScroll(float yoffset)
        {
            Zoom -= yoffset;
            if (Zoom < 1.0f)
                Zoom = 1.0f;
            if (Zoom > 45.0f)
                Zoom = 45.0f;
        }

    }
}