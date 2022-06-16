using System.Diagnostics;

using Spectre.Console;

namespace Observer.Common;

public static class CoolLog
{

	public static List<string> ResponsiveTexts = new List<string>()
	{
		"Baking cookies...", "Laundering money...", "Meditating...", "Crying in the corner...",  "Watching movies...", "Setting up the server, I guess...",
		"Honestly, just Control+C at this point...", "Logging into Outlook...",
	};

	public static void WriteError(string task, string format, params object[] args)
	{
		AnsiConsole.MarkupLine($"observer [bold red]{task} (err):[/] {format}", args);
	}

	public static void WriteSuccess(string task, string format, params object[] args)
	{
		AnsiConsole.MarkupLine($"observer [yellow]{task}:[/] {format}", args);

	}
	
}