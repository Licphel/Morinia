using Kinetic.Gui;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client;
using Morinia.Client.TheItem;
using Morinia.Content.TheAccessBridge;
using Morinia.Content.TheBlockEntity;
using Morinia.World.TheItem;
using Morinia.World.TheRecipe;

namespace Morinia.Content.TheGui;

public class GuiFurnace : GuiAccessBridge
{

	ElementImageDynamic d1, d2;

	public GuiFurnace(AccessBridge container)
	: base(container, GameTextures.GuiFurnace, new Vector2(176, 186))
	{
	}

	public override void InitComponents()
	{
		base.InitComponents();

		Join(d1 = new ElementImageDynamic(TexturePart.BySize(Background, 0, 186, 28, 17), ElementImageDynamic.Style.RightShrink));
		Join(d2 = new ElementImageDynamic(TexturePart.BySize(Background, 28, 186, 7, 13), ElementImageDynamic.Style.UpShrink));
	}

	public override void RelocateComponents()
	{
		base.RelocateComponents();

		d1.Bound.Set(i + 92, j + 126, 28, 17);
		d2.Bound.Set(i + 48, j + 111, 7, 13);
	}

	public override void Update(VaryingVector2 cursor)
	{
		base.Update(cursor);

		BlockEntityFurnace f = ((AccessBridgeFurnace) Bridge).Furnace;
		d1.Progress = FloatMath.SafeDiv(f.Cooktime, f.MaxTime);
		d2.Progress = FloatMath.SafeDiv(f.Fuel, f.MaxFuel);
	}

}
