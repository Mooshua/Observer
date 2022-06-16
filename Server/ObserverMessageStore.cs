using System.Buffers;

using NLua;

using Observer.Backends;
using Observer.Common;

using Raven.Client.Documents.Session;

using SmtpServer;
using SmtpServer.Mail;
using SmtpServer.Protocol;
using SmtpServer.Storage;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace Observer.Server;

public class ObserverMessageStore : MessageStore, IMessageStoreFactory
{
	public ObserverMessageStore()
	{
	}
	
	public static ObserverContext Info;
	
	public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
	{
		
		AnsiConsole.MarkupLine("[magenta]captured:[/] message from {0}", transaction.From.AsAddress());
		try
		{
			await using var streamBuffer = new MemoryStream();

			{
				var position = buffer.GetPosition(0);
				while (buffer.TryGet(ref position, out var memory))
				{
					await streamBuffer.WriteAsync(memory, cancellationToken);
				}
			}

			streamBuffer.Seek(0, SeekOrigin.Begin);

			var message = await MimeKit.MimeMessage.LoadAsync(streamBuffer, cancellationToken);

			var emails = new ModelMessage(message, cancellationToken);

			if (Info.Hook.Invoke<bool>(Info.Hook.OnStore, out var should, emails, transaction.From, transaction.To))
			{
				if (should)
					foreach (IBackend infoBackend in Info.Backends)
					{
						await infoBackend.Accept(Info, transaction, emails, buffer, cancellationToken);
					}
			}
			else
				//	Default to yes if no adequate response.
				foreach (IBackend infoBackend in Info.Backends)
				{
					await infoBackend.Accept(Info, transaction, emails, buffer, cancellationToken);
				}
			
			if (Info.Hook.Invoke<LuaTable>(Info.Hook.OnRespond, out LuaTable response,  emails, transaction.From, transaction.To))
			{
				if (response!["code"] is not SmtpReplyCode)
				{
					CoolLog.WriteError("lua", "OnRespond: did not provide a 'code' value (got {0}), defaulting to OK.", response!["code"]);
					response["code"] = SmtpReplyCode.Ok;
				}
				
				AnsiConsole.MarkupLine("[green]lua:[/] got response [cyan]{0}[/] [yellow]'{1}'[/]",  (SmtpReplyCode) response["code"], response["message"] as String);
				
				return new SmtpResponse((SmtpReplyCode) response!["code"], response["message"] as String ?? String.Empty);
			}


		}
		catch (Exception e)
		{
			CoolLog.WriteError("observer", "{0}" ,e.ToString());
		}
		
		
		return SmtpResponse.Ok;
	}

	public IMessageStore CreateInstance(ISessionContext context)
	{
		return new ObserverMessageStore();
	}
}