using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Input : Component
    {
        public Input()
        {
            InputSystem.Register(this);
        }

        public override void Delete()
        {
            base.Delete();
            InputSystem.Deregister(this);
        }
    }
}
