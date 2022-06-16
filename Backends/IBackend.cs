using System.Buffers;

using Observer.Common;
using Observer.Config;

using SmtpServer;

namespace Observer.Backends;

public interface IBackend
{

	Task Accept(ObserverContext config, IMessageTransaction transaction, ModelMessage message, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken);

}