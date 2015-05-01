using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dungeon.Generator
{
	public class DungeonTree
	{
		private DungeonTreeNode root_;
		private Dictionary<string, DungeonTreeNode> nodes_;

		public class Iterator
		{
			private List<object> queue_;

			public bool Done
			{
				get
				{
					return queue_.Count == 0;
				}
			}
			private object Current
			{
				get
				{
					return queue_[0];
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
			public bool IsEdge
			{
				get
				{
					return !Done && Current is DungeonTreeEdge;
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
				queue_.Add(root);
			}

			public void Next()
			{
				queue_.RemoveAt(0);

				if (IsNode)
				{
					DungeonTreeNode node = Node;
					foreach (DungeonTreeEdge edge in node.children)
						queue_.Add(edge);
				}
				else
					queue_.Add(Edge.to);
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

	public class DungeonTreeNode
	{
		public string name;

		public DungeonTreeEdge parent;
		public List<DungeonTreeEdge> children;

		public DungeonTreeNode(string name_)
		{
			name = name_;

			parent = null;
			children = new List<DungeonTreeEdge>();
		}
	}

	public class DungeonTreeEdge
	{
		public DungeonTreeNode from;
		public DungeonTreeNode to;

		public DungeonTreeEdge(DungeonTreeNode from_, DungeonTreeNode to_)
		{
			from = from_;
			to = to_;
		}
	}
}
