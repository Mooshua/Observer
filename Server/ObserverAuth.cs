using SmtpServer;
using SmtpServer.Authentication;

using Spectre.Console;

namespace Observer.Server;

public class ObserverAuth : UserAuthenticator, IUserAuthenticatorFactory
{
	public override async Task<bool> AuthenticateAsync(ISessionContext context, string user, string password, CancellationToken cancellationToken)
	{
		AnsiConsole.WriteLine("[cyan]auth:[/] Login Attempt: {0} [red]*{1}*[/]", user, password);
		return false;
	}

	public IUserAuthenticator CreateInstance(ISessionContext context)
	{
		return new ObserverAuth();
	}
}