using System;
using OpenTK.Graphics.OpenGL;
using PT_Sim;
    public class CLPCassette : IDisposable
    {
        private float scale;
        private int numVertices;
        private int numIndices;
        private int vao, vbo, ebo;
        private float[] positions;
        private uint[] indices;

        float x1, y1, x2, y2, x3, y3, x4, y4;

        public CLPCassette(float scale, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            this.scale = scale;
            this.x1 = x1; this.y1 = y1;
            this.x2 = x2; this.y2 = y2;
            this.x3 = x3; this.y3 = y3;
            this.x4 = x4; this.y4 = y4;

            numVertices = 4;
            numIndices = 6;
        }

        ~CLPCassette()
        {
            Dispose();
        }

        public void Initialize()
        {
            positions = new float[]
            {
                scale * x1, scale * y1,
                scale * x2, scale * y2,
                scale * x3, scale * y3,
                scale * x4, scale * y4
            };

            indices = new uint[] { 0, 1, 2, 1, 2, 3 };

            GL.GenVertexArrays(1, out vao);
            GL.GenBuffers(1, out vbo);
            GL.GenBuffers(1, out ebo);
            if (vao == 0 || vbo == 0 || ebo == 0)
            {
                Logger.Log("Error:", "VAO, VBO, or EBO not initialized correctly");
                return;
            }

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        public void Render(Shader shader)
        {
            shader.Bind();
            int location = shader.GetUniformLocation("u_Color");

            if (location != -1)
            {
                GL.LineWidth(2.0f);

                GL.BindVertexArray(vao);

                // Draw outline
                GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
                GL.DrawElements(PrimitiveType.LineLoop, numIndices, DrawElementsType.UnsignedInt, 0);

                // Draw filled interior
                GL.Uniform4(location, 0.098f, 0.361f, 0.380f, 1.0f);
                GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, 0);

                GL.BindVertexArray(0);
            }

            shader.Unbind();
        }

        public void Dispose()
        {
            if (vao != 0) GL.DeleteVertexArray(vao);
            if (vbo != 0) GL.DeleteBuffer(vbo);
            if (ebo != 0) GL.DeleteBuffer(ebo);

            vao = vbo = ebo = 0;
            GC.SuppressFinalize(this);
        }
    }
