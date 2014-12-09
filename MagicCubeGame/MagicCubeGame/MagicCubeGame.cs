using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MagicCubeGame
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class MagicCubeGame : Microsoft.Xna.Framework.Game
	{
		#region a lot of parameter

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private MagicCubeGComponent magicCubeGame;
		private Camera mcCameraFront;
		private Cursor remoteCursor;
		private SpriteFont _font;
		private Texture2D _background;
		private Texture2D _isComplete;

		#endregion a lot of parameter end


		#region «Øºc¤l
		public MagicCubeGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}
		#endregion

		public SpriteBatch SpriteBatch
		{
			get { return spriteBatch; }
		}

		public Cursor RemoteCursor
		{
			get { return remoteCursor; }
		}

		#region Initialize
		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			remoteCursor = new Cursor(this, GraphicsDevice);
			Components.Add(remoteCursor);

			InitializeCamera();
			InitializeMagicCubeGame();

			base.Initialize();
		}

		private void InitializeMagicCubeGame()
		{
			SimpleMagicCube sMC = new SimpleMagicCube(
				GraphicsDevice,
				Matrix.Identity,
				mcCameraFront.View,
				mcCameraFront.Projection,
				Content.Load<Texture2D>(@"Layer\square"));

			magicCubeGame = new MagicCubeGComponent(
				this,
				GraphicsDevice,
				mcCameraFront,
				remoteCursor,
				sMC);

			Components.Add(magicCubeGame);
		}

		private void InitializeCamera()
		{
			Viewport vpFront = GraphicsDevice.Viewport;

			mcCameraFront = new Camera(
				this,
				new Vector3(0.0f, 0.0f, 8.0f),
				Vector3.Zero,
				Vector3.Up,
				remoteCursor,
				vpFront);

			Components.Add(mcCameraFront);
		}
		#endregion Initialize end

		#region LoadContent
		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			_font = Content.Load<SpriteFont>("SpriteFont");
			_background = Content.Load<Texture2D>("Layer/palyBG");
			_isComplete = Content.Load<Texture2D>("Layer/palyComplete");
		}

		#endregion

		#region UnloadContent
		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}
		#endregion

		#region Update
		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}
		#endregion

		#region Draw
		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.DarkGreen);

			IsMouseVisible = true;

			spriteBatch.Begin();
			DrawBackground();
			spriteBatch.End();

			base.Draw(gameTime);

			spriteBatch.Begin();
			if (magicCubeGame.IsComplete)
			{
				spriteBatch.Draw(_isComplete,
					new Vector2((graphics.PreferredBackBufferWidth- _isComplete.Bounds.Width) / 2,
						(graphics.PreferredBackBufferHeight - _isComplete.Bounds.Height) / 2),
						Color.White);
			}
			spriteBatch.End();
			//DrawText();
		}

		private void DrawBackground()
		{
			spriteBatch.Draw(_background,Vector2.Zero,Color.White);
		}

		private void DrawText()
		{
			spriteBatch.Begin();
			spriteBatch.DrawString(
				_font, String.Format("{0}", magicCubeGame.GetInformation()),
				new Vector2(10f, 10f), Color.White);
			spriteBatch.End();
		}

		#endregion Draw end
	}

}
