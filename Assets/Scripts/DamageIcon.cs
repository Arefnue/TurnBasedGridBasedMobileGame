using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIcon : MonoBehaviour
{
    public Sprite[] damageSprites;

    public float lifetime;
    public GameObject effect;

    private void Start() 
    {
        Invoke("Destruction",lifetime);
    }
    public void Setup(int damage)
    {
        if (damage > 6) damage = 6;
        if (damage < 1) damage = 1;
        GetComponent<SpriteRenderer>().sprite = damageSprites[damage-1];
    }

    void Destruction()
    {
        Instantiate(effect,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}
