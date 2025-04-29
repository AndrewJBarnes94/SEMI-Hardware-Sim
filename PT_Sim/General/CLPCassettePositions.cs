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
        (float, float) x1y1 = Formulas.MovePointTowards(x1, y1, x4, y4, distanceMoved * 1.5f);
        (float, float) x2y2 = Formulas.MovePointTowards(x2, y2, x3, y3, distanceMoved * 1.5f);
        (float, float) x3y3 = Formulas.MovePointTowards(x3, y3, x2, y2, distanceMoved * 1.5f);
        (float, float) x4y4 = Formulas.MovePointTowards(x4, y4, x1, y1, distanceMoved * 1.5f);

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
        (float, float) sqBottomLeft = Formulas.FindPerpendicularPoint_Vector(sqTopLeft.Item1, sqTopLeft.Item2, lowRecSlope, -sqEdgeDistance * 1.5f);
        (float, float) sqBottomRight = Formulas.FindPerpendicularPoint_Vector(sqTopRight.Item1, sqTopRight.Item2, lowRecSlope, -sqEdgeDistance * 1.5f);

        sqTopLeft = Formulas.MovePointTowards(sqTopLeft.Item1, sqTopLeft.Item2, sqTopRight.Item1, sqTopRight.Item2, distanceMoved * 0.7f);
        sqTopRight = Formulas.MovePointTowards(sqTopRight.Item1, sqTopRight.Item2, sqTopLeft.Item1, sqTopLeft.Item2, distanceMoved * 0.7f);
        sqBottomLeft = Formulas.MovePointTowards(sqBottomLeft.Item1, sqBottomLeft.Item2, sqBottomRight.Item1, sqBottomRight.Item2, distanceMoved * 0.7f);
        sqBottomRight = Formulas.MovePointTowards(sqBottomRight.Item1, sqBottomRight.Item2, sqBottomLeft.Item1, sqBottomLeft.Item2, distanceMoved * 0.7f);

        (float, float) upperRectangleTopLeft = Formulas.MovePointTowards(x1y1.Item1, x1y1.Item2, x3y3.Item1, x3y3.Item2, distanceMoved * 2);
        (float, float) upperRectangleTopRight = Formulas.MovePointTowards(x2y2.Item1, x2y2.Item2, x4y4.Item1, x4y4.Item2, distanceMoved * 2);
        (float, float) upperRectangleBottomLeft = Formulas.MovePointTowards(x3y3.Item1, x3y3.Item2, x1y1.Item1, x1y1.Item2, distanceMoved * -1);
        (float, float) upperRectangleBottomRight = Formulas.MovePointTowards(x4y4.Item1, x4y4.Item2, x2y2.Item1, x2y2.Item2, distanceMoved * -1);

        // Shared bottom left of upper rectangle
        (float, float) upperTri1Top = sqTopLeft;
        (float, float) upperTri1Bottom = Formulas.MovePointTowards(sqBottomLeft.Item1, sqBottomLeft.Item2, sqTopLeft.Item1, sqTopLeft.Item2, distanceMoved * -0.8f);

        // Shared bottom right of upper rectangle
        (float, float) upperTri2Top = sqTopRight;
        (float, float) upperTri2Bottom = Formulas.MovePointTowards(sqBottomRight.Item1, sqBottomRight.Item2, sqTopRight.Item1, sqTopRight.Item2, distanceMoved * -0.8f);

        float upperRecSlope = Formulas.slope(upperRectangleTopLeft.Item1, upperRectangleTopLeft.Item2, upperRectangleTopRight.Item1, upperRectangleTopRight.Item2);

        // Upper small rectangle1
        (float, float) upperSmallRec1BottomLeft = upperRectangleTopLeft;
        (float, float) upperSmallRec1BottomRight = Formulas.MovePointTowards(upperRectangleTopLeft.Item1, upperRectangleTopLeft.Item2, upperRectangleTopRight.Item1, upperRectangleTopRight.Item2, distanceMoved);
        (float, float) upperSmallRec1TopLeft = Formulas.MovePointTowards(upperSmallRec1BottomLeft.Item1, upperSmallRec1BottomLeft.Item2, x1y1.Item1, x1y1.Item2, distanceMoved * 2.5f);
        (float, float) upperSmallRec1TopRight = Formulas.FindPerpendicularPoint_Vector(upperSmallRec1BottomRight.Item1, upperSmallRec1BottomRight.Item2, upperRecSlope, distanceMoved * 2.5f);

        // Upper small rectangle2
        (float, float) upperSmallRec2BottomRight = upperRectangleTopRight;
        (float, float) upperSmallRec2BottomLeft = Formulas.MovePointTowards(upperRectangleTopRight.Item1, upperRectangleTopRight.Item2, upperRectangleTopLeft.Item1, upperRectangleTopLeft.Item2, distanceMoved);
        (float, float) upperSmallRec2TopRight = Formulas.MovePointTowards(upperSmallRec2BottomRight.Item1, upperSmallRec2BottomRight.Item2, x2y2.Item1, x2y2.Item2, distanceMoved * 2.5f);
        (float, float) upperSmallRec2TopLeft = Formulas.FindPerpendicularPoint_Vector(upperSmallRec2BottomLeft.Item1, upperSmallRec2BottomLeft.Item2, upperRecSlope, distanceMoved * 2.5f);

        float smallRec1Slope = Formulas.slope(upperSmallRec1BottomRight.Item1, upperSmallRec1BottomRight.Item2, upperSmallRec1TopRight.Item1, upperSmallRec1TopRight.Item2);

        // upper irregular trapezoid1
        (float, float) upperIrregularTrap1BottomLeft = upperSmallRec1BottomRight;
        (float, float) upperIrregularTrap1TopLeft = Formulas.MovePointTowards(upperIrregularTrap1BottomLeft.Item1, upperIrregularTrap1BottomLeft.Item2, upperSmallRec1TopRight.Item1, upperSmallRec1TopRight.Item2, distanceMoved * 1.5f);
        (float, float) upperIrregularTrap1BottomRight = Formulas.MovePointTowards(upperIrregularTrap1BottomLeft.Item1, upperIrregularTrap1BottomLeft.Item2, upperRectangleTopRight.Item1, upperRectangleTopRight.Item2, distanceMoved * 1.5f);
        (float, float) upperIrregularTrap1TopRight = Formulas.FindPerpendicularPoint_Vector(upperIrregularTrap1TopLeft.Item1, upperIrregularTrap1TopLeft.Item2, smallRec1Slope, distanceMoved * 1.0f);

        float smallRec2Slope = Formulas.slope(upperSmallRec2BottomRight.Item1, upperSmallRec2BottomRight.Item2, upperSmallRec2TopRight.Item1, upperSmallRec2TopRight.Item2);

        // upper irregular trapezoid1
        (float, float) upperIrregularTrap2BottomRight = upperSmallRec2BottomLeft;
        (float, float) upperIrregularTrap2TopRight = Formulas.MovePointTowards(upperIrregularTrap2BottomRight.Item1, upperIrregularTrap2BottomRight.Item2, upperSmallRec2TopLeft.Item1, upperSmallRec2TopLeft.Item2, distanceMoved * 1.5f);
        (float, float) upperIrregularTrap2BottomLeft = Formulas.MovePointTowards(upperIrregularTrap2BottomRight.Item1, upperIrregularTrap2BottomRight.Item2, upperRectangleTopLeft.Item1, upperRectangleTopLeft.Item2, distanceMoved * 1.5f);
        (float, float) upperIrregularTrap2TopLeft = Formulas.FindPerpendicularPoint_Vector(upperIrregularTrap2TopRight.Item1, upperIrregularTrap2TopRight.Item2, smallRec1Slope, -distanceMoved * 1.0f);

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
            sqBottomRight.Item1, sqBottomRight.Item2,

            // Upper Rectangle
            upperRectangleTopLeft.Item1, upperRectangleTopLeft.Item2,
            upperRectangleTopRight.Item1, upperRectangleTopRight.Item2,
            upperRectangleBottomLeft.Item1, upperRectangleBottomLeft.Item2,
            upperRectangleBottomRight.Item1, upperRectangleBottomRight.Item2,

            // Upper Triangle 1
            upperTri1Top.Item1, upperTri1Top.Item2,
            upperTri1Bottom.Item1, upperTri1Bottom.Item2,
            upperRectangleBottomLeft.Item1, upperRectangleBottomLeft.Item2,

            // Upper Triangle 2
            upperTri2Top.Item1, upperTri2Top.Item2,
            upperTri2Bottom.Item1, upperTri2Bottom.Item2,
            upperRectangleBottomRight.Item1, upperRectangleBottomRight.Item2,

            // Upper Small Rectangle 1
            upperSmallRec1BottomLeft.Item1, upperSmallRec1BottomLeft.Item2,
            upperSmallRec1BottomRight.Item1, upperSmallRec1BottomRight.Item2,
            upperSmallRec1TopLeft.Item1, upperSmallRec1TopLeft.Item2,
            upperSmallRec1TopRight.Item1, upperSmallRec1TopRight.Item2,

            // Upper Small Rectangle 2
            upperSmallRec2BottomLeft.Item1, upperSmallRec2BottomLeft.Item2,
            upperSmallRec2BottomRight.Item1, upperSmallRec2BottomRight.Item2,
            upperSmallRec2TopLeft.Item1, upperSmallRec2TopLeft.Item2,
            upperSmallRec2TopRight.Item1, upperSmallRec2TopRight.Item2,

            // Upper Irregular Trapezoid 1
            upperIrregularTrap1BottomLeft.Item1, upperIrregularTrap1BottomLeft.Item2,
            upperIrregularTrap1BottomRight.Item1, upperIrregularTrap1BottomRight.Item2,
            upperIrregularTrap1TopLeft.Item1, upperIrregularTrap1TopLeft.Item2,
            upperIrregularTrap1TopRight.Item1, upperIrregularTrap1TopRight.Item2,

            // Upper Irregular Trapezoid 2
            upperIrregularTrap2BottomLeft.Item1, upperIrregularTrap2BottomLeft.Item2,
            upperIrregularTrap2BottomRight.Item1, upperIrregularTrap2BottomRight.Item2,
            upperIrregularTrap2TopLeft.Item1, upperIrregularTrap2TopLeft.Item2,
            upperIrregularTrap2TopRight.Item1, upperIrregularTrap2TopRight.Item2
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

        (float, float) x1y1 = Formulas.MovePointTowards(x1, y1, x4, y4, distanceMoved * 1.5f);
        (float, float) x2y2 = Formulas.MovePointTowards(x2, y2, x3, y3, distanceMoved * 1.5f);
        (float, float) x3y3 = Formulas.MovePointTowards(x3, y3, x2, y2, distanceMoved * 1.5f);
        (float, float) x4y4 = Formulas.MovePointTowards(x4, y4, x1, y1, distanceMoved * 1.5f);

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
        (float, float) sqBottomLeft = Formulas.FindPerpendicularPoint_Vector(sqTopLeft.Item1, sqTopLeft.Item2, lowRecSlope, sqEdgeDistance * 1.5f);
        (float, float) sqBottomRight = Formulas.FindPerpendicularPoint_Vector(sqTopRight.Item1, sqTopRight.Item2, lowRecSlope, sqEdgeDistance * 1.5f);

        sqTopLeft = Formulas.MovePointTowards(sqTopLeft.Item1, sqTopLeft.Item2, sqTopRight.Item1, sqTopRight.Item2, distanceMoved * 0.7f);
        sqTopRight = Formulas.MovePointTowards(sqTopRight.Item1, sqTopRight.Item2, sqTopLeft.Item1, sqTopLeft.Item2, distanceMoved * 0.7f);
        sqBottomLeft = Formulas.MovePointTowards(sqBottomLeft.Item1, sqBottomLeft.Item2, sqBottomRight.Item1, sqBottomRight.Item2, distanceMoved * 0.7f);
        sqBottomRight = Formulas.MovePointTowards(sqBottomRight.Item1, sqBottomRight.Item2, sqBottomLeft.Item1, sqBottomLeft.Item2, distanceMoved * 0.7f);

        (float, float) upperRectangleTopLeft = Formulas.MovePointTowards(x1y1.Item1, x1y1.Item2, x3y3.Item1, x3y3.Item2, distanceMoved * 2);
        (float, float) upperRectangleTopRight = Formulas.MovePointTowards(x2y2.Item1, x2y2.Item2, x4y4.Item1, x4y4.Item2, distanceMoved * 2);
        (float, float) upperRectangleBottomLeft = Formulas.MovePointTowards(x3y3.Item1, x3y3.Item2, x1y1.Item1, x1y1.Item2, distanceMoved * -1);
        (float, float) upperRectangleBottomRight = Formulas.MovePointTowards(x4y4.Item1, x4y4.Item2, x2y2.Item1, x2y2.Item2, distanceMoved * -1);

        // Shared bottom left of upper rectangle
        (float, float) upperTri1Top = sqTopLeft;
        (float, float) upperTri1Bottom = Formulas.MovePointTowards(sqBottomLeft.Item1, sqBottomLeft.Item2, sqTopLeft.Item1, sqTopLeft.Item2, distanceMoved * -0.8f);

        // Shared bottom right of upper rectangle
        (float, float) upperTri2Top = sqTopRight;
        (float, float) upperTri2Bottom = Formulas.MovePointTowards(sqBottomRight.Item1, sqBottomRight.Item2, sqTopRight.Item1, sqTopRight.Item2, distanceMoved * -0.8f);

        float upperRecSlope = Formulas.slope(upperRectangleTopLeft.Item1, upperRectangleTopLeft.Item2, upperRectangleTopRight.Item1, upperRectangleTopRight.Item2);

        // Upper small rectangle1
        (float, float) upperSmallRec1BottomLeft = upperRectangleTopLeft;
        (float, float) upperSmallRec1BottomRight = Formulas.MovePointTowards(upperRectangleTopLeft.Item1, upperRectangleTopLeft.Item2, upperRectangleTopRight.Item1, upperRectangleTopRight.Item2, distanceMoved);
        (float, float) upperSmallRec1TopLeft = Formulas.MovePointTowards(upperSmallRec1BottomLeft.Item1, upperSmallRec1BottomLeft.Item2, x1y1.Item1, x1y1.Item2, distanceMoved * 2.5f);
        (float, float) upperSmallRec1TopRight = Formulas.FindPerpendicularPoint_Vector(upperSmallRec1BottomRight.Item1, upperSmallRec1BottomRight.Item2, upperRecSlope, -distanceMoved * 2.5f);

        // Upper small rectangle2
        (float, float) upperSmallRec2BottomRight = upperRectangleTopRight;
        (float, float) upperSmallRec2BottomLeft = Formulas.MovePointTowards(upperRectangleTopRight.Item1, upperRectangleTopRight.Item2, upperRectangleTopLeft.Item1, upperRectangleTopLeft.Item2, distanceMoved);
        (float, float) upperSmallRec2TopRight = Formulas.MovePointTowards(upperSmallRec2BottomRight.Item1, upperSmallRec2BottomRight.Item2, x2y2.Item1, x2y2.Item2, distanceMoved * 2.5f);
        (float, float) upperSmallRec2TopLeft = Formulas.FindPerpendicularPoint_Vector(upperSmallRec2BottomLeft.Item1, upperSmallRec2BottomLeft.Item2, upperRecSlope, -distanceMoved * 2.5f);

        float smallRec1Slope = Formulas.slope(upperSmallRec1BottomRight.Item1, upperSmallRec1BottomRight.Item2, upperSmallRec1TopRight.Item1, upperSmallRec1TopRight.Item2);

        // upper irregular trapezoid1
        (float, float) upperIrregularTrap1BottomLeft = upperSmallRec1BottomRight;
        (float, float) upperIrregularTrap1TopLeft = Formulas.MovePointTowards(upperIrregularTrap1BottomLeft.Item1, upperIrregularTrap1BottomLeft.Item2, upperSmallRec1TopRight.Item1, upperSmallRec1TopRight.Item2, distanceMoved * 1.5f);
        (float, float) upperIrregularTrap1BottomRight = Formulas.MovePointTowards(upperIrregularTrap1BottomLeft.Item1, upperIrregularTrap1BottomLeft.Item2, upperRectangleTopRight.Item1, upperRectangleTopRight.Item2, distanceMoved * 1.5f);
        (float, float) upperIrregularTrap1TopRight = Formulas.FindPerpendicularPoint_Vector(upperIrregularTrap1TopLeft.Item1, upperIrregularTrap1TopLeft.Item2, smallRec1Slope, -distanceMoved * 1.0f);

        float smallRec2Slope = Formulas.slope(upperSmallRec2BottomRight.Item1, upperSmallRec2BottomRight.Item2, upperSmallRec2TopRight.Item1, upperSmallRec2TopRight.Item2);

        // upper irregular trapezoid1
        (float, float) upperIrregularTrap2BottomRight = upperSmallRec2BottomLeft;
        (float, float) upperIrregularTrap2TopRight = Formulas.MovePointTowards(upperIrregularTrap2BottomRight.Item1, upperIrregularTrap2BottomRight.Item2, upperSmallRec2TopLeft.Item1, upperSmallRec2TopLeft.Item2, distanceMoved * 1.5f);
        (float, float) upperIrregularTrap2BottomLeft = Formulas.MovePointTowards(upperIrregularTrap2BottomRight.Item1, upperIrregularTrap2BottomRight.Item2, upperRectangleTopLeft.Item1, upperRectangleTopLeft.Item2, distanceMoved * 1.5f);
        (float, float) upperIrregularTrap2TopLeft = Formulas.FindPerpendicularPoint_Vector(upperIrregularTrap2TopRight.Item1, upperIrregularTrap2TopRight.Item2, smallRec1Slope, distanceMoved * 1.0f);

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
            sqBottomRight.Item1, sqBottomRight.Item2,

            // Upper Rectangle
            upperRectangleTopLeft.Item1, upperRectangleTopLeft.Item2,
            upperRectangleTopRight.Item1, upperRectangleTopRight.Item2,
            upperRectangleBottomLeft.Item1, upperRectangleBottomLeft.Item2,
            upperRectangleBottomRight.Item1, upperRectangleBottomRight.Item2,

            // Upper Triangle 1
            upperTri1Top.Item1, upperTri1Top.Item2,
            upperTri1Bottom.Item1, upperTri1Bottom.Item2,
            upperRectangleBottomLeft.Item1, upperRectangleBottomLeft.Item2,

            // Upper Triangle 2
            upperTri2Top.Item1, upperTri2Top.Item2,
            upperTri2Bottom.Item1, upperTri2Bottom.Item2,
            upperRectangleBottomRight.Item1, upperRectangleBottomRight.Item2,

            // Upper Small Rectangle 1
            upperSmallRec1BottomLeft.Item1, upperSmallRec1BottomLeft.Item2,
            upperSmallRec1BottomRight.Item1, upperSmallRec1BottomRight.Item2,
            upperSmallRec1TopLeft.Item1, upperSmallRec1TopLeft.Item2,
            upperSmallRec1TopRight.Item1, upperSmallRec1TopRight.Item2,

            // Upper Small Rectangle 2
            upperSmallRec2BottomLeft.Item1, upperSmallRec2BottomLeft.Item2,
            upperSmallRec2BottomRight.Item1, upperSmallRec2BottomRight.Item2,
            upperSmallRec2TopLeft.Item1, upperSmallRec2TopLeft.Item2,
            upperSmallRec2TopRight.Item1, upperSmallRec2TopRight.Item2,

            // Upper Irregular Trapezoid 1
            upperIrregularTrap1BottomLeft.Item1, upperIrregularTrap1BottomLeft.Item2,
            upperIrregularTrap1BottomRight.Item1, upperIrregularTrap1BottomRight.Item2,
            upperIrregularTrap1TopLeft.Item1, upperIrregularTrap1TopLeft.Item2,
            upperIrregularTrap1TopRight.Item1, upperIrregularTrap1TopRight.Item2,

            // Upper Irregular Trapezoid 2
            upperIrregularTrap2BottomLeft.Item1, upperIrregularTrap2BottomLeft.Item2,
            upperIrregularTrap2BottomRight.Item1, upperIrregularTrap2BottomRight.Item2,
            upperIrregularTrap2TopLeft.Item1, upperIrregularTrap2TopLeft.Item2,
            upperIrregularTrap2TopRight.Item1, upperIrregularTrap2TopRight.Item2
        );
    }
}
