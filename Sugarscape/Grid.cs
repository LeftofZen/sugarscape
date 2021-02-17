using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Sugarscape
{
	static class GridExtensions
	{
		public static void ForEachCell(this IGridCell[,] grid, Func<IGridCell> func)
		{
			for (var y = 0; y < grid.GetLength(0); ++y)
			{
				for (var x = 0; x < grid.GetLength(1); ++x)
				{
					grid[y, x] = func();
				}
			}
		}

		//public static void ForEachCell(this IGridCell[,] grid, Func<IGridCell, IGridCell> func)
		//{
		//	for (var y = 0; y < grid.GetLength(0); ++y)
		//	{
		//		for (var x = 0; x < grid.GetLength(1); ++x)
		//		{
		//			grid[y,x] = func(grid[y, x]);
		//		}
		//	}
		//}

		public static void ForEachCell(this IGridCell[,] grid, Func<IGridCell, int, int, IGridCell> func)
		{
			for (var y = 0; y < grid.GetLength(0); ++y)
			{
				for (var x = 0; x < grid.GetLength(1); ++x)
				{
					func(grid[y, x], x, y);
				}
			}
		}

		public static void ForEachCell(this IGridCell[,] grid, Action<IGridCell, int, int> action)
		{
			for (var y = 0; y < grid.GetLength(0); ++y)
			{
				for (var x = 0; x < grid.GetLength(1); ++x)
				{
					action(grid[y, x], x, y);
				}
			}
		}
	}

	interface IGridCell
	{
		IGrid ParentGrid { get; set; }

		IGridCell Update();
		int GetLife();
		bool GetAlive();

		(int x, int y) Location { get; set; }
	}

	interface IGrid
	{
		IGridCell GetGridCell(int x, int y);
	}

	class GoLCell : IGridCell
	{
		const int maxLife = 255;
		int life;
		bool isAlive;

		static readonly Random rnd = new Random(1);
		public GoLCell()
		{
			life = rnd.Next(0, maxLife);
			isAlive = rnd.Next(0, 2) == 1;
		}
		public GoLCell(bool isAlive)
		{
			//life = rnd.Next(0, maxLife);
			this.isAlive = isAlive;
		}

		public int GetLife() => life;
		public bool GetAlive() => isAlive;

		public IGridCell Update()
		{
			var aliveNeighbours = 0;
			aliveNeighbours += ParentGrid.GetGridCell(Location.x - 1, Location.y - 1).GetAlive() ? 1 : 0;
			aliveNeighbours += ParentGrid.GetGridCell(Location.x   , Location.y - 1).GetAlive() ? 1 : 0;
			aliveNeighbours += ParentGrid.GetGridCell(Location.x + 1, Location.y - 1).GetAlive() ? 1 : 0;
			aliveNeighbours += ParentGrid.GetGridCell(Location.x + 1, Location.y).GetAlive() ? 1 : 0;
			aliveNeighbours += ParentGrid.GetGridCell(Location.x + 1, Location.y + 1).GetAlive() ? 1 : 0;
			aliveNeighbours += ParentGrid.GetGridCell(Location.x   , Location.y + 1).GetAlive() ? 1 : 0;
			aliveNeighbours += ParentGrid.GetGridCell(Location.x - 1, Location.y + 1).GetAlive() ? 1 : 0;
			aliveNeighbours += ParentGrid.GetGridCell(Location.x - 1, Location.y).GetAlive() ? 1 : 0;


			bool shouldBeAlive = (aliveNeighbours == 2 || aliveNeighbours == 3) || (!isAlive && aliveNeighbours == 3);
			return new GoLCell(shouldBeAlive);

			//if ()

			//life -= 1;
			//if (life <= 0)
			//{
			//	life = maxLife;
			//}

			// identity
			//return this;
		}

		public (int x, int y) Location { get; set; }

		public IGrid ParentGrid { get; set; }
	}

	class Grid<T> : IGrid where T : IGridCell, new()
	{
		IGridCell[,] bufA;
		IGridCell[,] bufB;

		public IGridCell[,] ptrUpdate;
		public IGridCell[,] ptrDraw;

		public Grid(int sizeX, int sizeY)
		{
			bufA = new IGridCell[sizeY, sizeX];
			bufB = new IGridCell[sizeY, sizeX];

			bufA.ForEachCell(() => new T());
			bufB.ForEachCell(() => new T());

			bufA.ForEachCell((cell, x, y) => { cell.Location = (x, y); cell.ParentGrid = this; });
			bufB.ForEachCell((cell, x, y) => { cell.Location = (x, y); cell.ParentGrid = this; });

			ptrUpdate = bufA;
			ptrDraw = bufB;
		}

		public void FlipBuffers()
		{
			var tmp = bufA;
			bufA = bufB;
			bufA = tmp;
		}

		public void Update()
		{
			ptrUpdate.ForEachCell((cell, x, y) => ptrDraw[y,x] = cell.Update());
			FlipBuffers();
		}

		public IGridCell GetGridCell(int x, int y)
		{
			var sizeX = bufA.GetLength(1);
			var sizeY = bufA.GetLength(0);
			x = ((x % sizeX) + sizeX) % sizeX;
			y = ((y % sizeY) + sizeY) % sizeY;
			return ptrDraw[y, x];
		}
	}
}
