using Kinetic.App;
using Kinetic.Visual;

namespace Morinia.Client;

public class GameTextures
{

	public static Texture ParticleBreakblock = Loads.Get("textures/particles/break.png");

	public static Texture EntityBiped = Loads.Get("textures/entities/biped.png");
	public static Texture EntityOrb = Loads.Get("textures/entities/magic_orb.png");
	public static Texture EntityArrow = Loads.Get("textures/entities/arrow.png");
	public static Texture EntityDrone = Loads.Get("textures/entities/drone.png");
	public static Texture EntityDroneAncient = Loads.Get("textures/entities/drone_ancient.png");

	public static Texture CeBody1 = Loads.Get("textures/ambient/body_1.png");
	public static Texture CeBody2 = Loads.Get("textures/ambient/body_2.png");
	public static Texture CeBg1 = Loads.Get("textures/ambient/background_1.png");
	public static Texture CeBg2 = Loads.Get("textures/ambient/background_2.png");
	public static Texture CeBg3 = Loads.Get("textures/ambient/background_4.png");
	public static Texture UCeBg1 = Loads.Get("textures/ambient/underground_1.png");
	public static Texture UCeBg2 = Loads.Get("textures/ambient/underground_2.png");
	
	public static Texture Hotbar = Loads.Get("textures/gui/hotbar.png");
	public static Texture RecipeEntry = Loads.Get("textures/gui/containers/recipe_entry.png");
	public static Texture GuiInventory = Loads.Get("textures/gui/containers/inventory.png");
	public static Texture GuiChest = Loads.Get("textures/gui/containers/chest.png");
	public static Texture GuiFurnace = Loads.Get("textures/gui/containers/furnace.png");

	public static Texture Button = Loads.Get("textures/gui/button.png");
	public static NinePatch ButtonP1 = new NinePatch(TexturePart.BySize(Button, 0, 0, 12, 12));
	public static NinePatch ButtonP2 = new NinePatch(TexturePart.BySize(Button, 0, 12, 12, 12));
	public static NinePatch ButtonP3 = new NinePatch(TexturePart.BySize(Button, 0, 24, 12, 12));
	
	public static Texture GuiMenuTitle = Loads.Get("textures/gui/menu.png");
	public static Texture GuiMenuTitleCN = Loads.Get("textures/gui/menu_chinese.png");

}
