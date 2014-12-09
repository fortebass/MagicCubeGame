using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;


namespace MagicCubeGame
{
	public class SimpleMagicCube
	{
		#region 參數設定
		private int square = 3;
		private Cube[, ,] mcArray;
		private Cube[,] rotateAPieceArray;
		private BoundingBox[, ,] mcBBoxArray;

		private int[] axis = new int[] { 0,//axisX 
													0,//axisY
													0 //axisZ
												  };

		private const float radiansSpeed = MathHelper.Pi / 18;
		private Vector3 desireRadians = new Vector3(0.0f, 0.0f, 0.0f);
		private Vector3 currentRadians = new Vector3(1.0f, 1.0f, 1.0f);
		private Vector3 direction = new Vector3(1.0f, 1.0f, 1.0f);

		private int modifyCubesCountX = 0;
		private int modifyCubesCountY = 0;
		private int modifyCubesCountZ = 0;

		private int[] pressFinger = new[] { -1, -1, -1 };
		private bool pressLock = true;
		private bool releaseLock = false;

		private Matrix world;
		private Matrix view;
		private Matrix projection;

		private Vector2 monitorStartPosition;
		private String rotateInfo;
		private bool isLockRoateX = true;
		private bool isLockRotateY = true;
		private bool isLockRotateZ = true;
		//private String testInfo;

		#endregion

		#region 建構子
		/// <summary>
		/// 建構子
		/// </summary>
		/// <param name="gDevice">GraphicsDevice</param>
		/// <param name="mcWorld"></param>
		/// <param name="cameraView"></param>
		/// <param name="cameraProj"></param>
		/// <param name="mcPic"></param>
		public SimpleMagicCube(GraphicsDevice gDevice, Matrix mcWorld, Matrix cameraView,
			Matrix cameraProj, Texture2D mcPic)
		{

			//未使用，用來偏移 MC 中心
			this.world = mcWorld;
			this.view = cameraView;
			this.projection = cameraProj;

			//一整片的 cubes
			rotateAPieceArray = new Cube[square, square];
			//Magic Cube
			mcArray = new Cube[square, square, square];
			//Magic Cube Bounding Box
			mcBBoxArray = new BoundingBox[square, square, square];

			#region 初始化所有 cubes 與 boundingBox
			int cubeID = 0;
			float row = 1.1f;
			float col = 1.1f;
			float dep = 1.1f;
			for (int i = 0; i < square; i++)
			{
				for (int j = 0; j < square; j++)
				{
					for (int k = 0; k < square; k++)
					{
						cubeID++;
						mcArray[i, j, k] = new Cube(cubeID, gDevice, mcPic, 0.55f);
						mcArray[i, j, k].CloneWorld = Matrix.CreateTranslation(row, col, dep);
						mcArray[i, j, k].View = cameraView;
						mcArray[i, j, k].Projection = cameraProj;
						mcArray[i, j, k].MoveActionsRecord(Matrix.Identity);

						mcBBoxArray[i, j, k] = new BoundingBox(
							Vector3.Transform(
								new Vector3(-1.0f, -1.0f, -1.0f) * 0.549f,
								Matrix.CreateTranslation(row, col, dep)),
							Vector3.Transform(
								new Vector3(1.0f, 1.0f, 1.0f) * 0.549f,
								Matrix.CreateTranslation(row, col, dep)));

						dep -= 1.1f;
					}
					col -= 1.1f;
					dep = 1.1f;
				}
				row -= 1.1f;
				col = 1.1f;
			}
			#endregion
		}
		#endregion

		public bool IsFingerToMe { get { return (pressFinger[0] == -1 ? false : true); } }

		#region 滑鼠判定單排 cube 以哪一軸順或逆時針旋轉
		/// <summary>
		/// 以虛擬的 X Y 軸來判定滑鼠點擊單排 cubes 旋轉
		/// </summary>
		/// <param name="_cursor"></param>
		/// <param name="illuX"></param>
		/// <param name="illuY"></param>
		public void ControlUsingMouse(Cursor remoteCursor, Vector3 illuX, Vector3 illuY)
		{
			if (remoteCursor.LeftButton == ButtonState.Pressed)
			{
				if (pressLock)
				{
					//按下紀錄位置(點)，並辦別哪一顆 cube 被選到
					pressFinger = FingerToMe(remoteCursor);
					monitorStartPosition = remoteCursor.MousePosition;
					pressLock = false;
					releaseLock = true;
				}
				//else if (pressFinger[0] == -1)
				//{
				//    //rotateCamera
				//}
			}
			else // ButtonState.Released
			{
				if (releaseLock)//指到方塊內才能旋轉單片 cubes
				{
					//放開紀錄目標位置(點)，並計算移動方向
					Vector2 monitorShift = remoteCursor.MousePosition - monitorStartPosition;
					if (pressFinger[0] != -1)
						CheckFingerToRotate(pressFinger, monitorShift, illuX, illuY);

					releaseLock = false;
					pressLock = true;
				}
			}

		}

		/// <summary>
		/// 滑鼠是否指到方塊(cube 被 fingerToMe 指插到的深度)
		/// </summary>
		/// <param name="_cursor"></param>
		/// <returns></returns>
		private int[] FingerToMe(Cursor remoteCursor)
		{
			Ray fingerToMe = remoteCursor.CalculateCursorRay(this.projection, this.view);
			float? minValue = 50.0f;
			float? floatORnull;
			int[] indexPosition = new[] { -1, -1, -1 };

			for (int irow = 0; irow < square; irow++)
				for (int icol = 0; icol < square; icol++)
					for (int idep = 0; idep < square; idep++)
					{
						floatORnull = mcBBoxArray[irow, icol, idep].Intersects(fingerToMe);
						if (floatORnull != null)
							if (minValue > floatORnull)
							{
								minValue = floatORnull;
								indexPosition[0] = irow;
								indexPosition[1] = icol;
								indexPosition[2] = idep;
							}
					}
			return indexPosition;
		}

		/// <summary>
		/// 根據點到的方塊檢查需要轉的是哪一面哪一排或哪一列
		/// </summary>
		/// <param name="press"></param>
		/// <param name="shift"></param>
		/// <param name="illuX"></param>
		/// <param name="illuY"></param>
		private void CheckFingerToRotate(int[] press, Vector2 shift, Vector3 illuX, Vector3 illuY)
		{
			int axisMaxX = GetTheMaxAxis(illuX);
			int axisMaxY = GetTheMaxAxis(illuY);
			int maxXIndex = Math.Abs(axisMaxX) - 1;
			int maxYIndex = Math.Abs(axisMaxY) - 1;

			axis[maxXIndex] = press[maxXIndex];
			axis[maxYIndex] = press[maxYIndex];
			if (Math.Abs(shift.X) >= Math.Abs(shift.Y))
			{
				//rotate illustrate X axis
				if (shift.X > 0)
				{
					rotateInfo = "rotate Right";
					CallRotateByAxisIndex(-axisMaxY);
				}
				else if (shift.X < 0)
				{
					rotateInfo = "rotate Left";
					CallRotateByAxisIndex(axisMaxY);
				}
			}
			else
			{
				//rotate illustrate Y axis
				if (shift.Y < 0)
				{
					rotateInfo = "rotate Up";
					CallRotateByAxisIndex(axisMaxX);
				}
				else if (shift.Y > 0)
				{
					rotateInfo = "rotate Down";
					CallRotateByAxisIndex(-axisMaxX);
				}
			}
		}

		/// <summary>
		/// 呼叫需要旋轉的軸與方向
		/// </summary>
		/// <param name="axisIndex"></param>
		private void CallRotateByAxisIndex(int axisIndex)
		{
			switch (axisIndex)
			{
				case 1:
					RotateClockWiseByAxisX();
					break;
				case 2:
					RotateClockWiseByAxisY();
					break;
				case 3:
					RotateClockWiseByAxisZ();
					break;
				case -1:
					RotateNotClockWiseByAxisX();
					break;
				case -2:
					RotateNotClockWiseByAxisY();
					break;
				case -3:
					RotateNotClockWiseByAxisZ();
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// 取出最大的那一軸之 index(有分正負號)
		/// </summary>
		/// <param name="illuAxis"></param>
		/// <returns></returns>
		private int GetTheMaxAxis(Vector3 illuAxis)
		{
			float maxValue = Math.Max(
				Math.Abs(illuAxis.X),
				Math.Max(Math.Abs(illuAxis.Y), Math.Abs(illuAxis.Z)));

			if (maxValue.Equals(Math.Abs(illuAxis.X)))
				return Math.Sign(illuAxis.X) * 1;//X
			else if (maxValue.Equals(Math.Abs(illuAxis.Y)))
				return Math.Sign(illuAxis.Y) * 2;//Y
			else
				return Math.Sign(illuAxis.Z) * 3;//Z
		}
		#endregion

		#region 旋轉設定函數
		private void RotateNotClockWiseByAxisX()
		{
			direction.X = 1.0f;
			SetCurrentAndDesire_X();
		}

		private void RotateClockWiseByAxisX()
		{
			direction.X = -1.0f;
			SetCurrentAndDesire_X();
		}

		private void SetCurrentAndDesire_X()
		{
			currentRadians.X = 0.0f;
			desireRadians.X = direction.X * MathHelper.PiOver2;
			isLockRoateX = false;
			isLockRotateY = true;
			isLockRotateZ = true;
		}

		private void RotateNotClockWiseByAxisY()
		{
			direction.Y = 1.0f;
			SetCurrentAndDesire_Y();
		}

		private void RotateClockWiseByAxisY()
		{
			direction.Y = -1.0f;
			SetCurrentAndDesire_Y();
		}

		private void SetCurrentAndDesire_Y()
		{
			currentRadians.Y = 0.0f;
			desireRadians.Y = direction.Y * MathHelper.PiOver2;
			isLockRoateX = true;
			isLockRotateY = false;
			isLockRotateZ = true;
		}

		private void RotateNotClockWiseByAxisZ()
		{
			direction.Z = 1.0f;
			SetCurrentAndDesire_Z();
		}

		private void RotateClockWiseByAxisZ()
		{
			direction.Z = -1.0f;
			SetCurrentAndDesire_Z();
		}

		private void SetCurrentAndDesire_Z()
		{
			currentRadians.Z = 0.0f;
			desireRadians.Z = direction.Z * MathHelper.PiOver2;
			isLockRoateX = true;
			isLockRotateY = true;
			isLockRotateZ = false;
		}
		#endregion


		#region 旋轉動作
		#region 每片的 cubes 旋轉
		/// <summary>
		/// 以Z軸旋轉一整片 cubes
		/// </summary>
		private void DoCubesRotateByZ()
		{
			//float currentRadians = 0.0f;
			if (direction.Z * desireRadians.Z > direction.Z * currentRadians.Z)
			{
				currentRadians.Z += direction.Z * radiansSpeed;
				for (int i = 0; i < square; i++)
					for (int j = 0; j < square; j++)
					{
						mcArray[i, j, axis[2]].MoveActions(Matrix.CreateRotationZ(currentRadians.Z));

						//轉到差不多就將實際的旋轉結果紀錄起來
						if (direction.Z * desireRadians.Z <= direction.Z * currentRadians.Z)
						{
							modifyCubesCountZ++;//紀錄第幾顆被旋轉過
							mcArray[i, j, axis[2]].MoveActionsRecord(
								Matrix.CreateRotationZ(direction.Z * MathHelper.PiOver2));
						}
					}
			}
		}

		/// <summary>
		/// 以Y軸旋轉一整片 cubes
		/// </summary>
		private void DoCubesRotateByY()
		{
			if (direction.Y * desireRadians.Y > direction.Y * currentRadians.Y)
			{
				currentRadians.Y += direction.Y * radiansSpeed;
				for (int i = 0; i < square; i++)
					for (int k = 0; k < square; k++)
					{
						mcArray[i, axis[1], k].MoveActions(Matrix.CreateRotationY(currentRadians.Y));

						//轉到差不多就將實際的旋轉結果紀錄起來
						if (direction.Y * desireRadians.Y <= direction.Y * currentRadians.Y)
						{
							modifyCubesCountY++;//紀錄第幾顆被旋轉過
							mcArray[i, axis[1], k].MoveActionsRecord(
								Matrix.CreateRotationY(direction.Y * MathHelper.PiOver2));
						}
					}
			}
		}

		/// <summary>
		/// 以X軸旋轉一整片 cubes
		/// </summary>
		private void DoCubesRotateByX()
		{
			if (direction.X * desireRadians.X > direction.X * currentRadians.X)
			{
				currentRadians.X += direction.X * radiansSpeed;
				for (int j = 0; j < square; j++)
					for (int k = 0; k < square; k++)
					{
						mcArray[axis[0], j, k].MoveActions(Matrix.CreateRotationX(currentRadians.X));

						//轉到差不多就將實際的旋轉結果紀錄起來
						if (direction.X * desireRadians.X <= direction.X * currentRadians.X)
						{
							modifyCubesCountX++;//紀錄第幾顆被旋轉過
							mcArray[axis[0], j, k].MoveActionsRecord(
								Matrix.CreateRotationX(direction.X * MathHelper.PiOver2));
						}
					}
			}
		}
		#endregion

		#region 每片的 indice 旋轉
		/// <summary>
		/// 將在所有應該旋轉的 cube 都轉過後，將轉過的 cubes 作修正
		/// 一次轉 9 粒
		/// </summary>
		private void DoIndeiceRotate()
		{
			if (modifyCubesCountX == 9)
			{
				DoIndiceRotateByX(axis[0]);
				modifyCubesCountX = 0;
			}
			else if (modifyCubesCountY == 9)
			{
				DoIndiceRotateByY(axis[1]);
				modifyCubesCountY = 0;
			}
			else if (modifyCubesCountZ == 9)
			{
				DoIndiceRotateByZ(axis[2]);
				modifyCubesCountZ = 0;
			}
		}

		/// <summary>
		/// 以指定的某一片做 Z 軸旋轉
		/// </summary>
		/// <param name="piece"></param>
		private void DoIndiceRotateByZ(int piece)//Z
		{
			for (int i = 0; i < square; i++)
				for (int j = 0; j < square; j++)
					rotateAPieceArray[i, j] = mcArray[i, j, piece];

			if (direction.Z < 0)
			{ indexClockwise(rotateAPieceArray); }
			else
			{ indexNotClockwise(rotateAPieceArray); }

			for (int i = 0; i < square; i++)
				for (int j = 0; j < square; j++)
					mcArray[i, j, piece] = rotateAPieceArray[i, j];
		}

		/// <summary>
		/// 以指定的某一片做 Y 軸旋轉
		/// </summary>
		/// <param name="piece"></param>
		private void DoIndiceRotateByY(int piece)//Y
		{
			for (int i = 0; i < square; i++)
				for (int j = 0; j < square; j++)
					rotateAPieceArray[i, j] = mcArray[i, piece, j];

			if (direction.Y > 0)
			{ indexClockwise(rotateAPieceArray); }
			else
			{ indexNotClockwise(rotateAPieceArray); }

			for (int i = 0; i < square; i++)
				for (int j = 0; j < square; j++)
					mcArray[i, piece, j] = rotateAPieceArray[i, j];
		}

		/// <summary>
		/// 以指定的某一片做 X 軸旋轉
		/// </summary>
		/// <param name="piece"></param>
		private void DoIndiceRotateByX(int piece)//X
		{
			for (int i = 0; i < square; i++)
				for (int j = 0; j < square; j++)
					rotateAPieceArray[i, j] = mcArray[piece, i, j];

			if (direction.X < 0)
			{ indexClockwise(rotateAPieceArray); }
			else
			{ indexNotClockwise(rotateAPieceArray); }

			for (int i = 0; i < square; i++)
				for (int j = 0; j < square; j++)
					mcArray[piece, i, j] = rotateAPieceArray[i, j];
		}
		#endregion

		#region 將整片 cubes 順時針或逆時針旋轉交換
		/// <summary>
		/// 將傳進來的一整片 cubes 做"逆"時針旋轉 
		/// </summary>
		/// <param name="roArr"></param>
		private void indexNotClockwise(Cube[,] roArr)
		{
			Cube temp = roArr[0, 2];
			roArr[0, 2] = roArr[2, 2];
			roArr[2, 2] = roArr[2, 0];
			roArr[2, 0] = roArr[0, 0];
			roArr[0, 0] = temp;

			temp = roArr[0, 1];
			roArr[0, 1] = roArr[1, 2];
			roArr[1, 2] = roArr[2, 1];
			roArr[2, 1] = roArr[1, 0];
			roArr[1, 0] = temp;
		}

		/// <summary>
		/// 將傳進來的一整片 cubes 做"順"時針旋轉
		/// </summary>
		/// <param name="roArr"></param>
		private void indexClockwise(Cube[,] roArr)
		{
			Cube temp = roArr[0, 2];
			roArr[0, 2] = roArr[0, 0];
			roArr[0, 0] = roArr[2, 0];
			roArr[2, 0] = roArr[2, 2];
			roArr[2, 2] = temp;

			temp = roArr[0, 1];
			roArr[0, 1] = roArr[1, 0];
			roArr[1, 0] = roArr[2, 1];
			roArr[2, 1] = roArr[1, 2];
			roArr[1, 2] = temp;
		}

		#endregion Clockwise and CounterClockwise Swap end

		#endregion rotateAction end

		#region 更新狀態
		/// <summary>
		/// 更新狀態
		/// </summary>
		public void Update()
		{
			if (!isLockRoateX)
				DoCubesRotateByX();
			else if (!isLockRotateY)
				DoCubesRotateByY();
			else if (!isLockRotateZ)
				DoCubesRotateByZ();

			DoIndeiceRotate();
		}
		#endregion


		public void Draw(Matrix cameraView)
		{
			this.view = cameraView;
			foreach (Cube cu in mcArray)
			{
				cu.View = cameraView;
				cu.Draw();
			}
		}

		public bool IsComplete()
		{
			int length = mcArray[0, 0, 0].IndexNum - mcArray[0, 0, 1].IndexNum;//秘密
			int width = mcArray[0, 0, 0].IndexNum - mcArray[0, 1, 0].IndexNum;
			int higth = mcArray[0, 0, 0].IndexNum - mcArray[1, 0, 0].IndexNum;
			for (int i = 0; i < square - 1; i++)
			{
				if (higth != mcArray[i, 0, 0].IndexNum - mcArray[i + 1, 0, 0].IndexNum)
					return false;
				for (int j = 0; j < square - 1; j++)
				{
					if (width != mcArray[i, j, 0].IndexNum - mcArray[i, j + 1, 0].IndexNum)
						return false;
					for (int k = 0; k < square - 1; k++)
					{
						if (length != mcArray[i, j, k].IndexNum - mcArray[i, j, k + 1].IndexNum)
							return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// MC 裡面數字矩陣
		/// </summary>
		/// <returns></returns>
		public StringBuilder GetMC_IndexNum()
		{
			StringBuilder buildString = new StringBuilder();
			buildString.Append("\n");
			int stringcount = 0;
			for (int i = 0; i < square; i++)
				for (int j = 0; j < square; j++)
					for (int k = 0; k < square; k++)
					{
						stringcount++;
						buildString.Append(" ").Append(mcArray[i, j, k].IndexNum.ToString());
						if (stringcount % square == 0)
						{
							buildString.Append("\n");
						}
					}
			return buildString;
		}

		public StringBuilder GetMC_pressFinger()
		{
			StringBuilder buildString = new StringBuilder();
			//String totalInfo = "";
			foreach (int item in pressFinger)
			{
				buildString.Append(item).Append(" ");
				//totalInfo += item + " ";
			}
			buildString.Append("\n").Append(rotateInfo);
			//totalInfo += "\n" + rotateInfo;
			return buildString;
		}

	}
}
