using Kinetic.App;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Logic;
using Morinia.World.ThePhysic;

namespace Morinia.Client.TheEntity;

public interface EntityTessellator
{

	public void Draw(SpriteBatch batch, Entity entity, Box box);

	public void Draw(SpriteBatch batch, Entity entity)
	{
		Box smoothbox = new Box();
		smoothbox.w = entity.VisualSize.x;
		smoothbox.h = entity.VisualSize.y;
		smoothbox.xcentral = Time.Lerp(entity.PosLastTick.x, entity.Pos.x);
		smoothbox.ycentral = Time.Lerp(entity.PosLastTick.y, entity.Pos.y);
		Draw(batch, entity, smoothbox);
	}

}
