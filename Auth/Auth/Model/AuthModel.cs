using System.Linq;
using System.Threading.Tasks;
using Auth.Common;
using MongoDB.Driver;

namespace Auth.Model
{
	public class AuthModel
	{
		private readonly IMongoCollection<AuthData> _collection;

		public AuthModel(string connection, string dbName, string collectionName)
		{
			var client = new MongoClient(connection);
			var database = client.GetDatabase(dbName);
			_collection = database.GetCollection<AuthData>(collectionName);
		}

		public async Task<string> Create(AuthData data)
		{
			var list = await _collection.Find(x => x.Email == data.Email)
				.ToListAsync();

			if (list != null)
			{
				if (list.Count != 0)
				{
					return $"User with Email=[{data.Email}] exist!";
				}
			}

			await _collection.InsertOneAsync(data);

			return null;
		}
		
		public async Task<string> Get(AuthData data)
		{
			var list = await _collection.Find(x => x.Email == data.Email)
				.ToListAsync();

			if (list == null)
			{
				return $"User with Email=[{data.Email}] not exist!";
			}

			if (list.Count == 0)
			{
				return $"User with Email=[{data.Email}] not exist!";
			}

			var user = list.First();
			if (user.Pass != data.Pass)
			{
				return $"User with Email=[{data.Email}] have another password!";
			}

			return null;
		}
	}
}