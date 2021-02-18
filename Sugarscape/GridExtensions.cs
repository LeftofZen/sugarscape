using System;

namespace Sugarscape
{
	static class GridExtensions
	{
		public static IGridCell At(this IGridCell[,] grid, int x, int y)
		{
			var sizeX = grid.GetLength(1);
			var sizeY = grid.GetLength(0);
			x = ((x % sizeX) + sizeX) % sizeX;
			y = ((y % sizeY) + sizeY) % sizeY;
			return grid[y, x];
		}

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
}
