using Morinia.Client.TheAudio;

namespace Morinia.World.TheBlock;

public class BlockMaterial
{

	public static BlockMaterial Unknown = new BlockMaterial(Sound.None, Sound.None, Sound.None);
	public static BlockMaterial Powder = new BlockMaterial(Sound.BlockBreakPowder, Sound.BlockBreakPowder, Sound.BlockDestructPowder);
	public static BlockMaterial Stone = new BlockMaterial(Sound.None, Sound.None, Sound.BlockDestructPowder);
	public static BlockMaterial Metal = new BlockMaterial(Sound.None, Sound.None, Sound.BlockDestructPowder);
	public static BlockMaterial Wooden = new BlockMaterial(Sound.None, Sound.None, Sound.BlockDestructPowder);
	public static BlockMaterial Glass = new BlockMaterial(Sound.None, Sound.None, Sound.BlockDestructPowder);
	public static BlockMaterial Foliage = new BlockMaterial(Sound.None, Sound.None, Sound.BlockDestructPowder);

	public Sound SoundStep;
	public Sound SoundBreak;
	public Sound SoundDestruct;

	public BlockMaterial(Sound sound1, Sound sound2, Sound sound3)
	{
		SoundStep = sound1;
		SoundBreak = sound2;
		SoundDestruct = sound3;
	}

}
