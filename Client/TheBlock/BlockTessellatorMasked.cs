using System.Runtime.CompilerServices;
using Kinetic.App;
using Kinetic.OpenGL;
using Kinetic.Visual;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheLight;
using OpenTK.Graphics.OpenGL;

namespace Morinia.Client.TheBlock;

public class BlockTessellatorMasked : BlockTessellator
{

	static readonly string vert = Loads.Get("shaders/block_vert.shd");
	static readonly string frag = Loads.Get("shaders/block_frag.shd");

	static readonly ShaderProgram Program = ShaderBuilds.Build(vert, frag, program =>
	{
		ShaderAttribute posAttrib = program.GetAttribute("i_position");
		posAttrib.Enable();
		ShaderAttribute colAttrib = program.GetAttribute("i_color");
		colAttrib.Enable();
		ShaderAttribute texAttrib = program.GetAttribute("i_texCoord");
		texAttrib.Enable();
		ShaderAttribute texAttrib1 = program.GetAttribute("i_texCoord1");
		texAttrib1.Enable();

		posAttrib.Ptr(VertexAttribPointerType.Float, 2, 40, 0);
		colAttrib.Ptr(VertexAttribPointerType.Float, 4, 40, 8);
		texAttrib.Ptr(VertexAttribPointerType.Float, 2, 40, 24);
		texAttrib1.Ptr(VertexAttribPointerType.Float, 2, 40, 32);

		program.GetUniform("u_texture").Set1(0);
		program.GetUniform("u_mask").Set1(1);
	});

	static float u1, v1, u2, v2;//tmp
	static OGLTexture prevMask;
	static readonly VertexAppender[] vertappd = new VertexAppender[4];
	static readonly UniformAppender uniappd;

	static BlockTessellatorMasked()
	{
		vertappd[0] = b =>
		{
			b.Write(u1, v2);
			b.NewVertex(2);
		};
		vertappd[1] = b =>
		{
			b.Write(u1, v1);
			b.NewVertex(2);
		};
		vertappd[2] = b =>
		{
			b.Write(u2, v1);
			b.NewVertex(2);
		};
		vertappd[3] = b =>
		{
			b.Write(u2, v2);
			b.NewVertex(2);
		};
		uniappd = b =>
		{
			((GLShaderProgram) b.Program).GetUniform("u_mask").SetTexUnit(prevMask.Id, 1);
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Draw(SpriteBatch batch, Level level, Chunk chunk, BlockState state, int x, int y, int z)
	{
		Texture mask = BlockRendering.GetMask(state);
		if(mask != prevMask)
		{
			if(prevMask != null) batch.Flush();
			prevMask = (OGLTexture) mask;
		}

		LightWare lights = chunk._LE_Bufpack(x, y, z);

		DrawDetailed(batch, level, chunk, state, x, y, z, lights);
	}

	public void SetState(SpriteBatch batch)
	{
		batch.UseShader(Program);
		batch.UniformAppender = uniappd;
		batch.UseVertAppenders(vertappd);
	}

	public void ResetState(SpriteBatch batch)
	{
		batch.UseDefaultShader();
		batch.UniformAppender = null;
		batch.EndVertAppenders();
		batch.NormalizeColor();
		prevMask = null;
	}

	public bool IsInSameState(BlockTessellator tessellator)
	{
		return tessellator is BlockTessellatorMasked;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void DrawDetailed(SpriteBatch batch, Level level, Chunk chunk, BlockState state, int x, int y, int z, LightWare lights)
	{
		int u = 0, v = 0;

		Chunk cleft = level.LightEngine.GetBufferedChunk(x - 1);
		Chunk cright = level.LightEngine.GetBufferedChunk(x + 1);

		if(cleft == null || cright == null || chunk == null) return;

		BlockState upBlock = chunk.GetBlock(x, y + 1, z);
		BlockState downBlock = chunk.GetBlock(x, y - 1, z);
		BlockState leftBlock = cleft.GetBlock(x - 1, y, z);
		BlockState rightBlock = cright.GetBlock(x + 1, y, z);

		bool up = y == Chunk.MaxY
		          || BlockRendering.IsConnectable(upBlock, state, Direction.Down)
		          || BlockRendering.IsConnectable(state, upBlock, Direction.Up);
		bool down = y == 0
		            || BlockRendering.IsConnectable(downBlock, state, Direction.Up)
		            || BlockRendering.IsConnectable(state, downBlock, Direction.Down);
		bool left =
		BlockRendering.IsConnectable(leftBlock, state, Direction.Right)
		|| BlockRendering.IsConnectable(state, leftBlock, Direction.Left);
		bool right =
		BlockRendering.IsConnectable(rightBlock, state, Direction.Left)
		|| BlockRendering.IsConnectable(state, rightBlock, Direction.Right);

		bool upc =
		BlockRendering.IsSpreadable(upBlock, state, Direction.Down)
		|| BlockRendering.IsSpreadable(state, upBlock, Direction.Up);
		bool downc =
		BlockRendering.IsSpreadable(downBlock, state, Direction.Up)
		|| BlockRendering.IsSpreadable(state, downBlock, Direction.Down);
		bool leftc =
		BlockRendering.IsSpreadable(leftBlock, state, Direction.Right)
		|| BlockRendering.IsSpreadable(state, leftBlock, Direction.Left);
		bool rightc =
		BlockRendering.IsSpreadable(rightBlock, state, Direction.Left)
		|| BlockRendering.IsSpreadable(state, rightBlock, Direction.Right);

		if(up && down) u = 0;
		else if(up && !down) u = 17;
		else if(!up && down) u = 34;
		else if(!up && !down) u = 51;

		if(left && right) v = 0;
		else if(left && !right) v = 17;
		else if(!left && right) v = 34;
		else if(!left && !right) v = 51;

		u1 = u / 128f;
		v1 = v / 128f;
		u2 = (u + 16) / 128f;
		v2 = (v + 16) / 128f;

		level.LightEngine.GetBlockLight(lights, batch.LinearCol4, 1);

		DrawInternal(batch, state, null, x, y);

		int pid = state.Block.Uid;

		if(leftc && left && !leftBlock.IsEmpty && BlockRendering.GetMask(leftBlock) == prevMask)
		{
			if(leftBlock.Block.Uid > pid)
			{
				lights = cleft._LE_Bufpack(x - 1, y, z);
				level.LightEngine.GetBlockLight(lights, batch.LinearCol4, 1);
				u1 = 0 / 128f;
				v1 = 68 / 128f;
				u2 = (0 + 16) / 128f;
				v2 = (68 + 16) / 128f;
				DrawInternal(batch, state, Direction.Left, x - 1, y);
			}
			else
			{
				lights = chunk._LE_Bufpack(x, y, z);
				level.LightEngine.GetBlockLight(lights, batch.LinearCol4, 1);
				u1 = 34 / 128f;
				v1 = 68 / 128f;
				u2 = (34 + 16) / 128f;
				v2 = (68 + 16) / 128f;
				DrawInternal(batch, leftBlock, Direction.Right, x, y);
			}
		}

		if(downc && down && !downBlock.IsEmpty && BlockRendering.GetMask(downBlock) == prevMask)
		{
			if(downBlock.Block.Uid > pid)
			{
				lights = chunk._LE_Bufpack(x, y - 1, z);
				level.LightEngine.GetBlockLight(lights, batch.LinearCol4, 1);
				u1 = 17 / 128f;
				v1 = 68 / 128f;
				u2 = (17 + 16) / 128f;
				v2 = (68 + 16) / 128f;
				DrawInternal(batch, state, Direction.Down, x, y - 1);
			}
			else
			{
				lights = chunk._LE_Bufpack(x, y, z);
				level.LightEngine.GetBlockLight(lights, batch.LinearCol4, 1);
				u1 = 51 / 128f;
				v1 = 68 / 128f;
				u2 = (51 + 16) / 128f;
				v2 = (68 + 16) / 128f;
				DrawInternal(batch, downBlock, Direction.Up, x, y);
			}
		}
	}

	public void DrawItemSymbol(SpriteBatch batch, BlockState state, float x, float y, float w, float h)
	{
		SetState(batch);
		prevMask = (OGLTexture) BlockRendering.GetMask(state);
		int u = 51, v = 51;
		u1 = u / 128f;
		v1 = v / 128f;
		u2 = (u + 16) / 128f;
		v2 = (v + 16) / 128f;
		DrawItemSymbolInternal(batch, state, x, y, w, h);
		ResetState(batch);
	}

	//Override this for simple customized behaviors.
	//If looking for advanced functions, you can extend the base class.
	protected virtual void DrawInternal(SpriteBatch batch, BlockState state, Direction overlapping, float x, float y, float w = 1, float h = 1)
	{
		Icon icon = BlockRendering.GetIcon(state);
		if(icon != null) batch.Draw(icon, x, y, w, h);
	}

	//Override this for simple customized behaviors.
	//If looking for advanced functions, you can extend the base class.
	protected virtual void DrawItemSymbolInternal(SpriteBatch batch, BlockState state, float x, float y, float w = 1, float h = 1)
	{
		Icon icon = BlockRendering.GetIcon(state);
		if(icon != null) batch.Draw(icon, x, y, w, h);
	}

}
