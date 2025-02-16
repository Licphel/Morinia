namespace Morinia.World.TheGen;

public interface Generator
{

	void Provide(int coord);

	bool ProvideAsync(int coord);

	Chunk ProvideEmpty(int coord);

	void GetLocationData(int x, int y, int surface, GenerateContext ctx);

}
