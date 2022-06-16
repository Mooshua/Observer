using NLua;
using NLua.Event;

using Observer.Common;
using Observer.Config;

namespace Observer.Startup;

public class ConfigTask : LoadTask
{
	public override string Name() => "observer_config";

	public override Task<Result> Run(ObserverData ctx)
	{
		try
		{
			var file = File.ReadAllText("config.lua");
			
			CoolLog.WriteSuccess("observer_config", $"Found lua configuration file at [cyan]'config.lua'[/]");

			using (Lua l = new Lua())
			{
				l["Config"] = ctx.Config;
				l["RavenDB"] = ctx.Raven;
				
				l.HookException += delegate(object? sender, HookExceptionEventArgs args)
				{
					CoolLog.WriteError("observer_config", "Lua error! {0}", args.Exception);
				};
				//	Finally, load the lua and execute it
				l.DoString(file);
			}
			
			CoolLog.WriteSuccess("observer_config", $"Lua configuration file executed successfully!");

			
			return Task.FromResult<Result>(Result.Good);

		}
		catch (Exception e)
		{
			CoolLog.WriteError("observer_config", e.Message);
			
			CoolLog.WriteError("observer_config", "do you have a config.lua in the current working directory?");
			
			return Task.FromResult<Result>(Result.Shutdown);
		}
	}
}