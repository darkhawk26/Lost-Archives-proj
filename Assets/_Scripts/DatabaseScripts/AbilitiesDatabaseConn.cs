using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;

public class AbilitiesDatabaseConn
{
    private string dbPath;
    private int abilityId;

    public AbilitiesDatabaseConn(string abilityName)
    {
        dbPath = "URI=file:" + Application.dataPath + "/Database.db";

        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT abilityId FROM Abilities WHERE abilityName = @abilityName";
                cmd.Parameters.Add(new SqliteParameter("@abilityName", abilityName));

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        abilityId = reader.GetInt32(0);
                    else
                        Debug.LogWarning($"Ability '{abilityName}' not found in database.");
                }
            }
        }
    }

    public float GetAbilityCooldown()
    {
        return GetFloatValue("abilityCooldown");
    }

    public float GetAbilityAttackDamage()
    {
        return GetFloatValue("attackDamage");
    }

    public string GetStringValue(string columnName)
    {
        if (abilityId == 0)
        {
            Debug.LogWarning("Invalid abilityId. Cannot fetch value.");
            return string.Empty;
        }

        string value = string.Empty;
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT {columnName} FROM Abilities WHERE abilityId = @abilityId";
                cmd.Parameters.Add(new SqliteParameter("@abilityId", abilityId));

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) value = reader.GetString(0);
                }
            }
        }
        return value;
    }

    public int GetIntValue(string columnName)
    {
        if (abilityId == 0)
        {
            Debug.LogWarning("Invalid abilityId. Cannot fetch int value.");
            return 0;
        }

        int value = 0;
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT {columnName} FROM Abilities WHERE abilityId = @abilityId";
                cmd.Parameters.Add(new SqliteParameter("@abilityId", abilityId));

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) value = reader.GetInt32(0);
                }
            }
        }
        return value;
    }

    public float GetFloatValue(string columnName)
    {
        if (abilityId == 0)
        {
            Debug.LogWarning("Invalid abilityId. Cannot fetch value.");
            return 0f;
        }

        float value = 0f;
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT {columnName} FROM Abilities WHERE abilityId = @abilityId";
                cmd.Parameters.Add(new SqliteParameter("@abilityId", abilityId));

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) value = reader.GetFloat(0);
                }
            }
        }
        return value;
    }
}
