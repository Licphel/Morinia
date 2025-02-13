using Kinetic.App;
using Kinetic.IO;
using Morinia.Logic;
using Morinia.Util;

namespace Morinia.Api;

public class Mod
{

	public ModAssembly Assembly;
	public string Description;

	public FileHandle ModFileRoot;//The root directory of the mod.
	public FileHandle ModFileContents;//The contents directory of the mod.

	public string Namespace;
	public SemanticVersion Version;

	//Here, a mod should scan in your #this.ModFileRoot and add a subloader to loader.
	//Please go to #Morinia.Morinia to learn how to use it.
	public virtual void Initialize(LoadingQueue loader) { }

	//Utilities methods

	public LoadingQueue GetUploadQueue()
	{
		LoadingQueue upl = GameResourceProcessing.GetUploadQueueWithProcessors(Namespace);
		upl.Filebase = ModFileContents;
		return upl;
	}

	public LoadingQueue GetOverridingQueue()
	{
		LoadingQueue upl = GameResourceProcessing.GetUploadQueueWithProcessors(Game.Namespace);
		upl.Filebase = ModFileRoot.Goto("overrides");
		return upl;
	}

}
