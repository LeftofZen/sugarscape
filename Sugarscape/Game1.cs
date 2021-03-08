using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using Sugarscape.Cells;

namespace Sugarscape
{
	public class Game1 : Game
	{
		GraphicsDeviceManager _graphics;
		SpriteBatch spriteBatch;
		World<SSCell> grid;
		int updateCounter = 0;
		int updateMod = 5;
		int cellDrawSize = 16;
		int gridSize = 32;
		Texture2D pixel;
		SpriteFont font;
		bool pauseSim;
		Keys[] prevKeysPressed;
		bool prevLeftButton;
		bool prevRightButton;
		Agent a = new Agent(0, 80);

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			grid = new World<SSCell>(gridSize);
			prevKeysPressed = new Keys[256];
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			_graphics.PreferredBackBufferWidth = 512;
			_graphics.PreferredBackBufferHeight = 512;
			_graphics.ApplyChanges();

			a.Location = new Point(gridSize / 2, gridSize / 2);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			pixel = new Texture2D(GraphicsDevice, 1, 1);
			pixel.SetData(new Color[] { Color.White });
			font = Content.Load<SpriteFont>("Calibri");

			// TODO: use this.Content to load your game content here
		}

		bool IsNewKeyPress(Keys[] prev, Keys[] curr, Keys key) => !prev.Contains(key) && curr.Contains(key);

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			var newKeysPressed = Keyboard.GetState().GetPressedKeys();

			var leftButton = Mouse.GetState().LeftButton == ButtonState.Pressed;
			if (leftButton && !prevLeftButton)
			{
				var pos = Mouse.GetState().Position;
				var (x, y) = (pos.X / cellDrawSize, pos.Y / cellDrawSize);
				grid.ptrDraw[y, x].Alive = !grid.ptrDraw[y, x].Alive;
			}
			prevLeftButton = leftButton;

			var rightButton = Mouse.GetState().RightButton == ButtonState.Pressed;
			if (rightButton && !prevRightButton)
			{
				var pos = Mouse.GetState().Position;
				var gridPos = new Point(pos.X / cellDrawSize, pos.Y / cellDrawSize);
				a.Location = gridPos;
			}
			prevRightButton = rightButton;

			if (IsNewKeyPress(prevKeysPressed, newKeysPressed, Keys.P))
			{
				pauseSim = !pauseSim;
			}

			if (IsNewKeyPress(prevKeysPressed, newKeysPressed, Keys.Space))
			{
				grid.Update();
				grid.FlipBuffers();
			}

			if (IsNewKeyPress(prevKeysPressed, newKeysPressed, Keys.A))
			{
				a.Update(grid.ptrDraw);
			}

			if (!pauseSim)
			{
				if (updateCounter++ % updateMod == 0)
				{
					grid.Update();
					a.Update(grid.ptrUpdate);
					grid.FlipBuffers();
				}
			}

			prevKeysPressed = newKeysPressed;
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();

			DrawGrid(gameTime, spriteBatch);

			var agentPercentHunger = a.hunger / a.maxHunger;
			var agentScreenSpace = GridExtensions.TransformWorldToLocal(grid.ptrDraw, a.Location);

			// draw agent
			spriteBatch.FillRectangle(
				new Rectangle(agentScreenSpace.X * cellDrawSize + 4, agentScreenSpace.Y * cellDrawSize + 4, cellDrawSize - 8, cellDrawSize - 8),
				new Color(agentPercentHunger, agentPercentHunger, 1 - agentPercentHunger));

			// draw agent looking-at cell
			var agentScreenSpaceLookAt = GridExtensions.TransformWorldToLocal(grid.ptrDraw, a.lookingAtCell);
			spriteBatch.DrawRectangle(
				new Rectangle(agentScreenSpaceLookAt.X * cellDrawSize, agentScreenSpaceLookAt.Y * cellDrawSize, cellDrawSize, cellDrawSize),
				Color.White);
			// draw agent looking-at cell
			var agentScreenSpaceLastLocation = GridExtensions.TransformWorldToLocal(grid.ptrDraw, a.lastLocation);
			spriteBatch.DrawRectangle(
				new Rectangle(agentScreenSpaceLastLocation.X * cellDrawSize, agentScreenSpaceLastLocation.Y * cellDrawSize, cellDrawSize, cellDrawSize),
				Color.Blue);

			spriteBatch.End();

			base.Draw(gameTime);
		}


		public void DrawGrid(GameTime gameTime, SpriteBatch sb)
		{
			//grid.ptrDraw.ForEachCell(
			//	(cell, x, y) =>
			//		sb.Draw(pixel, new Rectangle(1 + x * 32, 1 + y * 32, 31, 31), new Color(cell.GetLife(), cell.GetLife(), cell.GetLife())));

			var borderSize = 0;

			grid.ptrDraw.ForEachCell(
				(cell, x, y) =>
				{
					var (xx, yy) = (borderSize + x * cellDrawSize, borderSize + y * cellDrawSize);
					var rect = new Rectangle(xx, yy, cellDrawSize - borderSize, cellDrawSize - borderSize);
					//sb.Draw(pixel, rect, cell.Alive ? Color.White : Color.Black);
					//var n = ((GoLCell)cell).GetNeighbours(grid.ptrDraw);

					((SSCell)cell).Draw(spriteBatch, grid.ptrDraw, rect, pixel);

				});

			var pauseSize = 10;
			spriteBatch.Draw(
				pixel,
				new Rectangle(sb.GraphicsDevice.Viewport.Width - pauseSize, sb.GraphicsDevice.Viewport.Height - pauseSize, pauseSize, pauseSize),
				pauseSim ? Color.Red : Color.Green);
		}
	}
}
