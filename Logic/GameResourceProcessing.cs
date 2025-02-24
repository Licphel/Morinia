using Kinetic;
using Kinetic.App;
using Kinetic.Auditory;
using Kinetic.IO;
using Kinetic.OpenAL;
using Kinetic.OpenGL;
using Kinetic.Visual;
using Loh;
using Morinia.Client.TheBlock;
using Morinia.Content;
using Morinia.World.TheBlock;
using Morinia.World.TheDict;
using Morinia.World.TheItem;
using Morinia.World.TheLoot;
using Morinia.World.TheRecipe;
using OpenTK.Graphics.OpenGL;

namespace Morinia.Logic;

public class GameResourceProcessing
{

	public static TextureAtlas BlockItemAtlas = new OGLTextureAtlas();

	public static LoadingQueue GetUploadQueueWithProcessors(string space)
	{
		LoadingQueue loader = new LoadingQueue(space);
		loader.BeginTask += BlockItemAtlas.Begin;
		loader.EndTask += BlockItemAtlas.End;
		loader.Processors["langs"] = LangProcessor;
		loader.Processors["textures"] = TextureProcessor;
		loader.Processors["musics"] = MusicSoundProcessor;
		loader.Processors["sounds"] = MusicSoundProcessor;
		loader.Processors["shaders"] = ShaderProcessor;
		loader.Processors["langs"] = LangProcessor;
		loader.Processors["models/blocks"] = BlockModelProcessor;
		loader.Processors["recipes"] = RecipeProcessor;
		loader.Processors["tags"] = TagProcessor;
		loader.Processors["loots"] = LootProcessor;
		return loader;
	}

	public static void Inject(LoadingQueue loader, FileHandle file)
	{
		loader.Scan(file.Goto("langs"), false);
		loader.Scan(file.Goto("textures"), false);
		loader.Enqueue(() => LoadFontMaps(Options.FontType));
		loader.Scan(file.Goto("musics"), false);
		loader.Scan(file.Goto("sounds"), false);
		loader.Scan(file.Goto("shaders"), false);
		// content-requiring tasks
		loader.Scan(file.Goto("models/blocks"), false);
		loader.Scan(file.Goto("tags"), false);
		loader.Enqueue(Tags.Init);
		loader.Scan(file.Goto("loots"), false);

		FileHandle fre = file.Goto("recipes");
		if(fre.Exists)
			foreach(var file1 in fre.Directories)
				loader.Scan(file1, false);
	}

	static void LoadFontMaps(string type)
	{
		if(type.StartsWith("bitmap"))
		{
			Texture[] pictures;
			
			if(type == "bitmap_fancyascii")
			{
				pictures = new Texture[257];
				pictures[0] = Loads.Get("textures/characters/ascii.png");
		
				for(int i = 1; i < pictures.Length; i++)
				{
					Texture picture = Loads.Get("textures/characters/unicode_page_" + (i - 1) + ".png");
					pictures[i] = picture;
					picture.SetPixelRenderMode("Linear|Nearest|Repeat|Repeat");
				}
			}
			else
			{
				pictures = new Texture[256];
				
				for(int i = 0; i < pictures.Length; i++)
				{
					Texture picture = Loads.Get("textures/characters/unicode_page_" + i + ".png");
					pictures[i] = picture;
					picture.SetPixelRenderMode("Linear|Nearest|Repeat|Repeat");
				}
			}

			IBinaryCompound c = BinaryIO.Read(Game.ContentsFile.Goto("textures/characters/unicode_glyphs.bin"));

			Font font = FontBitmap.Load(pictures, GetPage, c.Get<int[]>("x"), c.Get<int[]>("y"), c.Get<float[]>("w"), c.Get<int>("h"));
			
			SpriteBatch.Global.Font = font;

			return;

			static int GetPage(char ch)
			{
				bool fancy = Options.FontType == "bitmap_fancyascii";
				if(ch < 128 && fancy) return 0;
				return (int) (ch / 256f) + (fancy ? 1 : 0);
			}
		}
		else if(type == "modern")
		{
			FileHandle fh = FileSystem.GetLocal("contents/font_noto.otf");

			Font font = OGLFontFreetype.Load([fh], 128, 16);
			
			SpriteBatch.Global.Font = font;
		}
	}

	public static UploadQueueProcessor LangProcessor = (resource, file) =>
	{
		if(!file.Format.Equals("loh", StringComparison.OrdinalIgnoreCase))
			return;

		IBinaryCompound dat = LohEngine.Exec(file);
		I18N.Load(resource.Space, dat.Get<string>("lang_group"), dat);
	};

	public static UploadQueueProcessor TagProcessor = (resource, file) =>
	{
		if(!file.Format.Equals("loh", StringComparison.OrdinalIgnoreCase))
			return;

		IBinaryList dat = LohEngine.Exec(file);
		if(resource.Key.StartsWith("tags/blocks"))
			TagManager.Parse(resource, dat, Blocks.Registry);
		else if(resource.Key.StartsWith("tags/items"))
			TagManager.Parse(resource, dat, Items.Registry);
	};

	public static UploadQueueProcessor LootProcessor = (resource, file) =>
	{
		if(!file.Format.Equals("loh", StringComparison.OrdinalIgnoreCase))
			return;

		IBinaryList dat = LohEngine.Exec(file);
		LootManager.Parse(resource, dat);
	};

	public static UploadQueueProcessor TextureProcessor = (resource, file) =>
	{
		if(!file.Format.Equals("png", StringComparison.OrdinalIgnoreCase))
			return;

		OGLTexture texture = new OGLTexture();
		texture.Upload(file);
		dynamic finalsub = texture;

		if(resource.Key.StartsWith("textures/blocks")
		   || resource.Key.StartsWith("textures/items"))
		{
			finalsub = BlockItemAtlas.Accept(texture);
		}

		FileHandle animpth = FileSystem.GetAbsolute(file.Path + ".loh");
		if(animpth.Exists)
		{
			IBinaryCompound compound = LohEngine.Exec(animpth);
			IBinaryList ints = compound.GetListSafely("frame_uv");
			finalsub = new IconAnim(finalsub,
				compound.Get<int>("frame_count"),
				ints.Get<int>(2), ints.Get<int>(3), ints.Get<int>(0), ints.Get<int>(1))
			.Seconds(compound.Get<float>("frame_length"));
		}

		Loads.Load(resource, finalsub);
	};

	public static UploadQueueProcessor MusicSoundProcessor = (resource, file) =>
	{
		if(!file.Format.Equals("wav", StringComparison.OrdinalIgnoreCase))
			return;

		AudioData data = OALAudioData.Read(file);
		Loads.Load(resource, data);
	};

	public static UploadQueueProcessor ShaderProcessor = (resource, file) =>
	{
		if(!file.Format.Equals("shd", StringComparison.OrdinalIgnoreCase))
			return;

		string @string = StringIO.Read(file);
		Loads.Load(resource, @string);
	};

	public static UploadQueueProcessor BlockModelProcessor = (resource, file) =>
	{
		if(!file.Format.Equals("loh", StringComparison.OrdinalIgnoreCase))
			return;

		BlockRendering.Load(resource, file);
	};

	public static UploadQueueProcessor RecipeProcessor = (resource, file) =>
	{
		if(!file.Format.Equals("loh", StringComparison.OrdinalIgnoreCase))
			return;

		RecipeManager.Decode(resource, file);
	};

}
