using System;
using System.Collections.Generic;
using System.Text;

namespace Sugarscape.Cells
{
	class SSCell : IGridCell
	{
		public SSCell()
		{

		}

		public void Update(IGridCell[,] currentGrid, IGridCell newCell) => throw new NotImplementedException();

		public bool Alive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public (int x, int y) Location { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
