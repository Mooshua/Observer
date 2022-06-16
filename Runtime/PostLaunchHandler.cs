using Observer.Common;

using Spectre.Console;

namespace Observer.Runtime;

public class PostLaunchHandler : LoadTask
{
	public override string Name()
	{
		return "observer";
	}

	public override async Task<Result> Run(ObserverContext ctx)
	{
		var cancelToken = new CancellationToken();
		
		AnsiConsole.MarkupLine("[green]Starting server[/]");
		
		await ctx.Server!.StartAsync(cancelToken);
		
		return Result.Good;
	}
}