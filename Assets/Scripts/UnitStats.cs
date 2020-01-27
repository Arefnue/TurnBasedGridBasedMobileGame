using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="UnitStats")]
public class UnitStats : ScriptableObject
{

    [Header("Attack Properties")]
    public bool straightDirection;
    public bool diagonalDirection;
    public bool canAttackOverBlock;
    
    public DamageIcon damageIcon; //Display the damage value
    public GameObject deathEffect; //Particle for death

    [Space]

    [Header("Stats")]
    public int hp;
    public int armor;
    public int damage;
    public int turnSpeed;
    public int attackRange;
    public int attackRangeMin=0;
    public int neighbourDirections;

}
