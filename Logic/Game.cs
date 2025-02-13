using Kinetic;
using Kinetic.App;
using Kinetic.Gui;
using Kinetic.Input;
using Kinetic.IO;
using Kinetic.Math;
using Kinetic.OpenAL;
using Kinetic.OpenGL;
using Kinetic.Visual;
using Loh;
using Loh.Values;
using Morinia.Api;
using Morinia.Client;
using Morinia.Client.TheGui;
using Morinia.Content.TheGui;
using FileSystem = Kinetic.IO.FileSystem;

namespace Morinia.Logic;

public class Game
{

	public static readonly string DisName = "Morinia";
	public static readonly string Namespace = "morinia";
	public static readonly SemanticVersion Version = new SemanticVersion("alpha-1.0.0");
	public static readonly Vector2 ScaledSize = new Vector2(800, 600);

	public static FileHandle ContentsFile;
	public static FileHandle LogFile;
	public static FileHandle OptionsFile;
	public static FileHandle ModsFile;
	static FileHandle PreloadSFile;

	public static MatrixerFlow MatrixerFlow;
	public static GameLogic GameLogic;

	public static void Main(string[] _)
	{
		Launch();
	}

	public static void Launch()
	{
		FileSystem.AsApplicationSource();
		ID.Converter = ModAssembly.GetFile;
		LohEngine.Init();

		//basic files.
		ContentsFile = FileSystem.GetLocal("contents");
		LogFile = FileSystem.GetLocal("logs");
		OptionsFile = FileSystem.GetLocal("options.loh");
		ModsFile = FileSystem.GetLocal("mods");
		PreloadSFile = ContentsFile.Goto("preloads.loh");

		Options.Load();

		//log output.
		Logger.StartStreamWriting(LogFile.Goto("latest.log"));

		//opengl window settings.
		//maybe there will be vulkan mod mmm.
		//(is that possible?)
		OGLDeviceSettings s = new OGLDeviceSettings();
		s.Maximized = Options.G_Maxm;
		s.Size = new Vector2(Options.G_Scsize.Get<float>(0), Options.G_Scsize.Get<float>(1));
		s.Title = Options.G_RwTitle;
		s.AutoIconify = Options.G_Ico;
		s.Cursor = ContentsFile.Goto(Options.G_CL);
		s.Icons = [ContentsFile.Goto(Options.G_IL)];
		s.Hotspot = new Vector2(Options.G_Hotspot.Get<float>(0), Options.G_Hotspot.Get<float>(1));

		//OGLDeviceSettings.FilterMin = TextureMinFilter.Linear;
		//OGLDeviceSettings.FilterMag = TextureMagFilter.Linear;

		OALAudioData.InitALDevice();
		OGLDevice.LoadSettings(s);
		OGLDevice.OpenWindow();
		//end

		MatrixerFlow = new MatrixerFlow();

		SpriteBatch batch = SpriteBatch.Global;

		//screening.
		Application.Update += () =>
		{
			RemoteKeyboard.Global.StartRoll();
			GameLogic?.Tick();
			for(int i = 0; i < ElementGui.Viewings.Count; i++)
			{
				ElementGui gui = ElementGui.Viewings[i];
				gui.Update(MatrixerFlow.Cursor);
			}
			RemoteKeyboard.Global.EndRoll();
		};

		Application.Draw += () =>
		{
			batch.Clear();
			GameLogic?.Draw(batch);

			for(int i = 0; i < ElementGui.Viewings.Count; i++)
			{
				ElementGui gui = ElementGui.Viewings[i];
				MatrixerFlow.DoTransform(gui, batch);
				batch.UseCamera(MatrixerFlow.Camera);
				if(gui != null)
				{
					//keep the font scale still.
					if(batch.Font != null) batch.Font.Scale = gui.FontScmul;
					gui.Draw(batch);
					if(batch.Font != null) batch.Font.Scale = 1;
				}
				batch.EndCamera(MatrixerFlow.Camera);
				batch.Flush();
			}

			GraphicsDevice.Global.SwapBuffer();
		};

		Application.Init += LoadInScreen;
		Application.Dispose += () =>
		{
			GameLogic?.Level?.Save();

			Coroutine.Wait();
			Logger.TryEndStreamWriting();
		};

		Application.MaxTps = 60;
		Application.Launch();
	}

	static void LoadInScreen()
	{
		LoadingQueue loader = GameResourceProcessing.GetUploadQueueWithProcessors(Namespace);

		loader.Filebase = ContentsFile;

		//get preloaded items
		LohArray files = LohEngine.Exec(PreloadSFile);

		foreach(string s in files)
			loader.Load(ContentsFile.Goto(s), true);

		//scan others
		GameResourceProcessing.Inject(loader, ContentsFile);
		ModAssembly.ScanLoad(Game.ModsFile);
		ModAssembly.InitMods(loader);

		loader.BeginTask();
		loader.DoPreLoad();

		//set some global vars
		//they shouldn't be changed unless there are mods' patching.
		SpriteBatch.Global.TexMissing = Loads.Get("textures/missing.png");
		SpriteBatch.Global.Texfil = Loads.Get("textures/rectangle.png");
		ElementGui.DefaultTooltipPatches = new NinePatch(Loads.Get("textures/gui/tooltip.png"));

		new GuiLoad(loader).Display();
	}

}
