using System;
using System.Collections.Generic;
using System.IO;
using static OOP_projects.OpenGL.GL;
using GlmSharp;
using System.Numerics;

namespace OOP_projects.GraphicsEngine
{
	class Shader
	{
		private string m_FilePath;
		private Dictionary<string, int> m_UniformLocationCache = new Dictionary<string, int>();
		private uint openglID;
		public uint ID { get { return openglID; } }
		private enum ShaderType
		{
			NONE = -1, VERTEX = 0, FRAGMENT = 1
		}
		private string[] ParseShader(string filepath)
        {
			string[] returnArray = new string[2];

			string projectDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

			string[] lines = File.ReadAllLines(projectDir + "/" + filepath);
			ShaderType type = ShaderType.NONE;
			for(int i = 0; i < lines.Length; i++)
            {
				lines[i] += '\n';
            }
			foreach(string line in lines)
            {
				if(line.Contains("#shader"))
                {
					if (line.Contains("vertex"))
						type = ShaderType.VERTEX;
					else if (line.Contains("fragment"))
						type = ShaderType.FRAGMENT;
                }
				else if (type != ShaderType.NONE)
                {
					returnArray[(int)type] += line;
                }


			}

			return returnArray;
		}


		unsafe private uint CompileShader(int type, string source)
		{
			uint id = glCreateShader(type);

			glShaderSource(id, source);
			
			glCompileShader(id);

			int result;
			glGetShaderiv(id, GL_COMPILE_STATUS, &result);
			if (result == GL_FALSE)
			{
				int length;
				glGetShaderiv(id, GL_INFO_LOG_LENGTH, &length);
				string message = glGetShaderInfoLog(id);
				


				Console.WriteLine($"Failed to compile [{(type == GL_VERTEX_SHADER ? "VERTEX" : "FRAGMENT")}] shader\n");
				Console.WriteLine(message);
				glDeleteShader(id);
				return 0;
			}

			return id;
		}
		private	uint CreateShader(string vertexShader, string fragmentShader)
		{
			uint program = glCreateProgram();

			uint vs = CompileShader(GL_VERTEX_SHADER, vertexShader);
			uint fs = CompileShader(GL_FRAGMENT_SHADER, fragmentShader);

			glAttachShader(program, vs);
			glAttachShader(program, fs);

			glLinkProgram(program);

			glValidateProgram(program);

			glDeleteShader(vs);
			glDeleteShader(fs);

			return program;
		}

		private int GetUniformLocation(string name)
        {
			if (m_UniformLocationCache.ContainsKey(name))
				return m_UniformLocationCache[name];


			int location = glGetUniformLocation(openglID, name);
			if (location == -1)
				Console.WriteLine($"[Warning]: uniform {name} location undefined!");


			m_UniformLocationCache[name] = location;

			return location;
		}

		public Shader(string filename)
		{
            var shader = ParseShader(filename);
			openglID = CreateShader(shader[(int)ShaderType.VERTEX], shader[(int)ShaderType.FRAGMENT]);
        }
		~Shader()
        {

        }

		public void Bind()
        {
			glUseProgram(openglID);
		}
		public void Unbind()
        {
			glUseProgram(0);
		}

		public void SetUniform1i(string name, int value)
        {
			glUniform1i(GetUniformLocation(name), value);
		}

		public void SetUniform1iv(string name, int size, int[] values)
		{
			glUniform1iv(GetUniformLocation(name), size, values);
		}
		public void SetUniform1f(string name, float value)
        {
			glUniform1f(GetUniformLocation(name), value);
		}
		public void SetUniform4f(string name, float v0, float v1, float v2, float v3)
        {
			glUniform4f(GetUniformLocation(name), v0, v1, v2, v3);
		}

		public void SetUniform3f(string name, float v0, float v1, float v2)
        {
			glUniform3f(GetUniformLocation(name), v0, v1, v2);

		}
		public void SetUniform3f(string name, vec3 vector)
        {
			glUniform3f(GetUniformLocation(name), vector.x, vector.y, vector.z);
		}

		public void SetUniform3f(string name, Vector3 vector)
		{
			glUniform3f(GetUniformLocation(name), vector.X, vector.Y, vector.Z);
		}

		unsafe public void SetUniformMat4f(string name, mat4 matrix)
        {
			fixed (float* val = matrix.Values)
			{
				glUniformMatrix4fv(GetUniformLocation(name), 1, false, val);
			}
		}
		public void SetUniformMat4f(string name, Matrix4x4 matrix)
		{
			glUniformMatrix4fv(GetUniformLocation(name), 1, false, GetMatrix4x4Values(matrix));	
		}

		private float[] GetMatrix4x4Values(Matrix4x4 m)
		{
			return new float[]
			{
		m.M11, m.M12, m.M13, m.M14,
		m.M21, m.M22, m.M23, m.M24,
		m.M31, m.M32, m.M33, m.M34,
		m.M41, m.M42, m.M43, m.M44
			};
		}

	}
}
