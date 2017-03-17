using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    public enum Directions
    {
        North,
        Northeast,
        East,
        Southeast,
        South,
        Southwest,
        West,
        Northwest,
    }

    static class DirectionsEx
    {
        public static bool IsIntermediate(this Directions direction)
        {
            return (direction == Directions.Northeast || 
                    direction == Directions.Northwest|| 
                    direction == Directions.Southeast || 
                    direction == Directions.Southwest);
        }
    }
}
