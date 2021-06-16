using System;
using System.Collections.Generic;
using System.Text;
using static OOP_projects.OpenGL.GL;

namespace OOP_projects.GraphicsEngine
{
	class VertexBuffer
	{
		private uint openglID;

		public uint ID { get { return openglID; } }
		unsafe public VertexBuffer(void* data,  int size)
		{
			openglID = glGenBuffer();
			glBindBuffer(GL_ARRAY_BUFFER, openglID);
			glBufferData(GL_ARRAY_BUFFER, size, data, GL_STATIC_DRAW);
		}

		~VertexBuffer()
		{

		}

		public void Bind()
		{
			glBindBuffer(GL_ARRAY_BUFFER, openglID);
		}
		public void Unbind()
		{
			glBindBuffer(GL_ARRAY_BUFFER, 0);
		}
	}
}
