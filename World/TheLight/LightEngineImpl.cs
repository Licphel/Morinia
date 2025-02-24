using System.Runtime.CompilerServices;
using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Morinia.Client.TheCelesphere;
using Morinia.World.TheBlock;
using Morinia.World.TheLiquid;
using Morinia.World.ThePhysic;

namespace Morinia.World.TheLight;

//The worst code in this game! Do not touch anything unless you want a crash!
public class LightEngineImpl : LightEngine
{

    static readonly float[,] _randpns = new float[Chunk.Width, Chunk.Height];

    readonly Queue<Response> _Rq_draw = new Queue<Response>();

    readonly List<Entity> cache = new List<Entity>();

    readonly Level level;

    readonly float[] sunlight = new float[3];
    readonly Chunk[] tmpChunks = new Chunk[10];
    int chkOrigin;
    public bool done;

    public long elapsed;
    Coroutine Tasking;

    static LightEngineImpl()
    {
        for(int x = 0; x < Chunk.Width; x++)
        {
            for(int y = 0; y < Chunk.Height; y++)
            {
                _randpns[x, y] = Seed.Global.NextFloat(0.9f, 1f);
            }
        }
    }

    public LightEngineImpl(Level level)
    {
        this.level = level;
    }

    public override bool IsStableRoll => true;

    public override void DrawEnqueue(float x, float y, float v1, float v2, float v3)
    {
        _Rq_draw.Enqueue(() => DrawLinearLight0(x, y, v1, v2, v3));
    }

    public void DrawLinearLight0(float x, float y, float v1, float v2, float v3)
    {
        if(!(v1 > 0) || !(v2 > 0) || !(v3 > 0))
            return;

        BlockPos chunkPos = new BlockPos(new PrecisePos(x, y));

        Direction directionX = (chunkPos.BlockX & 15) >= 8 ? Direction.Right : Direction.Left;
        Direction directionY = (chunkPos.BlockY & 15) >= 8 ? Direction.Up : Direction.Down;

        for(int i = 0; i < 7; i++)
        {
            if(i % 4 == 0)
            {
                chunkPos = directionX.Step(chunkPos);
            }
            else if(i % 4 == 1)
            {
                chunkPos = directionY.Step(chunkPos);
            }
            else if(i % 4 == 2)
            {
                chunkPos = directionX.Opposite.Step(chunkPos);
            }
            else
            {
                chunkPos = directionY.Opposite.Step(chunkPos);
            }
            DrawDirect(chunkPos.BlockX, chunkPos.BlockY, v1, v2, v3);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void DrawDirect(int x, int y, float v1, float v2, float v3)
    {
        Chunk chunk = GetBufferedChunk(x);
        if(chunk == null)
        {
            return;
        }

        if(y < 0 || y > Chunk.Height)
        {
            return;
        }

        LightWare tl = chunk._LE_Surfpack(x, y, 0);
        tl.rgb[0] = Math.Max(tl.rgb[0], v1);
        tl.rgb[1] = Math.Max(tl.rgb[1], v2);
        tl.rgb[2] = Math.Max(tl.rgb[2], v3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Vector3 GetLinearLight(float x, float y)
    {
        int ix = FloatMath.Floor(x);
        int iy = FloatMath.Floor(y);

        Chunk chunk = level.GetChunkByBlock(ix);

        if(chunk == null)
        {
            return new Vector3(1, 1, 1);
        }

        float[] rgb00 = chunk._LE_Bufpack(ix, iy, 0).rgb;
        float[] rgb01 = chunk._LE_Bufpack(ix, iy, 1).rgb;

        Vector3 rgb = new Vector3();

        rgb.x = FloatMath.Clamp(Math.Max(rgb00[0], rgb01[0] * sunlight[0]), 0, 1);
        rgb.y = FloatMath.Clamp(Math.Max(rgb00[1], rgb01[1] * sunlight[1]), 0, 1);
        rgb.z = FloatMath.Clamp(Math.Max(rgb00[2], rgb01[2] * sunlight[2]), 0, 1);

        return rgb;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void GetBlockLight(LightWare coords, Vector4[] color, float a)
    {
        coords.GetVec4Out(ref color[0], 0, a);
        coords.GetVec4Out(ref color[1], 1, a);
        coords.GetVec4Out(ref color[2], 2, a);
        coords.GetVec4Out(ref color[3], 3, a);
    }

    public override void Glow(int x, int y, float v1, float v2, float v3)
    {
        Chunk chunk = GetBufferedChunk(x);
        if(chunk == null)
        {
            return;
        }

        if(y < 0 || y > Chunk.Height)
        {
            return;
        }

        LightWare p = chunk._LE_Surfpack(x, y, 0);
        p.rgb[0] = Math.Max(p.rgb[0], v1);
        p.rgb[1] = Math.Max(p.rgb[1], v2);
        p.rgb[2] = Math.Max(p.rgb[2], v3);
    }

    public override void CalculateByViewdim(Box cam)
    {
        Vector3 sun = CelestComputation.LightingSunlight(level);
        sunlight[0] = sun.x;
        sunlight[1] = sun.y;
        sunlight[2] = sun.z;

        float spd = TimeSchedule.PeriodicTask(0.5f) ? MaxValue / Unit : 1;

        if(done)
        {
            //Swap surface buffer and back buffer (back buffer is used to render).
            Swap();
            done = false;
        }

        if(Tasking == null || Tasking.IsCompleted)
        {
            //Write the result into surface buffer, in coroutines.
            Tasking = new Coroutine(() =>
            {
                Calculate(cam,
                    (int)(cam.x - spd),
                    (int)(cam.y - spd),
                    (int)(cam.xprom + spd),
                    (int)(cam.yprom + spd));

                done = true;
            });
            Tasking.Start();
        }

        elapsed++;
    }

    public void Swap()
    {
        foreach(Chunk chk in tmpChunks)
        {
            chk?._LE_SwapBuffer();
        }
    }

    //Two steps contained. One is in-screen process, the other is out-screen buffer zone process.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Calculate(Box cam, int x0, int y0, int x1, int y1)
    {
        chkOrigin = FloatMath.Floor(cam.xcentral / 16f) - 5;

        for(int i = chkOrigin; i < chkOrigin + 10; i++)
        {
            tmpChunks[i - chkOrigin] = level.GetChunk(i);
        }

        y0 = FloatMath.Clamp(y0, 0, Chunk.MaxY);
        y1 = FloatMath.Clamp(y1, 0, Chunk.MaxY);

        for(int x = x0; x <= x1; x++)
        {
            Chunk chunk = GetBufferedChunk(x);
            if(chunk == null)
            {
                continue;
            }

            for(int y = y0; y <= y1; y++)
            {
                LightWare s0 = chunk._LE_Surfpack(x, y, 0);
                s0.rgb[0] = ShedLight(chunk, x, y, 0);
                s0.rgb[1] = ShedLight(chunk, x, y, 1);
                s0.rgb[2] = ShedLight(chunk, x, y, 2);

                LightWare s1 = chunk._LE_Surfpack(x, y, 1);
                s1.rgb[0] = ShineLight(chunk, x, y, 0);
                s1.rgb[1] = ShineLight(chunk, x, y, 1);
                s1.rgb[2] = ShineLight(chunk, x, y, 2);
            }
        }

        Box aabb = new Box();
        aabb.Resize(x1 - x0, y1 - y0);
        aabb.LocateCentral((x1 + x0) / 2f, (y1 + y0) / 2f);
        level.GetNearbyEntities(cache, aabb);

        //Must after the block shedding.
        //Or, the entity shedding will be replaced.

        foreach(Entity e in cache)
        {
            float l11 = e.CastLight(LightPass.Red);
            float l12 = e.CastLight(LightPass.Green);
            float l13 = e.CastLight(LightPass.Blue);

            if(l11 == 0 && l12 == 0 && l13 == 0)
            {
                continue;
            }

            float sx = e.Pos.x;
            float sy = e.Pos.y;

            DrawDirect(FloatMath.Floor(sx), FloatMath.Floor(sy), l11, l12, l13);
        }

        while(_Rq_draw.Count != 0)
        {
            _Rq_draw.Dequeue().Invoke();
        }

        for(int x = x1; x >= x0; x--)
        {
            Chunk chunk = GetBufferedChunk(x);
            Chunk cn1 = GetBufferedChunk(x - 1);
            Chunk cp1 = GetBufferedChunk(x + 1);
            if(chunk == null)
            {
                continue;
            }

            for(int y = y1; y >= y0; y--)
            {
                Calculate0(chunk, cn1, cp1, chunk._LE_Surfpack(x, y, 0), x, y, 0);
                Calculate0(chunk, cn1, cp1, chunk._LE_Surfpack(x, y, 1), x, y, 1);
            }
        }

        for(int x = x0; x <= x1; x++)
        {
            Chunk chunk = GetBufferedChunk(x);
            Chunk cn1 = GetBufferedChunk(x - 1);
            Chunk cp1 = GetBufferedChunk(x + 1);

            if(chunk == null)
            {
                continue;
            }

            for(int y = y0; y <= y1; y++)
            {
                Calculate0(chunk, cn1, cp1, chunk._LE_Surfpack(x, y, 0), x, y, 0);
                Calculate0(chunk, cn1, cp1, chunk._LE_Surfpack(x, y, 1), x, y, 1);
            }
        }
        
        for(int x = x0; x <= x1; x++)
        {
            Chunk chunk = GetBufferedChunk(x);
            Chunk cn1 = GetBufferedChunk(x - 1);
            Chunk cp1 = GetBufferedChunk(x + 1);
            if(chunk == null)
            {
                continue;
            }

            for(int y = y1; y >= y0; y--)
            {
                Calculate0(chunk, cn1, cp1, chunk._LE_Surfpack(x, y, 0), x, y, 0);
                Calculate0(chunk, cn1, cp1, chunk._LE_Surfpack(x, y, 1), x, y, 1);
            }
        }

        for(int x = x1; x >= x0; x--)
        {
            Chunk chunk = GetBufferedChunk(x);
            Chunk cn1 = GetBufferedChunk(x - 1);
            Chunk cp1 = GetBufferedChunk(x + 1);

            if(chunk == null)
            {
                continue;
            }

            for(int y = y0; y <= y1; y++)
            {
                Calculate0(chunk, cn1, cp1, chunk._LE_Surfpack(x, y, 0), x, y, 0);
                Calculate0(chunk, cn1, cp1, chunk._LE_Surfpack(x, y, 1), x, y, 1);
            }
        }

        for(int x = x0; x <= x1; x++)
        {
            Chunk chunk = GetBufferedChunk(x);

            if(chunk == null)
            {
                continue;
            }

            for(int y = y0; y <= y1; y++)
            {
                LightWare p0 = chunk._LE_Surfpack(x, y, 0);
                LightWare p1 = chunk._LE_Surfpack(x, y, 1);
                p0.active(x, y);
                p1.active(x, y);
                p0.writeCoords(p0.coordsOut);
                p0.writeCoords(p1.coordsOut);
                p1.extendCoords(p0.coordsOut, sunlight);
                p1.extendCoords(p1.coordsOut, sunlight);
                p0.extendAO(p0.coordsOut, 0.75f);
            }
        }
    }

    public void Calculate0(Chunk chunk, Chunk cn1, Chunk cp1, LightWare s, int x, int y, int t)
    {
        Calculate0(chunk, cn1, cp1, s, x, y, 0, t);
        Calculate0(chunk, cn1, cp1, s, x, y, 1, t);
        Calculate0(chunk, cn1, cp1, s, x, y, 2, t);
    }

    public void Calculate0(Chunk chunk, Chunk cn1, Chunk cp1, LightWare s, int x, int y, byte idx, int t)
    {
        float l1;
        float l2 = 0, l3 = 0, l4 = 0, l5 = 0;
        if(y >= 0 && y <= Chunk.MaxY)
        {
            if(cn1 != null)
            {
                l2 = cn1._LE_Surfpack(x - 1, y, t).rgb[idx];
            }

            if(cp1 != null)
            {
                l3 = cp1._LE_Surfpack(x + 1, y, t).rgb[idx];
            }

            if(y > 0)
            {
                //out of bound!
                l4 = chunk._LE_Surfpack(x, y - 1, t).rgb[idx];
            }

            if(y < Chunk.MaxY)
            {
                l5 = chunk._LE_Surfpack(x, y + 1, t).rgb[idx];
            }

            l1 = Math.Max(l2, Math.Max(l3, Math.Max(l4, l5)));
        }
        else
        {
            if(y <= 0)
            {
                l1 = 0;
            }
            else
            {
                l1 = 1;
            }
        }

        if(l1 < 0.005f)
        {
            s.rgb[idx] = 0; //No need to calc in this case.
        }
        else
        {
            l1 = chunk.GetBlock(x, y).FilterLight(idx, l1, x, y);
            LiquidStack lqs = chunk.LiquidMap.Get(x, y);
            l1 = lqs.Liquid.FilterLight(idx, l1, x, y, lqs.Amount);
            s.rgb[idx] = Math.Max(Min[idx], l1);
            s.rgb[idx] = Math.Min(Max[idx], l1);
        }
    }

    public float ShedLight(Chunk chunk, int x, int y, byte pipe)
    {
        BlockState b1 = chunk.GetBlock(x, y);
        BlockState b2 = chunk.GetBlock(x, y, 0);
        LiquidStack lqs = chunk.LiquidMap.Get(x, y);
        float l1 = b1.CastLight(pipe, x, y);
        float l2 = b2.CastLight(pipe, x, y);
        float l3 = lqs.Liquid.CastLight(pipe, x, y, lqs.Amount);
        return Math.Max(l1, Math.Max(l2, l3));
    }

    public float ShineLight(Chunk chunk, int x, int y, byte pipe)
    {
        if(y < Chunk.SeaLevel - 5)
        {
            return 0;
        }

        BlockState b1 = chunk.GetBlock(x, y);
        BlockState b2 = chunk.GetBlock(x, y, 0);
        float amp = y >= Chunk.SeaLevel ? 1 : Math.Max(0, 1 - (Chunk.SeaLevel - y) * 0.25f);
        BlockShape s1 = b1.GetShape();
        BlockShape s2 = b2.GetShape();
        LiquidStack lqs = chunk.LiquidMap.Get(x, y);
        if(s1 == BlockShape.Vacuum && s2 == BlockShape.Vacuum)
        {
            return lqs.Liquid.FilterLight(pipe, amp, x, y, lqs.Amount);
        }

        if(s1 != BlockShape.Solid && s2 != BlockShape.Solid)
        {
            float v = b2.FilterLight(pipe, amp, x, y);
            return lqs.Liquid.FilterLight(pipe, v, x, y, lqs.Amount);
        }

        return 0;
    }

    public int Corof(int xb)
    {
        return FloatMath.Floor(xb / 16f) - chkOrigin;
    }

    public override Chunk GetBufferedChunk(int xb)
    {
        int x = Corof(xb);
        if(x < 0 || x >= tmpChunks.Length || tmpChunks[x] == null)
        {
            return level.GetChunkByBlock(x);
        }

        return tmpChunks[x];
    }

}