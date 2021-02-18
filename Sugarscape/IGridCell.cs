namespace Sugarscape
{
	interface IGridCell
	{
		void Update(IGridCell[,] currentGrid, IGridCell newCell);

		bool Alive { get; set; }

		(int x, int y) Location { get; set; }
	}
}
