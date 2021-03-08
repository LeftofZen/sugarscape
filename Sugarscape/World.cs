using Microsoft.Xna.Framework;

namespace Sugarscape
{
	class World<T> : IWorld where T : IGridCell, new()
	{
		IGridCell[,] bufA;
		IGridCell[,] bufB;

		public IGridCell[,] ptrUpdate;
		public IGridCell[,] ptrDraw;

		public World(int size) : this(size, size) { }

		public World(int sizeX, int sizeY)
		{
			bufA = new IGridCell[sizeY, sizeX];
			bufB = new IGridCell[sizeY, sizeX];

			bufA.ForEachCell(() => new T());
			bufB.ForEachCell(() => new T());

			bufA.ForEachCell((cell, x, y) => { cell.Location = new Point(x, y); });
			bufB.ForEachCell((cell, x, y) => { cell.Location = new Point(x, y); });

			ptrUpdate = bufA;
			ptrDraw = bufB;
		}

		public void FlipBuffers()
		{
			var tmp = ptrUpdate;
			ptrUpdate = ptrDraw;
			ptrDraw = tmp;
		}

		public void Update()
		{
			ptrDraw.ForEachCell((cell, x, y) => cell.Update(ptrDraw, ptrUpdate[y,x]));
		}
	}
}
