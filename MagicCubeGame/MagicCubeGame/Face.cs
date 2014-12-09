using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagicCubeGame
{
	public class Face
	{
		private GraphicsDevice gDevice;
		private VertexPositionNormalTexture[] vertexArray = new VertexPositionNormalTexture[6];
		private ushort[] indices = new ushort[6];
		private VertexBuffer vertexbuffer;
		private IndexBuffer indexbuffer;
		private BasicEffect faceEffect;
		private float scale;
		private Vector3 normalVector;

		private Texture2D picture;
		public Texture2D Picture
		{
			get { return picture; }
			set { picture = value; }
		}

		public Face(GraphicsDevice device,Vector3 normal, Texture2D pic,float scale)
		{
			this.scale = scale;
			this.normalVector = normal;
			Picture = pic;
			gDevice = device;
			SetRetangleFaceVertex();
			SetBuffers();
			SetEffect();
		}

		private void SetRetangleFaceVertex()
		{
			Vector3[] referPoints = new Vector3[]{new Vector3(0.5f,0.5f,0.0f)*scale,
																		new Vector3(0.5f,-0.5f,0.0f)*scale,
																		new Vector3(-0.5f,-0.5f,0.0f)*scale,
																		new Vector3(-0.5f,0.5f,0.0f)*scale};
			vertexArray[0] = new VertexPositionNormalTexture(
										referPoints[0], normalVector, new Vector2(0.0f, 0.0f));
			vertexArray[1] = new VertexPositionNormalTexture(
										referPoints[1], normalVector, new Vector2(1.0f, 0.0f));
			vertexArray[2] = new VertexPositionNormalTexture(
										referPoints[2], normalVector, new Vector2(1.0f, 1.0f));

			vertexArray[3] = new VertexPositionNormalTexture(
										referPoints[0], normalVector, new Vector2(0.0f, 0.0f));
			vertexArray[4] = new VertexPositionNormalTexture(
										referPoints[2], normalVector, new Vector2(1.0f, 1.0f));
			vertexArray[5] = new VertexPositionNormalTexture(
										referPoints[3], normalVector, new Vector2(0.0f, 1.0f));

			for (ushort i = 0; i < indices.Length; i++)
			{
				indices[i] = i;
			}

		}

		private void SetBuffers()
		{
			vertexbuffer = new VertexBuffer(gDevice,
				typeof(VertexPositionNormalTexture), vertexArray.Length, BufferUsage.WriteOnly);
			vertexbuffer.SetData<VertexPositionNormalTexture>(vertexArray);

			indexbuffer = new IndexBuffer(gDevice, typeof(ushort),
				indices.Length, BufferUsage.WriteOnly);
			indexbuffer.SetData<ushort>(indices);

		}

		private void SetEffect()
		{
			faceEffect = new BasicEffect(gDevice);
			faceEffect.EnableDefaultLighting();
		}

		public Matrix World 
		{
			get { return faceEffect.World; } 
			set { faceEffect.World = value; } 
		}

		public Matrix View 
		{
			get { return faceEffect.View ; } 
			set { faceEffect.View = value; } 
		}

		public Matrix Projection
		{ 
			get { return faceEffect.Projection ; } 
			set { faceEffect.Projection = value; } 
		}

		public void Draw()
		{
			faceEffect.Texture = Picture;
			faceEffect.TextureEnabled = true;
			//faceEffect.DiffuseColor = Color.BurlyWood.ToVector3();
			//faceEffect.LightingEnabled = false;

			GraphicsDevice device = faceEffect.GraphicsDevice;
			device.SetVertexBuffer(vertexbuffer);
			device.Indices = indexbuffer;

			foreach (EffectPass ep in faceEffect.CurrentTechnique.Passes)
			{
				ep.Apply();
				int primitivecount = vertexArray.Length / 3;
				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexArray.Length,
					0, primitivecount);
			}
		}
	}
}
