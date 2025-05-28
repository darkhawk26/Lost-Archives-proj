using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.UI;

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

    private static float accumulatedAbilityDamage = 0f; 
    private static float ultimateThreshold = 400f; 
    private static bool ultimateReady = false;
    public Slider slider;

    private bool isOnCooldown = false;
    private string dbPath;

    private float cooldownTimer = 0f;
    public Image cooldownMask;

    public float GetRemainingCooldown()
    {
        return Mathf.Max(0, cooldownTimer);
    }

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
        if (isOnCooldown)
        {
            yield break;
        }

        if (abilityName.ToLower() == "ultimate" && !ultimateReady)
        {
            yield break;
        }

        InvokeAbilityMethod(abilityName);

        isOnCooldown = true;
        cooldownTimer = abilityCooldown;
        if (cooldownMask != null)
            StartCoroutine(CooldownVisualRoutine());

        while (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(abilityCooldown);
        isOnCooldown = false;

        Debug.Log("[DEBUG] Cooldown ended.");
    }
    private IEnumerator CooldownVisualRoutine()
    {
        float cooldown = abilityCooldown;
        float timeLeft = cooldown;

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            if (cooldownMask != null)
                cooldownMask.fillAmount = timeLeft / cooldown;

     

            yield return null;
        }

        if (cooldownMask != null)
            cooldownMask.fillAmount = 0;

        
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
            accumulatedAbilityDamage += attackDamage;
           

            if (accumulatedAbilityDamage >= ultimateThreshold)
            {
                ultimateReady = true;
                Debug.Log("[Ultimate] READY!");
            }
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
       
        accumulatedAbilityDamage += attackDamage;
        if (accumulatedAbilityDamage >= ultimateThreshold)
        {
            ultimateReady = true;
            Debug.Log("[Ultimate] READY!");
        }
        if (projectile != null)
        {
            projectile.Initialize(attackDamage, projectileSpeed, abilityRange, enemyLayer);
        }
       
        rb.AddForce(shotPoint.right * projectileSpeed, ForceMode2D.Impulse);

    }
    
    private void ActivateUltimate()
    {
        if (!ultimateReady)
        {
            Debug.Log("[Ultimate] Not ready!");
            return;
        }

        

        if (abilityEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(abilityEffectPrefab, transform.position, Quaternion.identity);

            Animator anim = effectInstance.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Cast");
            }
        }

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, abilityRange, enemyLayer);
        foreach (Collider2D enemy in enemies)
        {
            enemy.GetComponent<Health>().TakeDamageFromAbilities(attackDamage);
        }

     
        accumulatedAbilityDamage = 0f;
        ultimateReady = false;
    }
    public static float GetCurrentUltimateProgress()
    {
        return accumulatedAbilityDamage;
    }
    void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, abilityRange);
    }
   
}
