using NLua;

using Observer.Backends;
using Observer.Config;

using SmtpServer = SmtpServer.SmtpServer;

namespace Observer.Common;

public class ObserverContext
{
	
	public List<IBackend> Backends = new List<IBackend>();

	public global::SmtpServer.SmtpServer? Server;

	public ObserverConfig Config = new ObserverConfig();

	public RavenDBConfig Raven = new RavenDBConfig();

	public HookConfig Hook = new HookConfig();

	public Lua State = new Lua();
}