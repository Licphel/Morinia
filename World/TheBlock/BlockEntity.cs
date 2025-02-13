using Kinetic;
using Kinetic.App;
using Kinetic.IO;
using Morinia.World.TheItem;

namespace Morinia.World.TheBlock;

public class BlockEntity
{

	public bool Dirty;

	public Level Level;
	public BlockState PeerBlock;
	public BlockPos Pos;

	public BlockEntity(BlockState state, Level level, BlockPos pos)
	{
		Level = level;
		Pos = pos;
		PeerBlock = state;
	}

	public virtual bool IsTickable => false;

	public virtual void Tick()
	{
	}

	public virtual void OnSpawned()
	{
	}

	//When peer block is destroyed. Invoked twice.
	public virtual void OnDespawned(bool isNewEntityPlaced)
	{
	}

	public virtual void OverrideBlockDrops(List<ItemStack> stacks)
	{
	}

	public virtual void Write(IBinaryCompound compound) { }

	public virtual void Read(IBinaryCompound compound) { }

}
