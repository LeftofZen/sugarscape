using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sugarscape.Cells
{
	class GoLCell : IGridCell
	{
		static readonly Random rnd = new Random(1);

		public GoLCell()
		{
			Alive = rnd.Next(0, 2) == 1;
		}

		public GoLCell(bool isAlive)
		{
			Alive = isAlive;
		}

		public void Update(IGridCell[,] currentGrid, IGridCell newCell)
		{
			var neighbours = GetNeighbours(currentGrid);
			var rule1 = Alive && (neighbours == 2 || neighbours == 3);
			var rule2 = !Alive && neighbours == 3;
			newCell.Alive = rule1 || rule2;
		}

		public int GetNeighbours(IGridCell[,] currentGrid)
		{
			var (x, y) = (Location.x, Location.y);
			var aliveNeighbours = 0;
			aliveNeighbours += currentGrid.At(x - 1, y - 1).Alive ? 1 : 0;
			aliveNeighbours += currentGrid.At(x, y - 1).Alive ? 1 : 0;
			aliveNeighbours += currentGrid.At(x + 1, y - 1).Alive ? 1 : 0;
			aliveNeighbours += currentGrid.At(x + 1, y).Alive ? 1 : 0;
			aliveNeighbours += currentGrid.At(x + 1, y + 1).Alive ? 1 : 0;
			aliveNeighbours += currentGrid.At(x, y + 1).Alive ? 1 : 0;
			aliveNeighbours += currentGrid.At(x - 1, y + 1).Alive ? 1 : 0;
			aliveNeighbours += currentGrid.At(x - 1, y).Alive ? 1 : 0;
			return aliveNeighbours;
		}

		public bool Alive { get; set; }

		public (int x, int y) Location { get; set; }
	}

	static class CellDrawExtension
	{
		public static void Draw(this GoLCell cell, SpriteBatch sb, IGridCell[,] currGrid, Rectangle bounds, Texture2D pixel)
		{
			sb.Draw(pixel, bounds, cell.Alive ? Color.White : Color.Black);
			//var n = cell.GetNeighbours(currGrid);
			//sb.DrawString(font, n.ToString(), new Vector2(xx + 1, yy + 1), Color.Black);
			//sb.DrawString(font, n.ToString(), new Vector2(xx, yy), Color.White);
		}
		public static void Draw(this SSCell cell, SpriteBatch sb, IGridCell[,] currGrid, Rectangle bounds, Texture2D pixel)
		{
			var percent = (float)cell.sugarLevel / cell.sugarCapacity;
			var green = new Color(percent, percent, 0f);
			sb.Draw(pixel, bounds, green);
			//var n = cell.GetNeighbours(currGrid);
			//sb.DrawString(font, n.ToString(), new Vector2(xx + 1, yy + 1), Color.Black);
			//sb.DrawString(font, n.ToString(), new Vector2(xx, yy), Color.White);
		}
	}
}
