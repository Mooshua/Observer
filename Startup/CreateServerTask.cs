using Observer.Common;
using Observer.Server;

using SmtpServer;
using SmtpServer.Authentication;
using SmtpServer.ComponentModel;
using SmtpServer.Mail;
using SmtpServer.Storage;

using Spectre.Console;

namespace Observer.Startup;

public class CreateServerTask : ConfigTask
{
	public override string Name() => "create_server";

	public override async Task<Result> Run(ObserverContext ctx)
	{
		try
		{
			var config = new SmtpServerOptionsBuilder();

			config.ServerName(ctx.Config.ServerName);

			foreach (ushort configSecurePort in ctx.Config.SecurePorts)
			{
				config.Endpoint(port =>
				{
					port.IsSecure(true);
					port.AllowUnsecureAuthentication(false);
					port.Port(configSecurePort, true);
					port.Certificate(ctx.Config.Certificate);
				});
			}

			foreach (ushort configUnsecurePort in ctx.Config.UnsecurePorts)
			{
				config.Endpoint(port =>
				{
					if (ctx.Config.Certificate is not null)
					{
						port.IsSecure(false);
						//port.AllowUnsecureAuthentication();
						port.Port(configUnsecurePort);
						port.Certificate(ctx.Config.Certificate);
					}
					else
					{
						port.IsSecure(false);
						port.Port(configUnsecurePort);
					}
				});
			}

			ISmtpServerOptions options = config.Build();

			ObserverMailboxFilter.Info = ctx;
			ObserverMessageStore.Info = ctx;

			var provider = new ServiceProvider();
			provider.Add((IMessageStoreFactory)new ObserverMessageStore());
			provider.Add((IMailboxFilterFactory)new ObserverMailboxFilter());
			provider.Add((IUserAuthenticatorFactory)new ObserverAuth());

			var server = new SmtpServer.SmtpServer(options, provider);

			ctx.Server = server;
		}
		catch (Exception e)
		{
			CoolLog.WriteError("create_server", "Exception while configuring server: {0}", e);
			return Result.Shutdown;
		}

		return Result.Good;

	}
}