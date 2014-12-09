using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;


namespace MagicCubeGame
{
	/// <summary>
	/// This is a game component that implements IUpdateable.
	/// </summary>
	public class MagicCubeGComponent : Microsoft.Xna.Framework.DrawableGameComponent
	{
		private GraphicsDevice mcGdevice;
		private SimpleMagicCube magicCube;
		private Camera mcCamera;
		private Cursor mcCursor;
		private bool testComplete;

		public MagicCubeGComponent(Game game, GraphicsDevice gDevice,
			Camera camera, Cursor cursor, SimpleMagicCube simpleMagicCube)
			: base(game)
		{
			mcGdevice = gDevice;
			mcCursor = cursor;
			magicCube = simpleMagicCube;
			mcCamera = camera;

		}

		public bool IsComplete { get { return magicCube.IsComplete(); } }

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
			testComplete = magicCube.IsComplete();

			magicCube.ControlUsingMouse(
				mcCursor,
				mcCamera.IllustrateXAxis,
				mcCamera.IllustrateYAxis);
			magicCube.Update();

			mcCamera.IsNeedUpdate = !magicCube.IsFingerToMe;
		}

		public override void Draw(GameTime gameTime)
		{
			DepthStencilState new_depth_state = new DepthStencilState();
			new_depth_state.DepthBufferEnable = true;
			GraphicsDevice.DepthStencilState = new_depth_state;

		    magicCube.Draw(mcCamera.View);
		}

		public String GetInformation()
		{
			StringBuilder infomation = new StringBuilder();
			infomation.Append("Is Complete?\n").Append(testComplete).
				Append(magicCube.GetMC_IndexNum()).
				Append("\n").
				Append(magicCube.GetMC_pressFinger());
			return infomation.ToString();
		}
	}
}
