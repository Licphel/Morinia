using Kinetic.App;
using Kinetic.Math;
using Morinia.Content;
using Morinia.Logic;
using Morinia.World.TheBlock;
using Morinia.World.ThePhysic;

namespace Morinia.World.TheLiquid;

public class Liquid : IDHolder
{

	public const int MaxAmount = 128;

    public static Liquid Empty => Liquids.Empty;

    public float CastLight(byte pipe, int x, int y, int am)
    {
        return 0;
    }

    public float FilterLight(byte pipe, float v, int x, int y, int am)
    {
        return v * 0.98f - am / MaxAmount * 0.01f;
    }

    public void OnEntityCollided(Entity entity)
    {
    }

    public static int SimVerticalDist = 32;
    public static int SimHorizontalDist = 32;

    static bool lastLeft;
    static bool spreadL;
    static int batch;
    static int px, py;

    public static void TickChunkLiquid(Chunk chunk)
    {
        px = Game.GameLogic.Player.Pos.BlockX;
        py = Game.GameLogic.Player.Pos.BlockY;

        Chunk chunk0 = chunk.Level.GetChunk(chunk.Coord - 1);
        Chunk chunk1 = chunk.Level.GetChunk(chunk.Coord + 1);

        spreadL = Seed.Global.NextFloat() < 0.5f;

        if(!lastLeft) {
            for(int x = 0; x < 16; x++)
            {
                if(Math.Abs(x + chunk.Coord * 16 - px) >= SimHorizontalDist)
                    continue;
                int my = Math.Min(Chunk.MaxY, py + SimVerticalDist);
                for(int y = Math.Max(0, py - SimVerticalDist); y <= my; y++)
                {
                    spreadLiquid(chunk, chunk0, chunk1, x, y, chunk.LiquidMap);
                }
            }
        }
        else {
            for(int x = 15; x >= 0; x--)
            {
                if(Math.Abs(x + chunk.Coord * 16 - px) >= SimHorizontalDist)
                    continue;
                int my = Math.Min(Chunk.MaxY, py + SimVerticalDist);
                for(int y = Math.Max(0, py - SimVerticalDist); y <= my; y++)
                {
                    spreadLiquid(chunk, chunk0, chunk1, x, y, chunk.LiquidMap);
                }
            }
        }

        lastLeft = !lastLeft;
    }

    static void spreadLiquid(Chunk chunk, Chunk chunk0, Chunk chunk1, int x, int y, LiquidMap arr)
    {
        LiquidStack stack = arr.Get(x, y);
        Liquid type = stack.Liquid;
        int a = stack.Amount;

        if(a == 0 || stack.Liquid == Liquid.Empty) {
            return;
        }

        BlockState bd = chunk.GetBlock(x, y - 1, 1);
        LiquidStack stackd = arr.Get(x, y - 1);
        int ad = stackd.Amount;

        if(bd.GetShape() != BlockShape.Solid) {
            if(ad < MaxAmount) {
                int ext = Math.Min(MaxAmount - ad, a);
                a -= ext;
                arr.Set(x, y, new LiquidStack(type, a));
                arr.Set(x, y - 1, new LiquidStack(type, ext + ad));

                if(ext > a / 2)
                    return;
            }
        }

        BlockState bl, br;
        LiquidMap arr0, arr1;
        int ar, al;

        if(x == 0) {
            if(chunk0 == null)
                return;
            bl = chunk0.GetBlock(x - 1, y, 1);
            br = chunk.GetBlock(x + 1, y, 1);
            arr0 = chunk0.LiquidMap;
            arr1 = chunk.LiquidMap;
            ar = arr.Get(x + 1, y).Amount;
            al = arr0.Get(x - 1, y).Amount;
        }
        else if(x == 15) {
            if(chunk1 == null)
                return;
            bl = chunk.GetBlock(x - 1, y, 1);
            br = chunk1.GetBlock(x + 1, y, 1);
            arr0 = chunk.LiquidMap;
            arr1 = chunk1.LiquidMap;
            ar = arr1.Get(x + 1, y).Amount;
            al = arr.Get(x - 1, y).Amount;
        }
        else
        {
            bl = chunk.GetBlock(x - 1, y, 1);
            br = chunk.GetBlock(x + 1, y, 1);
            arr0 = chunk.LiquidMap;
            arr1 = chunk.LiquidMap;
            ar = arr.Get(x + 1, y).Amount;
            al = arr.Get(x - 1, y).Amount;
        }

        if(spreadL && bl.GetShape() != BlockShape.Solid) {
            int ext = Math.Min(MaxAmount - al, Math.Min((int) Math.Ceiling((a - al) / 2f), a));
            a -= ext;
            arr.Set(x, y, new LiquidStack(type, a));
            arr0.Set(x - 1, y, new LiquidStack(type, ext + al));
        }
        else if(br.GetShape() != BlockShape.Solid) {
            int ext = Math.Min(MaxAmount - ar, Math.Min((int) Math.Ceiling((a - ar) / 2f), a));
            a -= ext;
            arr.Set(x, y, new LiquidStack(type, a));
            arr1.Set(x + 1, y, new LiquidStack(type, ext + ar));
        }
    }

}
