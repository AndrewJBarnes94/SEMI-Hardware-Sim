using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _gradient1, -_length
        );

        (float, float) perpendicularPoint2 = Formulas.FindPerpendicularPoint_Vector(
            _chamber.GetPositionMap("bottom")[0], _chamber.GetPositionMap("bottom")[1],
            _gradient1, -_length
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

        // Define chuck positions
        float chuckCenterX = midpoint.Item1;
        float chuckCenterY = midpoint.Item2;
        float chuckRadius = _length / 6;

        return new HA75Aligner(
            1.0f,

            // Main rectangular body
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2,
            perpendicularPoint3.Item1, perpendicularPoint3.Item2,
            perpendicularPoint4.Item1, perpendicularPoint4.Item2,

            // Chuck
            chuckCenterX, chuckCenterY, chuckRadius
        );
    }
}
