using System.Reflection;
using System.Text;
using Kinetic.App;
using Kinetic.IO;
using Loh;
using Morinia.Logic;

namespace Morinia.Api;

public class ModAssembly
{

	public static Dictionary<string, Mod> Mods = new Dictionary<string, Mod>();

	public Assembly LibColct;
	public Type MainClass;
	public Mod MainObj;

	public static void InitMods(LoadingQueue uploader)
	{
		foreach(Mod m in Mods.Values)
		{
			m.Initialize(uploader);
			Logger.Info($"Init mod '{m.Namespace}' in directory {m.ModFileRoot.Name}.");
		}
	}

	public static void ScanLoad(FileHandle root)
	{
		foreach(FileHandle file in root.Directories)
			Load(file);
	}

	public static ModAssembly Load(FileHandle file)
	{
		if(file.Goto("disabled").Exists) return null;

		try
		{
			IBinaryCompound compound = LohEngine.Exec(file.Goto("modmain.loh"));

			string mid = compound.Get<string>("id");
			string mv = compound.Get<string>("version");
			int mvi = compound.Get<int>("version_iteration");
			string mdesc = compound.Get<string>("description");
			string mtype = compound.Get<string>("main_class");
			string mdll = compound.Get<string>("library");

			Assembly assembly = Assembly.LoadFrom(file.Goto(mdll).Path);

			ModAssembly ma = new ModAssembly();

			ma.LibColct = assembly;
			ma.MainClass = assembly.GetType(mtype);
			ma.MainObj = (Mod) Activator.CreateInstance(ma.MainClass);

			ma.MainObj.ModFileRoot = file;
			ma.MainObj.ModFileContents = file.Goto("contents");
			ma.MainObj.Assembly = ma;
			ma.MainObj.Namespace = mid;
			ma.MainObj.Description = mdesc;
			ma.MainObj.Version = new SemanticVersion(mv);

			Mods[mid] = ma.MainObj;
			return ma;
		}
		catch(Exception e)
		{
			Logger.Fatal("Mod loading failed: " + e.Message);
		}

		return null;
	}

	public static FileHandle GetFile(ID idt)
	{
		if(idt.Space == ID.DefaultNamespace)
			return FileSystem.GetLocal("contents/" + idt.Key);
		Mod mod = Mods[idt.Space];
		return mod.ModFileRoot.Goto("contents/" + idt.Key);
	}

	public static string GetCombinedVersionIdentity()
	{
		StringBuilder k = new StringBuilder();
		k.AppendJoin('-', Game.Version.Iteration);
		foreach(Mod mod in Mods.Values)
		{
			k.AppendJoin('-', mod.Namespace + ":" + mod.Version.Iteration);
		}
		return k.ToString();
	}

}
