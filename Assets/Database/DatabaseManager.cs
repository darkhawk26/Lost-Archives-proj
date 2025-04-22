using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;
    private PlayerStatsDatabaseConn db;

    void Awake()
    {
        dbPath = "URI=file:" + Application.dataPath + "/Database.db";
        InitializeDatabase();
    }

    void Start()
    {
        db = new PlayerStatsDatabaseConn();
        db.InitializeDatabase(); // Създаваме таблиците, ако липсват
    }

    private void InitializeDatabase()
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                // 📌 Създаване на таблицата за умения (Abilities)
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Abilities (
                        abilityId INTEGER PRIMARY KEY AUTOINCREMENT,
                        abilityName TEXT NOT NULL UNIQUE,
                        abilityType TEXT NOT NULL,
                        abilityCooldown FLOAT,
                        attackDamage FLOAT,
                        abilityRange FLOAT,
                        projectileSpeed FLOAT,
                        damageReduction INTEGER,
                        abilityDuration FLOAT,
                        abilityKeyCode TEXT
                    );
                ";
                cmd.ExecuteNonQuery();

                InsertAbilityIfNotExists(cmd, "Fire Ring", "AOE", 5.0f, 20, 3.0f, 0, 0, 0, "Q");
                InsertAbilityIfNotExists(cmd, "Wind Slash", "Projectile", 3.0f, 15, 5.0f, 10, 0, 0, "E");
                InsertAbilityIfNotExists(cmd, "Ultimate", "Explosion", 20.0f, 50, 5.0f, 0, 0, 0, "R");

                

                // 📌 Създаване на таблицата за характеристиките на играча (PlayerStats)
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS PlayerStats (
                        playerId INTEGER PRIMARY KEY AUTOINCREMENT,
                        health INTEGER DEFAULT 100,
                        speed FLOAT DEFAULT 2.0,
                        attackPower FLOAT DEFAULT 10.0
                    );
                ";
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }

    private void InsertAbilityIfNotExists(SqliteCommand cmd, string name, string type, float cooldown, float damage, float range, float speed, int reduction, float duration, string keyCode)
    {
        cmd.CommandText = @"
            INSERT INTO Abilities (abilityName, abilityType, abilityCooldown, attackDamage, abilityRange, projectileSpeed, damageReduction, abilityDuration, abilityKeyCode)
            SELECT @abilityName, @abilityType, @abilityCooldown, @attackDamage, @abilityRange, @projectileSpeed, @damageReduction, @abilityDuration, @abilityKeyCode
            WHERE NOT EXISTS (SELECT 1 FROM Abilities WHERE abilityName = @abilityName);
        ";

        cmd.Parameters.Clear();
        cmd.Parameters.Add(new SqliteParameter("@abilityName", name));
        cmd.Parameters.Add(new SqliteParameter("@abilityType", type));
        cmd.Parameters.Add(new SqliteParameter("@abilityCooldown", cooldown));
        cmd.Parameters.Add(new SqliteParameter("@attackDamage", damage));
        cmd.Parameters.Add(new SqliteParameter("@abilityRange", range));
        cmd.Parameters.Add(new SqliteParameter("@projectileSpeed", speed));
        cmd.Parameters.Add(new SqliteParameter("@damageReduction", reduction));
        cmd.Parameters.Add(new SqliteParameter("@abilityDuration", duration));
        cmd.Parameters.Add(new SqliteParameter("@abilityKeyCode", keyCode));

        cmd.ExecuteNonQuery();
    }

    
}
