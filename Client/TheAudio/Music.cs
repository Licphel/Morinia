using Kinetic.App;
using Kinetic.Auditory;

namespace Morinia.Client.TheAudio;

public class Music
{

	public static Music None = new Music(null);
	public static Music Track1 = new Music("musics/Fall.wav");
	public static Music Track2 = new Music("musics/Electric Heritage.wav");

	public static List<Music> Musics = new List<Music>() { Track1, Track2 };

	ID? SoundLocation = null;

	public Music(string soundloc)
	{
		if(soundloc != null)
			SoundLocation = new ID(soundloc);
	}

	public float PlayMusic()
	{
		if(SoundLocation == null)
			return 0;
		AudioData audioData = Loads.Get(SoundLocation.Value);
		AudioClip clip = audioData.CreateClip();
		clip.Set(ClipController.Gain, 0.75f);
		clip.Play();
		return audioData.GetLengthSeconds();
	}

}
