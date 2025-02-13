using Kinetic.App;
using Kinetic.IO;
using Morinia.Content;
using Morinia.World.TheItem;

namespace Morinia.World.TheLoot;

public class LootManager
{

	static Dictionary<ID, Loot> _dictionary = new Dictionary<ID, Loot>();

	public static Loot Get(ID key)
	{
		return _dictionary.GetValueOrDefault(key.Relocate("loots/", ".loh"), null);
	}

	public static Loot Get(string key)
	{
		return Get(new ID(key));
	}

	public static void Parse(ID id, IBinaryList list)
	{
		HashSet<object> set = new HashSet<object>();

		Loot loot = new Loot();

		foreach(IBinaryCompound compound in list)
		{
			Item item = Items.Registry[compound.Get<string>("id")];
			ItemStack stack = item.Instantiate(compound.Get<int>("count", 1));
			bool bonus = compound.Get<bool>("bonus", false);
			float chance = compound.Get<float>("chance", 1);
			int times = compound.Get<int>("times", 1);
			Loot.Roll roll = new Loot.Roll() { Stack = stack, Chance = chance, Times = times };
			if(bonus)
				loot.BonusRolls.Add(roll);
			else
				loot.Rolls.Add(roll);
		}

		_dictionary[id] = loot;
	}

}
