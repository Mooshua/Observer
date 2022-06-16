using System.Buffers;

namespace Observer.Extensions;

public static class ReadOnlySequenceExtensions
{
	public static async Task<MemoryStream> ToStream(this ReadOnlySequence<byte> buffer, CancellationToken cancel)
	{
		var streamBuffer = new MemoryStream();

		{
			var position = buffer.GetPosition(0);
			while (buffer.TryGet(ref position, out var memory))
			{
				await streamBuffer.WriteAsync(memory, cancel);
			}
		}

		streamBuffer.Seek(0, SeekOrigin.Begin);

		return streamBuffer;
	}
}