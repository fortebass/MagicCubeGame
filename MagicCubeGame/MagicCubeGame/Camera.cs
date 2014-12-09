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
	public class Camera : Microsoft.Xna.Framework.GameComponent
	{
		#region 變數宣告
		private const float piOver180 = MathHelper.PiOver4 / 45;
		private const float rotateSpeed = 0.5f;
		/// <summary>
		/// 滑鼠前一狀態
		/// </summary>
		private MouseState preMS;
		/// <summary>
		/// (螢幕)假的 x 軸
		/// </summary>
		private Vector3 illustrateXAxis;
		/// <summary>
		/// (螢幕)假的 y 軸
		/// </summary>
		private Vector3 illustrateYAxis;
		/// <summary>
		/// 滑鼠 y 方向偏移
		/// </summary>
		private int offSetY;
		/// <summary>
		/// 滑鼠 x 方向偏移
		/// </summary>
		private int offSetX;
		/// <summary>
		/// 遙控滑鼠
		/// </summary>
		private Cursor _cursor;
		private Viewport _viewport;
		private Vector3 _target;
		private Vector3 _direction;
		private Vector3 _up;

		private Vector3 CameraPosition { get; set; }
		public bool IsNeedUpdate { get; set; }
		public Vector3 IllustrateXAxis { get { return illustrateXAxis; } }
		public Vector3 IllustrateYAxis { get { return illustrateYAxis; } }
		public Matrix View { get; protected set; }
		public Matrix Projection { get; protected set; }
		public Viewport CameraViewport
		{
			get { return _viewport; }
			set { _viewport = value; }
		}

		#endregion

		#region 建構子
		/// <summary>
		/// 建構子
		/// </summary>
		/// <param name="game"></param>
		/// <param name="pos"></param>
		/// <param name="target"></param>
		/// <param name="up"></param>
		public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up,
			Cursor cursor, Viewport viewp)
			: base(game)
		{
			CameraPosition = pos;
			this._target = target;
			this._direction = target - pos;
			this._direction.Normalize();
			this._up = up;
			this.illustrateYAxis = _up;
			this.illustrateXAxis = Vector3.Cross(_direction, illustrateYAxis);
			CreateLookAt();

			this._viewport = viewp;
			this.Projection = Matrix.CreatePerspectiveFieldOfView(
				MathHelper.PiOver4,
				(float) _viewport.Width /
				(float) _viewport.Height,
				1, 100);

			this._cursor = cursor;

		}
		#endregion


		#region Initialize
		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
		{
			//preMS = _cursor.GetMouseState;

			base.Initialize();
		}
		#endregion

		#region Update
		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{

			// 以目標中心旋轉旋轉：旋轉是以CUBE為中心操作，故以背景移動會造成錯亂
			if (IsNeedUpdate && _cursor.LeftButton == ButtonState.Pressed)
			{
				Matrix transformMatrix;
				offSetX = preMS.X - _cursor.X;
				offSetY = preMS.Y - _cursor.Y;
				if (offSetX != 0)
				{
					transformMatrix = Matrix.CreateFromAxisAngle(
						illustrateYAxis, piOver180 * offSetX * 0.5f);
					//旋轉後更改 CameraPosition 位置
					CameraPosition = Vector3.Transform(CameraPosition, transformMatrix);
					//更改攝影機方向(面對單一物體)
					_direction = _target - CameraPosition;
					_direction.Normalize();
					//同時更改假的 x 軸
					illustrateXAxis = Vector3.Cross(_direction, illustrateYAxis);
					illustrateXAxis.Normalize();
				}
				if (offSetY != 0)
				{
					transformMatrix = Matrix.CreateFromAxisAngle(
						illustrateXAxis, piOver180 * offSetY * 0.5f);
					//旋轉後更改 CameraPosition 位置
					CameraPosition = Vector3.Transform(CameraPosition, transformMatrix);
					//更改攝影機方向(面對單一物體)
					_direction = _target - CameraPosition;
					_direction.Normalize();
					//同時更改假的 y 軸
					illustrateYAxis = Vector3.Cross(illustrateXAxis, _direction);
					illustrateYAxis.Normalize();
					//y 軸旋轉要更改向上的向量
					_up = illustrateYAxis;
				}
			}

			CreateLookAt();
			preMS = _cursor.GetMouseState;
			base.Update(gameTime);
		}

		private void ControlUsingKeyboard()
		{

			KeyboardState cueeKS = Keyboard.GetState();
			if (cueeKS.IsKeyDown(Keys.A))
			{
				CameraPosition = Vector3.Transform(
					CameraPosition, Matrix.CreateTranslation(-illustrateXAxis * 0.3f));
				//更改攝影機方向(面對單一物體)
				_direction = -CameraPosition;
				_direction.Normalize();
			}
			if (cueeKS.IsKeyDown(Keys.D))
			{
				CameraPosition = Vector3.Transform(
					CameraPosition, Matrix.CreateTranslation(illustrateXAxis * 0.3f));
				//更改攝影機方向(面對單一物體)
				_direction = -CameraPosition;
				_direction.Normalize();
			}
			if (cueeKS.IsKeyDown(Keys.W))
			{
				CameraPosition = Vector3.Transform(
					CameraPosition, Matrix.CreateTranslation(-illustrateYAxis * 0.3f));
				//更改攝影機方向(面對單一物體)
				_direction = -CameraPosition;
				_direction.Normalize();
			}
			if (cueeKS.IsKeyDown(Keys.S))
			{
				CameraPosition = Vector3.Transform(
					CameraPosition, Matrix.CreateTranslation(illustrateYAxis * 0.3f));
				//更改攝影機方向(面對單一物體)
				_direction = -CameraPosition;
				_direction.Normalize();
			}
		}
		#endregion

		#region CreateLookAt
		private void CreateLookAt()
		{
			View = Matrix.CreateLookAt(
				CameraPosition, CameraPosition + _direction, _up);
		}
		#endregion

	}
}
