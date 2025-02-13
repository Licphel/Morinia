using Kinetic.App;
using Kinetic.Auditory;
using Kinetic.Math;

namespace Morinia.Client.TheAudio;

public class Sound
{

	public static Sound None = new Sound(null);
	public static Sound BlockDestructPowder = new Sound("sounds/blocks/destruct_powder.wav");
	public static Sound BlockBreakPowder = new Sound("sounds/blocks/break_powder.wav");

	ID? SoundLocation = null;

	public Sound(string soundloc)
	{
		if(soundloc != null)
			SoundLocation = new ID(soundloc);
	}

	public void PlaySound()
	{
		if(SoundLocation == null)
			return;
		AudioData audioData = Loads.Get(SoundLocation.Value);
		AudioClip clip = audioData.CreateClip();
		clip.Set(ClipController.Gain, Seed.Global.NextFloat(0.75f, 1.25f));
		clip.Set(ClipController.Pitch, Seed.Global.NextFloat(0.8f, 1.2f));
		clip.Play();
	}

}
