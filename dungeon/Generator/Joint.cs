﻿using Pb.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dungeon.Generator
{
    public class Joint
    {
        /**
        * The cell the Joint is in.
        */
        public readonly IVector3 location;
        /**
         * The side of the cell that the Joint is on.
         */
        public readonly int direction;
        public readonly int distanceFromSource;

        public Joint(IVector3 location_, int direction_, int distanceFromSource_)
        {
            location = location_;
            direction = direction_;
            distanceFromSource = distanceFromSource_;
        }

        public override string ToString()
        {
            return "Joint(" + location.ToString() + ":" + direction + ")";
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Joint Joint = (Joint)obj;
            return Joint.location.Equals(location) && Joint.direction.Equals(direction);
        }

        public override int GetHashCode()
        {
            return location.GetHashCode() ^ direction.GetHashCode();
        }

        public IVector3 GetExitLocation()
        {
            return location + Direction.Vector[direction];
        }

        public Joint getOpposite()
        {
            return new Joint(GetExitLocation(), (direction + 3) % 6, distanceFromSource);
        }
    }
}
