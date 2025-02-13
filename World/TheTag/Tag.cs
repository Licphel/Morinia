using Kinetic.App;

namespace Morinia.World.TheDict;

public abstract class Tag : IDHolder
{

	public abstract bool Contains<T>(T o) where T : IDHolder;

	public virtual IEnumerable<dynamic> GetContents()
	{
		yield return this;
	}

	public ID GetAbbreviatedID()
	{
		return new ID(Uid.Space, Uid.Key.Replace("tags/", "").Replace(".loh", ""));
	}

	public class HashSetTag : Tag
	{

		public HashSet<object> Constituents = new HashSet<object>();

		public override bool Contains<T>(T o)
		{
			return Constituents.Contains(o);
		}

		public override IEnumerable<dynamic> GetContents()
		{
			return Constituents;
		}

	}

	public static Tag Collect(string id) => Collect(new ID(id));

	public static Tag Collect(ID id)
	{
		Tag tag = TagManager.Get(id);
		if(tag != null)
		{
			//Try add back.
			TagManager._dictionary[id] = tag;
			return tag;
		}
		else
		{
			HashSetTag tag0 = new HashSetTag();
			foreach(var kv in TagManager._dictionary)
			{
				if(kv.Key.Space != id.Space)
					continue;
				if(!kv.Key.Key.StartsWith(id.Key))
					continue;
				foreach(var content in kv.Value.GetContents())
					tag0.Constituents.Add(content);
			}
			//Try add back.
			TagManager._dictionary[id] = tag0;
			return tag0;
		}
	}

	public static Tag Combine(string id, params Tag[] tags) => Combine(new ID(id), tags);

	public static Tag Combine(ID id, params Tag[] tags)
	{
		HashSetTag tagc = new HashSetTag();
		foreach(var tag in tags)
		{
			foreach(var o in tag.GetContents())
			{
				tagc.Constituents.Add(o);
			}
		}
		//Try add back.
		TagManager._dictionary[id] = tagc;
		return tagc;
	}

}