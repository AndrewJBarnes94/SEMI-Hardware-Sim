using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PT_Sim;
using PT_Sim.General;

class HA75AlignerPositions
{
    private HA600TMChamber _chamber;

    private float _length;

    private float _gradient1;
    private float _gradient2;

    public HA75AlignerPositions(
        HA600TMChamber chamber
    )
    {
        _chamber = chamber;
        Initialize();
    }

    private void Initialize()
    {
        _length = Formulas.distance(
            _chamber.GetPositionMap("bottomRight")[0],
            _chamber.GetPositionMap("bottomRight")[1],
            _chamber.GetPositionMap("bottom")[0],
            _chamber.GetPositionMap("bottom")[1]
        );

        _length = _length / 3;

        _gradient1 = Formulas.slope(
            _chamber.GetPositionMap("bottom")[0], _chamber.GetPositionMap("bottom")[1],
            _chamber.GetPositionMap("bottomRight")[0], _chamber.GetPositionMap("bottomRight")[1]
        );
    }

    public HA75Aligner GetHA75Aligner()
    {
        (float, float) perpendicularPoint1 = Formulas.FindPerpendicularPoint_Vector(
            _chamber.GetPositionMap("bottomRight")[0], _chamber.GetPositionMap("bottomRight")[1],
            _gradient1, -_length * 0.5f
        );

        (float, float) perpendicularPoint2 = Formulas.FindPerpendicularPoint_Vector(
            _chamber.GetPositionMap("bottom")[0], _chamber.GetPositionMap("bottom")[1],
            _gradient1, -_length * 0.5f
        );

        (float, float) perpendicularPoint3 = Formulas.FindPerpendicularPoint_Vector(
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            _gradient1, -_length / 2f
        );

        (float, float) perpendicularPoint4 = Formulas.FindPerpendicularPoint_Vector(
            perpendicularPoint2.Item1, perpendicularPoint2.Item2,
            _gradient1, -_length / 2f
        );

        (float, float) midpoint = Formulas.FindMiddlePoint(
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2,
            perpendicularPoint3.Item1, perpendicularPoint3.Item2,
            perpendicularPoint4.Item1, perpendicularPoint4.Item2
        );

        Logger.Log("1", perpendicularPoint1);
        Logger.Log("2", perpendicularPoint2);
        Logger.Log("3", perpendicularPoint3);
        Logger.Log("4", perpendicularPoint4);

        (float, float) adjustedPerpendicularPoint1 = Formulas.MovePointTowards(
            perpendicularPoint4.Item1, perpendicularPoint4.Item2,
            perpendicularPoint3.Item1, perpendicularPoint3.Item2,
            _length * 2.5f // Move 10% of the length
        );

        // Move perpendicularPoint1 slightly closer to perpendicularPoint3
        (float, float) adjustedPerpendicularPoint2 = Formulas.MovePointTowards(
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2,
            _length * 1.8f // Move 10% of the length
        );

        (float, float) adjustedPerpendicularPoint3 = Formulas.MovePointTowards(
            perpendicularPoint2.Item1, perpendicularPoint2.Item2,
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            _length * 2.5f // Move 10% of the length
        );

        (float, float) adjustedPerpendicularPoint4 = Formulas.MovePointTowards(
            perpendicularPoint3.Item1, perpendicularPoint3.Item2,
            perpendicularPoint4.Item1, perpendicularPoint4.Item2,
            _length * 1.8f // Move 10% of the length
        );

        Logger.Log("1", adjustedPerpendicularPoint1);
        Logger.Log("2", adjustedPerpendicularPoint2);
        Logger.Log("3", adjustedPerpendicularPoint3);
        Logger.Log("4", adjustedPerpendicularPoint4);

        (float, float) outerSensorA = Formulas.MovePointTowards(
            adjustedPerpendicularPoint1.Item1, adjustedPerpendicularPoint1.Item2,
            adjustedPerpendicularPoint3.Item1, adjustedPerpendicularPoint3.Item2,
            _length * 0.1f // Move 10% of the length
        );

        (float, float) adjustedPerpendicularPoint6 = Formulas.MovePointTowards(
            adjustedPerpendicularPoint2.Item1, adjustedPerpendicularPoint2.Item2,
            adjustedPerpendicularPoint4.Item1, adjustedPerpendicularPoint4.Item2,
            _length * 0.1f // Move 10% of the length
        );

        (float, float) outerSensorB = Formulas.MovePointTowards(
            adjustedPerpendicularPoint3.Item1, adjustedPerpendicularPoint3.Item2,
            adjustedPerpendicularPoint1.Item1, adjustedPerpendicularPoint1.Item2,
            _length * 0.1f // Move 10% of the length
        );

        (float, float) adjustedPerpendicularPoint8 = Formulas.MovePointTowards(
            adjustedPerpendicularPoint4.Item1, adjustedPerpendicularPoint4.Item2,
            adjustedPerpendicularPoint2.Item1, adjustedPerpendicularPoint2.Item2,
            _length * 0.1f // Move 10% of the length
        );

        (float, float) outerSensorC = Formulas.MovePointTowards(
            adjustedPerpendicularPoint8.Item1, adjustedPerpendicularPoint8.Item2,
            outerSensorA.Item1, outerSensorA.Item2,
            _length * 0.7f // Move 10% of the length
        );

        (float, float) outerSensorD = Formulas.MovePointTowards(
            adjustedPerpendicularPoint6.Item1, adjustedPerpendicularPoint6.Item2,
            outerSensorB.Item1, outerSensorB.Item2,
            _length * 0.7f // Move 10% of the length
        );

        // Define chuck positions
        float chuckCenterX = midpoint.Item1;
        float chuckCenterY = midpoint.Item2;
        float chuckRadius = _length / 6;

        // Define half circle points
        float halfCircle1StartX = adjustedPerpendicularPoint4.Item1;
        float halfCircle1StartY = adjustedPerpendicularPoint4.Item2;
        float halfCircle1EndX = adjustedPerpendicularPoint2.Item1;
        float halfCircle1EndY = adjustedPerpendicularPoint2.Item2;

        float halfCircle2StartX = adjustedPerpendicularPoint3.Item1;
        float halfCircle2StartY = adjustedPerpendicularPoint3.Item2;
        float halfCircle2EndX = adjustedPerpendicularPoint1.Item1;
        float halfCircle2EndY = adjustedPerpendicularPoint1.Item2;

        (float, float) sensorMidpoint = Formulas.FindMiddlePoint(
            outerSensorA.Item1, outerSensorA.Item2,
            outerSensorB.Item1, outerSensorB.Item2,
            outerSensorC.Item1, outerSensorC.Item2,
            outerSensorD.Item1, outerSensorD.Item2
            );

        //(float, float) innerSensorA = Formulas.FindMiddlePoint(sensorMidpoint.Item1, sensorMidpoint.Item2, outerSensorA.Item1, outerSensorA.Item2);
        //(float, float) innerSensorB = Formulas.FindMiddlePoint(sensorMidpoint.Item1, sensorMidpoint.Item2, outerSensorB.Item1, outerSensorB.Item2);
        //(float, float) innerSensorC = Formulas.FindMiddlePoint(sensorMidpoint.Item1, sensorMidpoint.Item2, outerSensorC.Item1, outerSensorC.Item2);
        //(float, float) innerSensorD = Formulas.FindMiddlePoint(sensorMidpoint.Item1, sensorMidpoint.Item2, outerSensorD.Item1, outerSensorD.Item2);

        (float, float) innerSensorA = Formulas.MovePointTowards(
            outerSensorA.Item1, outerSensorA.Item2,
            sensorMidpoint.Item1, sensorMidpoint.Item2,
            _length * 0.15f
        );

        (float, float) innerSensorB = Formulas.MovePointTowards(
            outerSensorB.Item1, outerSensorB.Item2,
            sensorMidpoint.Item1, sensorMidpoint.Item2,
            _length * 0.15f
        );

        (float, float) innerSensorC = Formulas.MovePointTowards(
            outerSensorC.Item1, outerSensorC.Item2,
            sensorMidpoint.Item1, sensorMidpoint.Item2,
            _length * 0.15f
        );

        (float, float) innerSensorD = Formulas.MovePointTowards(
            outerSensorD.Item1, outerSensorD.Item2,
            sensorMidpoint.Item1, sensorMidpoint.Item2,
            _length * 0.15f
        );

        return new HA75Aligner(
            1.0f,

            // Main rectangular body
            adjustedPerpendicularPoint3.Item1, adjustedPerpendicularPoint3.Item2,
            adjustedPerpendicularPoint2.Item1, adjustedPerpendicularPoint2.Item2,
            adjustedPerpendicularPoint1.Item1, adjustedPerpendicularPoint1.Item2,
            adjustedPerpendicularPoint4.Item1, adjustedPerpendicularPoint4.Item2,

            // Chuck
            chuckCenterX, chuckCenterY, chuckRadius,

            // Half circles
            halfCircle1StartX, halfCircle1StartY,
            halfCircle1EndX, halfCircle1EndY,
            halfCircle2StartX, halfCircle2StartY,
            halfCircle2EndX, halfCircle2EndY,

            // Outer Sensor
            outerSensorA.Item1, outerSensorA.Item2,
            outerSensorB.Item1, outerSensorB.Item2,
            outerSensorC.Item1, outerSensorC.Item2,
            outerSensorD.Item1, outerSensorD.Item2,

            // Inner Sensor
            innerSensorA.Item1, innerSensorA.Item2,
            innerSensorB.Item1, innerSensorB.Item2,
            innerSensorC.Item1, innerSensorC.Item2,
            innerSensorD.Item1, innerSensorD.Item2
        );
    }
}

