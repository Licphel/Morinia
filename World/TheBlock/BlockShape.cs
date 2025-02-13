namespace Morinia.World.TheBlock;

public delegate float FilterLight(BlockState i, int pipe, float v, int x, int y);

public class BlockShape
{

	public static readonly BlockShape Solid = new BlockShape((i, p, v, x, y) => v * 0.8f - 0.05f, true);
	public static readonly BlockShape Parital = new BlockShape((i, p, v, x, y) => v * 0.9f - 0.025f, true);
	public static readonly BlockShape Hollow = new BlockShape((i, p, v, x, y) => v * 0.95f - 0.015f, false);
	public static readonly BlockShape Vacuum = new BlockShape((i, p, v, x, y) => v * 0.95f - 0.01f, false);

	public FilterLight FilterLight;
	public bool IsFull;

	public BlockShape(FilterLight fl, bool ful)
	{
		FilterLight = fl;
		IsFull = ful;
	}

}
