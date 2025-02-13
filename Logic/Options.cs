using Kinetic.App;
using Kinetic.Gui;
using Kinetic.IO;
using Loh;

namespace Morinia.Logic;

public class Options
{

	public static string LanguageGroup;
	public static IBinaryList G_Scsize;
	public static bool G_Ico, G_Maxm, G_Autor;
	public static string G_RwTitle, G_CL, G_IL;
	public static IBinaryList G_Hotspot;

	public static void Load()
	{
		IBinaryCompound compound = LohEngine.Exec(Game.OptionsFile);

		LanguageGroup = compound.Get<string>("language_group");

		G_Scsize = compound.Search<IBinaryList>("graphics.scaled_size");
		G_CL = compound.Search<string>("graphics.cursor_loc");
		G_Hotspot = compound.Search<IBinaryList>("graphics.cursor_hotspot");
		G_IL = compound.Search<string>("graphics.icon_loc");
		G_RwTitle = compound.Search<string>("graphics.title");
		G_RwTitle = G_RwTitle.Replace("${game_version}", Game.Version.FullName);
		G_Ico = compound.Search<bool>("graphics.auto_iconify");
		G_Maxm = compound.Search<bool>("graphics.maximized");
		G_Autor = compound.Search<bool>("graphics.auto_resolution");

		//Settings builtin
		I18N.LangKey = LanguageGroup;
		Resolution.AllowResolution = G_Autor;
	}

}
