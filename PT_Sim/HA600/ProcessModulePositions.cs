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
        return new ProcessModule(
            1.0f,
            _slitValve1.GetPositionMap("C")[0], _slitValve1.GetPositionMap("C")[1],
            _slitValve1.GetPositionMap("D")[0], _slitValve1.GetPositionMap("D")[1],
            _slitValve1.GetPositionMap("C")[0] - _length, _slitValve1.GetPositionMap("C")[1],
            _slitValve1.GetPositionMap("D")[0] - _length, _slitValve1.GetPositionMap("D")[1]
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
        return new ProcessModule(
            1.0f,
            _slitValve2.GetPositionMap("C")[0], _slitValve2.GetPositionMap("C")[1],
            _slitValve2.GetPositionMap("D")[0], _slitValve2.GetPositionMap("D")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2
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
        return new ProcessModule(
            1.0f,
            _slitValve3.GetPositionMap("C")[0], _slitValve3.GetPositionMap("C")[1],
            _slitValve3.GetPositionMap("D")[0], _slitValve3.GetPositionMap("D")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2
        );
    }

    public ProcessModule GetProcessModule4()
    {
        return new ProcessModule(
            1.0f,
            _slitValve4.GetPositionMap("C")[0], _slitValve4.GetPositionMap("C")[1],
            _slitValve4.GetPositionMap("D")[0], _slitValve4.GetPositionMap("D")[1],
            _slitValve4.GetPositionMap("C")[0] + _length, _slitValve4.GetPositionMap("C")[1],
            _slitValve4.GetPositionMap("D")[0] + _length, _slitValve4.GetPositionMap("D")[1]
        );
    }
}
