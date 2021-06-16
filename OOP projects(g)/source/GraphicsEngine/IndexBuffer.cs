using System;
using System.Collections.Generic;
using System.Text;
using static OOP_projects.OpenGL.GL;


namespace OOP_projects.GraphicsEngine
{
	unsafe class IndexBuffer
	{
		private uint openglID;
		private uint count;

		public uint ID { get { return openglID; } }

		public IndexBuffer(uint* data, uint count)
		{
			this.count = count;
			openglID = glGenBuffer();
			glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, openglID);
			glBufferData(GL_ELEMENT_ARRAY_BUFFER, (int)count * sizeof(uint), data, GL_STATIC_DRAW);
		}
		~IndexBuffer()
		{
		}

		public void Bind() 
		{
			glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, openglID);
		}

		public void Unbind()
		{
			glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
		}

		public uint Count
		{
			get { return count; }
		}
	}
}
