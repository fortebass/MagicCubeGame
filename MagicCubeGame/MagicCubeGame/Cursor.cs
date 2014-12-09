using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace MagicCubeGame
{
	/// <summary>
	/// This is a game component that implements IUpdateable.
	/// </summary>
	public class Cursor : Microsoft.Xna.Framework.GameComponent
	{
		private int x;
		private int y;
		private Vector2 mousePosition;
		private ButtonState leftButton;
		private ButtonState rightButton;
		private MouseState currMS;
		private GraphicsDevice cursorGDevice;

		public Cursor(Game game, GraphicsDevice gDevice)
			: base(game)
		{
			cursorGDevice = gDevice;
		}

		public int X
		{
			get { return x; }
			set { x = value; }
		}

		public int Y
		{
			get { return y; }
			set { y = value; }
		}

		public Vector2 MousePosition 
		{
			set { mousePosition = value; } 
			get { return mousePosition; }
		}

		public ButtonState LeftButton
		{ 
			get { return leftButton; } 
			set { leftButton = value; }
		}

		public ButtonState RightButton 
		{ 
			get { return rightButton; }
			set { rightButton = value; } 
		}

		public MouseState GetMouseState
		{ 
			get { return currMS; } 
		}

		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
		{

			base.Initialize();
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			currMS = Mouse.GetState();
			UpdatePosition(currMS);
			currMS = UpdateButtons(currMS);

			base.Update(gameTime);
		}

		private MouseState UpdateButtons(MouseState currMS)
		{
			leftButton = currMS.LeftButton;
			rightButton = currMS.RightButton;
			return currMS;
		}

		private void UpdatePosition(MouseState currMS)
		{
			X = currMS.X;
			Y = currMS.Y;
			mousePosition.X = currMS.X;
			mousePosition.Y = currMS.Y;
		}

		/// <summary>
		/// 參考至 Picking 程式碼片段
		/// </summary>
		/// <param name="projectionMatrix"></param>
		/// <param name="viewMatrix"></param>
		/// <returns></returns>
		public Ray CalculateCursorRay(Matrix projectionMatrix, Matrix viewMatrix)
		{
			// create 2 positions in screenspace using the cursor position. 0 is as
			// close as possible to the camera, 1 is as far away as possible.
			Vector3 nearSource = new Vector3(MousePosition, 0f);
			Vector3 farSource = new Vector3(MousePosition, 1f);

			// use Viewport.Unproject to tell what those two screen space positions
			// would be in world space. we'll need the projection matrix and cameraView
			// matrix, which we have saved as member variables. We also need a world
			// matrix, which can just be identity.
			Vector3 nearPoint = cursorGDevice.Viewport.Unproject(nearSource,
				projectionMatrix, viewMatrix, Matrix.Identity);

			Vector3 farPoint = cursorGDevice.Viewport.Unproject(farSource,
				projectionMatrix, viewMatrix, Matrix.Identity);

			// find the direction vector that goes from the nearPoint to the farPoint
			// and normalize it....
			Vector3 direction = farPoint - nearPoint;
			direction.Normalize();

			// and then create a new ray using nearPoint as the source.
			return new Ray(nearPoint, direction);
		}

	}
}
