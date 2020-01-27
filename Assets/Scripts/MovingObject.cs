using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MovingObject : MonoBehaviour
{

    #region Base
    [Header("Base")]
    public LayerMask blockLayer;
    public LayerMask unwalkableLayer;
    public LayerMask walkableLayer;
    public LayerMask targetLayer;
    public int moveTimeSpeed = 40;
    Collider2D coll;
    [HideInInspector] public List<Vector3> walkableTiles = new List<Vector3>();
    [HideInInspector] public Vector3 target;
    [HideInInspector] public int neighbourDirections;
    [HideInInspector] public Vector3 lastPos = new Vector3(0, 0, 99);
    [HideInInspector] public bool canAttack;

    public GameObject attackCursor;
    #endregion

    #region Stats
    [HideInInspector] public int hp;
    [HideInInspector] public int armor;
    [HideInInspector] public int damage;
    [HideInInspector] public int turnSpeed;
    [HideInInspector] public int attackRange;
    [HideInInspector] public int attackRangeMin;
    #endregion

    protected virtual void Start()
    {
        coll = GetComponent<Collider2D>();
        walkableTiles = FindWalkableTiles(transform.position);
    }


    protected Vector3 ChooseNextPath(List<Vector3> walkableTiles,Vector3 target,bool isClosest) 
    {
        Vector3 bestVector = new Vector3(0, 0, 99);

        if(isClosest == true) 
        {
            float closestDistanceSqr = Mathf.Infinity;

            foreach (Vector3 tile in walkableTiles)
            {
                if (Vector3.Distance(tile, target) < closestDistanceSqr)
                {
                    closestDistanceSqr = Vector3.Distance(tile, target);
                    bestVector = tile;
                }

            }

            if (bestVector == new Vector3(0, 0, 99))
            {
                bestVector = transform.position;
            }
            
            float bestToTarget = Vector3.Distance(bestVector, target);
            float lastToTarget = Vector3.Distance(lastPos, target);
            if (Mathf.Abs(bestToTarget) > Mathf.Abs(lastToTarget))
            {
                bestVector = transform.position;
            }


        }
        else if(isClosest == false) 
        {
            float closestDistanceSqr = Mathf.Epsilon;

            foreach (Vector3 tile in walkableTiles)
            {
                if (Vector3.Distance(tile, target) > closestDistanceSqr)
                {
                    closestDistanceSqr = Vector3.Distance(tile, target);
                    bestVector = tile;
                }

            }

            if (bestVector == new Vector3(0, 0, 99))
            {
                bestVector = transform.position;
            }

            float bestToTarget = Vector3.Distance(bestVector, target);
            float lastToTarget = Vector3.Distance(lastPos, target);
            if (Mathf.Abs(bestToTarget) < Mathf.Abs(lastToTarget))
            {
                bestVector = transform.position;
            }

        }

        lastPos = bestVector;
        return bestVector;
    }


    protected List<Vector3> FindWalkableTiles(Vector3 worldPos) 
    {
        Vector3 start = worldPos;
        Vector3Int end = new Vector3Int(Mathf.RoundToInt(start.x),Mathf.RoundToInt(start.y),0);

        List<Vector3> neighbours = new List<Vector3>();
        List<Vector3> blocked = new List<Vector3>();
        List<Vector3> walkableTiles = new List<Vector3>();
        neighbours = FindNeighbours(end,neighbourDirections);

        for(int i = 0; i < neighbours.Count; i++) 
        {
            RaycastHit2D hit;
            RaycastHit2D targetHit;
            coll.enabled = false;

            hit = Physics2D.Linecast(start, neighbours[i], unwalkableLayer);
            targetHit = Physics2D.Linecast(start, neighbours[i], targetLayer);
            coll.enabled = true;

            if(hit.transform != null) 
            {
                blocked.Add(neighbours[i]);
            }
            else 
            {
                walkableTiles.Add(neighbours[i]);
            }

            if(targetHit.transform != null) 
            {
                walkableTiles.Remove(neighbours[i]);
            }
             
        }

        bool debugTry = false;

        if(debugTry == true) 
        {
            for (int i = 0; i < walkableTiles.Count; i++)
            {
                Debug.DrawLine(start, walkableTiles[i], Color.blue, 5f);
            }
        }
        
        return walkableTiles;
    }
   

    protected List<Vector3> FindNeighbours(Vector3Int worldPos,int whichDirections) 
    {
        List<Vector3> neighbours = new List<Vector3>();

        if (whichDirections == 0) 
        {
            // 8-way neigh
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int xCheck = worldPos.x + x;
                    int yCheck = worldPos.y + y;

                    neighbours.Add(new Vector3(xCheck, yCheck, 0));

                }
            }
        }
        else if (whichDirections == 1) 
        {
            //Straight 4way
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if ((x == 1 && y == 1) || (x == 1 && y == -1) ||
                       (x == -1 && y == 1) || (x == -1 && y == -1)) continue;

                    int xCheck = worldPos.x + x;
                    int yCheck = worldPos.y + y;

                    neighbours.Add(new Vector3(xCheck, yCheck, 0));

                }
            }
        }
        else if(whichDirections == 2) 
        {
            //Diagonal 4way
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if ((x == 0 && y == 1) || (x == 0 && y == -1) ||
                       (x == 1 && y == 0) || (x == -1 && y == 0)) continue;

                    int xCheck = worldPos.x + x;
                    int yCheck = worldPos.y + y;

                    neighbours.Add(new Vector3(xCheck, yCheck, 0));

                }
            }
        }
        
        return neighbours;
        
    }

    protected virtual Dictionary<string, List<GameObject>> FindAttackableTiles(List<Vector3> directions,int rangeMax,int rangeMin = 0) 
    {
        RaycastHit2D blockCheck;
        RaycastHit2D targetCheck;
        
        List<GameObject> attackableTiles = new List<GameObject>();
        List<GameObject> targetTiles = new List<GameObject>();

        Dictionary<string,List<GameObject>> tilesDict = new Dictionary<string, List<GameObject>>();
        
        for (int i = 0; i < directions.Count; i++) 
        {
            Vector3 direction = directions[i].normalized;
            Vector3 start = transform.position + direction * rangeMin;
            Vector3 end = transform.position + direction * rangeMax;

            coll.enabled = false;
            blockCheck = Physics2D.Linecast(start, end, blockLayer);
            coll.enabled = true;

            RaycastHit2D[] walkableHits;
            RaycastHit2D[] targetHits;


            if (blockCheck.transform != null)
            {
                walkableHits = Physics2D.LinecastAll(start, blockCheck.transform.position - direction, walkableLayer);
                targetHits = Physics2D.LinecastAll(start, blockCheck.transform.position, targetLayer);
            }
            else
            {
                walkableHits = Physics2D.LinecastAll(start, end, walkableLayer);
                targetHits = Physics2D.LinecastAll(start, end, targetLayer);

            }

            for (int j = 1; j < walkableHits.Length; j++)
            {
                attackableTiles.Add(walkableHits[j].transform.gameObject);
            }

            if(targetHits != null) 
            {
                for(int h = 0; h < targetHits.Length; h++) 
                {
                    targetTiles.Add(targetHits[h].transform.gameObject);
                }
            }
            
        }

        tilesDict.Add("attackable", attackableTiles);
        tilesDict.Add("target", targetTiles);
        
        return tilesDict;
    }

    protected void HighlightAttackTiles(Dictionary<string, List<GameObject>> tiles) 
    {
        List<GameObject> tilesAttack = tiles["attackable"];
        List<GameObject> tilesTarget = tiles["target"];

        for(int i = 0; i < tilesAttack.Count; i++) 
        {
            TileScript ab = tilesAttack[i].GetComponent<TileScript>();
            ab.AttackableHighlight();
        }

        for(int i = 0; i < tilesTarget.Count; i++) 
        {
            tilesTarget[i].SendMessage("ShowTargeted",tilesTarget[i]);
        }
        
        
    }

    public void ShowTargeted(GameObject target) 
    {
        MovingObject mo = target.GetComponent<MovingObject>();
        mo.attackCursor.SetActive(true);
        mo.canAttack = true;
    }

    

    public void MoveTo(Vector3 target, bool isClose = true)
    {
        List<Vector3> walkableTiles = FindWalkableTiles(transform.position);

        Vector3 moveToThis = ChooseNextPath(walkableTiles, target,isClose);
        StartCoroutine(SmoothMovement(moveToThis));

        GameMaster.instance.ResetShowTargeted();
        GameMaster.instance.ResetTiles();
    }

    IEnumerator SmoothMovement(Vector3 end)
    {
        GameMaster.instance.playerIsMoving = true;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, moveTimeSpeed * Time.deltaTime);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
        GameMaster.instance.playerIsMoving = false;
    }

    protected virtual List<Vector3> FindDirections(
                                                   bool diaDir = false,
                                                   bool strDir = false,
                                                   bool left = false,
                                                   bool right = false,
                                                   bool up = false,
                                                   bool down = false,
                                                   bool upR = false,
                                                   bool upL = false,
                                                   bool downR = false,
                                                   bool downL = false) 
    {
        List<Vector3> directions = new List<Vector3>();
        
        if (strDir) 
        {
            left = true;
            right = true;
            up = true;
            down = true;
        }
        if (diaDir) 
        {
            upR = true;
            upL = true;
            downR = true;
            downL = true;
        }

        if (left) directions.Add(Vector3.left);
        if (right) directions.Add(Vector3.right);
        if (up) directions.Add(Vector3.up);
        if (down) directions.Add(Vector3.down);
        if (upR) directions.Add(new Vector3(0.5f,0.5f,0));
        if (upL) directions.Add(new Vector3(-0.5f, 0.5f, 0));
        if (downR) directions.Add(new Vector3(0.5f, -0.5f, 0));
        if (downL) directions.Add(new Vector3(-0.5f, -0.5f, 0));

        return directions;
    }

}
