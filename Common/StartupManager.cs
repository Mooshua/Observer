using System.Diagnostics;
using System.Text;

using Spectre.Console;

namespace Observer.Common;

public class StartupManager
{

	protected List<LoadTask> Tasks = new List<LoadTask>();

	protected List<LoadTask> Post = new List<LoadTask>();
	
	public StartupManager Add(LoadTask task)
	{
		Tasks.Add(task);

		return this;
	}
	
	public StartupManager AddPost(LoadTask task)
	{
		Post.Add(task);

		return this;
	}

	public void Run()
	{

		var load_ctx = new ObserverContext();
		
		Console.OutputEncoding = Encoding.UTF8;		

		AnsiConsole.Status()
			.AutoRefresh(true)
			.Spinner(Spinner.Known.Dots2)
			.SpinnerStyle(Style.Parse("yellow bold"))
			.StartAsync(CoolLog.ResponsiveTexts[Random.Shared.Next(CoolLog.ResponsiveTexts.Count)], async ctx =>
			{
				foreach (LoadTask loadTask in Tasks)
				{

					Stopwatch s = Stopwatch.StartNew();

					ctx.Status($"[yellow]{CoolLog.ResponsiveTexts[Random.Shared.Next(CoolLog.ResponsiveTexts.Count)]} ({loadTask.Name()})[/]");

					LoadTask.Result result = LoadTask.Result.Shutdown;
					try
					{
						result = await loadTask.Run(load_ctx);
					}
					catch (Exception e)
					{
						AnsiConsole.MarkupLine($"[red]{loadTask.Name()}[/] {s.ElapsedMilliseconds}: {e.Message}");
						//	Re-throw to carry it to the top
						throw e;
					}

					if (result == LoadTask.Result.Good)
						AnsiConsole.MarkupLine($"[green]{loadTask.Name()}[/] {s.ElapsedMilliseconds}ms");

					if (result == LoadTask.Result.Shutdown)
					{

						AnsiConsole.MarkupLine("[red]Stopping...[/]");

						throw new Exception();
					}

				}

				ctx.Spinner = Spinner.Known.Dots2;
				ctx.Status("[yellow]Listening...[/]");

				foreach (LoadTask loadTask in Post)
				{
					AnsiConsole.MarkupLine($"[cyan]post_task[/] {loadTask.Name()}");
					//var t = new Thread( () =>
					//{
					try
					{
						loadTask.Run(load_ctx).Wait();
					}
					catch (Exception e)
					{
						AnsiConsole.MarkupLine($"[red]{loadTask.Name()}[/] {e.Message}");
					}
					//}) { IsBackground = false };

					//t.Start();
				}
			}).Wait();



	}
	
}