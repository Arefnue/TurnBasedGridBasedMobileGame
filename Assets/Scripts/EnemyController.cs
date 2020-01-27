using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyController : MovingObject,IPointerDownHandler,IPointerUpHandler
{
    public float moveTime = 0.5f;
    private PlayerController playerObject;
    List<Vector3> directions = new List<Vector3>();
    public UnitStats profile;
    private bool isSelected;
    [Header("Stats")]
    #region Stats
    public int fearRange = 0;
    #endregion


    public enum MoveState 
    {   
        attack,
        move,
        run
    
    };

    private MoveState _moveState;

    protected override void Start()
    {
        GameMaster.instance.AddEnemyToList(this);

        playerObject = FindObjectOfType<PlayerController>();

        target = playerObject.transform.position;
        SetStats();

        _moveState = default;
        directions = FindDirections(profile.diagonalDirection,profile.straightDirection);

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

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale += Vector3.one * 0.1f;


        if (GameMaster.instance.playerTurn == true)
        {
            
            if (canAttack == true)
            {
                playerObject.Attack(this);
                GameMaster.instance.ResetShowTargeted();
                GameMaster.instance.ResetTiles();
            }
            else
            {
                
                
                if (isSelected == true)
                {
                    GameMaster.instance.ResetShowTargeted();
                    GameMaster.instance.ResetTiles();
                    isSelected = false;
                }
                else 
                {
                    isSelected = true;
                    HighlightAttackTiles(FindAttackableTiles(directions, attackRange, attackRangeMin));
                }

            }

        }


    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale -= Vector3.one * 0.1f;
    }

    private void DetermineMoveState(Vector3 target) 
    {
        int distanceToPlayer = Mathf.RoundToInt(Vector3.Distance(transform.position, target));
        

        Dictionary<string,List<GameObject>> attackDict = FindAttackableTiles(directions, attackRange);


        if (attackDict["target"].Count > 0)
        {
            _moveState = MoveState.attack;
        }
        else 
        {
            if (distanceToPlayer <= fearRange)
            {
                _moveState = MoveState.run;
            }
            else
            {
                _moveState = MoveState.move;
            }
        }
    }

    public void Attack() 
    {
        int enemyDamage = damage - playerObject.armor;

        if(enemyDamage >= 1) 
        {
            DamageIcon instance = Instantiate(profile.damageIcon, playerObject.transform.position, Quaternion.identity);
            instance.Setup(enemyDamage);
            playerObject.hp -= enemyDamage;
        }
        if(playerObject.hp <= 0) 
        {
            Instantiate(playerObject.profile.deathEffect, playerObject.transform.position, Quaternion.identity);
            //GameMaster.instance.GameOver();
            Destroy(playerObject);
        }
    }



    public void MoveHandler() 
    {
        target = playerObject.transform.position;
        DetermineMoveState(target);

        switch (_moveState) 
        {
            case MoveState.attack:
                
                Attack();
                break;
            case MoveState.move:
                
                MoveTo(target);
                break;
            case MoveState.run:
                
                MoveTo(target, false);
                break;
            default:
                
                break;
        }

    }

}
