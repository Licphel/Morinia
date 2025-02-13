using Kinetic.Math;
using Kinetic.Visual;
using Morinia.World.ThePhysic;

namespace Morinia.Client.TheEntityBone;

public class BonedRendering
{

	public static void Draw(SpriteBatch batch, BoneGroup group, Entity entity, Box box)
	{
		List<Bone> begin = group.Begin, end = group.End;

		if(entity.Face.x < 0)
		{
			begin = group.End;
			end = group.Begin;
		}

		foreach(Bone b in begin)
		{
			b.FindPosRelatively(entity);
			b.Draw(batch, entity, box.x, box.y);
		}

		batch.Draw(group.Body, box);

		foreach(Bone b in end)
		{
			b.FindPosRelatively(entity);
			b.Draw(batch, entity, box.x, box.y);
		}
	}

}
