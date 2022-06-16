using System.Buffers;

using Observer.Common;

using Raven.Client.Documents.Session;

using SmtpServer;
using SmtpServer.Mail;
using SmtpServer.Protocol;
using SmtpServer.Storage;

using Spectre.Console;

namespace Observer.Server;

public class ObserverMessageStore : MessageStore, IMessageStoreFactory
{
	public ObserverMessageStore()
	{
	}
	
	public static ObserverData Info;
	
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

			if (Info.Raven.Enabled)
			{
				using (IAsyncDocumentSession session = Info.Raven.Database!.OpenAsyncSession(new SessionOptions()
				       {
					       Database = Info.Raven.Base,
				       }))
				{
					//	Store the message as a new entity
					await session.StoreAsync(new Emails (message, cancellationToken)
					{
						Id = $"{Info.Raven.Collection}/{Guid.NewGuid().ToString()}",
						Time = DateTime.Now,
						From = transaction.From.AsAddress(),
					}, cancellationToken);

					//	Save the changes
					await session.SaveChangesAsync();
				}
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