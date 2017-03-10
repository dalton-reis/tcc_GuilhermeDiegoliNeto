using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.TerrainAlgorithm
{
    public class WindDecaySimConfigs
    {
        public bool Active { get; set; }

        public int Range { get; set; }
        public float Factor { get; set; }
        public Directions WindDirection { get; set; }
    }
}
