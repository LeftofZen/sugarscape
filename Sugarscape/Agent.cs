using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sugarscape.Cells;

namespace Sugarscape
{
	interface IAgent
	{ }

	class Agent : IAgent
	{
		public void Update(IGridCell[,] grid)
		{
			var list = new List<(IGridCell, (int, int))>()
			{
				(grid.At(Location.x    , Location.y - 1), (-1, 0)),
				(grid.At(Location.x    , Location.y + 1), (+1, 0)),
				(grid.At(Location.x + 1, Location.y), (0, +1)),
				(grid.At(Location.x - 1, Location.y), (0, -1)),
			};

			var max = list.Max(v => (SSCell)v.Item1);
			var item = list.Where(v => v.Item1 == max).Single();
			Location = (Location.x - item.Item2.Item1, Location.y - item.Item2.Item2);
		}

		public (int x, int y) Location { get; set; }

		float hunger = 0f;
		float metabolism = 0f;
	}
}
