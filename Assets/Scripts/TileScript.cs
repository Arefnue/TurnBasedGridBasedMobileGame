using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour,IPointerUpHandler,IPointerDownHandler
{
    private PlayerController playerController;

    private SpriteRenderer rend;

    private Color defaultColor;

    public Color highlightedColor;

    public Color attackableColor;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        rend = GetComponent<SpriteRenderer>();
        defaultColor = rend.color;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale += Vector3.one * 0.1f;
        Vector3 pointerWorldPosition = new Vector3(transform.position.x, transform.position.y, 0f);

        if(GameMaster.instance.playerTurn == true && GameMaster.instance.playerIsMoving != true) 
        {
            playerController.MoveHandler(pointerWorldPosition);
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale -= Vector3.one * 0.1f;
        
    }

    public void Highlight()
    {
        rend.color = highlightedColor;
    }

    public void ResetTileColor()
    {
        rend.color = defaultColor;
    }

    public void AttackableHighlight()
    {
        rend.color = attackableColor;
    }

}
