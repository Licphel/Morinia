using Kinetic.App;
using Kinetic.IO;
using Morinia.Content;
using Morinia.World.TheBlock;
using Morinia.World.TheItem;

namespace Morinia.World.TheDict;

public class TagManager
{

	public static Dictionary<ID, Tag> _dictionary = new Dictionary<ID, Tag>();

	public static Tag Get(ID key)
	{
		return _dictionary.GetValueOrDefault(key.Relocate("tags/", ".loh"), null);
	}

	public static Tag Get(string key)
	{
		return Get(new ID(key));
	}

	public static void Parse(ID id, IBinaryList list, dynamic reg)
	{
		HashSet<object> set = new HashSet<object>();

		foreach(string s in list)
		{
			set.Add(reg[s]);
		}

		_dictionary[id] = new Tag.HashSetTag() { Constituents = set, Uid = id };
	}

}
