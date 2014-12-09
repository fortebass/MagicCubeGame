using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagicCubeGame
{
	public class Cube
	{
		#region parameters
		private List<VertexPositionNormalTexture> cubeVertex;
		private List<ushort> cubeIndice;
		private Vector3 normal;
		private VertexBuffer cubeVertexBuffer;
		private IndexBuffer cubeIndexBuffer;
		private BasicEffect cubeBasicEffect;
		private int indexNum = 0;
		private Matrix cloneWorld;
		private Texture2D picture;
		

		public int IndexNum
		{
			get { return indexNum; }
			set { indexNum = value; }
		}

		/// <summary>
		/// 拷貝一份 World
		/// </summary>
		public Matrix CloneWorld 
		{ 
			get { return cloneWorld; } 
			set
			{
				cubeBasicEffect.World = cloneWorld = value; 
			} 
		}

		public Texture2D Picture 
		{
			get { return picture; }
			set { picture = value; }
		}

		#endregion

		#region cubeBasicEffect world cameraView projection 
		public Matrix World
		{
			get { return cubeBasicEffect.World; }
			set { cubeBasicEffect.World = value; }
		}

		public Matrix View
		{
			get { return cubeBasicEffect.View; }
			set { cubeBasicEffect.View = value; }
		}

		public Matrix Projection
		{
			get { return cubeBasicEffect.Projection; }
			set { cubeBasicEffect.Projection = value; }
		}

		#endregion

		#region cube 初始化
		/// <summary>
		/// 建構子
		/// </summary>
		/// <param name="iNum">第 iNum 個 cube</param>
		/// <param name="gDevice">GraphicsDevice </param>
		/// <param name="pic">貼圖</param>
		/// <param name="scale">放大或縮小之倍率</param>
		public Cube(int iNum, GraphicsDevice gDevice, Texture2D pic, float scale)
		{
			IndexNum = iNum;
			Picture = pic;

			#region 設置頂點與貼圖
			cubeVertex = new List<VertexPositionNormalTexture>();
			cubeIndice = new List<ushort>();
			Vector3[] referPoints = new Vector3[]{
				new Vector3(1.0f,1.0f,1.0f)*scale,//0
				new Vector3(1.0f,-1.0f,1.0f)*scale,//1
				new Vector3(1.0f,-1.0f,-1.0f)*scale,//2
				new Vector3(1.0f,1.0f,-1.0f)*scale,//3
				new Vector3(-1.0f,1.0f,-1.0f)*scale,//4
				new Vector3(-1.0f,1.0f,1.0f)*scale,//5
				new Vector3(-1.0f,-1.0f,1.0f)*scale,//6
				new Vector3(-1.0f,-1.0f,-1.0f)*scale//7
				};

			int indexShift = 0;
			SetPlusXaxis(indexShift, referPoints);//激W

			indexShift += 4;
			SetMinusZaxis(indexShift, referPoints);//特G

			indexShift += 4;
			SetMinusXaxis(indexShift, referPoints);//蛋Y

			indexShift += 4;
			SetPlusZaxis(indexShift, referPoints);//敵B

			indexShift += 4;
			SetPlusYaxis(indexShift, referPoints);//隱R

			indexShift += 4;
			SetMinusYaxis(indexShift, referPoints);//雜O 
			#endregion

			InitializeCube(gDevice);
		}

		private void InitializeCube(GraphicsDevice gDevice)
		{
			cubeVertexBuffer = new VertexBuffer(
				gDevice, typeof(VertexPositionNormalTexture),cubeVertex.Count, BufferUsage.None);
			cubeVertexBuffer.SetData(cubeVertex.ToArray());

			cubeIndexBuffer = new IndexBuffer(
				gDevice, typeof(ushort), cubeIndice.Count, BufferUsage.None);
			cubeIndexBuffer.SetData(cubeIndice.ToArray());

			cubeBasicEffect = new BasicEffect(gDevice);
			cubeBasicEffect.EnableDefaultLighting();
		}

		private void SetMinusYaxis(int indexShift, Vector3[] referPoints)
		{
			normal = new Vector3(0.0f, -1.0f, 0.0f);//-Y 
			SetCubeIndice(indexShift);
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[1], normal, new Vector2(1.0f, 0.25f)));//雜
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[2], normal, new Vector2(1.0f, 0.5f)));
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[7], normal, new Vector2(0.75f, 0.5f)));
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[6], normal, new Vector2(0.75f, 0.25f)));
		}

		private void SetPlusYaxis(int indexShift, Vector3[] referPoints)
		{
			normal = new Vector3(0.0f, 1.0f, 0.0f);//Y
			SetCubeIndice(indexShift);
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[3], normal, new Vector2(1.0f, 0.0f)));//隱
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[0], normal, new Vector2(1.0f, 0.25f)));
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[5], normal, new Vector2(0.75f, 0.25f)));
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[4], normal, new Vector2(0.75f, 0.0f)));
		}

		private void SetPlusZaxis(int indexShift, Vector3[] referPoints)
		{
			normal = new Vector3(0.0f, 0.0f, 1.0f);//Z
			SetCubeIndice(indexShift);
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[0], normal, new Vector2(0.75f, 0.75f)));//敵
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[1], normal, new Vector2(0.75f, 1.0f)));
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[6], normal, new Vector2(0.5f, 1.0f)));
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[5], normal, new Vector2(0.5f, 0.75f)));
		}

		private void SetMinusXaxis(int indexShift, Vector3[] referPoints)
		{
			normal = new Vector3(-1.0f, 0.0f, 0.0f);//-X
			SetCubeIndice(indexShift);
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[5], normal, new Vector2(0.75f, 0.5f)));//蛋

			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[6], normal, new Vector2(0.75f, 0.75f)));
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[7], normal, new Vector2(0.5f, 0.75f)));
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[4], normal, new Vector2(0.5f, 0.5f)));
		}

		private void SetMinusZaxis(int indexShift, Vector3[] referPoints)
		{
			normal = new Vector3(0.0f, 0.0f, -1.0f);//-Z
			SetCubeIndice(indexShift);
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[4], normal, new Vector2(0.75f, 0.25f)));//特
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[7], normal, new Vector2(0.75f, 0.5f)));
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[2], normal, new Vector2(0.5f, 0.5f)));
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[3], normal, new Vector2(0.5f, 0.25f)));
		}

		private void SetPlusXaxis(int indexShift, Vector3[] referPoints)
		{
			SetCubeIndice(indexShift);
			normal = new Vector3(1.0f, 0.0f, 0.0f);//+X 
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[3], normal, new Vector2(0.75f, 0.0f)));//激
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[2], normal, new Vector2(0.75f, 0.25f)));
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[1], normal, new Vector2(0.5f, 0.25f)));
			cubeVertex.Add(
				new VertexPositionNormalTexture(
					referPoints[0], normal, new Vector2(0.5f, 0.0f)));
		}

		private void SetCubeIndice(int indexShift)
		{
			cubeIndice.Add((ushort) (indexShift + 0));
			cubeIndice.Add((ushort) (indexShift + 1));
			cubeIndice.Add((ushort) (indexShift + 2));

			cubeIndice.Add((ushort) (indexShift + 0));
			cubeIndice.Add((ushort) (indexShift + 2));
			cubeIndice.Add((ushort) (indexShift + 3));
		}

		#endregion

		#region cubes 所有移動
		/// <summary>
		/// 將實際移動或轉移，並紀錄
		/// </summary>
		/// <param name="changes"></param>
		public void MoveActionsRecord(Matrix changes)
		{
			cloneWorld*= changes;
			this.World = cloneWorld;
		}

		/// <summary>
		/// 移動或轉移，但不紀錄
		/// </summary>
		/// <param name="changes"></param>
		public void MoveActions(Matrix changes)
		{
			this.World = cloneWorld * changes;
		}
		#endregion
		
		#region 繪圖
		/// <summary>
		/// 繪出單一 cube
		/// </summary>
		public void Draw()
		{
			cubeBasicEffect.Texture = Picture;
			cubeBasicEffect.TextureEnabled = true;
			cubeBasicEffect.LightingEnabled = false;

			GraphicsDevice device = cubeBasicEffect.GraphicsDevice;
			device.SetVertexBuffer(cubeVertexBuffer);
			device.Indices = cubeIndexBuffer;

			foreach (EffectPass ep in cubeBasicEffect.CurrentTechnique.Passes)
			{
				ep.Apply();
				int primitivecount = cubeIndice.Count / 3;
				device.DrawIndexedPrimitives(
					PrimitiveType.TriangleList, 0, 0,cubeVertex.Count, 0, primitivecount);
			}
		}
		
		#endregion

	}
}
