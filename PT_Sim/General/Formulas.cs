using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_Sim.General
{
    class Formulas
    {
        public static float distance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        public static float pythagorean(float a, float b)
        {
            return (float)Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
        }

        public static float slope(float x1, float y1, float x2, float y2)
        {
            return (y2 - y1) / (x2 - x1);
        }

        public static float yIntercept(float x1, float y1, float m)
        {
            return y1 - m * x1;
        }
        
        public static (float, float) FindPerpendicularPoint_Vector(float x0, float y0, float m, float d)
        {
            // Perpendicular slope (negative reciprocal)
            float m_perp = -1f / m;

            // Compute dx and dy using vector formula
            float dx = (float)(d / Math.Sqrt(1 + (m_perp * m_perp)));
            float dy = m_perp * dx;

            // Compute new point
            float x1 = x0 + dx;
            float y1 = y0 + dy;

            return (x1, y1);
        }


    }
}
