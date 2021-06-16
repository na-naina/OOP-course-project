using System.Collections.Generic;
using static OOP_projects.OpenGL.GL;


namespace OOP_projects.GraphicsEngine
{
    class VertexBufferLayout
    {
		private List<VertexBufferElement> m_Elements = new List<VertexBufferElement>();
		private uint m_Stride;

		
		public VertexBufferLayout()
        {
			m_Stride = 0;
        }
		public void Push(uint count)
		{
		}

		
		public void Push<T>(uint count)
		{
			if (typeof(T) == typeof(float)) {
				m_Elements.Add(new VertexBufferElement(GL_FLOAT, count, GL_FALSE));
				m_Stride += VertexBufferElement.GetTypeSize(GL_FLOAT) * count;
			}

			if (typeof(T) == typeof(uint))
			{
				m_Elements.Add(new VertexBufferElement(GL_UNSIGNED_INT, count, GL_FALSE));
				m_Stride += VertexBufferElement.GetTypeSize(GL_UNSIGNED_INT) * count;
			}
			if (typeof(T) == typeof(char))
			{
				m_Elements.Add(new VertexBufferElement(GL_UNSIGNED_BYTE, count, GL_TRUE));
				m_Stride += VertexBufferElement.GetTypeSize(GL_UNSIGNED_BYTE) * count;
			}
		}

		public List<VertexBufferElement> Elements{
			get { return m_Elements; }
		}
		public uint Stride
		{
			get { return m_Stride; }
		}
	}


	struct VertexBufferElement
	{
		public uint type;
		public uint count;
		public int normalized;
		
		public VertexBufferElement(uint type, uint count, int normalized) 
		{
			this.type = type;
			this.count = count;
			this.normalized = normalized;
		}
		public static uint GetTypeSize(uint type)
		{
			switch (type)
			{
				case (GL_FLOAT): return 4;
				case (GL_UNSIGNED_INT): return 4;
				case (GL_UNSIGNED_BYTE): return 1;
			}

			return 0;
		}
	};

}
