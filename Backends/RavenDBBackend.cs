using System.Buffers;

using Observer.Common;
using Observer.Config;
using Observer.Extensions;

using Raven.Client.Documents.Session;

using SmtpServer;
using SmtpServer.Mail;

namespace Observer.Backends;

public class RavenDbBackend : IBackend
{

	private class Emails
	{
		public string Id { get; set; }
		
		public string From { get; set; }

		public List<string> To { get; set; }
		
		public ModelMessage Mime { get; set; }
	}

	public async Task Accept(ObserverContext ctx, IMessageTransaction transaction, ModelMessage message, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
	{
		using (IAsyncDocumentSession session = ctx.Raven.Database!.OpenAsyncSession(new SessionOptions()
		       {
			       Database = ctx.Raven.Base,
		       }))
		{

			List<string> to = new List<string>();
			
			foreach (IMailbox mailbox in transaction.To)
			{
				to.Add(mailbox.AsAddress());
			}
			
			var id = $"{ctx.Raven.Collection}/{Guid.NewGuid().ToString()}";
			//	Store the message as a new entity
			await session.StoreAsync(new Emails()
			{
				From = transaction.From.AsAddress(),
				Id = id,
				To = to,
				Mime = message
			});
			
			session.Advanced.Attachments.Store(id, "message.bin", await buffer.ToStream(cancellationToken), "application/octet-stream");

			//	Save the changes
			await session.SaveChangesAsync();
		}
	}
	
}