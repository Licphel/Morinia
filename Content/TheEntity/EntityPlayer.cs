using Kinetic;
using Kinetic.App;
using Kinetic.IO;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client;
using Morinia.Client.TheEntity;
using Morinia.Client.TheEntityBone;
using Morinia.Logic;
using Morinia.World;
using Morinia.World.TheItem;
using Morinia.World.ThePhysic;

namespace Morinia.Content.TheEntity;

public class EntityPlayer : Creature
{

	static readonly int loadArea = 3;
	public ItemStack GuiHeldStack = ItemStack.Empty;

	public Inventory Inv;
	public int InvCursor = 0;
	public bool IsPlayerControl;
	public AccessBridge OpenContainer;

	readonly Tessellator TheTessellator;

	public EntityPlayer(int yof = 36)
	{
		Box.Resize(8 / 16f, 18 / 16f);
		VisualSize.x = 12 / 16f;
		VisualSize.y = 18 / 16f;
		Inv = new Inventory(27);
		TheTessellator = new Tessellator(yof);
		Health = MaxHealth = 1000;
		Mana = MaxMana = 1000;
		Hunger = MaxHunger = 1000;
		Thirst = MaxThirst = 1000;
	}

	public override EntityBuilder Builder => Entities.Player;

	public override void Tick()
	{
		base.Tick();

		ApplyGravity(Vector2.One);
		RestrictVelocity();
		Move();
		CheckVelocity();

		Box box2 = Box;
		box2.Scale(4f, 3f);
		List<Entity> lst = Level.GetNearbyEntities(box2, e => e is EntityItem itee && itee.Protection <= 0);

		foreach(Entity e in lst)
		{
			EntityItem e1 = (EntityItem) e;
			Box buf = Box;
			buf.Scale(0.5f, 0.5f);

			if(buf.Interacts(e.Box))
			{
				e1.Pickup(Inv);
			}
			else
			{
				ItemStack stack = e1.Stack;

				if(!Inv.Add(stack.Copy(), true).Is(stack))
				{
					float spd = 2f / Posing.Distance(e.Pos, Pos) + e.Velocity.Len;
					float rad = Posing.PointRad(e.Pos, Pos);
					e.Velocity.x = spd * FloatMath.CosRad(rad);
					e.Velocity.y = spd * FloatMath.SinRad(rad);
				}
			}
		}

		UpdateLoadingChunks(Level, ChunkUnit.Pos.UnitX);
	}

	public override void Write(IBinaryCompound compound)
	{
		base.Write(compound);
		compound.Set("guistack", ItemStack.Serialize(GuiHeldStack));
		compound.Set("pinv", Inventory.Serialize(Inv));
	}

	public override void Read(IBinaryCompound compound)
	{
		base.Read(compound);
		GuiHeldStack = ItemStack.Deserialize(compound.GetCompoundSafely("guistack"));
		Inv = Inventory.Deserialize(compound.GetCompoundSafely("pinv"));
	}

	public void UpdateLoadingChunks(Level level, int sx)
	{
		if(!IsPlayerControl) return;

		for(int i = sx - loadArea - 1; i < sx + loadArea + 1; i++)
		{
			level.Generator.ProvideAsync(i);

			Chunk chunk = level.GetChunk(i);

			if(chunk == null) continue;

			chunk.RefreshTime = Chunk.RefreshTimeNormal;
		}
	}

	public override float CastLight(byte pipe)
	{
		return 1f;
	}

	public override EntityTessellator GetTessellator()
	{
		return TheTessellator;
	}

	class Tessellator : EntityTessellator
	{

		readonly BoneGroup group;
		readonly Bone HAND_LEFT;
		readonly Bone HAND_RIGHT;

		readonly IconAnim standing;
		readonly IconAnim walking;

		float lastSwing;

		public Tessellator(int yof)
		{
			standing = new IconAnim(GameTextures.EntityBiped, 6, 12, 18, 0, yof).Seconds(0.25f);
			walking = new IconAnim(GameTextures.EntityBiped, 6, 12, 18, 0, 18 + yof);
			HAND_RIGHT = GetHand(yof);
			HAND_LEFT = GetHand(yof);
			group = new BoneGroup();
			group.AddToBack(HAND_LEFT);
			group.AddToFront(HAND_RIGHT);
		}

		public void Draw(SpriteBatch batch, Entity entity, Box box)
		{
			EntityPlayer player = (EntityPlayer) entity;

			VaryingVector2 cursor = Game.GameLogic.Transform.Cursor;

			float deg = player.IsPlayerControl
			? FloatMath.AtanDeg(-box.ycentral + cursor.y, -box.xcentral + cursor.x)
			: -90;
			HAND_RIGHT.SetSwing(deg);
			HAND_RIGHT.SetHeldItemStack(player.Inv[player.InvCursor]);

			float nowSwing = -90 + -entity.Velocity.x * 4;
			HAND_LEFT.SetSwing(Time.Lerp(lastSwing, nowSwing));
			lastSwing = nowSwing;

			group.SetBody(GetAnim(player));

			batch.Color4(entity.SmoothedLight);
			if(player.HurtCooldown > 0)
			{
				batch.Merge4(1, 0.3f, 0.3f);
			}
			MatrixStack matrixStack = batch.Matrices;
			matrixStack.Push();
			matrixStack.Translate(box.w / 2, box.h / 2);
			matrixStack.Scale(entity.Face.x < 0 ? -1 : 1, 1);
			matrixStack.Translate(-box.w / 2, -box.h / 2);
			BonedRendering.Draw(batch, group, entity, box);
			matrixStack.Pop();

			batch.NormalizeColor();
		}

		static Bone GetHand(int yof)
		{
			return new HandPlayer()
			.SetIcon(TexturePart.BySize(GameTextures.EntityBiped, 73, yof, 5, 1))
			.Anchor(0, 0)
			.Resize(5, 1);
		}

		public IconAnim GetAnim(EntityPlayer entity)
		{
			IconAnim frame;

			if(Math.Abs(entity.Velocity.x) < 0.1f || entity.TouchLeft || entity.TouchRight)
			{
				return standing;
			}
			frame = walking;

			frame.Seconds(0.35f - Math.Min(0.27f, Math.Abs(entity.Velocity.x / 20f)));
			return frame;
		}

		class HandPlayer : Bone
		{

			public override void FindPosRelatively(Entity entity)
			{
				base.FindPosRelatively(entity);
				px = 5;
				py = 8;
			}

		}

	}

}
