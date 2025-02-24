using System;
using PT_Sim;
using PT_Sim.General;

public class SlitValvePositions
{
    private HA600TMChamber _chamber;
    private float _slitValveLength;
    private float _gradient;

    public SlitValvePositions(HA600TMChamber chamber)
    {
        _chamber = chamber;
        Initialize();
    }

    private void Initialize()
    {
        _slitValveLength = Formulas.pythagorean(
            _chamber.GetPositionMap("topLeft")[0],
            _chamber.GetPositionMap("top")[1] - _chamber.GetPositionMap("topLeft")[1]
        );

        _gradient = Formulas.slope(
            _chamber.GetPositionMap("topLeft")[0], _chamber.GetPositionMap("topLeft")[1],
            _chamber.GetPositionMap("top")[0], _chamber.GetPositionMap("top")[1]
        );
    }

    public SlitValve GetSlitValve1()
    {
        return new SlitValve(
            1.0f,
            _chamber.GetPositionMap("topLeft")[0], _chamber.GetPositionMap("topLeft")[1],
            _chamber.GetPositionMap("topLeft")[0], _chamber.GetPositionMap("topLeft")[1] - _slitValveLength,
            _chamber.GetPositionMap("topLeft")[0] - _slitValveLength / 6, _chamber.GetPositionMap("topLeft")[1],
            _chamber.GetPositionMap("topLeft")[0] - _slitValveLength / 6, _chamber.GetPositionMap("topLeft")[1] - _slitValveLength
        );
    }

    public SlitValve GetSlitValve2()
    {
        (float, float) perpendicularPoint1 = Formulas.FindPerpendicularPoint_Vector(
            _chamber.GetPositionMap("topLeft")[0], _chamber.GetPositionMap("topLeft")[1],
            _gradient, -_slitValveLength / 6
        );

        (float, float) perpendicularPoint2 = Formulas.FindPerpendicularPoint_Vector(
            _chamber.GetPositionMap("top")[0], _chamber.GetPositionMap("top")[1],
            _gradient, -_slitValveLength / 6
        );

        return new SlitValve(
            1.0f,
            _chamber.GetPositionMap("topLeft")[0], _chamber.GetPositionMap("topLeft")[1],
            _chamber.GetPositionMap("top")[0], _chamber.GetPositionMap("top")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2
        );
    }

    public SlitValve GetSlitValve3()
    {
        (float, float) perpendicularPoint1 = Formulas.FindPerpendicularPoint_Vector(
            _chamber.GetPositionMap("topRight")[0], _chamber.GetPositionMap("topRight")[1],
            -_gradient, _slitValveLength / 6
        );

        (float, float) perpendicularPoint2 = Formulas.FindPerpendicularPoint_Vector(
            _chamber.GetPositionMap("top")[0], _chamber.GetPositionMap("top")[1],
            -_gradient, _slitValveLength / 6
        );

        return new SlitValve(
            1.0f,
            _chamber.GetPositionMap("topRight")[0], _chamber.GetPositionMap("topRight")[1],
            _chamber.GetPositionMap("top")[0], _chamber.GetPositionMap("top")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2
        );
    }

    public SlitValve GetSlitValve4()
    {
        return new SlitValve(
             1.0f,
             _chamber.GetPositionMap("topRight")[0], _chamber.GetPositionMap("topRight")[1],
             _chamber.GetPositionMap("topRight")[0], _chamber.GetPositionMap("topRight")[1] - _slitValveLength,
             _chamber.GetPositionMap("topRight")[0] + _slitValveLength / 6, _chamber.GetPositionMap("topRight")[1],
             _chamber.GetPositionMap("topRight")[0] + _slitValveLength / 6, _chamber.GetPositionMap("topRight")[1] - _slitValveLength
         );
    }

    public SlitValve GetSlitValve5()
    {
        (float, float) perpendicularPoint1 = Formulas.FindPerpendicularPoint_Vector(
            _chamber.GetPositionMap("bottomRight")[0], _chamber.GetPositionMap("bottomRight")[1],
            _gradient, _slitValveLength / 6
        );

        (float, float) perpendicularPoint2 = Formulas.FindPerpendicularPoint_Vector(
            _chamber.GetPositionMap("bottom")[0], _chamber.GetPositionMap("bottom")[1],
            _gradient, _slitValveLength / 6
        );

        return new SlitValve(
            1.0f,
            _chamber.GetPositionMap("bottomRight")[0], _chamber.GetPositionMap("bottomRight")[1],
            _chamber.GetPositionMap("bottom")[0], _chamber.GetPositionMap("bottom")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2
        );
    }

    public SlitValve GetSlitValve6()
    {
        (float, float) perpendicularPoint1 = Formulas.FindPerpendicularPoint_Vector(
            _chamber.GetPositionMap("bottomLeft")[0], _chamber.GetPositionMap("bottomLeft")[1],
            -_gradient, -_slitValveLength / 6
        );

        (float, float) perpendicularPoint2 = Formulas.FindPerpendicularPoint_Vector(
            _chamber.GetPositionMap("bottom")[0], _chamber.GetPositionMap("bottom")[1],
            -_gradient, -_slitValveLength / 6
        );

        return new SlitValve(
            1.0f,
            _chamber.GetPositionMap("bottomLeft")[0], _chamber.GetPositionMap("bottomLeft")[1],
            _chamber.GetPositionMap("bottom")[0], _chamber.GetPositionMap("bottom")[1],
            perpendicularPoint1.Item1, perpendicularPoint1.Item2,
            perpendicularPoint2.Item1, perpendicularPoint2.Item2
        );
    }
}
