using System.Text;

using NLua;
using NLua.Event;

using Observer.Common;
using Observer.Config;

using SmtpServer.Protocol;
using SmtpServer.Storage;

using Spectre.Console;

namespace Observer.Startup;

public class ConfigTask : LoadTask
{
	public override string Name() => "observer_config";
	
	public void Print(params object[] print)
	{
		StringBuilder asString = new StringBuilder();
					
		for (var i = 0; i < print.Length; i++)
		{
			asString.Append(print[i].ToString());
			asString.Append('\t');
		}
					
		AnsiConsole.MarkupLine("[green]lua:[/] {0}", asString.ToString().EscapeMarkup());
	}

	public override Task<Result> Run(ObserverContext ctx)
	{
		try
		{
			var file = File.ReadAllText("config.lua");
			
			CoolLog.WriteSuccess("observer_config", $"Found lua configuration file at [cyan]'config.lua'[/]");

			Lua l = ctx.State;
			
			l["print"] = Print;
			l["Config"] = ctx.Config;
			l["RavenDB"] = ctx.Raven;
			l["Hooks"] = ctx.Hook;
			//	Constants
			l["ResponseCode"] = new SmtpReplyCode();
			l["FilterCode"] = new MailboxFilterResult();
			
			l.HookException += delegate(object? sender, HookExceptionEventArgs args)
			{
				CoolLog.WriteError("observer_config", "Lua error! {0}", args.Exception.ToString().EscapeMarkup());
			};
			//	Finally, load the lua and execute it
			l.DoString(file);
			
			
			CoolLog.WriteSuccess("observer_config", $"Lua configuration file executed successfully!");

			
			return Task.FromResult<Result>(Result.Good);

		}
		catch (Exception e)
		{
			CoolLog.WriteError("observer_config", e.Message.EscapeMarkup());
			
			CoolLog.WriteError("observer_config", "do you have a config.lua in the current working directory?");
			
			return Task.FromResult<Result>(Result.Shutdown);
		}
	}
}