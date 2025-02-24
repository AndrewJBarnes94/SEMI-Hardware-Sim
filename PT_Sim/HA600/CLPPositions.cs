using PT_Sim;
using PT_Sim.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class CLPPositions
{
    private SlitValve _slitValve5;
    private SlitValve _slitValve6;

    private float _length;

    private float _gradient5;
    private float _gradient6;

    public CLPPositions(SlitValve slitValve5, SlitValve slitValve6)
    {
        _slitValve5 = slitValve5;
        _slitValve6 = slitValve6;

        Initialize();
    }

    private void Initialize()
    {
        _length = Formulas.distance(
            _slitValve5.GetPositionMap("C")[0],
            _slitValve5.GetPositionMap("C")[1],
            _slitValve5.GetPositionMap("D")[0],
            _slitValve5.GetPositionMap("D")[1]
        );
        _gradient5 = Formulas.slope(
            _slitValve5.GetPositionMap("C")[0], _slitValve5.GetPositionMap("C")[1],
            _slitValve5.GetPositionMap("D")[0], _slitValve5.GetPositionMap("D")[1]
        );
        _gradient6 = Formulas.slope(
            _slitValve6.GetPositionMap("C")[0], _slitValve6.GetPositionMap("C")[1],
            _slitValve6.GetPositionMap("D")[0], _slitValve6.GetPositionMap("D")[1]
        );
    }

    public CLP GetAL()
    {

        (float, float) perpendicularPoint1 = Formulas.FindPerpendicularPoint_Vector(
            _slitValve6.GetPositionMap("C")[0], _slitValve6.GetPositionMap("C")[1],
            _gradient6, -_length
        );
        (float, float) perpendicularPoint2 = Formulas.FindPerpendicularPoint_Vector(
            _slitValve6.GetPositionMap("D")[0], _slitValve6.GetPositionMap("D")[1],
            _gradient6, -_length
        );
        return new CLP(
            1.0f,
            _slitValve6.GetPositionMap("C")[0], _slitValve6.GetPositionMap("C")[1],
            _slitValve6.GetPositionMap("D")[0], _slitValve6.GetPositionMap("D")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2
        );
    }

    public CLP GetBL()
    {

        (float, float) perpendicularPoint1 = Formulas.FindPerpendicularPoint_Vector(
            _slitValve5.GetPositionMap("C")[0], _slitValve5.GetPositionMap("C")[1],
            _gradient5, _length
        );
        (float, float) perpendicularPoint2 = Formulas.FindPerpendicularPoint_Vector(
            _slitValve5.GetPositionMap("D")[0], _slitValve5.GetPositionMap("D")[1],
            _gradient5, _length
        );
        return new CLP(
            1.0f,
            _slitValve5.GetPositionMap("C")[0], _slitValve5.GetPositionMap("C")[1],
            _slitValve5.GetPositionMap("D")[0], _slitValve5.GetPositionMap("D")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2
        );

    }
}