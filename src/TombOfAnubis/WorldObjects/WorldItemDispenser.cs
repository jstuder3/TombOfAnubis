using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis.WorldObjects
{
    enum DispenserType
    {
        BodyPowerup,
        WisdomPowerup,
        ResurrectionPowerup,
        None
    }

    internal class WorldItemDispenser : WorldObject
    {
        int usedCounter = 0;
        int maxUses = 0;
        DispenserType dispenserType = DispenserType.None;
    }
}
