using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dungeon.Generator
{
    public class DungeonTree
    {
        public DungeonTreeNode root_;
        private Dictionary<string, DungeonTreeNode> nodes_ = new Dictionary<string, DungeonTreeNode>();

        public class Iterator
        {
            private Queue<object> queue_ = new Queue<object>();

            public bool Done
            {
                get
                {
                    return !queue_.Any();
                }
            }
            private object Current
            {
                get
                {
                    return queue_.Peek();
                }
            }
            public bool IsNode
            {
                get
                {
                    return !Done && Current is DungeonTreeNode;
                }
            }
            public DungeonTreeNode Node
            {
                get
                {
                    return (DungeonTreeNode)Current;
                }
            }
            public DungeonTreeEdge Edge
            {
                get
                {
                    return (DungeonTreeEdge)Current;
                }
            }

            public Iterator(DungeonTreeNode root)
            {
                queue_.Enqueue(root);
            }

            public void Next()
            {
                if (IsNode)
                {
                    DungeonTreeNode node = Node;
                    foreach (DungeonTreeEdge edge in node.children)
                    {
                        queue_.Enqueue(edge);
                        queue_.Enqueue(edge.to);
                    }
                }
                queue_.Dequeue();
            }
        }

        public System.Collections.IEnumerable GetNodes()
        {
            Iterator iter = Begin();
            while (!iter.Done)
            {
                if (iter.IsNode)
                {
                    yield return iter.Node;
                }
                iter.Next();
            }
        }

        public System.Collections.IEnumerable GetEdges()
        {
            Iterator iter = Begin();
            while (!iter.Done)
            {
                if (!iter.IsNode)
                {
                    yield return iter.Edge;
                }
                iter.Next();
            }
        }

        public Iterator Begin()
        {
            return new Iterator(root_);
        }

        // TODO add more parameters to customize the node
        public void AddNode(string name)
        {
            if (nodes_.ContainsKey(name))
                throw new System.InvalidOperationException("Attempted to add dungeon tree node that already existed");

            DungeonTreeNode node = new DungeonTreeNode(name);

            if (root_ == null)
                root_ = node;

            nodes_[name] = node;
        }
        // TODO add more parameters to customize the edge
        public void AddEdge(string from, string to)
        {
            // Check that the two nodes already exist
            if (!nodes_.ContainsKey(from) || !nodes_.ContainsKey(to))
                throw new System.InvalidOperationException("Attempted to create a dungeon tree edge between nodes that do not exist");

            DungeonTreeNode fromNode = nodes_[from];
            DungeonTreeNode toNode = nodes_[to];
            // Check if that edge already exists
            foreach (DungeonTreeEdge edge in fromNode.children)
                if (edge.to == toNode)
                    throw new System.InvalidOperationException("Attempted to create a dungeon tree edge between nodes that are already connected");

            fromNode.children.Add(new DungeonTreeEdge(fromNode, toNode));
        }
    }

    public enum NodeType
    {
        BigRoom
    }

    public class DungeonTreeNode
    {
        public NodeType type;
        public string name;

        public DungeonTreeEdge parent;
        public List<DungeonTreeEdge> children;

        public DungeonTreeNode(string name_)
        {
            name = name_;

            parent = null;
            children = new List<DungeonTreeEdge>();
        }

        public override string ToString()
        {
            return "Node(" + name + ")";
        }
    }

    public class DungeonTreeEdge
    {
        public DungeonTreeNode from;
        public DungeonTreeNode to;
        public int minDist = 100;

        public DungeonTreeEdge(DungeonTreeNode from_, DungeonTreeNode to_)
        {
            from = from_;
            to = to_;
        }

        public override string ToString()
        {
            return "Edge(" + from.name + "," + to.name + ")";
        }
    }
}
