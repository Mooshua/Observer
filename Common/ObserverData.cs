using Observer.Config;

using SmtpServer = SmtpServer.SmtpServer;

namespace Observer.Common;

public class ObserverData
{

	public global::SmtpServer.SmtpServer? Server;

	public ObserverConfig Config = new ObserverConfig();

	public RavenDBConfig Raven = new RavenDBConfig();

}