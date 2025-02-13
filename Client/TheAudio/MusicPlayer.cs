using Kinetic;
using Kinetic.App;
using Kinetic.Math;

namespace Morinia.Client.TheAudio;

public class MusicPlayer
{

	public float SecondsToSwitch = Seed.Global.NextFloat(1, 10);

	public void Update()
	{
		SecondsToSwitch -= Time.Delta;

		if(SecondsToSwitch <= 0)
		{
			Music music = Seed.Global.Select(Music.Musics);
			SecondsToSwitch = music.PlayMusic() + Seed.Global.NextFloat(5, 120);
		}
	}

}
