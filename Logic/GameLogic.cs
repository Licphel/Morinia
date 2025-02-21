using Kinetic;
using Kinetic.App;
using Kinetic.Input;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client;
using Morinia.Client.TheAudio;
using Morinia.Client.TheCelesphere;
using Morinia.Content;
using Morinia.Content.TheAccessBridge;
using Morinia.Content.TheEntity;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheItem;
using Morinia.World.TheLiquid;

namespace Morinia.Logic;

public class GameLogic
{

	public static VaryingVector2 ViewLimit = new VaryingVector2(0.005f, 0.035f);

	public PerspectiveCamera Camera = new PerspectiveCamera();
	public float cdx, cdy;

	//World assoc
	public Celesphere Celesphere;
	public float cx, cy;
	public BlockPos HoverPos;
	public BlockPos BreakPos;
	public BlockState HoverBlockState = BlockState.Empty;
	public float BreakProgress;
	public float Hdnow;
	public Level Level;

	float nowScale = 0.05f;
	public bool PausingItn;
	public float pcdx, pcdy;
	public float pcx, pcy;
	public EntityPlayer Player;
	float pScale = 0.05f;

	//Camera smoothing
	float sAcc;
	float scrollBuf;
	float sDir;

	public MatrixerFloat Transform = new MatrixerFloat();
	public MatrixerAbsolute TransformCelesph = new MatrixerAbsolute();

	public Box Viewdim;
	public MusicPlayer MusicPlayer = new MusicPlayer();

	public GameLogic()
	{
		Level = new Level();//Need registry firstly.
		Level.TryLoad();

		if(Level.SavedPlayer == null)
		{
			Level.SavedPlayer = Player = new EntityPlayer { Pos = new PrecisePos(0, 128) };
			Player.Inv.Add(Items.IronPickaxe.Instantiate());
			Player.Inv.Add(Items.IronShovel.Instantiate());
			Player.Inv.Add(Items.IronAxe.Instantiate());
			Level.Generator.Provide(Player.Pos.UnitX);
		}
		else
		{
			Player = Level.SavedPlayer;
			Level.Generator.Provide(Player.Pos.UnitX);
			Player.Locate(Player.Pos.x, Level.GetChunk(Player.Pos.UnitX).Surface[Player.Pos.BlockX & 15] + 2);
		}
		
		Player.IsPlayerControl = true;
		Level.AddEntity(Player);

		Celesphere = new Celesphere(Level);
		cx = Player.Pos.x;
		cy = Player.Pos.y;
	}

	public bool Pausing => PausingItn;

	public void Tick()
	{
		if(Level == null) return;
		TickPausable();
	}

	public void TickPausable()
	{
		MusicPlayer.Update();

		if(!Pausing)
		{
			//Make 6-block jump a little difficult.
			const float s = 8.6f;
			const float j = 6.5f;
			float a = s * 6 * Time.Delta;

			//Player move
			if(KeyBinds.LEFT_MOVE.Holding() && Player.Velocity.x > -s)
			{
				Player.Velocity.x = FloatMath.Clamp(Player.Velocity.x - a, -s, 9999);
			}
			if(KeyBinds.RIGHT_MOVE.Holding() && Player.Velocity.x < s)
			{
				Player.Velocity.x = FloatMath.Clamp(Player.Velocity.x + a, -9999, s);
			}
			if(KeyBinds.UP_MOVE.Holding())
			{
				Player.Velocity.y += j / 10f * Player.BlockStaying.GetFloatingForce(Player);
			}
			if(KeyBinds.DOWN_MOVE.Holding())
			{
				float f = Math.Max(Player.BlockStaying.GetFloatingForce(Player), Player.BlockStepping.GetFloatingForce(Player));
				Player.Velocity.y -= j / 10f * f;
			}
			if(KeyBinds.JUMP.Holding() && Player.TouchDown)
			{
				Player.Velocity.y = j;
			}

			//PLY

			float cmx = Transform.Cursor.x;
			float cmy = Transform.Cursor.y;

			HoverPos = new BlockPos(FloatMath.Floor(cmx), FloatMath.Floor(cmy), KeyBinds.CONTROL.Holding() ? 0 : 1);
			BlockState HoverInst = HoverBlockState = Level.GetBlock(HoverPos);

			//Use items or click blocks
			if(KeyBinds.PLACE.Holding())
			{
				InterResult ir = Player.Inv.Get(Player.InvCursor).OnClickBlock(Player, Level, HoverPos);

				if(KeyBinds.PLACE.Pressed() && ir != InterResult.Success && HoverInst.HasCABridge(HoverPos, Player))
				{
					AccessBridge cab = HoverInst.CreateCABridge(HoverPos, Player);
					AccessBridge.OpenGracefully(Player, cab);
					PausingItn = !PausingItn;
				}
			}

			//Break blocks
			if(KeyBinds.BREAK.Holding())
			{
				if(BreakProgress > 0 && BreakPos != HoverPos)
					BreakProgress = 0;
				BreakPos = HoverPos;
				BlockState state = Level.GetBlock(BreakPos);

				if(!state.IsEmpty)
				{
					Hdnow = state.GetHardness();
					float boost = Player.Inv[Player.InvCursor].GetToolEfficiency();
					bool boostapply = Player.Inv[Player.InvCursor].GetToolType().IsTarget(state.GetMaterial());
					BreakProgress += Time.Delta * (boostapply ? boost : 1);
					if(Application.PeriodicTask(0.4f))
					{
						state.GetMaterial().SoundBreak.PlaySound();
					}
					if(BreakProgress >= Hdnow)
					{
						Level.BreakBlock(BreakPos, true);
						BreakProgress = 0;
					}
				}
				else
				{
					BreakProgress = 0;
				}
			}
			else
			{
				BreakProgress = 0;
			}

			//Open inventory
			if(KeyBinds.INVENTORY.Pressed())
			{
				AccessBridgeInventory c = new AccessBridgeInventory(Player);
				AccessBridge.OpenGracefully(Player, c);
				PausingItn = !PausingItn;
			}

			if(KeyBinds.CONFIRM.Holding())
			{
				Player.Locate(Player.Pos.x, 150);
			}

			Player.Face.x = cmx < Player.Pos.x ? -1 : 1;
		}

		ListenScrollEvent();
		UpdateCamera();
		Level.LightEngine.CalculateByViewdim(Viewdim);
		Level.Tick();
		Celesphere.Tick();
	}

	public void Draw(SpriteBatch batch)
	{
		if(Level == null) return;

		float x1 = Time.Lerp(pcdx, cdx) + Time.Lerp(pcx, cx);
		float y1 = Time.Lerp(pcdy, cdy) + Time.Lerp(pcy, cy);

		Celesphere.CenterMoved(x1, y1 - Chunk.SeaLevel - 15);

		float screenWidth = GraphicsDevice.Global.Size.x;
		float screenHeight = GraphicsDevice.Global.Size.y;
		float screenRatio = screenHeight / screenWidth;

		if(float.IsNaN(screenRatio)) return;//means the window is back running.

		float w = Camera.Viewport.w * Camera.ScaleX;
		float h = Camera.Viewport.h * Camera.ScaleY;

		if(float.IsNaN(w) || float.IsNaN(h)) return;//means the window is back running.

		if(y1 + h / 2f >= Chunk.MaxY)
		{
			y1 = Chunk.MaxY - h / 2f;
		}
		if(y1 - h / 2f <= 0)
		{
			y1 = h / 2f;//limit
		}

		Viewdim.Resize(w, h);
		Viewdim.LocateCentral(x1, y1);

		Camera.Viewport.Locate(0, 0);
		Camera.Viewport.Resize(Game.ScaledSize.y / screenRatio, Game.ScaledSize.y);
		Camera.Center.x = x1;
		Camera.Center.y = y1;
		Camera.ScaleX = Camera.ScaleY = FloatMath.Clamp(Time.Lerp(pScale, nowScale), ViewLimit.x, ViewLimit.y);
		Camera.Push();

		TransformCelesph.DoTransform(batch);
		batch.UseCamera(TransformCelesph.Camera);
		Celesphere.Draw(batch, Level.Ticks, Level.TicksPerDay, Player.Pos);
		batch.EndCamera(TransformCelesph.Camera);

		Transform.DoTransform(batch, Camera);
		batch.UseCamera(Camera);
		WorldTessellator.Draw(batch, Level, Viewdim);
		if(BreakProgress > 0)
		{
			int brx = (int) (BreakProgress / Hdnow * 8) * 16;
			batch.Draw(GameTextures.ParticleBreakblock, BreakPos.BlockX, BreakPos.BlockY, 1, 1, brx, 0, 16, 16);
		}
		if(!Level.GetBlock(HoverPos).IsEmpty)
		{
			batch.Color4(1, 1, 1, 0.05f + 0.005f * FloatMath.SinRad(Time.Seconds / 3));
			batch.Fill(HoverPos.BlockX, HoverPos.BlockY, 1, 1);
			batch.NormalizeColor();
		}
		batch.EndCamera(Camera);
	}

	void ListenScrollEvent()
	{
		if(Pausing)
			return;

		int slot = Player.InvCursor;

		switch(RemoteKeyboard.Global.ScrollDirection)
		{
			case ScrollDirection.DOWN:
				Player.InvCursor = slot + 1;
				RemoteKeyboard.Global.ConsumeCursorScroll();
				break;
			case ScrollDirection.UP:
				Player.InvCursor = slot - 1;
				RemoteKeyboard.Global.ConsumeCursorScroll();
				break;
		}
		if(Player.InvCursor == 9)
		{
			Player.InvCursor = 0;
		}
		if(Player.InvCursor == -1)
		{
			Player.InvCursor = 8;
		}
	}

	void UpdateCamera()
	{
		bool smv = false;

		if(KeyBinds.ZOOM_OUT.Holding() && !Pausing)
		{
			sAcc = 0.5f * nowScale;
			sDir = 1;
			smv = true;
		}
		if(KeyBinds.ZOOM_IN.Holding() && !Pausing)
		{
			sAcc = 0.4f * nowScale;
			sDir = -1;
			smv = true;
		}

		if(!smv)
		{
			sAcc *= 0.9f;
		}

		pScale = nowScale;
		nowScale += sAcc * sDir * 0.05f;
		nowScale = FloatMath.Clamp(nowScale, ViewLimit.x, ViewLimit.y);

		pcdx = cdx;
		pcdy = cdy;
		pcx = cx;
		pcy = cy;

		VaryingVector2 cursor = Game.MatrixerFlow.Cursor;
		float dx = cursor.x - GraphicsDevice.Global.Size.x / 2f;
		float dy = cursor.y - GraphicsDevice.Global.Size.y / 2f;

		float dt = 0.015f;//a %.2f limit so try div it
		float dl = 0.1f;

		//check to keep validity while back running
		if(!float.IsNaN(dx) && !float.IsNaN(dy) && !float.IsNaN(dl) && !Pausing)
		{
			dx *= dt / 16f;
			dy *= dt / 16f;
			cdx += (dx - cdx) * dl;
			cdy += (dy - cdy) * dl;
		}

		cx += (Player.Pos.x - cx) * dl;
		cy += (Player.Pos.y - cy) * dl;
	}

}
