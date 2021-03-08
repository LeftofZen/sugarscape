using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sugarscape.Cells
{
	class SSCell : IGridCell, IComparable
	{
		static readonly Random rnd = new Random(1);

		public SSCell() : this(0, 1000, rnd.Next(1, 10))
		{
			sugarLevel = rnd.Next(0, (int)sugarCapacity);
		}

		public SSCell(float level, float capacity, float regrowth)
		{
			sugarLevel = level;
			sugarCapacity = capacity;
			sugarRegrowthRate = regrowth;

			if (rnd.NextDouble() > 0.99)
			{
				// magical cell
				sugarRegrowthRate *= 10;
			}
			if (rnd.NextDouble() < 0.001)
			{
				// magical cell
				sugarRegrowthRate = 0;
			}
		}

		public void Update(IGridCell[,] currentGrid, IGridCell newCell)
		{
			var currCell = (SSCell)currentGrid.At(Location);

			// regular regrowth
			var distanceToCenter = new Vector2(Location.X - currentGrid.GetLength(1) / 2f, Location.Y - currentGrid.GetLength(0) / 2f).Length();
			var scale = 1f / (distanceToCenter + 1);
			scale *= 2f;
			//scale = 1f;
			var extra = sugarRegrowthRate * scale;
			((SSCell)newCell).sugarLevel = Math.Min(currCell.sugarLevel + extra, sugarCapacity);

			// bonus
			var sumOfSurroundingCells = 0f;
			sumOfSurroundingCells += ((SSCell)currentGrid.At(Location)).sugarLevel;
			sumOfSurroundingCells += ((SSCell)currentGrid.At(Location - new Point(0, 1))).sugarLevel;
			sumOfSurroundingCells += ((SSCell)currentGrid.At(Location - new Point(1, 0))).sugarLevel;
			sumOfSurroundingCells += ((SSCell)currentGrid.At(Location - new Point(0, -1))).sugarLevel;
			sumOfSurroundingCells += ((SSCell)currentGrid.At(Location - new Point(-1, 0))).sugarLevel;
			sumOfSurroundingCells += ((SSCell)currentGrid.At(Location - new Point(1, 1))).sugarLevel;
			sumOfSurroundingCells += ((SSCell)currentGrid.At(Location - new Point(1, -1))).sugarLevel;
			sumOfSurroundingCells += ((SSCell)currentGrid.At(Location - new Point(-1, 1))).sugarLevel;
			sumOfSurroundingCells += ((SSCell)currentGrid.At(Location - new Point(-1, -1))).sugarLevel;

			if (sumOfSurroundingCells > 4500)
			{
				//dying from overcrowding
				((SSCell)newCell).sugarLevel *= 0.95f;
			}
			else if (sumOfSurroundingCells < 500)
			{
				// thriving from no competition
				((SSCell)newCell).sugarLevel *= 1.05f;
			}
			//else
			{
				//average out neighbours
				var avg = sumOfSurroundingCells / 9f;
				var diff = avg - currCell.sugarLevel;
				//((SSCell)newCell).sugarLevel += (Math.Abs(diff) / 100f);
				((SSCell)newCell).sugarLevel += diff / 100f;
			}

			// cell 'death'
			if (rnd.NextDouble() > 0.999 * (scale * 8f))
			{
				((SSCell)newCell).sugarLevel = 0;
			}
			if (rnd.NextDouble() < 0.001)// * (scale * 6f))
			{
				((SSCell)newCell).sugarLevel = ((SSCell)newCell).sugarCapacity;
			}
		}

		public float Eat(float amount)
		{
			var toBeEaten = Math.Max(sugarLevel - amount, sugarLevel);
			sugarLevel -= toBeEaten;
			return toBeEaten;
		}

		public bool Alive
		{
			get => sugarLevel > 0;
			set => sugarLevel = sugarCapacity;
		}

		public Point Location { get; set; }

		public float sugarLevel;
		public float sugarCapacity;
		public float sugarRegrowthRate; // sometimes called alpha

		public int CompareTo(object obj)
		{
			if (obj is SSCell sscell)
			{
				return sugarLevel.CompareTo(sscell.sugarLevel);
			}
			return 0;
		}
	}
}
