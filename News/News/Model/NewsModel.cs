using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MySqlConnector;
using News.Common.Data;

namespace News.Model
{
	public class NewsModel
	{
		private const string Database = "NewsSchema";
		
		private MySqlConnection _connection;
		
		public NewsModel(string server, int port, string userId, string password)
		{
			var builder = new MySqlConnectionStringBuilder
			{
				Server = server,
				Port = (uint)port,
				UserID = userId,
				Password = password,
				Database = Database
			};
			_connection = new MySqlConnection(builder.ConnectionString);
		}

		public async Task Create(NewsData data)
		{
			await _connection.OpenAsync();
			
			var command = _connection.CreateCommand();
			
			command.CommandText = "INSERT INTO News(Title,Description,Content,Date)" +
			                      "VALUES(@title,@desc,@content,@date)";
			command.Parameters.AddWithValue("@title", data.Title);
			command.Parameters.AddWithValue("@desc", data.Description);
			command.Parameters.AddWithValue("@content", data.Content);
			command.Parameters.AddWithValue("@date", DateTime.Now);
			
			await command.ExecuteNonQueryAsync();
			
			await _connection.CloseAsync();
		}
	}
}