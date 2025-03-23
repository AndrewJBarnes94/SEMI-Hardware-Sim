using PT_Sim;
using PT_Sim.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProcessModulePositions
{
    private SlitValve _slitValve1;
    private SlitValve _slitValve2;
    private SlitValve _slitValve3;
    private SlitValve _slitValve4;

    private float _length;

    private float _gradient1;
    private float _gradient2;
    private float _gradient3;
    private float _gradient4;

    public ProcessModulePositions(
        SlitValve slitValve1,
        SlitValve slitValve2,
        SlitValve slitValve3,
        SlitValve slitValve4
    )
    {
        _slitValve1 = slitValve1;
        _slitValve2 = slitValve2;
        _slitValve3 = slitValve3;
        _slitValve4 = slitValve4;

        Initialize();
    }

    private void Initialize()
    {
        _length = Formulas.distance(
            _slitValve1.GetPositionMap("C")[0],
            _slitValve1.GetPositionMap("C")[1],
            _slitValve1.GetPositionMap("D")[0],
            _slitValve1.GetPositionMap("D")[1]
        );

        _gradient1 = Formulas.slope(
            _slitValve1.GetPositionMap("C")[0], _slitValve2.GetPositionMap("C")[1],
            _slitValve1.GetPositionMap("D")[0], _slitValve1.GetPositionMap("D")[1]
        );

        _gradient2 = Formulas.slope(
            _slitValve2.GetPositionMap("C")[0], _slitValve2.GetPositionMap("C")[1],
            _slitValve2.GetPositionMap("D")[0], _slitValve2.GetPositionMap("D")[1]
        );

        _gradient3 = Formulas.slope(
            _slitValve3.GetPositionMap("C")[0], _slitValve3.GetPositionMap("C")[1],
            _slitValve3.GetPositionMap("D")[0], _slitValve3.GetPositionMap("D")[1]
        );

        _gradient4 = Formulas.slope(
            _slitValve4.GetPositionMap("C")[0], _slitValve4.GetPositionMap("C")[1],
            _slitValve4.GetPositionMap("D")[0], _slitValve4.GetPositionMap("D")[1]
        );
    }

    public ProcessModule GetProcessModule1()
    {
        (float, float) pm1MidPoint = Formulas.FindMiddlePoint(
            _slitValve1.GetPositionMap("C")[0], _slitValve1.GetPositionMap("C")[1],
            _slitValve1.GetPositionMap("D")[0], _slitValve1.GetPositionMap("D")[1],
            _slitValve1.GetPositionMap("C")[0] - _length, _slitValve1.GetPositionMap("C")[1],
            _slitValve1.GetPositionMap("D")[0] - _length, _slitValve1.GetPositionMap("D")[1]
        );

        float waferPlatformRadius = _length / 3.32f;

        float pm1HalfCircGradient = Formulas.slope(
            _slitValve1.GetPositionMap("C")[0], _slitValve1.GetPositionMap("C")[1],
            _slitValve1.GetPositionMap("C")[0] -_length, _slitValve1.GetPositionMap("C")[1]
        );

        (float, float) waferPlatformCircPoint1 = Formulas.FindPerpendicularPoint_Vector(
            pm1MidPoint.Item1, pm1MidPoint.Item2,
            pm1HalfCircGradient, waferPlatformRadius
        );

        (float, float) waferPlatformCircPoint2 = Formulas.FindPerpendicularPoint_Vector(
            pm1MidPoint.Item1, pm1MidPoint.Item2,
            pm1HalfCircGradient, -waferPlatformRadius
        );

        (float, float) outerWallMidpoint1 = Formulas.FindMiddlePoint(
            _slitValve1.GetPositionMap("C")[0], _slitValve1.GetPositionMap("C")[1],
            _slitValve1.GetPositionMap("C")[0] - _length, _slitValve1.GetPositionMap("C")[1]
        );

        (float, float) outerWallMidpoint2 = Formulas.FindMiddlePoint(
            _slitValve1.GetPositionMap("D")[0], _slitValve1.GetPositionMap("D")[1],
            _slitValve1.GetPositionMap("D")[0] - _length, _slitValve1.GetPositionMap("D")[1]
        );

        (float, float) halfCircleA = Formulas.FindMiddlePoint(
            waferPlatformCircPoint1.Item1, waferPlatformCircPoint1.Item2,
            outerWallMidpoint1.Item1, outerWallMidpoint1.Item2
        );

        (float, float) halfCircleB = Formulas.FindMiddlePoint(
            waferPlatformCircPoint2.Item1, waferPlatformCircPoint2.Item2,
            outerWallMidpoint2.Item1, outerWallMidpoint2.Item2
        );

        float distance = Formulas.distance(
            waferPlatformCircPoint2.Item1, waferPlatformCircPoint2.Item2,
            outerWallMidpoint2.Item1, outerWallMidpoint2.Item2
        );

        return new ProcessModule(
            // Scale
            1.0f,

            // Outer Chamber
            _slitValve1.GetPositionMap("C")[0], _slitValve1.GetPositionMap("C")[1],
            _slitValve1.GetPositionMap("D")[0], _slitValve1.GetPositionMap("D")[1],
            _slitValve1.GetPositionMap("C")[0] - _length, _slitValve1.GetPositionMap("C")[1],
            _slitValve1.GetPositionMap("D")[0] - _length, _slitValve1.GetPositionMap("D")[1],

            // Wafer Platform
            pm1MidPoint, waferPlatformRadius,

            // Inner Chamber Half-Circle
            halfCircleA.Item1, halfCircleA.Item2,
            halfCircleB.Item1, halfCircleB.Item2,

            // Inner Chamber Rectangle
            halfCircleA.Item1, halfCircleA.Item2,
            halfCircleB.Item1, halfCircleB.Item2,
            _slitValve1.GetPositionMap("C")[0], _slitValve1.GetPositionMap("C")[1] - distance/2f,
            _slitValve1.GetPositionMap("D")[0], _slitValve1.GetPositionMap("D")[1] + distance/2f
        );
    }

    public ProcessModule GetProcessModule2()
    {
        (float, float) perpendicularPoint1 = Formulas.FindPerpendicularPoint_Vector(
            _slitValve2.GetPositionMap("C")[0], _slitValve2.GetPositionMap("C")[1],
            _gradient2, -_length
        );
        (float, float) perpendicularPoint2 = Formulas.FindPerpendicularPoint_Vector(
            _slitValve2.GetPositionMap("D")[0], _slitValve2.GetPositionMap("D")[1],
            _gradient2, -_length
        );

        (float, float) pm2MidPoint = Formulas.FindMiddlePoint(
            _slitValve2.GetPositionMap("C")[0], _slitValve2.GetPositionMap("C")[1],
            _slitValve2.GetPositionMap("D")[0], _slitValve2.GetPositionMap("D")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2
        );

        float waferPlatformRadius = _length / 3.32f;

        float pm2HalfCircGradient = Formulas.slope(
            _slitValve2.GetPositionMap("C")[0], _slitValve2.GetPositionMap("C")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2
        );

        (float, float) waferPlatformCircPoint1 = Formulas.FindPerpendicularPoint_Vector(
            pm2MidPoint.Item1, pm2MidPoint.Item2,
            pm2HalfCircGradient, waferPlatformRadius
        );

        (float, float) waferPlatformCircPoint2 = Formulas.FindPerpendicularPoint_Vector(
            pm2MidPoint.Item1, pm2MidPoint.Item2,
            pm2HalfCircGradient, -waferPlatformRadius
        );

        (float, float) outerWallMidpoint1 = Formulas.FindMiddlePoint(
            _slitValve2.GetPositionMap("C")[0], _slitValve2.GetPositionMap("C")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2
        );

        (float, float) outerWallMidpoint2 = Formulas.FindMiddlePoint(
            _slitValve2.GetPositionMap("D")[0], _slitValve2.GetPositionMap("D")[1],
            perpendicularPoint2.Item1, perpendicularPoint2.Item2
        );

        (float, float) halfCircleA = Formulas.FindMiddlePoint(
            waferPlatformCircPoint1.Item1, waferPlatformCircPoint1.Item2,
            outerWallMidpoint2.Item1, outerWallMidpoint2.Item2
        );

        (float, float) halfCircleB = Formulas.FindMiddlePoint(
            waferPlatformCircPoint2.Item1, waferPlatformCircPoint2.Item2,
            outerWallMidpoint1.Item1, outerWallMidpoint1.Item2
        );

        float distance1 = Formulas.distance(
            halfCircleA.Item1, halfCircleA.Item2,
            outerWallMidpoint2.Item1, outerWallMidpoint2.Item2
        );

        float distance2 = Formulas.distance(
            halfCircleB.Item1, halfCircleB.Item2,
            outerWallMidpoint1.Item1, outerWallMidpoint1.Item2
        );

        (float, float) innerRectangleA = Formulas.MovePointTowards(

            _slitValve2.GetPositionMap("C")[0], _slitValve2.GetPositionMap("C")[1],
            _slitValve2.GetPositionMap("D")[0], _slitValve2.GetPositionMap("D")[1],
            distance1
        );

        (float, float) innerRectangleB = Formulas.MovePointTowards(

            _slitValve2.GetPositionMap("D")[0], _slitValve2.GetPositionMap("D")[1],
            _slitValve2.GetPositionMap("C")[0], _slitValve2.GetPositionMap("C")[1],
            distance2
        );

        return new ProcessModule(
            1.0f,
            _slitValve2.GetPositionMap("C")[0], _slitValve2.GetPositionMap("C")[1],
            _slitValve2.GetPositionMap("D")[0], _slitValve2.GetPositionMap("D")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2,

            pm2MidPoint, waferPlatformRadius,

            halfCircleA.Item1, halfCircleA.Item2,
            halfCircleB.Item1, halfCircleB.Item2,

            halfCircleA.Item1, halfCircleA.Item2,
            halfCircleB.Item1, halfCircleB.Item2,
            innerRectangleB.Item1, innerRectangleB.Item2,
            innerRectangleA.Item1, innerRectangleA.Item2
        );
    }


    public ProcessModule GetProcessModule3()
    {
        (float, float) perpendicularPoint1 = Formulas.FindPerpendicularPoint_Vector(
            _slitValve3.GetPositionMap("C")[0], _slitValve3.GetPositionMap("C")[1],
            _gradient3, _length
        );
        (float, float) perpendicularPoint2 = Formulas.FindPerpendicularPoint_Vector(
            _slitValve3.GetPositionMap("D")[0], _slitValve3.GetPositionMap("D")[1],
            _gradient3, _length
        );

        (float, float) pm3MidPoint = Formulas.FindMiddlePoint(
            _slitValve3.GetPositionMap("C")[0], _slitValve3.GetPositionMap("C")[1],
            _slitValve3.GetPositionMap("D")[0], _slitValve3.GetPositionMap("D")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2
        );

        float waferPlatformRadius = _length / 3.32f;

        float pm3HalfCircGradient = Formulas.slope(
            _slitValve3.GetPositionMap("C")[0], _slitValve3.GetPositionMap("C")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2
        );

        (float, float) waferPlatformCircPoint1 = Formulas.FindPerpendicularPoint_Vector(
            pm3MidPoint.Item1, pm3MidPoint.Item2,
            pm3HalfCircGradient, waferPlatformRadius
        );

        (float, float) waferPlatformCircPoint2 = Formulas.FindPerpendicularPoint_Vector(
            pm3MidPoint.Item1, pm3MidPoint.Item2,
            pm3HalfCircGradient, -waferPlatformRadius
        );

        (float, float) outerWallMidpoint1 = Formulas.FindMiddlePoint(
            _slitValve3.GetPositionMap("C")[0], _slitValve3.GetPositionMap("C")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2
        );

        (float, float) outerWallMidpoint2 = Formulas.FindMiddlePoint(
            _slitValve3.GetPositionMap("D")[0], _slitValve3.GetPositionMap("D")[1],
            perpendicularPoint2.Item1, perpendicularPoint2.Item2
        );

        (float, float) halfCircleA = Formulas.FindMiddlePoint(
            waferPlatformCircPoint1.Item1, waferPlatformCircPoint1.Item2,
            outerWallMidpoint1.Item1, outerWallMidpoint1.Item2
        );

        (float, float) halfCircleB = Formulas.FindMiddlePoint(
            waferPlatformCircPoint2.Item1, waferPlatformCircPoint2.Item2,
            outerWallMidpoint2.Item1, outerWallMidpoint2.Item2
        );

        float distance1 = Formulas.distance(
            halfCircleA.Item1, halfCircleA.Item2,
            outerWallMidpoint2.Item1, outerWallMidpoint2.Item2
        );

        float distance2 = Formulas.distance(
            halfCircleB.Item1, halfCircleB.Item2,
            outerWallMidpoint1.Item1, outerWallMidpoint1.Item2
        );

        (float, float) innerRectangleA = Formulas.MovePointTowards(

            _slitValve3.GetPositionMap("C")[0], _slitValve3.GetPositionMap("C")[1],
            _slitValve3.GetPositionMap("D")[0], _slitValve3.GetPositionMap("D")[1],
            distance1
        );

        (float, float) innerRectangleB = Formulas.MovePointTowards(

            _slitValve3.GetPositionMap("D")[0], _slitValve3.GetPositionMap("D")[1],
            _slitValve3.GetPositionMap("C")[0], _slitValve3.GetPositionMap("C")[1],
            distance2
        );

        return new ProcessModule(
            1.0f,
            _slitValve3.GetPositionMap("C")[0], _slitValve3.GetPositionMap("C")[1],
            _slitValve3.GetPositionMap("D")[0], _slitValve3.GetPositionMap("D")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2,

            pm3MidPoint, waferPlatformRadius,

            halfCircleA.Item1, halfCircleA.Item2,
            halfCircleB.Item1, halfCircleB.Item2,

            halfCircleA.Item1, halfCircleA.Item2,
            halfCircleB.Item1, halfCircleB.Item2,
            innerRectangleB.Item1, innerRectangleB.Item2,
            innerRectangleA.Item1, innerRectangleA.Item2
        );
    }


    public ProcessModule GetProcessModule4()
    {
        (float, float) pm4MidPoint = Formulas.FindMiddlePoint(
            _slitValve4.GetPositionMap("C")[0], _slitValve4.GetPositionMap("C")[1],
            _slitValve4.GetPositionMap("D")[0], _slitValve4.GetPositionMap("D")[1],
            _slitValve4.GetPositionMap("C")[0] + _length, _slitValve4.GetPositionMap("C")[1],
            _slitValve4.GetPositionMap("D")[0] + _length, _slitValve4.GetPositionMap("D")[1]
        );

        float waferPlatformRadius = _length / 3.32f;

        float pm4HalfCircGradient = Formulas.slope(
            _slitValve4.GetPositionMap("C")[0], _slitValve4.GetPositionMap("C")[1],
            _slitValve4.GetPositionMap("C")[0] + _length, _slitValve4.GetPositionMap("C")[1]
        );

        (float, float) waferPlatformCircPoint1 = Formulas.FindPerpendicularPoint_Vector(
            pm4MidPoint.Item1, pm4MidPoint.Item2,
            pm4HalfCircGradient, waferPlatformRadius
        );

        (float, float) waferPlatformCircPoint2 = Formulas.FindPerpendicularPoint_Vector(
            pm4MidPoint.Item1, pm4MidPoint.Item2,
            pm4HalfCircGradient, -waferPlatformRadius
        );

        (float, float) outerWallMidpoint1 = Formulas.FindMiddlePoint(
            _slitValve4.GetPositionMap("C")[0], _slitValve4.GetPositionMap("C")[1],
            _slitValve4.GetPositionMap("C")[0] + _length, _slitValve4.GetPositionMap("C")[1]
        );

        (float, float) outerWallMidpoint2 = Formulas.FindMiddlePoint(
            _slitValve4.GetPositionMap("D")[0], _slitValve4.GetPositionMap("D")[1],
            _slitValve4.GetPositionMap("D")[0] + _length, _slitValve4.GetPositionMap("D")[1]
        );

        (float, float) halfCircleA = Formulas.FindMiddlePoint(
            waferPlatformCircPoint1.Item1, waferPlatformCircPoint1.Item2,
            outerWallMidpoint1.Item1, outerWallMidpoint1.Item2
        );

        (float, float) halfCircleB = Formulas.FindMiddlePoint(
            waferPlatformCircPoint2.Item1, waferPlatformCircPoint2.Item2,
            outerWallMidpoint2.Item1, outerWallMidpoint2.Item2
        );

        float distance = Formulas.distance(
            waferPlatformCircPoint2.Item1, waferPlatformCircPoint2.Item2,
            outerWallMidpoint2.Item1, outerWallMidpoint2.Item2
        );

        return new ProcessModule(
            1.0f,
            _slitValve4.GetPositionMap("C")[0], _slitValve4.GetPositionMap("C")[1],
            _slitValve4.GetPositionMap("D")[0], _slitValve4.GetPositionMap("D")[1],
            _slitValve4.GetPositionMap("C")[0] + _length, _slitValve4.GetPositionMap("C")[1],
            _slitValve4.GetPositionMap("D")[0] + _length, _slitValve4.GetPositionMap("D")[1],

            pm4MidPoint, waferPlatformRadius,
            
            halfCircleB.Item1, halfCircleB.Item2,
            halfCircleA.Item1, halfCircleA.Item2,

            halfCircleB.Item1, halfCircleB.Item2,
            halfCircleA.Item1, halfCircleA.Item2,
            _slitValve4.GetPositionMap("D")[0], _slitValve4.GetPositionMap("D")[1] + distance/2f,
            _slitValve4.GetPositionMap("C")[0], _slitValve4.GetPositionMap("C")[1] - distance / 2f
        );
    }
}
