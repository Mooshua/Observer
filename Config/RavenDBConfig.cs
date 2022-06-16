using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Observer.Common;

using Raven.Client.Documents;

namespace Observer.Config;

public class RavenDBConfig
{

	internal bool Enabled = false;

	internal string Collection = "ObserverEmails";

	internal string Base = "Observer";

	internal List<string> Urls = new List<string>();

	internal X509Certificate2? Certificate;

	internal IDocumentStore? Database { get; set; }

	public void Enable()
	{
		Enabled = true;
		CoolLog.WriteSuccess("observer_config", "[magenta]RavenDB[/] enabled!");
	}

	public void AddUrl(string url)
	{
		Urls.Add(url);
		CoolLog.WriteSuccess("observer_config", $"Added [magenta]RavenDB[/] url: {url}");
	}

	public void SetCollection(string collection)
	{
		Collection = collection;
		CoolLog.WriteSuccess("observer_config", $"Set [magenta]RavenDB[/] collection: {collection}");
	}

	public void SetDatabase(string database)
	{
		Base = database;
		CoolLog.WriteSuccess("observer_config", $"Set [magenta]RavenDB[/] database: {database}");
	}
	
	/// <summary>
	/// Set the certificate by it's path
	/// </summary>
	/// <param name="path"></param>
	public void SetCertificate(string key, string password)
	{
		try
		{
			var contents = File.ReadAllBytes(key);

			var cert = new X509Certificate2(contents, password);
			
			if (!cert.HasPrivateKey)
				throw new Exception($"[magenta]RavenDB[/] Certificate '{key}' does not have a private key attached");

			if (cert.NotAfter.Ticks <= DateTime.Now.Ticks)
				throw new Exception($"[magenta]RavenDB[/] Certificate '{key}' is expired ({cert.NotAfter})");

			Certificate = cert;
			CoolLog.WriteSuccess("observer_config", $"Set [magenta]RavenDB[/] certificate to be for {cert.Subject}");
		}
		catch (CryptographicException e)
		{
			CoolLog.WriteError("observer_config", $"[red]Cryptographic error[/]: {e.Message} ({e.HelpLink})");
		}
		catch (Exception e)
		{
			CoolLog.WriteError("observer_config", e.ToString());
		}
	}
	
}