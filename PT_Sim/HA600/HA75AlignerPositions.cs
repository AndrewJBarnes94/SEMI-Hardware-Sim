using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PT_Sim;
using PT_Sim.General;

class HA75AlignerPositions
{
    private const float PI = 3.14159265358979323846f;

    private HA600TMChamber _chamber;

    private float _length;

    private float _gradient1;
    private float _gradient2;

    private float halfCircle1StartX;
    private float halfCircle1StartY;
    private float halfCircle1EndX;
    private float halfCircle1EndY;

    private float halfCircle2StartX;
    private float halfCircle2StartY;
    private float halfCircle2EndX;
    private float halfCircle2EndY;


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
        halfCircle1StartX = adjustedPerpendicularPoint4.Item1;
        halfCircle1StartY = adjustedPerpendicularPoint4.Item2;
        halfCircle1EndX = adjustedPerpendicularPoint2.Item1;
        halfCircle1EndY = adjustedPerpendicularPoint2.Item2;

        halfCircle2StartX = adjustedPerpendicularPoint3.Item1;
        halfCircle2StartY = adjustedPerpendicularPoint3.Item2;
        halfCircle2EndX = adjustedPerpendicularPoint1.Item1;
        halfCircle2EndY = adjustedPerpendicularPoint1.Item2;

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

        (float, float) sensorTopMidpoint = Formulas.FindMiddlePoint(
            outerSensorB.Item1, outerSensorB.Item2,
            outerSensorD.Item1, outerSensorD.Item2
        );

        (float, float) sensorBottomMidpoint = Formulas.FindMiddlePoint(
            outerSensorA.Item1, outerSensorA.Item2,
            outerSensorC.Item1, outerSensorC.Item2
        );

        (float, float) innerSensorA = Formulas.MovePointTowards(
            outerSensorA.Item1, outerSensorA.Item2,
            sensorTopMidpoint.Item1, sensorTopMidpoint.Item2,
            _length * 0.13f
        );

        (float, float) innerSensorB = Formulas.MovePointTowards(
            outerSensorB.Item1, outerSensorB.Item2,
            sensorBottomMidpoint.Item1, sensorBottomMidpoint.Item2,
            _length * 0.13f
        );

        (float, float) innerSensorC = Formulas.MovePointTowards(
            outerSensorC.Item1, outerSensorC.Item2,
            sensorTopMidpoint.Item1, sensorTopMidpoint.Item2,
            _length * 0.13f
        );

        (float, float) innerSensorD = Formulas.MovePointTowards(
            outerSensorD.Item1, outerSensorD.Item2,
            sensorBottomMidpoint.Item1, sensorBottomMidpoint.Item2,
            _length * 0.13f
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

    public HA75Aligner GetMirroredHA75Aligner(HA75Aligner originalAligner)
    {
        // Mirror the points about the y-axis
        float MirrorX(float x) => -x;

        // Calculate the mirrored half circle 1 points
        float halfCircle1CenterX = (originalAligner.HalfCircle1StartX + originalAligner.HalfCircle1EndX) / 2;
        float halfCircle1CenterY = (originalAligner.HalfCircle1StartY + originalAligner.HalfCircle1EndY) / 2;
        float halfCircle1Radius = Formulas.distance(originalAligner.HalfCircle1StartX, originalAligner.HalfCircle1StartY, originalAligner.HalfCircle1EndX, originalAligner.HalfCircle1EndY) / 2;
        float angle1 = (float)Math.Atan2(originalAligner.HalfCircle1EndY - originalAligner.HalfCircle1StartY, originalAligner.HalfCircle1EndX - originalAligner.HalfCircle1StartX);

        // Calculate the mirrored half circle 2 points
        float halfCircle2CenterX = (originalAligner.HalfCircle2StartX + originalAligner.HalfCircle2EndX) / 2;
        float halfCircle2CenterY = (originalAligner.HalfCircle2StartY + originalAligner.HalfCircle2EndY) / 2;
        float halfCircle2Radius = Formulas.distance(originalAligner.HalfCircle2StartX, originalAligner.HalfCircle2StartY, originalAligner.HalfCircle2EndX, originalAligner.HalfCircle2EndY) / 2;
        float angle2 = (float)Math.Atan2(originalAligner.HalfCircle2EndY - originalAligner.HalfCircle2StartY, originalAligner.HalfCircle2EndX - originalAligner.HalfCircle2StartX);

        // Mirror the half circle start and end points
        float mirroredHalfCircle1StartX = MirrorX(originalAligner.HalfCircle1StartX);
        float mirroredHalfCircle1EndX = MirrorX(originalAligner.HalfCircle1EndX);
        float mirroredHalfCircle2StartX = MirrorX(originalAligner.HalfCircle2StartX);
        float mirroredHalfCircle2EndX = MirrorX(originalAligner.HalfCircle2EndX);

        return new HA75Aligner(
            originalAligner.Scale,
            MirrorX(originalAligner.PosAx), originalAligner.PosAy,
            MirrorX(originalAligner.PosBx), originalAligner.PosBy,
            MirrorX(originalAligner.PosCx), originalAligner.PosCy,
            MirrorX(originalAligner.PosDx), originalAligner.PosDy,
            MirrorX(originalAligner.ChuckCenterX), originalAligner.ChuckCenterY,
            originalAligner.ChuckRadius,
            -halfCircle1EndX, halfCircle1EndY,
            -halfCircle1StartX, halfCircle1StartY,
            -halfCircle2EndX, halfCircle2EndY,
            -halfCircle2StartX, halfCircle2StartY,
            MirrorX(originalAligner.OuterSensorAx), originalAligner.OuterSensorAy,
            MirrorX(originalAligner.OuterSensorBx), originalAligner.OuterSensorBy,
            MirrorX(originalAligner.OuterSensorCx), originalAligner.OuterSensorCy,
            MirrorX(originalAligner.OuterSensorDx), originalAligner.OuterSensorDy,
            MirrorX(originalAligner.InnerSensorAx), originalAligner.InnerSensorAy,
            MirrorX(originalAligner.InnerSensorBx), originalAligner.InnerSensorBy,
            MirrorX(originalAligner.InnerSensorCx), originalAligner.InnerSensorCy,
            MirrorX(originalAligner.InnerSensorDx), originalAligner.InnerSensorDy
        );
    }
}

