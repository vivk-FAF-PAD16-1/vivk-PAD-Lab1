using MongoDB.Bson;

namespace Auth.Common
{
	public class AuthData
	{
		public ObjectId Id { get; set; }
		public string Email { get; set; }
		public string Pass { get; set; }

		public bool IsValid()
		{
			return Email != null && Email.Length == 0 &&
			       Pass != null && Pass.Length == 0;
		}
	}
}