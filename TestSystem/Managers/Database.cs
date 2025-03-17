using Microsoft.Data.Sqlite;

namespace TestSystem.Managers;

public partial class Database
{
    private SqliteConnection _connection;

    private Database()
    {
        _connection = new SqliteConnection("Data Source=\"G:\\магистратура\\СППР\\TestSystem\\DataBase.db\"");
        _connection.Open();
    }

    public void ExecuteNonQuery(string query, params SqliteParameter[] parameters)
    {
        CreateCommand(query, parameters).ExecuteNonQuery();
    }

    public SqliteDataReader ExecuteQuery(string query, params SqliteParameter[] parameters)
    {
        return CreateCommand(query, parameters).ExecuteReader();
    }

    public object? ExecuteScalar(string query, params SqliteParameter[] parameters)
    {
        return CreateCommand(query, parameters).ExecuteScalar();
    }

    private SqliteCommand CreateCommand(string query, params SqliteParameter[] parameters)
    {
        var command = new SqliteCommand(query, _connection);

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        return command;
    }
}

public partial class Database
{
    private static Database _instance;

    public static Database Instance
    {
        get
        {
            _instance ??= new Database();
            return _instance;
        }
    }
}

