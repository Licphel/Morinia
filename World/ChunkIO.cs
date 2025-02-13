using Kinetic.IO;
using Kinetic.Math;

namespace Morinia.World;

public class ChunkIO
{

	readonly Dictionary<int, ChunkBuffer> buffers = new Dictionary<int, ChunkBuffer>();

	readonly FileHandle file0;
	readonly List<int> forremove = new List<int>();

	public ChunkIO()
	{
		file0 = FileSystem.GetLocal("world/level_" + 0);
		file0.Mkdirs();
	}

	public void _CheckOldBuffers(Level level)
	{
		foreach(KeyValuePair<int, ChunkBuffer> kv in buffers)
		{
			if(!kv.Value.IsBufferAvailable(level))
			{
				forremove.Add(kv.Key);
			}
		}
		foreach(int i in forremove)
		{
			buffers.Remove(i);
		}
		forremove.Clear();
	}

	public ChunkBuffer _Buffer(Chunk chunk)
	{
		int coord = FloatMath.Floor((float) chunk.Coord / ChunkBuffer.Size);
		if(buffers.ContainsKey(coord))
		{
			return buffers[coord];
		}
		buffers[coord] = new ChunkBuffer(file0, coord);
		return buffers[coord];
	}

	public bool IsChunkArchived(Chunk chunk)
	{
		return _Buffer(chunk).IsChunkArchived(chunk);
	}

	public void Read(Chunk chunk)
	{
		_Buffer(chunk).TryRead(chunk);
	}

	public void WriteToBuffer(Chunk chunk, bool removal = false)
	{
		_Buffer(chunk).WriteToBuffer(chunk, removal);
	}

	public void WriteToDisk()
	{
		foreach(ChunkBuffer buffer in buffers.Values)
		{
			buffer.TryWrite();
		}
	}

}
