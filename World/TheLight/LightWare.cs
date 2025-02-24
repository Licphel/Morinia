using System.Runtime.CompilerServices;
using Kinetic.Math;
using Morinia.World.TheBlock;

namespace Morinia.World.TheLight;

public unsafe class LightWare
{

	public Chunk chunk;
	public float[][] coords = _newarray(4, 3);
	public float[] coordsAO = new float[4];

	public float[][] coordsOut = _newarray(4, 3);

	public float[] rgb = new float[3];
	public int type;

	public LightWare(Chunk chunk, int t)
	{
		this.chunk = chunk;
		type = t;
	}

	static float[][] _newarray(int x, int y)
	{
		float[][] farr = new float[x][];
		for(int i = 0; i < x; i++)
			farr[i] = new float[y];
		return farr;
	}

	public static void merge3(float[] _arr, float x, float y, float z)
	{
		fixed(float* arr = _arr)
		{
			arr[0] *= x;
			arr[1] *= y;
			arr[2] *= z;
		}
	}

	public static void merge3(float[] _arr, float x)
	{
		fixed(float* arr = _arr)
		{
			arr[0] *= x;
			arr[1] *= x;
			arr[2] *= x;
		}
	}

	public void GetVec4Out(ref Vector4 v, int i, float a)
	{
		float[] _arr = coordsOut[i];
		fixed(float* arr = _arr)
		{
			v.x = arr[0] * LightEngine.Amplifier;
			v.y = arr[1] * LightEngine.Amplifier;
			v.z = arr[2] * LightEngine.Amplifier;
			v.w = 1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void writeCoords(float[][] color)
	{
		fixed(float[]* coords = this.coords)
		{
			Array.Copy(coords[0], 0, color[0], 0, 3);
			Array.Copy(coords[1], 0, color[1], 0, 3);
			Array.Copy(coords[2], 0, color[2], 0, 3);
			Array.Copy(coords[3], 0, color[3], 0, 3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void extendAO(float[][] _color, float addit = 1)
	{
		fixed(float* coordsAO = this.coordsAO)
		{
			fixed(float[]* color = _color)
			{
				merge3(color[0], coordsAO[0] * addit);
				merge3(color[1], coordsAO[1] * addit);
				merge3(color[2], coordsAO[2] * addit);
				merge3(color[3], coordsAO[3] * addit);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void extendCoords(float[][] _color, float[] sunlight)
	{
		fixed(float[]* color = _color)
		{
			for(int i = 0; i < 4; i++)
			{
				fixed(float* c2 = coords[i])
				fixed(float* c = color[i])
				{
					c[0] = Math.Max(sunlight[0] * c2[0], c[0]);
					c[1] = Math.Max(sunlight[1] * c2[1], c[1]);
					c[2] = Math.Max(sunlight[2] * c2[2], c[2]);
				}
			}
		}
	}

	void set(int i, float v, int idx)
	{
		coords[i][idx] = v;
	}

	void setAO(int i, float v)
	{
		coordsAO[i] = v;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void updateCoords(int x, int y, int idx)
	{
		LightEngine le = chunk.Level.LightEngine;

		Chunk c0 = le.GetBufferedChunk(x - 1);
		Chunk c1 = le.GetBufferedChunk(x);
		Chunk c2 = le.GetBufferedChunk(x + 1);

		if(c0 == null || c1 == null || c2 == null) return;

		BlockState b = c1.GetBlock(x, y);

		const float aoS = 0.1f;

		int c = 0;
		if(b.GetShape() == BlockShape.Solid)
		{
			setAO(0, 1 - aoS * 1.5f);
			setAO(1, 1 - aoS * 1.5f);
			setAO(2, 1 - aoS * 1.5f);
			setAO(3, 1 - aoS * 1.5f);
		}
		else
		{
			bool bcc1, bcc3, bcc4, bcc6;
			BlockState b0 = c0.GetBlock(x - 1, y - 1);
			BlockState b1 = c0.GetBlock(x - 1, y);
			BlockState b2 = c0.GetBlock(x - 1, y + 1);

			BlockState b3 = c1.GetBlock(x, y - 1);
			BlockState b4 = c1.GetBlock(x, y + 1);

			BlockState b5 = c2.GetBlock(x + 1, y - 1);
			BlockState b6 = c2.GetBlock(x + 1, y);
			BlockState b7 = c2.GetBlock(x + 1, y + 1);

			if(b0.GetShape() == BlockShape.Solid) c++;
			if(bcc1 = b1.GetShape() == BlockShape.Solid) c++;
			if(bcc3 = b3.GetShape() == BlockShape.Solid) c++;
			setAO(0, 1 - c * aoS);

			c = 0;
			if(bcc1) c++;
			if(b2.GetShape() == BlockShape.Solid) c++;
			if(bcc4 = b4.GetShape() == BlockShape.Solid) c++;
			setAO(1, 1 - c * aoS);

			c = 0;
			if(bcc4) c++;
			if(bcc6 = b6.GetShape() == BlockShape.Solid) c++;
			if(b7.GetShape() == BlockShape.Solid) c++;
			setAO(2, 1 - c * aoS);

			c = 0;
			if(bcc3) c++;
			if(b5.GetShape() == BlockShape.Solid) c++;
			if(bcc6) c++;
			setAO(3, 1 - c * aoS);
		}

		float arr1 = c0._LE_Surfpack(x - 1, y, type).rgb[idx];
		float arr2 = c2._LE_Surfpack(x + 1, y, type).rgb[idx];
		float arr3 = c1._LE_Surfpack(x, y - 1, type).rgb[idx];
		float arr4 = c1._LE_Surfpack(x, y + 1, type).rgb[idx];
		float arr5 = c0._LE_Surfpack(x - 1, y - 1, type).rgb[idx];
		float arr6 = c2._LE_Surfpack(x + 1, y + 1, type).rgb[idx];
		float arr7 = c0._LE_Surfpack(x - 1, y + 1, type).rgb[idx];
		float arr8 = c2._LE_Surfpack(x + 1, y - 1, type).rgb[idx];
		float l0 = rgb[idx];

		set(0, (l0 + arr1 + arr3 + arr5) / 4, idx);
		set(1, (l0 + arr1 + arr4 + arr7) / 4, idx);
		set(2, (l0 + arr2 + arr4 + arr6) / 4, idx);
		set(3, (l0 + arr2 + arr3 + arr8) / 4, idx);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void active(int x, int y)
	{
		updateCoords(x, y, 0);
		updateCoords(x, y, 1);
		updateCoords(x, y, 2);
	}

}
