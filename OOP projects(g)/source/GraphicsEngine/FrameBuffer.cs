using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text;
using static OOP_projects.OpenGL.GL;

namespace OOP_projects.GraphicsEngine
{
    unsafe class FrameBuffer
    {
        private uint openglID;
        private uint rbo;
        private uint texColorBuffer;
        
        public uint ID { get { return openglID; } }
        public uint TextureID { get { return texColorBuffer; } }

        private string directory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/";




        public FrameBuffer()
        {
            openglID = glGenFramebuffer();
            glBindFramebuffer(GL_FRAMEBUFFER, openglID);

            fixed (uint* ptr = &texColorBuffer)
            {
                glGenTextures(1, ptr);
            }
            glBindTexture(GL_TEXTURE_2D, texColorBuffer);
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, 800, 600, 0, GL_RGBA, GL_UNSIGNED_BYTE, NULL);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            glBindTexture(GL_TEXTURE_2D, 0);

            glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, texColorBuffer, 0);

            
            rbo = glGenRenderbuffer();
            glBindRenderbuffer(rbo);
            glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, 800, 600);
            glBindRenderbuffer(0);

            glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, rbo);


            if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
            {
                Console.WriteLine("ERROR: Frambuffer object is incomplete!");
            }

            glBindFramebuffer(GL_FRAMEBUFFER, 0);

        }

        ~FrameBuffer()
        {
        }

        public void Bind()
        {
            glBindFramebuffer(GL_FRAMEBUFFER, openglID);
        }

        public void Unbind()
        {
            glBindFramebuffer(GL_FRAMEBUFFER, 0);
        }

        public void BindTexture(int slot = 0)
        {  
            glActiveTexture(slot + GL_TEXTURE0);
            glBindTexture(GL_TEXTURE_2D, texColorBuffer);  
        }

        unsafe public Texture GetTexture()
        {
            int temp;
            glGetIntegerv(GL_FRAMEBUFFER_BINDING, &temp);
            Bind();
            byte[] data = new byte[800 * 600 * 4];

            
            glReadPixels(0, 0, 800, 600, GL_BGRA, GL_UNSIGNED_BYTE, data);

            
            glBindFramebuffer(GL_FRAMEBUFFER, (uint)temp);
            Texture returnVal;
            Bitmap bmp = new Bitmap(directory + "Resources/Textures/temp.png");
            BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            IntPtr ptr = bmpData.Scan0;

            System.Runtime.InteropServices.Marshal.Copy(data, 0, ptr, Math.Abs(bmpData.Stride) * bmp.Height);

            bmp.UnlockBits(bmpData);

            returnVal = new Texture(bmp);

            return returnVal;

        }

        
    }
}
