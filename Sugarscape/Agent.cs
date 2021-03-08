using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sugarscape.Cells;

namespace Sugarscape
{
	interface IAgent
	{ }

	class Agent : IAgent
	{
		public Agent(float hunger, float metabolism)
		{
			this.hunger = hunger;
			this.activeMetabolism = metabolism;
		}

		public void Update(IGridCell[,] grid)
		{
			var surroundingSquares = new List<(IGridCell cell, Point coords)>()
			{
				(grid.At(Location.X    , Location.Y    ), new Point(0,  0)),

				(grid.At(Location.X    , Location.Y + 1), new Point(0, +1)),
				(grid.At(Location.X    , Location.Y - 1), new Point(0, -1)),
				(grid.At(Location.X + 1, Location.Y), new Point(+1, 0)),
				(grid.At(Location.X - 1, Location.Y), new Point(-1, 0)),

				(grid.At(Location.X + 1, Location.Y + 1), new Point(+1, +1)),
				(grid.At(Location.X + 1, Location.Y - 1), new Point(+1, -1)),
				(grid.At(Location.X - 1, Location.Y + 1), new Point(-1, +1)),
				(grid.At(Location.X - 1, Location.Y - 1), new Point(-1, -1)),
			};

			surroundingSquares = surroundingSquares
				.Where(a => a.coords != lastLocation && a.coords != Location && ((SSCell)a.cell).sugarLevel > minLandValueToEat)
				.ToList();

			if (surroundingSquares.Count > 0)
			{
				var max = surroundingSquares.Max(v => (SSCell)v.cell);
				var item = surroundingSquares.Where(v => v.cell == max).Single();

				lookingAtCell = max.Location;

				// if we're hungry
				if (hunger >20f)
				{
					Location = lookingAtCell;
					var ssCell = (SSCell)item.cell;
					hunger -= ssCell.Eat(hunger);
					hunger += activeMetabolism * new Vector2(item.coords.X, item.coords.Y).Length();
				}
			}

			hunger += idleMetabolism;

			hunger = Math.Clamp(hunger, 0, maxHunger);
		}

		public Point Location
		{
			get => location;
			set
			{
				lastLocation = location;
				location = value;
			}
		}
		Point location;

		public float maxHunger = 1000f;
		public float hunger = 0f;
		float activeMetabolism = 10f;
		float idleMetabolism = 5f;
		float minLandValueToEat = 100f;
		public Point lastLocation;
		public Point lookingAtCell;

	}
}
