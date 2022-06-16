﻿using Observer.Common;

using Raven.Client.Documents;

using Spectre.Console;

namespace Observer.Startup;

public class CreateRavenTask : LoadTask
{
	public override string Name() => "create_raven";

	public override async Task<Result> Run(ObserverData ctx)
	{
		if (ctx.Raven.Enabled)
		{
			CoolLog.WriteSuccess("create_raven", "RavenDB enabled!");
			try
			{
				ctx.Raven.Database = new DocumentStore()
				{
					Urls = ctx.Raven.Urls.ToArray(),

					Conventions =
					{
						UseOptimisticConcurrency = true
					},
					
					Certificate = ctx.Raven.Certificate,

					Database = ctx.Raven.Base,
				}.Initialize();
				CoolLog.WriteSuccess("create_raven", "Created RavenDB connection");

			}
			catch (Exception e)
			{
				AnsiConsole.MarkupLine($"Raven err? {e.ToString()}");
			}
		}
		return Result.Good;
	}
}