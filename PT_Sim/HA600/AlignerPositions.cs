using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PT_Sim.General;


class AlignerPositions
{

    private HA600TMChamber _chamber;

    private float _length;

    private float _gradient1;
    private float _gradient2;

    public AlignerPositions(
        HA600TMChamber chamber
    )
    {
        _chamber = chamber;
        Initialize();
    }

    private void Initialize()
    {
    }

}
