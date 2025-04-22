using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public int health;
    public int baseHealth;
    public int maxHealth;
    public float baseSpeed; 
    public float speed; 
    public float attackPower;
    public float baseAttackPower;
    public List<Ability> abilities = new List<Ability>();

    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithReference;
    [SerializeField]
    private bool isDead = false;

    private PlayerStatsDatabaseConn db;

    void Awake()
    {
        db = new PlayerStatsDatabaseConn();
        LoadBaseStats();

        abilities = new List<Ability>(GetComponents<Ability>());
    }

    public void ResetAbilities()
    {
        foreach (var ability in abilities)
        {
            ability.ResetToBaseStats(); 
        }
    }

    public void GetHit(int amount, GameObject sender)
    {
        if (isDead)
            return;
        if (sender.layer == gameObject.layer)
            return;

        health -= amount;

        if (health > 0)
        {
            OnHitWithReference?.Invoke(sender);
        }
        else
        {
            OnDeathWithReference?.Invoke(sender);
            isDead = true;
            ResetPlayerStats();
            Destroy(gameObject);
            SceneManager.LoadScene("GameOver");
        }
    }
    public void LoadBaseStats()
    {
        
        baseHealth = db.GetHealth();
        baseSpeed = db.GetSpeed();
        baseAttackPower = db.GetAttackPower();

        
        speed = baseSpeed;
        health = baseHealth;
        maxHealth = baseHealth;
        baseAttackPower = attackPower;

        Debug.Log($"Health: {health}, Speed: {speed}, AttackPower: {attackPower}");
    }

    public void IncreaseSpeed(float amount)
    {
       
        speed += amount;
        Debug.Log($"Speed increased by {amount}! New speed: {speed}");
    }

    public void ResetSpeed()
    {
       
        speed = baseSpeed;
        Debug.Log($"Speed reset to base value: {speed}");
    }

  
    public void IncreaseAttackPower(float amount)
    {
        attackPower += amount;
        Debug.Log($"Attack Power increased by {amount}! New attack: {attackPower}");
    }

   
    public void ResetPlayerStats()
    {
        LoadBaseStats(); 
        Debug.Log("Player stats have been reset to base values.");
    }
}
