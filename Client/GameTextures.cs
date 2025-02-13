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
	public static Texture Hotbar = Loads.Get("textures/gui/overlays/hotbar.png");
	public static Texture RecipeEntry = Loads.Get("textures/gui/containers/recipe_entry.png");
	public static Texture GuiInventory = Loads.Get("textures/gui/containers/inventory.png");
	public static Texture GuiChest = Loads.Get("textures/gui/containers/chest.png");
	public static Texture GuiFurnace = Loads.Get("textures/gui/containers/furnace.png");

	public static NinePatch GUI_NP_B1 = new NinePatch(Loads.Get("textures/gui/widgets/button_0.png"));
	public static NinePatch GUI_NP_B2 = new NinePatch(Loads.Get("textures/gui/widgets/button_1.png"));
	public static NinePatch GUI_NP_B3 = new NinePatch(Loads.Get("textures/gui/widgets/button_2.png"));
	public static Texture GUI_CB1 = Loads.Get("textures/gui/widgets/checkbox_0.png");
	public static Texture GUI_CB2 = Loads.Get("textures/gui/widgets/checkbox_1.png");
	public static Texture GUI_CB3 = Loads.Get("textures/gui/widgets/checkbox_2.png");

	public static Texture GUI_MENU_BG = Loads.Get("textures/gui/menu_bg.png");
	public static Texture GUI_MENU_TITLE_EN = Loads.Get("textures/gui/menu.png");
	public static Texture GUI_MENU_TITLE_CN = Loads.Get("textures/gui/menu_chinese.png");
	public static Texture GUI_WINDOW_CLOSER = Loads.Get("textures/gui/widgets/button_win_close.png");
	public static Texture GUI_WINDOW_MID = Loads.Get("textures/gui/widgets/window_m.png");
	public static Texture GUI_WINDOW_MID_EV = Loads.Get("textures/gui/widgets/window_m_event.png");
	public static Texture GUI_WINDOW_LARGE = Loads.Get("textures/gui/widgets/window_l.png");

}
