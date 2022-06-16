namespace Observer.Common;

public class LoadTask
{

	public enum Result
	{
		Good,
		Shutdown,
	}

	public virtual string Name() => "unknown";
	
	public async virtual Task<Result> Run(ObserverData ctx) => Result.Good;
	
}