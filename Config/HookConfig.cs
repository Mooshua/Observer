using NLua;

using Observer.Common;

using Raven.Client.ServerWide.Operations;

namespace Observer.Config;

public class HookConfig
{
	
	/// <summary>
	/// Whether or not to consider the following mail transmission.
	/// Arg 1: FromAddress
	/// Arg 2: Size
	/// </summary>
	public LuaFunction? OnTransmit { get; set; }
	
	/// <summary>
	/// Whether or not to allow this mail in to the storage.
	/// Arg 1: FromAddress
	/// Arg 2: ToAddress
	/// </summary>
	public LuaFunction?	OnReceive { get; set; }
	
	/// <summary>
	/// Whether or not we should store this mail item
	/// Arg 1: Mime Entity
	/// Arg 2: FromAddress
	/// Arg 3: ToAddress
	/// </summary>
	public LuaFunction? OnStore { get; set; }
	
	/// <summary>
	/// Generate a response code for this mail item
	/// Arg 1: Mime Entity
	/// Arg 2: FromAddress
	/// Arg 3: ToAddress
	///
	/// Returns: Number
	/// </summary>
	public LuaFunction? OnRespond { get; set; }

	public bool Invoke<TResult>(LuaFunction? hook, out TResult? result, params object[] input)
	{
		result = default;
		
		if (hook is null)
			return false;
		try
		{
			var call = hook.Call(input);

			if (call is null)
				return false;

			if (call.Length == 0)
				return false;

			if (call[0] is TResult)
			{
				result = (TResult)call[0];
				return true;
			}
		}
		catch (Exception e)
		{
			CoolLog.WriteError("lua", "Hook error: {0}", e.ToString());
		}
		return false;
	}

}