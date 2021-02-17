using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sugarscape
{
	public class Game1 : Game
	{
		GraphicsDeviceManager _graphics;
		SpriteBatch spriteBatch;
		Grid<GoLCell> grid;
		int updateCounter = 0;
		int updateMod = 1;
		Texture2D pixel;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			grid = new Grid<GoLCell>(10, 10);
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			pixel = new Texture2D(GraphicsDevice, 1, 1);
			pixel.SetData(new Color[] { Color.White });

			// TODO: use this.Content to load your game content here
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (updateCounter++ % updateMod == 0)
			{
				grid.Update();
			}

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
			grid.ptrDraw.ForEachCell(
				(cell, x, y) => 
					sb.Draw(pixel, new Rectangle(1 + x * 32, 1 + y * 32, 31, 31), new Color(cell.GetLife(), cell.GetLife(), cell.GetLife())));
		}
	}
}
