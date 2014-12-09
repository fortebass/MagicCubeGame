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
		#region �ܼƫŧi
		private const float piOver180 = MathHelper.PiOver4 / 45;
		private const float rotateSpeed = 0.5f;
		/// <summary>
		/// �ƹ��e�@���A
		/// </summary>
		private MouseState preMS;
		/// <summary>
		/// (�ù�)���� x �b
		/// </summary>
		private Vector3 illustrateXAxis;
		/// <summary>
		/// (�ù�)���� y �b
		/// </summary>
		private Vector3 illustrateYAxis;
		/// <summary>
		/// �ƹ� y ��V����
		/// </summary>
		private int offSetY;
		/// <summary>
		/// �ƹ� x ��V����
		/// </summary>
		private int offSetX;
		/// <summary>
		/// �����ƹ�
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

		#region �غc�l
		/// <summary>
		/// �غc�l
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

			// �H�ؼФ��߱������G����O�HCUBE�����߾ާ@�A�G�H�I�����ʷ|�y������
			if (IsNeedUpdate && _cursor.LeftButton == ButtonState.Pressed)
			{
				Matrix transformMatrix;
				offSetX = preMS.X - _cursor.X;
				offSetY = preMS.Y - _cursor.Y;
				if (offSetX != 0)
				{
					transformMatrix = Matrix.CreateFromAxisAngle(
						illustrateYAxis, piOver180 * offSetX * 0.5f);
					//������� CameraPosition ��m
					CameraPosition = Vector3.Transform(CameraPosition, transformMatrix);
					//�����v����V(�����@����)
					_direction = _target - CameraPosition;
					_direction.Normalize();
					//�P�ɧ�ﰲ�� x �b
					illustrateXAxis = Vector3.Cross(_direction, illustrateYAxis);
					illustrateXAxis.Normalize();
				}
				if (offSetY != 0)
				{
					transformMatrix = Matrix.CreateFromAxisAngle(
						illustrateXAxis, piOver180 * offSetY * 0.5f);
					//������� CameraPosition ��m
					CameraPosition = Vector3.Transform(CameraPosition, transformMatrix);
					//�����v����V(�����@����)
					_direction = _target - CameraPosition;
					_direction.Normalize();
					//�P�ɧ�ﰲ�� y �b
					illustrateYAxis = Vector3.Cross(illustrateXAxis, _direction);
					illustrateYAxis.Normalize();
					//y �b����n���V�W���V�q
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
				//�����v����V(�����@����)
				_direction = -CameraPosition;
				_direction.Normalize();
			}
			if (cueeKS.IsKeyDown(Keys.D))
			{
				CameraPosition = Vector3.Transform(
					CameraPosition, Matrix.CreateTranslation(illustrateXAxis * 0.3f));
				//�����v����V(�����@����)
				_direction = -CameraPosition;
				_direction.Normalize();
			}
			if (cueeKS.IsKeyDown(Keys.W))
			{
				CameraPosition = Vector3.Transform(
					CameraPosition, Matrix.CreateTranslation(-illustrateYAxis * 0.3f));
				//�����v����V(�����@����)
				_direction = -CameraPosition;
				_direction.Normalize();
			}
			if (cueeKS.IsKeyDown(Keys.S))
			{
				CameraPosition = Vector3.Transform(
					CameraPosition, Matrix.CreateTranslation(illustrateYAxis * 0.3f));
				//�����v����V(�����@����)
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
