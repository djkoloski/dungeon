using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Pb.Collections;

namespace dungeon.Generator
{
    public class DungeonFactory
    {
        IVector3 startingPosition = IVector3.zero;
		DungeonTree tree_;

        public DungeonFactory(DungeonTree tree)
        {
			tree_ = tree;
        }

        public Dungeon Generate()
        {
            Dungeon dungeon = new Dungeon();

            //Queue<String> roomsToMake = new Queue<String>();
            //roomsToMake.Enqueue(head);
            
			DungeonTree.Iterator iterator = tree_.Begin();

			while (!iterator.Done)
            {
				if (iterator.IsEdge)
				{
					// ...
				}
				else
				{
					// ...
				}
                String cur = roomsToMake.Dequeue();
                //Send out each child digger
                IVector3 availableJoint = dungeon.getRandomJoint(cur);
                Digger digger = new Digger(dungeon, -1, availableJoint, lengthToRoom);
                //Have the digger dig out to the room and make the room
                //Add all of the children of the room to the queue of things to make
                //Set each one to start from one of the joints of the father component

				iterator.Next();
            }

            return dungeon;
        }
    }
}
