using Observer.Common;

using SmtpServer;
using SmtpServer.Mail;
using SmtpServer.Storage;

using Spectre.Console;

namespace Observer.Server;

public class ObserverMailboxFilter : MailboxFilter, IMailboxFilterFactory
{
	public static ObserverData Info;

	public override async Task<MailboxFilterResult> CanAcceptFromAsync(ISessionContext context, IMailbox from, int size, CancellationToken cancellationToken)
	{
		AnsiConsole.MarkupLine("[cyan]request[/] from {0}", from.AsAddress());
		
		if (size >= Info.Config.MaxSize)
		{
			AnsiConsole.MarkupLine("[cyan]Size limit:[/] message from {0} exceeded {1} byte limit with [red]{2}[/]bytes", from.ToString(), Info.Config.MaxSize, size);
			return MailboxFilterResult.SizeLimitExceeded;
		}

		return MailboxFilterResult.Yes;
	}


	public override async Task<MailboxFilterResult> CanDeliverToAsync(ISessionContext context, IMailbox to, IMailbox from, CancellationToken cancellationToken)
	{
		AnsiConsole.MarkupLine("[cyan]mail[/] from {0} to {1}", from.AsAddress(), to.AsAddress());
		return MailboxFilterResult.Yes;
	}

	public IMailboxFilter CreateInstance(ISessionContext context)
	{
		return new ObserverMailboxFilter();
	}
}