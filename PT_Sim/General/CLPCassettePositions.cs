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
        return new CLPCassette(
            1.0f,
            _al.GetPositionMap("A70")[0],
            _al.GetPositionMap("A70")[1],
            _al.GetPositionMap("B70")[0],
            _al.GetPositionMap("B70")[1],
            _al.GetPositionMap("C70")[0],
            _al.GetPositionMap("C70")[1],
            _al.GetPositionMap("D70")[0],
            _al.GetPositionMap("D70")[1]
        );
    }

    public CLPCassette GetBLCassettePlatform()
    {
        return new CLPCassette(
            1.0f,
            _bl.GetPositionMap("A70")[0],
            _bl.GetPositionMap("A70")[1],
            _bl.GetPositionMap("B70")[0],
            _bl.GetPositionMap("B70")[1],
            _bl.GetPositionMap("C70")[0],
            _bl.GetPositionMap("C70")[1],
            _bl.GetPositionMap("D70")[0],
            _bl.GetPositionMap("D70")[1]
        );
    }
}
