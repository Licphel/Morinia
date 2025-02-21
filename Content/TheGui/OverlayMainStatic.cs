using Kinetic.App;
using Kinetic.Gui;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client;
using Morinia.Client.TheItem;
using Morinia.Content.TheEntity;
using Morinia.Logic;
using Morinia.Util;
using Morinia.World.TheBlock;
using Morinia.World.TheItem;

namespace Morinia.Content.TheGui;

public partial class OverlayMain
{

    public void DrawPStates(SpriteBatch batch, EntityPlayer player)
    {
        const float x = 4;
        float spc = 8 + 2;
        float y = 4 + spc * 3;
        int v = 41;
        
        DrawBar(player.Health, player.MaxHealth);
        DrawBar(player.Mana, player.MaxMana);
        DrawBar(player.Hunger, player.MaxHunger);
        DrawBar(player.Thirst, player.MaxThirst);

        void DrawBar(float p, float pm)
        {
            float per = FloatMath.SafeDiv(p, pm);
            batch.Draw(GameTextures.Hotbar, x, y, 86, 6, 0, v, 86, 6);
            batch.Draw(GameTextures.Hotbar, x, y, 78 * per, 6, 87, v, 78 * per, 6);
            batch.Color4(0.5f, 0.55f, 0.85f, 0.75f);
            string str = NumUtil.GetCompress((int) p) + " / " + NumUtil.GetCompress(pm);
            batch.Draw(str, x + 88, y - 1);
            batch.NormalizeColor();
            y -= spc;
            v += 7;
        }
    }

    public void DrawHotBarAndHint(SpriteBatch batch, EntityPlayer player)
    {
        const float w = 188f;
        const float h = 20;
        const float lptr = 0;
        const float dptr = 0;
        const float lptri = 1;
        const float dptri = 1;
        const float st = 20;
        const float off = 20;
		
        float bx = (Size.x - w) / 2;
        float by = 2;
        
        batch.Draw(GameTextures.Hotbar, bx, by, w, h, 0, 0, w, h);
        batch.Draw(GameTextures.Hotbar, bx + lptr + (st + 1) * player.InvCursor, by + dptr, st, st, 0, off, st, st);

        for(int i = 0; i < 9; i++)
        {
            ItemStack stack = player.Inv.Get(i);
            ItemRendering.GetTessellator(stack).Draw(batch, bx + (lptri + 1) + (st + 1) * i, by + (dptri + 1), 16, 16, stack);
        }
		
        Block block = Game.GameLogic.HoverBlockState.Block;
		
        if(block != Block.Empty)
        {
            string name = I18N.GetText($"{block.Uid.Space}:blocks.{block.Uid.Key}");
            string fromw = block.Uid.Space;
            GlyphBounds gb = batch.Font.GetBounds(name);
            GlyphBounds gb1 = batch.Font.GetBounds(fromw);
            float frw = Math.Max(gb.Width, gb1.Width) + 8;
            float frh = gb.Height + gb1.Height + 6;
            
            batch.Draw(ElementGui.DefaultTooltipPatches, (Size.x - frw) / 2, Size.y - frh - 6, frw, frh);
			
            batch.Draw(name, Size.x / 2, Size.y - frh + 5, Align.CENTER);
            batch.Color4(1, 1, 1, 0.5f);
            batch.Draw(fromw, Size.x / 2, Size.y - frh - 3, Align.CENTER);
            batch.NormalizeColor();
        }
    }

}