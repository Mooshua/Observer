using System.Collections;

using MimeKit;

using Newtonsoft.Json;

namespace Observer;

public class Emails
{

	public Emails(MimeMessage m, CancellationToken t)
	{
		Mime = new ModelMessage(m, t);
	}

	public class ModelMessage
	{
		public class ModelAddressList :  IList<string>
		{

			public class WhyDoIHaveToDoThis : IEnumerator<string>
			{
				[JsonIgnore]
				private IEnumerator<InternetAddress> _l;

				public WhyDoIHaveToDoThis(IEnumerator<InternetAddress> l)
				{
					_l = l;
				}

				public bool MoveNext()
				{
					if (!_l.MoveNext())
						return false;

					Current = _l.Current.Name;
					
					return true;
				}

				public void Reset()
				{
					_l.Reset();
					Current = null;
				}

				public string Current { get; private set; }

				object IEnumerator.Current => Current;

				public void Dispose()
				{
					_l.Dispose();
				}
			}

			[JsonIgnore]
			private InternetAddressList List;
			
			public ModelAddressList(InternetAddressList l)
			{
				List = l;
			}


			public IEnumerator<string> GetEnumerator()
			{
				return new WhyDoIHaveToDoThis(List.GetEnumerator());
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void Add(string item)
			{
				throw new NotImplementedException();
			}

			public void Clear()
			{
				throw new NotImplementedException();
			}

			public bool Contains(string item)
			{
				throw new NotImplementedException();
			}

			public void CopyTo(string[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			public bool Remove(string item)
			{
				throw new NotImplementedException();
			}

			public int Count => List.Count;

			public bool IsReadOnly { get; } = true;

			public int IndexOf(string item)
			{
				throw new NotImplementedException();
			}

			public void Insert(int index, string item)
			{
				throw new NotImplementedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotImplementedException();
			}

			public string this[int index]
			{
				get => List[index].Name;
				set => throw new NotImplementedException();
			}
		}
		
		public class ModelEntity
		{

			public ModelEntity(MimeEntity e, CancellationToken t)
			{

				MemoryStream s = new MemoryStream();
				
				e.WriteTo(new FormatOptions()
				{
					
				}, s, true, t);

				Contents = s.ToArray();

				Type = e.ContentType?.ToString();

				Base = e.ContentBase?.ToString();

				Location = e.ContentLocation?.ToString();

				Disposition = e?.ContentDisposition;

			}

			public string? Id;

			public string? Base;

			public ContentDisposition? Disposition;

			public string? Location;

			public string? Type;

			public byte[] Contents;
		}
		
		public ModelMessage(MimeMessage m, CancellationToken t)
		{
			_redirect = m;

			if (m.Body is not null)
			{
				//	Handle Mime entities
				Body = new ModelEntity(m.Body, t);

				//	If body is multipart, deserialize all parts
				foreach (MimeEntity mBodyPart in m.BodyParts)
				{
					MultipartBody.Add(
						new ModelEntity(mBodyPart, t)
						);
				}

				foreach (MimeEntity mAttachment in m.Attachments)
				{
					Attachments.Add(
						new ModelEntity(mAttachment, t));
				}
			}
		}
		
		[JsonIgnore]
		internal MimeMessage _redirect;

		#region Metadata
		
		public MessagePriority Priority => _redirect.Priority;

		public XMessagePriority XMessagePriority => _redirect.XPriority;
		
		public MessageImportance Importance => _redirect.Importance;

		public MailboxAddress Sender => _redirect.Sender;
		
		public ModelAddressList From => new ModelAddressList(_redirect.From);

		public ModelAddressList ResentFrom => new ModelAddressList(_redirect.ResentFrom);

		public ModelAddressList ReplyingTo => new ModelAddressList(_redirect.ReplyTo);

		public ModelAddressList ReplyingToResent => new ModelAddressList(_redirect.ResentReplyTo);

		public ModelAddressList Recipients => new ModelAddressList(_redirect.To);

		public ModelAddressList ResentRecipients => new ModelAddressList(_redirect.ResentTo);

		public ModelAddressList CarbonCopy => new ModelAddressList(_redirect.Cc);

		public ModelAddressList ResentCarbonCopy => new ModelAddressList(_redirect.ResentCc);

		public ModelAddressList BlindCarbonCopy => new ModelAddressList(_redirect.Bcc);

		public ModelAddressList ResentBlindCarbonCopy => new ModelAddressList(_redirect.ResentBcc);

		public string Subject => _redirect.Subject;

		public DateTimeOffset Date => _redirect.Date;

		public DateTimeOffset ResentDate => _redirect.ResentDate;

		public MessageIdList ThreadChain => _redirect.References;

		public string ThreadParent => _redirect.InReplyTo;
		
		#endregion

		public string MessageId => _redirect.MessageId;

		public string ResentMessageId => _redirect.ResentMessageId;

		public ModelEntity Body;

		public List<ModelEntity> MultipartBody = new List<ModelEntity>();

		public List<ModelEntity> Attachments = new List<ModelEntity>();

	}

	public string Id = Guid.NewGuid().ToString();

	public string? MessageId => Mime?.MessageId;

	public Version? MimeVersion => Mime?._redirect?.MimeVersion;
	
	public DateTime Time { get; set; }
	
	public string From { get; set; }
	
	public ModelMessage? Mime { get; set; }
}