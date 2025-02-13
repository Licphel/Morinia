using Morinia.World.TheBlock;

namespace Morinia.World.TheItem;

public class ItemToolType
{

	public static ItemToolType None = new ItemToolType();
	public static ItemToolType Pickaxe = new ItemToolType();
	public static ItemToolType Axe = new ItemToolType();
	public static ItemToolType Shovel = new ItemToolType();

	static ItemToolType()
	{
		Pickaxe.AddTargetMaterial(BlockMaterial.Stone, BlockMaterial.Glass, BlockMaterial.Metal);
		Axe.AddTargetMaterial(BlockMaterial.Wooden, BlockMaterial.Foliage);
		Shovel.AddTargetMaterial(BlockMaterial.Powder);
	}

	HashSet<BlockMaterial> MaterialsDiggable = new HashSet<BlockMaterial>();

	public void AddTargetMaterial(params BlockMaterial[] mat)
	{
		foreach(var mat1 in mat)
			MaterialsDiggable.Add(mat1);
	}

	public bool IsTarget(BlockMaterial mat)
	{
		return MaterialsDiggable.Contains(mat);
	}

}
