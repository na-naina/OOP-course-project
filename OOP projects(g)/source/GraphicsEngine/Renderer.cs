using System;
using System.Numerics;
using System.Runtime.InteropServices;
using static OOP_projects.OpenGL.GL;


namespace OOP_projects.GraphicsEngine
{
    unsafe static class Renderer
    {
        const uint MaxQuadCount = 10000;
        const uint MaxVertexCount = MaxQuadCount * 4;
        const uint MaxIndexCount = MaxQuadCount * 6;
        const uint MaxTexturesCount = 32;

        static private HoldsData r_Data = new HoldsData();

        public static void Init()
        {
            r_Data.QuadVA = glGenVertexArray();
            glBindVertexArray(r_Data.QuadVA);

            r_Data.QuadVB = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, r_Data.QuadVB);
            glBufferData(GL_ARRAY_BUFFER, (int)MaxVertexCount * sizeof(Vertex), NULL, GL_DYNAMIC_DRAW);


            glEnableVertexAttribArray(0);
            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(Vertex), (IntPtr)0);

            glEnableVertexAttribArray(1);
            glVertexAttribPointer(1, 4, GL_FLOAT, false, sizeof(Vertex), (IntPtr)12);

            glEnableVertexAttribArray(2);
            glVertexAttribPointer(2, 2, GL_FLOAT, false, sizeof(Vertex), (IntPtr)28);

            glEnableVertexAttribArray(3);
            glVertexAttribPointer(3, 1, GL_FLOAT, false, sizeof(Vertex), (IntPtr)36);

            uint[] indicies = new uint[MaxIndexCount];
            uint offset = 0;
            for(int i = 0; i < MaxIndexCount; i += 6)
            {
                indicies[i + 0] = 0 + offset;
                indicies[i + 1] = 1 + offset;
                indicies[i + 2] = 2 + offset;

                indicies[i + 3] = 2 + offset;
                indicies[i + 4] = 3 + offset;
                indicies[i + 5] = 0 + offset;

                offset += 4;
            }

            r_Data.QuadIB = glGenBuffer();
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, r_Data.QuadIB);
            fixed (void* ptr = &indicies[0]) {
                glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(uint) * (int)MaxIndexCount, ptr, GL_STATIC_DRAW);
            }

            r_Data.WhiteTexture = glGenTexture();
            glBindTexture(GL_TEXTURE_2D, r_Data.WhiteTexture);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);

            uint color = 0xffffffff;
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, 1, 1, 0, GL_RGBA, GL_UNSIGNED_BYTE, &color);

            r_Data.TextureSlots[0] = r_Data.WhiteTexture;
            for (int i = 1; i < MaxTexturesCount; i++)
                r_Data.TextureSlots[i] = r_Data.WhiteTexture;
        }

        public static void Terminate()
        {
            glDeleteVertexArray(r_Data.QuadVA);
            glDeleteBuffer(r_Data.QuadVB);
            glDeleteBuffer(r_Data.QuadIB);
            glDeleteTexture(r_Data.WhiteTexture);
        }

        public static void BeginBatch()
        {
            r_Data.currentVertex = 0;
        }

        public static void EndBatch()
        {
            uint size = r_Data.currentVertex*(uint)(sizeof(Vertex)/sizeof(byte));
            glBindBuffer(GL_ARRAY_BUFFER, r_Data.QuadVB);
            fixed (void* ptr = &r_Data.QuadBuffer[0])
            {
                glBufferSubData(GL_ARRAY_BUFFER, 0, size, ptr);
            }
        }

        public static void Flush()
        {
            for (int i = 0; i < r_Data.currentTexture; i++)
            {
                glActiveTexture(i + GL_TEXTURE0);
                glBindTexture(GL_TEXTURE_2D, r_Data.TextureSlots[i]);
            }
                

            glBindVertexArray(r_Data.QuadVA);
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, r_Data.QuadIB);
            glDrawElements(GL_TRIANGLES, (int)r_Data.IndexCount, GL_UNSIGNED_INT, NULL);

            r_Data.IndexCount = 0;
            r_Data.currentTexture = 1;
        }

        public static void DrawQuad(Vector2 position, Vector2 size, Vector4 color, float z_val = 0)
        {
            if(r_Data.IndexCount >= MaxIndexCount)
            {
                EndBatch();
                Flush();
                BeginBatch();
            }

            float textureIndex = 0.0f;

            r_Data.QuadBuffer[r_Data.currentVertex].position = new Vector3(position.X, position.Y, z_val);
            r_Data.QuadBuffer[r_Data.currentVertex].color = color;
            r_Data.QuadBuffer[r_Data.currentVertex].texCoords = new Vector2(0.0f, 0.0f);
            r_Data.QuadBuffer[r_Data.currentVertex].texturedId = textureIndex;
            r_Data.currentVertex++;

            r_Data.QuadBuffer[r_Data.currentVertex].position = new Vector3(position.X + size.X, position.Y, z_val);
            r_Data.QuadBuffer[r_Data.currentVertex].color = color;
            r_Data.QuadBuffer[r_Data.currentVertex].texCoords = new Vector2(1.0f, 0.0f);
            r_Data.QuadBuffer[r_Data.currentVertex].texturedId = textureIndex;
            r_Data.currentVertex++;

            r_Data.QuadBuffer[r_Data.currentVertex].position = new Vector3(position.X + size.X, position.Y + size.Y, z_val);
            r_Data.QuadBuffer[r_Data.currentVertex].color = color;
            r_Data.QuadBuffer[r_Data.currentVertex].texCoords = new Vector2(1.0f, 1.0f);
            r_Data.QuadBuffer[r_Data.currentVertex].texturedId = textureIndex;
            r_Data.currentVertex++;

            r_Data.QuadBuffer[r_Data.currentVertex].position = new Vector3(position.X, position.Y + +size.Y, z_val);
            r_Data.QuadBuffer[r_Data.currentVertex].color = color;
            r_Data.QuadBuffer[r_Data.currentVertex].texCoords = new Vector2(0.0f, 1.0f);
            r_Data.QuadBuffer[r_Data.currentVertex].texturedId = textureIndex;
            r_Data.currentVertex++;

            r_Data.IndexCount += 6;

        }

        public static void DrawQuad(Vector2 position, Vector2 size, uint textureID, float z_val = 0)
        {
            if (r_Data.IndexCount >= MaxIndexCount || r_Data.currentTexture >= MaxTexturesCount)
            {
                EndBatch();
                Flush();
                BeginBatch();
            }

            float textureIndex = 0.0f;

            for(int i = 0; i < r_Data.currentTexture; i++)
            {
                if(r_Data.TextureSlots[i] == textureID)
                {
                    textureIndex = (float)i;
                    break;
                }
            }

            if(textureIndex == 0.0f)
            {
                textureIndex = (float)r_Data.currentTexture;
                r_Data.TextureSlots[r_Data.currentTexture] = textureID;
                r_Data.currentTexture++;
            }

            r_Data.QuadBuffer[r_Data.currentVertex].position = new Vector3(position.X, position.Y, z_val);
            r_Data.QuadBuffer[r_Data.currentVertex].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            r_Data.QuadBuffer[r_Data.currentVertex].texCoords = new Vector2(0.0f, 0.0f);
            r_Data.QuadBuffer[r_Data.currentVertex].texturedId = textureIndex;
            r_Data.currentVertex++;

            r_Data.QuadBuffer[r_Data.currentVertex].position = new Vector3(position.X + size.X, position.Y, z_val);
            r_Data.QuadBuffer[r_Data.currentVertex].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); ;
            r_Data.QuadBuffer[r_Data.currentVertex].texCoords = new Vector2(1.0f, 0.0f);
            r_Data.QuadBuffer[r_Data.currentVertex].texturedId = textureIndex;
            r_Data.currentVertex++;

            r_Data.QuadBuffer[r_Data.currentVertex].position = new Vector3(position.X + size.X, position.Y + size.Y, z_val);
            r_Data.QuadBuffer[r_Data.currentVertex].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); ;
            r_Data.QuadBuffer[r_Data.currentVertex].texCoords = new Vector2(1.0f, 1.0f);
            r_Data.QuadBuffer[r_Data.currentVertex].texturedId = textureIndex;
            r_Data.currentVertex++;

            r_Data.QuadBuffer[r_Data.currentVertex].position = new Vector3(position.X, position.Y + +size.Y, z_val);
            r_Data.QuadBuffer[r_Data.currentVertex].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); ;
            r_Data.QuadBuffer[r_Data.currentVertex].texCoords = new Vector2(0.0f, 1.0f);
            r_Data.QuadBuffer[r_Data.currentVertex].texturedId = textureIndex;
            r_Data.currentVertex++;

            r_Data.IndexCount += 6;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        struct Vertex
        {
            public Vector3 position;
            public Vector4 color;
            public Vector2 texCoords;
            public float texturedId;
        }
        private class HoldsData
        {
            public uint QuadVA = 0;
            public uint QuadVB = 0;
            public uint QuadIB = 0;

            public uint WhiteTexture = 0;
            public uint WhiteTextureSlot = 0;

            public uint IndexCount = 0;

            public Vertex[] QuadBuffer = new Vertex[MaxVertexCount];
            public uint currentVertex;

            public uint[] TextureSlots = new uint[MaxTexturesCount];
            public uint currentTexture = 1;

        }

    }
}
