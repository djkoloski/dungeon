using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pb.Collections;

namespace dungeon.Generator
{
	public class Digger
	{
		private static Random random_ = new Random();

		private Dungeon dungeon_;
		private int component_;

		public Digger(Dungeon dungeon, int component)
		{
			dungeon_ = dungeon;
			if (component >= 0)
				component_ = component;
			else
				component_ = dungeon.AddComponent();
		}

		/// <summary>
		/// Steps the digger once
		/// </summary>
		/// <returns>Whether the digger should step again</returns>
		public virtual bool Step()
		{
			throw new System.NotImplementedException();
		}
	}
}
