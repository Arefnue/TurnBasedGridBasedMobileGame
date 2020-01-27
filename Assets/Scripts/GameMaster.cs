using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance = null;

    public bool playerTurn = true;
    public bool enemyTurn = false;
    public bool playerIsMoving = false;
    public bool enemiesMoving;
    public List<EnemyController> enemies;
    public float turnDelay = 0.1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        playerTurn = true;
        enemies = new List<EnemyController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            playerTurn = true;
            enemyTurn = false;
        }
        if (Input.GetKeyDown(KeyCode.O)) 
        {
            playerTurn = false;
            enemyTurn = true;
        }
        if (enemyTurn != true || enemiesMoving == true || playerTurn == true) return;

        StartCoroutine(MoveEnemies());


    }

    //Coroutine to move enemies in sequence.
    IEnumerator MoveEnemies()
    {
        //While enemiesMoving is true player is unable to move.
        enemiesMoving = true;

        //Wait for turnDelay seconds, defaults to .1 (100 ms).
        yield return new WaitForSeconds(turnDelay);

        //If there are no enemies spawned (IE in first level):
        if (enemies.Count == 0)
        {
            //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
            yield return new WaitForSeconds(turnDelay);
        }

        //Loop through List of Enemy objects.
        for (int i = 0; i < enemies.Count; i++)
        {
            //Call the MoveEnemy function of Enemy at index i in the enemies List.
            enemies[i].MoveHandler();

            //Wait for Enemy's moveTime before moving next Enemy, 
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        //Once Enemies are done moving, set playersTurn to true so player can move.
        playerTurn = true;

        //Enemies are done moving, set enemiesMoving to false.
        enemiesMoving = false;
    }

    //Add enemies and init them
    public void AddEnemyToList(EnemyController script)
    {
        //Add Enemy to List enemies.
        enemies.Add(script);
    }

    public void ResetTiles() 
    {
        foreach(TileScript tile in FindObjectsOfType<TileScript>()) 
        {
            tile.ResetTileColor();
        }
    }

    public void ResetShowTargeted()
    {
        foreach (MovingObject unit in FindObjectsOfType<MovingObject>())
        {
            unit.attackCursor.SetActive(false);
            unit.canAttack = false;
        }
    }



}
