using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public List<Ability> abilities;

    private DatabaseManager db;

    public float moveSpeed;
    private PlayerStats playerStats;

    private Rigidbody2D rb2d;
    float speedX, speedY;

    private AgentAnimations agentAnimations;
    private WeaponParent weaponParent;
    private Vector2 pointerInput, movementInput;

    public Vector2 PointerInput { get => pointerInput; set => pointerInput = value; }
    public Vector2 MovementInput { get => movementInput; set => movementInput = value; }

    Vector2 mousePos;
   
    private void Start()
    {
        db = FindObjectOfType<DatabaseManager>();
        playerStats = GetComponent<PlayerStats>();
        moveSpeed = playerStats.speed;  
        Debug.Log("speed Awake(): " + moveSpeed);

        agentAnimations = GetComponentInChildren<AgentAnimations>();
        weaponParent = GetComponentInChildren<WeaponParent>();
        rb2d = GetComponent<Rigidbody2D>();

        abilities = GetComponentsInChildren<Ability>().ToList();
    }
    
    private void Update()
    {
       
        speedX = Input.GetAxisRaw("Horizontal") * moveSpeed;
        speedY = Input.GetAxisRaw("Vertical") * moveSpeed;

        rb2d.velocity = new Vector2(speedX, speedY);

       

        weaponParent.PointerPosition = pointerInput;
        AnimateCharacter();

        foreach (var ability in abilities)
        {
            if (Input.GetKeyDown(ability.abilityKey))
            {
                StartCoroutine(ability.UseAbility());
            }
        }
    }

    public void PerformAttack()
    {
        weaponParent.Attack();
    }

    private void AnimateCharacter()
    {
        Vector2 lookDirection = pointerInput - (Vector2)transform.position;
        agentAnimations.RotateToPointer(lookDirection);
        agentAnimations.PlayAnimation(MovementInput);
    }

    
}
