using Pb.Collections;
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
        public readonly int distanceFromSource;//Only used for special things. Not really a part and not used in hashing.

        public Joint(IVector3 location_, int direction_, int distanceFromSource_)
        {
            location = location_;
            direction = direction_;
            distanceFromSource = distanceFromSource_;
        }

        public Joint(IVector3 location_, int direction_)
        {
            location = location_;
            direction = direction_;
            distanceFromSource = -1;
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
            Joint joint = (Joint)obj;
            return joint.location.Equals(location) && joint.direction == direction;
        }

        public override int GetHashCode()
        {
            return location.GetHashCode() * direction.GetHashCode();
        }

        public IVector3 GetExitLocation()
        {
            return location + Direction.Vector[direction];
        }
    }
}
