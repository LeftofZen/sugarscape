using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sugarscape.Cells;

namespace Sugarscape
{
	public class Game1 : Game
	{
		GraphicsDeviceManager _graphics;
		SpriteBatch spriteBatch;
		World<GoLCell> grid;
		int updateCounter = 0;
		int updateMod = 6;
		int cellDrawSize = 16;
		Texture2D pixel;
		SpriteFont font;
		bool pauseSim;
		Keys[] prevKeysPressed;
		bool prevLeftButton;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			grid = new World<GoLCell>(32);
			prevKeysPressed = new Keys[256];
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			_graphics.PreferredBackBufferWidth = 512;
			_graphics.PreferredBackBufferHeight = 512;
			_graphics.ApplyChanges();

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
				Exit();

			var newKeysPressed = Keyboard.GetState().GetPressedKeys();


			if (IsNewKeyPress(prevKeysPressed, newKeysPressed, Keys.P))
			{
				pauseSim = !pauseSim;
			}

			if (IsNewKeyPress(prevKeysPressed, newKeysPressed, Keys.Space))
			{
				grid.Update();
			}

			var leftButton = Mouse.GetState().LeftButton == ButtonState.Pressed;
			if (leftButton && !prevLeftButton)
			{
				var pos = Mouse.GetState().Position;
				var (x, y) = (pos.X / cellDrawSize, pos.Y / cellDrawSize);
				grid.ptrDraw[y, x].Alive = !grid.ptrDraw[y, x].Alive;
			}
			prevLeftButton = leftButton;

			if (!pauseSim)
			{
				if (updateCounter++ % updateMod == 0)
				{
					grid.Update();
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
			spriteBatch.End();

			base.Draw(gameTime);
		}


		public void DrawGrid(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
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

					((GoLCell)cell).Draw(spriteBatch, grid.ptrDraw, rect, pixel);

				});

			var pauseSize = 10;
			spriteBatch.Draw(
				pixel,
				new Rectangle(sb.GraphicsDevice.Viewport.Width - pauseSize, sb.GraphicsDevice.Viewport.Height - pauseSize, pauseSize, pauseSize),
				pauseSim ? Color.Red : Color.Green);
		}
	}
}
