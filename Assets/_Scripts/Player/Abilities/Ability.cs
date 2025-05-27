using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using static UnityEngine.EventSystems.EventTrigger;

public class Ability : MonoBehaviour
{

    public Transform shotPoint;

    private PlayerController playerController;

    public GameObject abilityEffectPrefab;

    [SerializeField]
    private LayerMask enemyLayer;

    public string abilityName;
    public string abilityType;
    public float abilityCooldown;
    public float attackDamage;
    public float abilityRange;
    public float projectileSpeed;
    public int damageReduction;
    public float abilityDuration;
    public KeyCode abilityKey;

    private bool isOnCooldown = false;
    private string dbPath;

    void Start()
    {
        dbPath = "URI=file:" + Application.dataPath + "/Database.db";
        LoadAbilityStats();
        Debug.Log($"Loaded ability: {abilityName}");

        if (!string.IsNullOrEmpty(abilityName))
        {
            Initialize(abilityName);
        }

        playerController = FindObjectOfType<PlayerController>();
    }

    private void LoadAbilityStats()
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Abilities WHERE abilityName = @name";
                cmd.Parameters.Add(new SqliteParameter("@name", abilityName));

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        abilityType = reader["abilityType"].ToString();
                        abilityCooldown = Convert.ToSingle(reader["abilityCooldown"]);
                        attackDamage = Convert.ToSingle(reader["attackDamage"]);
                        abilityRange = Convert.ToSingle(reader["abilityRange"]);
                        projectileSpeed = Convert.ToSingle(reader["projectileSpeed"]);
                        damageReduction = Convert.ToInt32(reader["damageReduction"]);
                        abilityDuration = Convert.ToSingle(reader["abilityDuration"]);
                        abilityKey = (KeyCode)Enum.Parse(typeof(KeyCode), reader["abilityKeyCode"].ToString());

                        Debug.Log($"[Ability] Loaded: {abilityName} - Damage: {attackDamage}, Cooldown: {abilityCooldown}");
                    }
                }
            }
            conn.Close();
        }
    }

    public void Initialize(string abilityName)
    {
        AbilitiesDatabaseConn db = new AbilitiesDatabaseConn(abilityName);

        this.abilityName = abilityName;
        abilityType = db.GetStringValue("abilityType");
        abilityCooldown = db.GetAbilityCooldown();
        attackDamage = db.GetAbilityAttackDamage();
        abilityRange = db.GetFloatValue("abilityRange");
        projectileSpeed = db.GetFloatValue("projectileSpeed");
        damageReduction = db.GetIntValue("damageReduction");
        abilityDuration = db.GetFloatValue("abilityDuration");

        string keyCodeStr = db.GetStringValue("abilityKeyCode");
        abilityKey = (KeyCode)Enum.Parse(typeof(KeyCode), keyCodeStr);

        Debug.Log($"Initialized {abilityName}: " +
                  $"Type={abilityType}, DMG={attackDamage}, CD={abilityCooldown}, Key={abilityKey}");
    }


    public void ModifyAbility(float extraDamage, float extraRange, float extraSpeed, float extraDuration, float bonusCooldown)
    {
        attackDamage += (int)extraDamage;
        abilityRange += extraRange;
        projectileSpeed += extraSpeed;
        abilityDuration += extraDuration;
        abilityCooldown -= bonusCooldown;

        Debug.Log($"{abilityName} modified! New Stats: Damage={attackDamage}, Range={abilityRange}, Speed={projectileSpeed}, Duration={abilityDuration}");
    }

    public void ResetToBaseStats()
    {
        AbilitiesDatabaseConn db = new AbilitiesDatabaseConn(abilityName);
        attackDamage = db.GetAbilityAttackDamage();
        abilityRange = db.GetFloatValue("abilityRange");
        projectileSpeed = db.GetFloatValue("projectileSpeed");
        abilityDuration = db.GetFloatValue("abilityDuration");

        Debug.Log($"{abilityName} reset! Damage={attackDamage}, Range={abilityRange}, Speed={projectileSpeed}, Duration={abilityDuration}");
    }


    void Update()
    {
       

    }
    private void InvokeAbilityMethod(string abilityName)
    {
        switch (abilityName.ToLower())
        {
            case "fire ring":
                Debug.Log("[DEBUG] Invoking ActivateFireRing()");
                Invoke(nameof(ActivateFireRing), 0f);
                break;
            case "wind slash":
                Debug.Log("[DEBUG] Invoking ActivateWindSlash()");
                Invoke(nameof(ActivateWindSlash), 0f);
                break;
            case "ultimate":
                Debug.Log("[DEBUG] Invoking ActivateUltimate()");
                Invoke(nameof(ActivateUltimate), 0f);
                break;
            default:
                Debug.LogWarning($"[DEBUG] No matching method found for abilityName: {abilityName}");
                break;
        }
    }
    public IEnumerator UseAbility()
    {
        Debug.Log($"[DEBUG] Ability {abilityName} from object: {gameObject.name}, tag: {gameObject.tag}, scene: {gameObject.scene.name}");

        if (isOnCooldown)
        {
            Debug.Log("[DEBUG] Ability is on cooldown, exiting.");
            yield break;
        }

        Debug.Log($"[DEBUG] Cooldown clear. Attempting to invoke {abilityName}");
       
        InvokeAbilityMethod(abilityName);

        isOnCooldown = true;
        yield return new WaitForSeconds(abilityCooldown);
        isOnCooldown = false;

        Debug.Log("[DEBUG] Cooldown ended.");
    }

    public void ActivateFireRing()
    {
        Debug.Log($"Activating {abilityName}");
        
        if (abilityEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(abilityEffectPrefab, transform.position, Quaternion.identity);

           
            Animator anim = effectInstance.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Play");
            }
        }

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, abilityRange, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            enemy.GetComponent<Health>().TakeDamageFromAbilities(attackDamage);
        }
    }


    private void ActivateWindSlash()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - shotPoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        shotPoint.rotation = Quaternion.Euler(0, 0, angle);

        GameObject slash = Instantiate(abilityEffectPrefab, shotPoint.position, shotPoint.rotation);
        Rigidbody2D rb = slash.GetComponent<Rigidbody2D>();
        Projectile projectile = slash.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(attackDamage, projectileSpeed, abilityRange, enemyLayer);
        }
        projectile.Initialize(attackDamage, projectileSpeed, abilityRange, enemyLayer);
        rb.AddForce(shotPoint.right * projectileSpeed, ForceMode2D.Impulse);

    }
    
    private void ActivateUltimate()
    {
        
        Debug.Log("Ultimate activated: Massive damage!");
    }

    void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, abilityRange);
    }
   
}
