using Kinetic.Visual;
using Morinia.World;
using Morinia.World.TheBlock;

namespace Morinia.Client.TheBlock;

public interface BlockTessellator
{

	public static BlockTessellatorMasked Masked = new BlockTessellatorMasked();
	public static BlockTessellatorMaskedRepeat MaskedRepeat = new BlockTessellatorMaskedRepeat();
	public static BlockTessellatorMaskedRandcoord MaskedRandcoord = new BlockTessellatorMaskedRandcoord();

	public void Draw(SpriteBatch batch, Level level, Chunk chunk, BlockState state, int x, int y, int z);

	public void DrawItemSymbol(SpriteBatch batch, BlockState state, float x, float y, float w, float h);

	public void SetState(SpriteBatch batch);

	public void ResetState(SpriteBatch batch);

	//requirements:
	//painter == null: false
	//painter == self: true
	public bool IsInSameState(BlockTessellator tessellator)
	{
		return tessellator == this;
	}

}
