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
			
			/*
			collection.InsertOne(new AuthData
			{
				Email = "name@email.mail",
				Pass = "Kek",
			});
			*/
		}

		public async Task<bool> Create(AuthData data)
		{
			var list = await _collection.Find(x => x.Email == data.Email)
				.ToListAsync();

			if (list != null)
			{
				if (list.Count != 0)
				{
					return false;
				}
			}

			await _collection.InsertOneAsync(data);

			return true;
		}
		
		public async Task<bool> Get(AuthData data)
		{
			var list = await _collection.Find(x => x.Email == data.Email && 
			                                       x.Pass == data.Pass)
				.ToListAsync();

			if (list == null)
			{
				return false;
			}

			if (list.Count == 0)
			{
				return false;
			}

			return true;
		}
	}
}