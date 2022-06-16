// See https://aka.ms/new-console-template for more information
using System.Reflection;

using Observer.Common;
using Observer.Runtime;
using Observer.Startup;

using Spectre.Console;


AnsiConsole.MarkupLine("[cyan bold]Observer[/] [cyan]by Mooshua & contributors[/]");

var s = new StartupManager();
{
	//	Configure startup tasks
	s.Add(new ConfigTask());
	s.Add(new CreateRavenTask());
	s.Add(new CreateServerTask());

	//	Register server posttask
	s.AddPost(new PostLaunchHandler());
}
//	Hello, world!
s.Run();