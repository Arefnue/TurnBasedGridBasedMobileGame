using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MovingObject,IPointerDownHandler,IPointerUpHandler
{
    [Space]

    List<Vector3> directions = new List<Vector3>();
    public UnitStats profile;

    protected override void Start()
    {
        SetStats();
        directions = FindDirections(profile.diagonalDirection, profile.straightDirection);

        base.Start();
    }

    private void SetStats()
    {
        hp = profile.hp;
        armor = profile.armor;
        damage = profile.damage;
        turnSpeed = profile.turnSpeed;
        attackRange = profile.attackRange;
        attackRangeMin = profile.attackRangeMin;
        neighbourDirections = profile.neighbourDirections;
    }

    private void Update()
    {
        if (GameMaster.instance.playerTurn == false || GameMaster.instance.playerIsMoving != false) return;

        if (Input.GetKeyDown(KeyCode.B)) 
        {
            HighlightAttackTiles(FindAttackableTiles(directions, attackRange,attackRangeMin));
            
        }

        if (Input.GetKeyDown(KeyCode.C)) 
        {
            GameMaster.instance.ResetShowTargeted();
        }
    }

    public void SetTarget(Vector3 targetPos) 
    {
        target = targetPos;
    }

    public void EndTurn() 
    {
        GameMaster.instance.ResetTiles();
        GameMaster.instance.ResetShowTargeted();

        turnSpeed = profile.turnSpeed;
        GameMaster.instance.playerTurn = false;
        GameMaster.instance.enemyTurn = true;
    }

    public void Attack(EnemyController enemy) 
    {
        int enemyDamage = damage - enemy.armor;

        if(enemyDamage >= 1) 
        {
            DamageIcon instance = Instantiate(enemy.profile.damageIcon, enemy.transform.position, Quaternion.identity);
            instance.Setup(enemyDamage);
            enemy.hp -= enemyDamage;
            
        }

        if(enemy.hp <= 0) 
        {
            Instantiate(enemy.profile.deathEffect, enemy.transform.position, Quaternion.identity);

            //enemy.OnDeath();
            GameMaster.instance.enemies.Remove(enemy);
            Destroy(enemy.gameObject);
        }
        ActionHandler();
    }

    public void MoveHandler(Vector3 targetPos) 
    {
        MoveTo(targetPos);
        ActionHandler();
    }

    public void ActionHandler() 
    {
        turnSpeed--;
        if (turnSpeed < 0) 
        {
            
            EndTurn();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale += Vector3.one * 0.1f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale -= Vector3.one * 0.1f;
    }

}
