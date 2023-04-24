using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public interface ICooldown
    {
        public bool IsOnCooldown();
        public void PutOnCooldown();
        public void EndCooldown();
    }
}
