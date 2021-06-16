using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using static OOP_projects.OpenGL.GL;
using System.Data.Common;



namespace OOP_projects.GraphicsEngine
{
    class Texture
    {
        private uint openglID;
        private string m_FilePath = "";
        private Bitmap m_Bitmap;

        public uint ID { get { return openglID; } }

        private string directory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/";

        private Bitmap SwapRedAndBlueChannels(Bitmap bitmap)
        {
            var imageAttr = new ImageAttributes();
            imageAttr.SetColorMatrix(new ColorMatrix(
                                         new[]
                                             {
                                                 new[] {0.0F, 0.0F, 1.0F, 0.0F, 0.0F},
                                                 new[] {0.0F, 1.0F, 0.0F, 0.0F, 0.0F},
                                                 new[] {1.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                 new[] {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                                                 new[] {0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
                                             }
                                         ));
            var temp = new Bitmap(bitmap.Width, bitmap.Height);
            GraphicsUnit pixel = GraphicsUnit.Pixel;
            using (Graphics g = Graphics.FromImage(temp))
            {
                g.DrawImage(bitmap, Rectangle.Round(bitmap.GetBounds(ref pixel)), 0, 0, bitmap.Width, bitmap.Height,
                            GraphicsUnit.Pixel, imageAttr);
            }

            return temp;
        }

        unsafe public Texture(int W, int H, int slot = 1)
        {
            glActiveTexture(GL_TEXTURE0 + slot);

            m_FilePath = directory;



            fixed (uint* ptr = &openglID)
            {
                glGenTextures(1, ptr);
            }

            glBindTexture(GL_TEXTURE_2D, openglID);

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);

            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, W, H, 0, GL_RGBA, GL_UNSIGNED_BYTE, NULL);

            glBindTexture(GL_TEXTURE_2D, 0);

        }

        unsafe public Texture(Bitmap bitmap, int slot = 1)
        {
            glActiveTexture(GL_TEXTURE0 + slot);

            m_FilePath = directory;


            m_Bitmap = bitmap;
            m_Bitmap = SwapRedAndBlueChannels(m_Bitmap);
            m_Bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

            BitmapData m_buffer = m_Bitmap.LockBits(new System.Drawing.Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height),
            ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            fixed (uint* ptr = &openglID)
            {
                glGenTextures(1, ptr);
            }

            glBindTexture(GL_TEXTURE_2D, openglID);

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);

            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, m_buffer.Width, m_buffer.Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, m_buffer.Scan0);

            glBindTexture(GL_TEXTURE_2D, 0);

            m_Bitmap.UnlockBits(m_buffer);

        }
        unsafe public Texture(string filepath, int slot = 1)
        {
            glActiveTexture(GL_TEXTURE0 + slot);
            m_FilePath = directory + filepath;


            m_Bitmap = new Bitmap(m_FilePath);
            m_Bitmap = SwapRedAndBlueChannels(m_Bitmap);
            m_Bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

            BitmapData m_buffer = m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            fixed (uint* ptr = &openglID)
            {
                glGenTextures(1, ptr);
            }
            
            glBindTexture(GL_TEXTURE_2D, openglID);

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);

            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, m_buffer.Width, m_buffer.Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, m_buffer.Scan0);

            glBindTexture(GL_TEXTURE_2D, 0);

            m_Bitmap.UnlockBits(m_buffer);

        }
        unsafe public void Write(Bitmap bitmap)
        {

            glBindTexture(GL_TEXTURE_2D, openglID);
            bitmap = SwapRedAndBlueChannels(bitmap);
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
            BitmapData m_buffer = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, m_buffer.Width, m_buffer.Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, m_buffer.Scan0);

            glBindTexture(GL_TEXTURE_2D, 0);

            bitmap.UnlockBits(m_buffer);
        }
        

        public void SafeImage(string filename)
        {
            m_Bitmap = SwapRedAndBlueChannels(m_Bitmap);
            m_Bitmap.Save(directory + filename);
            m_Bitmap = SwapRedAndBlueChannels(m_Bitmap);
        }
  
        public void Bind(int slot = 0)
        {
            glActiveTexture(slot + GL_TEXTURE0);
            glBindTexture(GL_TEXTURE_2D, openglID);
        }
        
        public void Unbind()
        {
            glBindTexture(GL_TEXTURE_2D, 0);
        }
        
        public int Width  { get{ return m_Bitmap.Width; } }
        public int Height  { get { return m_Bitmap.Height; } }
    }
}
