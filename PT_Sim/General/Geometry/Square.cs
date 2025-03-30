using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_Sim.General.Geometry
{
    public class Square
    {
        public float[] Vertices { get; private set; }
        public uint[] Indices { get; private set; }

        public Square(float size)
        {
            Vertices = new float[]
            {
            size, size,   // Top right
            size, -size,  // Bottom right
            -size, -size, // Bottom left
            -size, size   // Top left
            };

            Indices = new uint[] { 0, 1, 2, 0, 2, 3 };
        }
    }

}
