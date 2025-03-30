using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_Sim.General.Geometry
{
    public class Circle
    {
        public float[] Vertices { get; private set; }
        public uint[] Indices { get; private set; }

        public Circle(float radius, int segments)
        {
            Vertices = new float[(segments + 1) * 2];
            Indices = new uint[segments * 3];

            Vertices[0] = 0;
            Vertices[1] = 0;

            for (int i = 0; i < segments; i++)
            {
                float theta = 2.0f * Formulas.PI * i / segments;
                Vertices[(i + 1) * 2] = radius * Formulas.Cos(theta);
                Vertices[(i + 1) * 2 + 1] = radius * Formulas.Sin(theta);

                Indices[i * 3] = 0;
                Indices[i * 3 + 1] = (uint)(i + 1);
                Indices[i * 3 + 2] = (uint)((i + 2) % (segments + 1));
            }
        }
    }

}
