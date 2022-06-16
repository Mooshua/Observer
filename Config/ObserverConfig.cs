using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Observer.Common;

namespace Observer.Config;

public class ObserverConfig
{
	
	internal int MaxSize = Int32.MaxValue;

	internal List<ushort> UnsecurePorts = new List<ushort>();

	internal List<ushort> SecurePorts = new List<ushort>();

	internal X509Certificate2? Certificate;

	internal string ServerName = "Observer Untitled";

	public void SetName(string name) => ServerName = name;
	
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
				throw new Exception($"Certificate '{key}' does not have a private key attached");

			if (cert.NotAfter.Ticks <= DateTime.Now.Ticks)
				throw new Exception($"Certificate '{key}' is expired ({cert.NotAfter})");

			Certificate = cert;
			CoolLog.WriteSuccess("observer_config", $"Set certificate to be for {cert.Subject}");
		}
		catch (CryptographicException e)
		{
			CoolLog.WriteError("observer_config", $"[red]Cryptographic error[/]: {e.Message} ({e.HelpLink})");
		}
		catch (Exception e)
		{
			CoolLog.WriteError("observer_config", e.Message);
		}
	}

	public void SetMaxSize(int maxsize)
	{
		CoolLog.WriteSuccess("observer_config", $"Set max size to [blue]{maxsize}[/] bytes");
		MaxSize = maxsize;
	}

	public void AddSecurePort(ushort port)
	{
		CoolLog.WriteSuccess("observer_config", $"Added [green]secure[/] port [blue]{port}[/]");
		SecurePorts.Add(port);
	}

	public void AddUnsecurePort(ushort port)
	{
		CoolLog.WriteSuccess("observer_config", $"Added [red]unsecure[/] port [blue]{port}[/]");
		UnsecurePorts.Add(port);
	}

}