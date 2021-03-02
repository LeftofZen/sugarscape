using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sugarscape.Cells
{
	class SSCell : IGridCell, IComparable
	{
		static readonly Random rnd = new Random(1);

		public SSCell()
		{
			sugarLevel = 0;
			sugarCapacity = 1000;
			sugarRegrowthRate = rnd.Next(1, 4);
		}

		public SSCell(float level, float capacity, float regrowth)
		{
			sugarLevel = level;
			sugarCapacity = capacity;
			sugarRegrowthRate = regrowth;
		}

		public void Update(IGridCell[,] currentGrid, IGridCell newCell)
		{
			if (sugarLevel < sugarCapacity)
			{
				var distanceToCenter = new Vector2(Location.x - currentGrid.GetLength(1) / 2f, Location.y - currentGrid.GetLength(0) / 2f).Length();
				var scale = 1f / (distanceToCenter + 1);
				scale *= 5f;
				((SSCell)newCell).sugarLevel = Math.Min(sugarLevel + sugarRegrowthRate * scale, sugarCapacity);
			}
		}

		public bool Alive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public (int x, int y) Location { get; set; }

		public float sugarLevel;
		public float sugarCapacity;
		public float sugarRegrowthRate; // sometimes called alpha

		public int CompareTo(object obj)
		{
			if (obj is SSCell sscell)
			{
				return sscell.sugarLevel.CompareTo(sugarLevel);
			}
			return 0;

		}
	}
}
