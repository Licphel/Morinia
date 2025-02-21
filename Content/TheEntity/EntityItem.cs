using Kinetic;
using Kinetic.App;
using Kinetic.IO;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client.TheEntity;
using Morinia.Client.TheItem;
using Morinia.World;
using Morinia.World.TheItem;
using Morinia.World.ThePhysic;

namespace Morinia.Content.TheEntity;

public class EntityItem : Entity
{

	public ItemStack Stack = ItemStack.Empty;
	public float Protection = 0;

	public EntityItem()
	{
		Box.Resize(8 / 16f, 8 / 16f);
		VisualSize.x = 8 / 16f;
		VisualSize.y = 8 / 16f;
	}

	public override EntityBuilder Builder => Entities.Item;

	public static void PopDrop(Level level, List<ItemStack> stacks, IPos pos)
	{
		foreach(ItemStack stack in stacks)
		{
			if(stack.IsEmpty)
				return;
			Entity itemEntity = new EntityItem { Stack = stack };
			float rx = Seed.Global.NextFloat(-0.25f, 0.25f);
			float ry = Seed.Global.NextFloat(-0.25f, 0.25f);
			itemEntity.Velocity.x = Seed.Global.NextFloat(0f, 2f);
			itemEntity.Velocity.y = Seed.Global.NextFloat(-2f, 2f);
			level.AddEntity(itemEntity, pos.x + rx, pos.y + ry);
		}
	}

	public override void Tick()
	{
		base.Tick();

		ApplyGravity(Vector2.One);
		RestrictVelocity();
		if(TouchHorizontal) Velocity.x *= 0.95f;
		if(TouchVertical) Velocity.y *= 0.95f;
		Move();
		CheckVelocity();

		if(Protection <= 0 && TimeSchedule.PeriodicTask(2))
		{
			Box box2 = Box;
			box2.Scale(2, 2);
			List<Entity> lst = Level.GetNearbyEntities(box2, e => e is EntityItem);
			if(lst.Count > 4)
				lst.ForEach(e => Absorb((EntityItem) e));
		}

		Protection -= Time.Delta;
	}

	void Absorb(EntityItem e)
	{
		if(e == this)
		{
			return;
		}

		if(Stack.Count >= Stack.MaxStackSize)
		{
			return;
		}

		ItemStack i1 = e.Stack;

		if(i1.Count > Stack.Count || i1.IsEmpty)
		{
			return;
		}

		if(Stack.CanMergePartly(i1))
		{
			ItemStack remain = Stack.Merge(i1);
			e.Stack = remain;

			if(e.Stack.IsEmpty)
			{
				e.IsDead = true;
			}
		}
	}

	//wonn't be fully picked up sometimes.
	public void Pickup(Inventory inv)
	{
		ItemStack trystack = inv.Add(Stack.Copy());
		if(trystack.IsEmpty)
		{
			IsDead = true;
		}
		Stack = trystack;
	}

	public override void Write(IBinaryCompound compound)
	{
		base.Write(compound);
		compound.Set("stack", ItemStack.Serialize(Stack));
	}

	public override void Read(IBinaryCompound compound)
	{
		base.Read(compound);
		Stack = ItemStack.Deserialize(compound.GetCompoundSafely("stack"));
	}

	public override EntityTessellator GetTessellator()
	{
		return Tessellator.Value0;
	}

	class Tessellator : EntityTessellator
	{

		public static readonly Tessellator Value0 = new Tessellator();

		public void Draw(SpriteBatch batch, Entity entity, Box box)
		{
			EntityItem eitem = (EntityItem) entity;
			int c = eitem.Stack.Count;
			if(c >= 3) c = 3;

			batch.Color4(entity.SmoothedLight);
			batch.Matrices.Push();
			batch.Matrices.RotateRad(entity.VeloRad, 4 / 16f, 4 / 16f);
			ItemRendering.GetTessellator(eitem.Stack).Draw(batch, box, eitem.Stack, false);
			batch.Matrices.Pop();
			batch.NormalizeColor();
		}

	}

}
