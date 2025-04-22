using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;

public class PlayerStatsDatabaseConn
{
    private string dbPath;

    public PlayerStatsDatabaseConn()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/Database.db";
        Debug.Log($"📂 Database Path: {dbPath}");
    }

    public int GetHealth()
    {
        return GetIntValue("health");
    }

    public float GetSpeed()
    {
        return GetFloatValue("speed");
    }

    public float GetAttackPower()
    {
        return GetFloatValue("attackPower");
    }

    public void UpdateHealth(int newHealth)
    {
        UpdateIntValue("health", newHealth);
    }

    public void UpdateSpeed(float newSpeed)
    {
        UpdateFloatValue("speed", newSpeed);
    }

    public void UpdateAttackPower(float newAttackPower)
    {
        UpdateFloatValue("attackPower", newAttackPower);
    }

    private int GetIntValue(string column)
    {
        int value = 0;
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT {column} FROM PlayerStats";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) value = reader.GetInt32(0);
                }
            }
            conn.Close();
        }
        return value;
    }

    private float GetFloatValue(string column)
    {
        float value = 0f;
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT {column} FROM PlayerStats";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) value = reader.GetFloat(0);
                }
            }
            conn.Close();
        }
        return value;
    }

    private void UpdateIntValue(string column, int newValue)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"UPDATE PlayerStats SET {column} = @value";
                cmd.Parameters.AddWithValue("@value", newValue);
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }

    private void UpdateFloatValue(string column, float newValue)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"UPDATE PlayerStats SET {column} = @value";
                cmd.Parameters.AddWithValue("@value", newValue);
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }

    public void InitializeDatabase()
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS PlayerStats (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    health INTEGER DEFAULT 100,
                    speed REAL DEFAULT 2.0,
                    attackPower REAL DEFAULT 10.0
                );
                
                INSERT INTO PlayerStats (health, speed, attackPower)
                SELECT 100, 2.0, 10.0 WHERE NOT EXISTS (SELECT 1 FROM PlayerStats);
            ";
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }

}
