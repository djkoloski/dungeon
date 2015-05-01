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
			private object current;

			public bool Done
			{
				get
				{
					return current == null;
				}
			}
			public bool IsNode
			{
				get
				{
					return !Done && current is DungeonTreeNode;
				}
			}
			public DungeonTreeNode Node
			{
				get
				{
					return (DungeonTreeNode)current;
				}
			}
			public bool IsEdge
			{
				get
				{
					return !Done && current is DungeonTreeEdge;
				}
			}
			public DungeonTreeEdge Edge
			{
				get
				{
					return (DungeonTreeEdge)current;
				}
			}

			public Iterator(DungeonTreeNode root)
			{
				current = root;
			}

			// Precondition: on a node
			private void MoveUp()
			{
				DungeonTreeNode node = Node;
				if (node.parent == null)
					current = null;

				DungeonTreeEdge edge = node.parent;
				DungeonTreeNode parent = edge.from;

				int index = parent.children.IndexOf(edge) + 1;
				if (index >= parent.children.Count)
					MoveUp();
				else
					current = parent.children[index];
			}
			public bool Next()
			{
				if (IsNode)
					MoveUp();
				else
					current = Edge.to;

				return current != null;
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
