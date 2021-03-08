using Microsoft.Xna.Framework;

namespace Sugarscape
{
	interface IGridCell
	{
		void Update(IGridCell[,] currentGrid, IGridCell newCell);

		bool Alive { get; set; }

		Point Location { get; set; }
	}
}
