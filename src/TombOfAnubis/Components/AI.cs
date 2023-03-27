using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class AI : Component
    {
        public MovementGraph MovementGraph { get; set; }
        public AI(Map map) {
            AISystem.Register(this);
            MovementGraph = new MovementGraph(map);
        }

        public override void Delete()
        {
            base.Delete();
            AISystem.Deregister(this);
            MovementGraph = null;
        }
    }
}
