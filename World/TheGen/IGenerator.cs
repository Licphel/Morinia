namespace Morinia.World.TheGen;

public interface IGenerator
{

	void Provide(int coord);

	bool GenerateAsync(int coord);

	Chunk MakeEmptyChunk(int coord);

}
