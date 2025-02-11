using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit, IMovable
{
    public static event Action<EnemyUnit> OnAnyUnitDead;
    [SerializeField] protected EnemyUnitHero unitHero;
    [SerializeField] bool canMove = true;
    protected EnemyUnitData unitData;
    protected EnemyUnitAnimation unitAnimation;

    public EnemyUnitHero UnitHero => unitHero;
    public EnemyUnitData UnitData
    {
        get
        {
            return unitData;
        }
        set
        {
            unitData = value;
        }
    }


    public new void Awake()
    {
        base.Awake();
        unitAnimation = GetComponent<EnemyUnitAnimation>();
    }

    // Update is called once per frame
    public new void Update()
    {
        base.Update();

        // Handle movement and attack
        if (canMove)
        {
            Move();
        }
        else if (!canMove && canAttack)
        {
            unitAnimation.PlayAttackAnimation((int)unitData.UnitHero);
            attackCooldown = attackSpeed;
            canAttack = false;
        }

        DetectEnemiesAndHandleAttack(); // Ensure enemy detection and handling is checked every frame
    }

    public void InitializeUnit(UnitType unitType, EnemyUnitData unitData, float attackDamageBoost, float unitHealthBoost, float moveSpeed, float attackSpeed)
    {
        // Set the type
        this.unitType = unitType;

        // Set the unit data
        this.unitData = unitData;

        // Set the move speed
        this.moveSpeed = moveSpeed;

        // Set the attack speed
        this.attackSpeed = attackSpeed;

        // Set the attack damage
        this.attackDamageBoost = attackDamageBoost;

        // Set the layer mask and tag
        gameObject.layer = LayerMask.NameToLayer(unitType.ToString());
        gameObject.tag = unitType.ToString();
        targetMask = LayerMask.GetMask(unitType == UnitType.Player ? "Enemy" : "Player");

        // Reset state
        healthSystem.ResetHealth(this.unitData.Health);

        // Set the move direction
        moveDirection = unitType == UnitType.Player ? Vector3.right : Vector3.left;

        // Set the scale
        visual.localScale = unitType == UnitType.Player
            ? new Vector3(Mathf.Abs(visual.localScale.x), visual.localScale.y, visual.localScale.z)
            : new Vector3(-Mathf.Abs(visual.localScale.x), visual.localScale.y, visual.localScale.z);

        // Set the animation to idle 
        unitAnimation.PlayIdleAnimation((int)unitData.UnitHero);
    }

    public override void HandleOnDead()
    {
        base.HandleOnDead();
        OnAnyUnitDead?.Invoke(this);
    }

    public void Move()
    {
        transform.position += moveSpeed * Time.deltaTime * moveDirection;
    }

    private void DetectEnemiesAndHandleAttack()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, unitData.AttackRadius, TargetMask);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.TryGetComponent<IAttackable>(out IAttackable unit))
            {
                targetUnit = unit;
                canMove = false;
                return;
            }
        }

        // Reset state if no valid targets are found
        targetUnit = null;
        canMove = true;
    }


}
