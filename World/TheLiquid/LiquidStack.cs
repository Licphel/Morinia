namespace Morinia.World.TheLiquid;

public ref struct LiquidStack
{

	public int Amount;
	public Liquid Liquid;
	public float Percent => (float) Amount / Liquid.MaxAmount;

	public LiquidStack(Liquid liquid, int amount)
	{
		Amount = Math.Clamp(amount, 0, Liquid.MaxAmount);
		Liquid = amount == 0 ? Liquid.Empty : liquid;
	}

}
