using Kinetic.Visual;
using Morinia.Client.TheItem;
using Morinia.World.TheItem;
using Morinia.World.ThePhysic;

namespace Morinia.Client.TheEntityBone;

public class Bone
{

	protected float AnchorX, AnchorY;
	protected bool Flip;
	protected ItemStack Held = ItemStack.Empty;
	protected Icon Icon;

	protected float px, py;
	protected float Swing;
	protected float w, h;

	public Bone SetIcon(Icon icon)
	{
		Icon = icon;
		return this;
	}

	public Bone Anchor(int x, int y)
	{
		AnchorX = x + 0.5f;
		AnchorY = y + 0.5f;
		return this;
	}

	public Bone Resize(float w, float h)
	{
		this.w = w;
		this.h = h;
		return this;
	}

	public void SetSwing(float swing)
	{
		Swing = swing;
	}

	public void SetHeldItemStack(ItemStack held)
	{
		Held = held;
	}

	public virtual void FindPosRelatively(Entity entity)
	{
		Flip = entity.Face.x < 0;
	}

	public void Draw(SpriteBatch batch, Entity entity, float x, float y)
	{
		MatrixStack matrixStack = batch.Matrices;

		matrixStack.Push();
		matrixStack.Translate(px / 16f, py / 16f);
		matrixStack.RotateDeg(Flip ? 180 - Swing : Swing, AnchorX / 16f, AnchorY / 16f);
		matrixStack.Translate(AnchorX / 16f + (w - 1) / 16f, AnchorY / 16f - 0.25f);
		ItemRendering.GetTessellator(Held).Draw(batch, x, y, 0.5f, 0.5f, Held, false);
		matrixStack.Pop();

		matrixStack.Push();
		matrixStack.Translate(px / 16f, py / 16f);
		matrixStack.RotateDeg(Flip ? 180 - Swing : Swing, AnchorX / 16f, AnchorY / 16f);
		batch.Draw(Icon, x, y, w / 16f, h / 16f);
		matrixStack.Pop();
	}

}
