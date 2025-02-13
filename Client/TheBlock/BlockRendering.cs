using Kinetic.App;
using Kinetic.IO;
using Kinetic.Visual;
using Loh;
using Loh.Values;
using Morinia.Content;
using Morinia.Util;
using Morinia.World;
using Morinia.World.TheBlock;

namespace Morinia.Client.TheBlock;

public class BlockRendering
{

	static Dictionary<IntPair, BlockRenderData> DataMap = new();

	class BlockRenderData
	{

		public BlockTessellator Tessellator;
		public Icon Icon;
		public Texture Mask;

	}

	public static void Load(ID resource, FileHandle file)
	{
		Block block = Blocks.Registry[resource.Space + ":" + file.Name];
		IBinaryList array = LohEngine.Exec(file);

		foreach(IBinaryCompound o in array)
		{
			int meta = o.Get<int>("meta");
			string path = o.Get<string>("texture_path").Replace("${self}", file.Name);
			string maskpath = o.Get<string>("mask_path");
			string tes = o.Get<string>("tessellator");
			BlockRenderData data = new BlockRenderData();
			data.Icon = Loads.Get(path);
			data.Tessellator = tes switch
			{
				"masked_repeat" => BlockTessellator.MaskedRepeat,
				"masked" => BlockTessellator.Masked,
				"masked_randcoord" => BlockTessellator.MaskedRandcoord,
			};
			data.Mask = Loads.Get(maskpath);
			DataMap[IntPair.Create(block.Uid, meta)] = data;
		}
	}

	public static BlockTessellator GetTessellator(BlockState state)
	{
		return DataMap[state.StateUid].Tessellator;
	}

	public static Icon GetIcon(BlockState state)
	{
		return DataMap[state.StateUid].Icon;
	}

	public static Texture GetMask(BlockState state)
	{
		return DataMap[state.StateUid].Mask;
	}

	public static bool IsConnectable(BlockState inst, BlockState other, Direction direction)
	{
		return other.GetShape().IsFull && inst.GetShape().IsFull;
	}

	public static bool IsSpreadable(BlockState inst, BlockState other, Direction direction)
	{
		return !other.IsStrictly(inst);
	}

}
