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
		IGridCell Update();
		int GetLife();
	}

	class GoLCell : IGridCell
	{
		const int maxLife = 255;
		int life;

		static readonly Random rnd = new Random(1);
		public GoLCell()
		{
			life = rnd.Next(0, maxLife);
		}

		public int GetLife() => life;

		public IGridCell Update()
		{
			life -= 1;
			if (life <= 0)
			{
				life = maxLife;
			}

			// identity
			return this;
		}
	}

	class Grid<T> where T : IGridCell, new()
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
	}
}
