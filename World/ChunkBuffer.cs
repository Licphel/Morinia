using Kinetic.IO;
using Kinetic.Math;

namespace Morinia.World;

public class ChunkBuffer
{

	public static int Size = 16;

	readonly int coord;
	readonly FileHandle file;

	IBinaryCompound temp;

	public ChunkBuffer(FileHandle file0, int coord)
	{
		this.coord = coord;
		file = file0.Goto("chunks_" + coord + ".bin");
	}

	public void TryWrite()
	{
		if(!file.Exists)
		{
			file.Mkfile();
		}

		BinaryIO.Write(temp, file);
	}

	public void TryRead(Chunk chunk)
	{
		readTemp();
		if(temp.Try("chunk_" + chunk.Coord, out dynamic compound1))
		{
			chunk.Read(compound1);
		}
	}

	public void WriteToBuffer(Chunk chunk, bool removal = false)
	{
		readTemp();
		IBinaryCompound compound1 = IBinaryCompound.New();
		chunk.Write(compound1, removal);
		temp.Set("chunk_" + chunk.Coord, compound1);
	}

	public bool IsChunkArchived(Chunk chunk)
	{
		readTemp();

		return temp.Has("chunk_" + chunk.Coord);
	}

	void readTemp()
	{
		if(temp == null)
		{
			if(file.Exists)
			{
				temp = BinaryIO.Read(file);
			}
			else
			{
				temp = IBinaryCompound.New();
			}
		}
	}

	public bool IsBufferAvailable(Level level)
	{
		foreach(Chunk chunk in level.ActiveChunks)
		{
			if(FloatMath.Floor((float) chunk.Coord / Size) == coord)
			{
				return true;
			}
		}
		return false;
	}

}
