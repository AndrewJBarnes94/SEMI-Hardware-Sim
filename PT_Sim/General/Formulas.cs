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

        public static (float, float) FindIntersectionPoint(float m1, float b1, float m2, float b2)
        {
            // Check if the lines are parallel
            if (m1 == m2)
            {
                throw new ArgumentException("The lines are parallel and do not intersect.");
            }

            // Calculate the intersection point
            float x = (b2 - b1) / (m1 - m2);
            float y = m1 * x + b1;

            return (x, y);
        }

        public static (float, float) FindMiddlePoint(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            float middleX = (x1 + x2 + x3 + x4) / 4;
            float middleY = (y1 + y2 + y3 + y4) / 4;

            return (middleX, middleY);
        }

        public static (float, float) FindMiddlePoint(float x1, float y1, float x2, float y2)
        {
            float middleX = (x1 + x2) / 2;
            float middleY = (y1 + y2) / 2;
            return (middleX, middleY);
        }

        public static (float, float)[] AdjustRectangle((float, float)[] points)
        {
            // Find leftmost and rightmost x-coordinates
            float leftX = float.MaxValue, rightX = float.MinValue;
            float topY = float.MinValue, bottomY = float.MaxValue;

            foreach (var point in points)
            {
                if (point.Item1 < leftX) leftX = point.Item1;
                if (point.Item1 > rightX) rightX = point.Item1;
                if (point.Item2 > topY) topY = point.Item2;
                if (point.Item2 < bottomY) bottomY = point.Item2;
            }

            // Compute width and height
            float originalWidth = rightX - leftX;
            float originalHeight = topY - bottomY;

            // Compute current area
            float originalArea = originalWidth * originalHeight;

            // Target area (half of original)
            float targetArea = originalArea / 2;

            // Compute new width
            float newWidth = targetArea / originalHeight;

            // Compute width reduction
            float widthReduction = originalWidth - newWidth;

            // 40% reduction from left, 60% from right
            float leftReduction = 0.4f * widthReduction;
            float rightReduction = 0.6f * widthReduction;

            // Adjust left and right x-coordinates
            float newLeftX = leftX + leftReduction;
            float newRightX = rightX - rightReduction;

            // Return the new rectangle coordinates
            return new (float, float)[]
            {
                (newRightX, topY),
                (newLeftX, bottomY),
                (newRightX, bottomY),
                (newLeftX, topY)
            };
        }

        public static (float, float) MovePointTowards(float x1, float y1, float x2, float y2, float distance)
        {
            // Calculate the direction vector
            float dx = x2 - x1;
            float dy = y2 - y1;

            // Calculate the length of the direction vector
            float length = (float)Math.Sqrt(dx * dx + dy * dy);

            // Normalize the direction vector
            float nx = dx / length;
            float ny = dy / length;

            // Scale the direction vector by the specified distance
            float scaledDx = nx * distance;
            float scaledDy = ny * distance;

            // Calculate the new point
            float newX = x1 + scaledDx;
            float newY = y1 + scaledDy;

            return (newX, newY);
        }

        public static (float, float, float, float, float, float, float, float) CreateSmallerRectangle(
     float ax, float ay,
     float bx, float by,
     float cx, float cy,
     float dx, float dy,
     float scaleFactor)
        {
            // Calculate the center point of the rectangle
            float centerX = (ax + bx + cx + dx) / 4;
            float centerY = (ay + by + cy + dy) / 4;

            // Find the minimum and maximum x and y coordinates
            float minX = Math.Min(Math.Min(ax, bx), Math.Min(cx, dx));
            float maxX = Math.Max(Math.Max(ax, bx), Math.Max(cx, dx));
            float minY = Math.Min(Math.Min(ay, by), Math.Min(cy, dy));
            float maxY = Math.Max(Math.Max(ay, by), Math.Max(cy, dy));

            // Calculate the width and height of the original rectangle
            float width = maxX - minX;
            float height = maxY - minY;

            // Calculate the new width and height based on the scale factor
            float newWidth = width * scaleFactor;
            float newHeight = height * scaleFactor;

            // Calculate the new rectangle points
            float newAx = centerX - newWidth / 2;
            float newAy = centerY - newHeight / 2;
            float newBx = centerX + newWidth / 2;
            float newBy = centerY - newHeight / 2;
            float newCx = centerX + newWidth / 2;
            float newCy = centerY + newHeight / 2;
            float newDx = centerX - newWidth / 2;
            float newDy = centerY + newHeight / 2;

            return (newAx, newAy, newBx, newBy, newCx, newCy, newDx, newDy);
        }

        public static (float, float, float, float, float, float, float, float) CreateScaledRectangle(
    float ax, float ay,
    float bx, float by,
    float cx, float cy,
    float dx, float dy,
    float scaleFactor)
        {
            // Calculate the direction vectors for each side of the rectangle
            float abx = bx - ax;
            float aby = by - ay;
            float bcx = cx - bx;
            float bcy = cy - by;
            float cdx = dx - cx;
            float cdy = dy - cy;
            float dax = ax - dx;
            float day = ay - dy;

            // Scale the direction vectors
            abx *= scaleFactor;
            aby *= scaleFactor;
            bcx *= scaleFactor;
            bcy *= scaleFactor;
            cdx *= scaleFactor;
            cdy *= scaleFactor;
            dax *= scaleFactor;
            day *= scaleFactor;

            // Calculate the new positions of the vertices
            float newAx = ax + dax;
            float newAy = ay + day;
            float newBx = bx + abx;
            float newBy = by + aby;
            float newCx = cx + bcx;
            float newCy = cy + bcy;
            float newDx = dx + cdx;
            float newDy = dy + cdy;

            return (newAx, newAy, newBx, newBy, newCx, newCy, newDx, newDy);
        }






    }
}
