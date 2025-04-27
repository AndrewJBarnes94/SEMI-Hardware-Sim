using PT_Sim.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class CLPCassettePositions
{
    private CLP _al;
    private CLP _bl;

    public CLPCassettePositions(CLP al, CLP bl)
    {
        _al = al;
        _bl = bl;
    }

    public CLPCassette GetALCassettePlatform()
    {
        float x1 = _al.GetPositionMap("A70")[0];
        float y1 = _al.GetPositionMap("A70")[1];
        float x2 = _al.GetPositionMap("B70")[0];
        float y2 = _al.GetPositionMap("B70")[1];
        float x3 = _al.GetPositionMap("C70")[0];
        float y3 = _al.GetPositionMap("C70")[1];
        float x4 = _al.GetPositionMap("D70")[0];
        float y4 = _al.GetPositionMap("D70")[1];

        float distanceMoved = 0.02f;

        // Move each point towards its opposing point
        (float, float) x1y1 = Formulas.MovePointTowards(x1, y1, x4, y4, distanceMoved);
        (float, float) x2y2 = Formulas.MovePointTowards(x2, y2, x3, y3, distanceMoved);
        (float, float) x3y3 = Formulas.MovePointTowards(x3, y3, x2, y2, distanceMoved);
        (float, float) x4y4 = Formulas.MovePointTowards(x4, y4, x1, y1, distanceMoved);

        // Move top two points towards bottom points
        x1y1 = Formulas.MovePointTowards(x1y1.Item1, x1y1.Item2, x3y3.Item1, x3y3.Item2, distanceMoved);
        x2y2 = Formulas.MovePointTowards(x2y2.Item1, x2y2.Item2, x4y4.Item1, x4y4.Item2, distanceMoved);

        // Move bottom two points towards top points
        x3y3 = Formulas.MovePointTowards(x3y3.Item1, x3y3.Item2, x1y1.Item1, x1y1.Item2, distanceMoved*4);
        x4y4 = Formulas.MovePointTowards(x4y4.Item1, x4y4.Item2, x2y2.Item1, x2y2.Item2, distanceMoved*4);

        (float, float) lowRecMidpoint = Formulas.FindMiddlePoint(x3y3.Item1, x3y3.Item2, x4y4.Item1, x4y4.Item2);
        float sqEdgeDistance = Formulas.distance(x3y3.Item1, x3y3.Item2, x4y4.Item1, x4y4.Item2) / 4;
        float lowRecSlope = Formulas.slope(x3y3.Item1, x3y3.Item2, x4y4.Item1, x4y4.Item2);

        (float, float) sqTopLeft = Formulas.FindMiddlePoint(lowRecMidpoint.Item1, lowRecMidpoint.Item2, x3y3.Item1, x3y3.Item2);
        (float, float) sqTopRight = Formulas.FindMiddlePoint(lowRecMidpoint.Item1, lowRecMidpoint.Item2, x4y4.Item1, x4y4.Item2);
        (float, float) sqBottomLeft = Formulas.FindPerpendicularPoint_Vector(sqTopLeft.Item1, sqTopLeft.Item2, lowRecSlope, -sqEdgeDistance);
        (float, float) sqBottomRight = Formulas.FindPerpendicularPoint_Vector(sqTopRight.Item1, sqTopRight.Item2, lowRecSlope, -sqEdgeDistance);

        return new CLPCassette(
            1.0f,

            // Lower Rectangle
            x1y1.Item1, x1y1.Item2,
            x2y2.Item1, x2y2.Item2,
            x3y3.Item1, x3y3.Item2,
            x4y4.Item1, x4y4.Item2,

            // Lower Square
            sqTopLeft.Item1, sqTopLeft.Item2,
            sqTopRight.Item1, sqTopRight.Item2,
            sqBottomLeft.Item1, sqBottomLeft.Item2,
            sqBottomRight.Item1, sqBottomRight.Item2
            
        );
    }

    public CLPCassette GetBLCassettePlatform()
    {

        float x1 = _bl.GetPositionMap("A70")[0];
        float y1 = _bl.GetPositionMap("A70")[1];
        float x2 = _bl.GetPositionMap("B70")[0];
        float y2 = _bl.GetPositionMap("B70")[1];
        float x3 = _bl.GetPositionMap("C70")[0];
        float y3 = _bl.GetPositionMap("C70")[1];
        float x4 = _bl.GetPositionMap("D70")[0];
        float y4 = _bl.GetPositionMap("D70")[1];

        float distanceMoved = 0.02f;

        (float, float) x1y1 = Formulas.MovePointTowards(x1, y1, x4, y4, distanceMoved);
        (float, float) x2y2 = Formulas.MovePointTowards(x2, y2, x3, y3, distanceMoved);
        (float, float) x3y3 = Formulas.MovePointTowards(x3, y3, x2, y2, distanceMoved);
        (float, float) x4y4 = Formulas.MovePointTowards(x4, y4, x1, y1, distanceMoved);

        // Move top two points towards bottom points
        x1y1 = Formulas.MovePointTowards(x1y1.Item1, x1y1.Item2, x3y3.Item1, x3y3.Item2, distanceMoved);
        x2y2 = Formulas.MovePointTowards(x2y2.Item1, x2y2.Item2, x4y4.Item1, x4y4.Item2, distanceMoved);

        // Move bottom two points towards top points
        x3y3 = Formulas.MovePointTowards(x3y3.Item1, x3y3.Item2, x1y1.Item1, x1y1.Item2, distanceMoved*4);
        x4y4 = Formulas.MovePointTowards(x4y4.Item1, x4y4.Item2, x2y2.Item1, x2y2.Item2, distanceMoved*4);

        (float, float) lowRecMidpoint = Formulas.FindMiddlePoint(x3y3.Item1, x3y3.Item2, x4y4.Item1, x4y4.Item2);
        float sqEdgeDistance = Formulas.distance(x3y3.Item1, x3y3.Item2, x4y4.Item1, x4y4.Item2) / 4;
        float lowRecSlope = Formulas.slope(x3y3.Item1, x3y3.Item2, x4y4.Item1, x4y4.Item2);

        (float, float) sqTopLeft = Formulas.FindMiddlePoint(lowRecMidpoint.Item1, lowRecMidpoint.Item2, x3y3.Item1, x3y3.Item2);
        (float, float) sqTopRight = Formulas.FindMiddlePoint(lowRecMidpoint.Item1, lowRecMidpoint.Item2, x4y4.Item1, x4y4.Item2);
        (float, float) sqBottomLeft = Formulas.FindPerpendicularPoint_Vector(sqTopLeft.Item1, sqTopLeft.Item2, lowRecSlope, sqEdgeDistance);
        (float, float) sqBottomRight = Formulas.FindPerpendicularPoint_Vector(sqTopRight.Item1, sqTopRight.Item2, lowRecSlope, sqEdgeDistance);

        return new CLPCassette(
            1.0f,

            // Lower Rectangle
            x1y1.Item1, x1y1.Item2,
            x2y2.Item1, x2y2.Item2,
            x3y3.Item1, x3y3.Item2,
            x4y4.Item1, x4y4.Item2,

            // Lower Square
            sqTopLeft.Item1, sqTopLeft.Item2,
            sqTopRight.Item1, sqTopRight.Item2,
            sqBottomLeft.Item1, sqBottomLeft.Item2,
            sqBottomRight.Item1, sqBottomRight.Item2

        );
    }
}
