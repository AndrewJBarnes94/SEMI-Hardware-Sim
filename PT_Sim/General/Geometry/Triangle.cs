using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_Sim.General.Geometry
{
    public class Triangle
    {
        public float[] Vertices { get; private set; }
        public uint[] Indices { get; private set; }

        public Triangle(float[] vertices)
        {
            if (vertices.Length != 6)
                throw new ArgumentException("A triangle must have 3 vertices (6 coordinates).");

            Vertices = vertices;
            Indices = new uint[] { 0, 1, 2 };
        }
    }

}
