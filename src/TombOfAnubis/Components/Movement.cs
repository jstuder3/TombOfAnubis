using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public enum Orientation
    {
        North,
        East,
        South,
        West
    }
    public class Movement : Component
    {
        public int MaxSpeed { get; set; }
        public bool IsWalking { get; set; }
        public bool IsTrapped { get; set; }

        public Orientation Orientation { get; set; }

        public Movement(int maxSpeed) {

            MaxSpeed = maxSpeed;
            IsWalking = false;
            IsTrapped = false;
            Orientation = Orientation.North;
        }


    }
}
