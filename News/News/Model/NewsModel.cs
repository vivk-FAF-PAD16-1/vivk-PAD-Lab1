using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MySqlConnector;
using News.Common;
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

		public async Task Get(int number, List<NewsData> dest)
		{
			await _connection.OpenAsync();

			var command = _connection.CreateCommand();

			command.CommandText = "SELECT * FROM News " +
			                      "ORDER BY Date DESC " +
			                      "LIMIT @number";
			command.Parameters.AddWithValue("@number", number);

			var reader = await command.ExecuteReaderAsync();
			while (reader.Read())
			{
				var data = new NewsData
				{
					Id = (int)reader["Id"],
					Title = (string)reader["Title"],
					Description = (string)reader["Description"],
					Content = (string)reader["Content"],
				};

				dest.Add(data);
			}

			await _connection.CloseAsync();
		}

		public async Task<(NewsData, bool)> Get(int id)
		{
			await _connection.OpenAsync();

			var command = _connection.CreateCommand();

			command.CommandText = "SELECT * FROM News " +
			                      "WHERE Id=@id";
			command.Parameters.AddWithValue("@id", id);

			var reader = await command.ExecuteReaderAsync();
			
			var data = new NewsData();
			var isFound = false;
			while (reader.Read())
			{
				data.Id = (int)reader["Id"];
				data.Title = (string)reader["Title"];
				data.Description = (string)reader["Description"];
				data.Content = (string)reader["Content"];

				isFound = true;
				break;
			}
			
			await _connection.CloseAsync();	

			return (data, isFound);
		}
	}
}