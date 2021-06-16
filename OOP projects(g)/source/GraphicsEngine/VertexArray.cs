using System;
using System.Collections.Generic;
using System.Text;
using static OOP_projects.OpenGL.GL;

namespace OOP_projects.GraphicsEngine
{
    class VertexArray
    {
        private uint openglID;

        public uint ID { get { return openglID; } }

        public VertexArray()
        {
            openglID = glGenVertexArray();
        }

        public void AddBuffer(VertexBuffer vb, VertexBufferLayout layout)
        {
            Bind();
            vb.Bind();

            var elements = layout.Elements;
            uint offset = 0;
            for(uint i = 0; i < elements.Count; i++)
            {
                var element = elements[(int)i];

                glEnableVertexAttribArray(i);
                glVertexAttribPointer(i, (int)element.count, (int)element.type, (element.normalized > 0 ? true : false),
                    (int)layout.Stride, (IntPtr)offset);

                offset += (uint)element.count * VertexBufferElement.GetTypeSize(element.type);
            }

        }

        public void Bind()
        {
            glBindVertexArray(openglID);
        }

        public void Unbind()
        {
            glBindVertexArray(0);
        }

    }
}
