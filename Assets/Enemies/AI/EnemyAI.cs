using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public GameObject ultimateTarget;
    public float baseSpeed = 10f;
    public float currentSpeed = 10f;
    public float attackRate = 3f;
    public int damage = 5;
    public float nextWaypointDistance = 1f;

    public float avoidDistance = 1f;
    public LayerMask avoidable;

    public float targetPlayerChance = 0.33f;
    public float targetDefenceChance = 0.33f;
    
    Animator animator;

    Path path;
    int currentWaypoint;
    bool finishedPath;

    Seeker seeker;
    Rigidbody2D rb;
    public Transform player;

    public bool engageDefences = false;
    public bool engagePlayer = false;
    public bool canTargetHigh = false;

    private bool engaged = false;
    private bool attacking = false;

    public float targetAttackRange = 1f;
    public float targetFollowRange = 2f;

    private dynamic currentTarget;
    private GameObject targetGameObject;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponentInChildren(typeof(Animator)) as Animator;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        currentTarget = ultimateTarget;

        setPath(ultimateTarget.transform.position);
        
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        if(Random.value < targetDefenceChance){
            // 50% chance that this enemy will target non-blocking defences
            engageDefences = true;
        }
        if(Random.value < targetPlayerChance){
            // 75% chance that this enemy will target player
            engagePlayer = true;
        }

        InvokeRepeating("checkAttack", 3f, 3f);
    }

    void OnPathComplete(Path newPath){
        if(!newPath.error){
            path = newPath;
            currentWaypoint = 0;
        }
    }

    private IEnumerator setPathAsync(Vector3 position){
        seeker.StartPath(rb.position, position, OnPathComplete);
        yield break;
    }

    public void setPath(Vector3 position){
        StartCoroutine(setPathAsync(position));
    }

    public GameObject FindClosestDefence()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Defence");
        GameObject closest = null;
        float distance = targetFollowRange;
        foreach (GameObject go in gos)
        {
            float curDistance = Vector2.Distance(go.transform.position, transform.position);
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    public void checkAttack(){
        if(engaged == false && (engageDefences || rb.velocity.magnitude < 0.05f)){
            var closestDefence = FindClosestDefence();
            if(closestDefence != null){
                var defence = closestDefence.transform.parent.gameObject.GetComponent<Defence>();
                Vector3 defencePos = closestDefence.transform.position;
                if(defence.gameInstances.ContainsKey(defencePos)){
                    var defenceInstance = defence.gameInstances[defencePos];
                    if(!defenceInstance.isOnWall || canTargetHigh){
                        engageObject(defenceInstance);
                    }
                }
            }
        }
        
        if(engaged == false && (engagePlayer &&  Vector2.Distance(player.position, transform.position) < targetFollowRange)){
            var playerHealth = player.gameObject.GetComponent<health>();
            engageObject(playerHealth);
        }
        
        if(engaged == false && (Vector2.Distance(ultimateTarget.transform.position, transform.position) < (targetAttackRange + 1))){
            var objective = ultimateTarget.GetComponent<health>();
            engageObject(objective);
        }
    }

    void startAttackCycle(){
        attacking = true;
        InvokeRepeating("attackCycle", attackRate, attackRate);
        animator.SetBool("isAttacking", true);
    }

    void stopAttackCycle(){
        attacking = false;
        CancelInvoke("attackCycle");
        animator.SetBool("isAttacking", false);
    }

    void engageObject(dynamic newTarget){
        engaged = true;
        path = null;
        CancelInvoke("attackCycle");
        CancelInvoke("trackObject");
        currentTarget = newTarget;
        targetGameObject = currentTarget.gameObject;
        InvokeRepeating("trackObject", 0f, 2f);
    }

    void trackObject(){
        if(targetGameObject == null){
            disengageObject();
        } else {
            setPath(targetGameObject.transform.position);
            var attackRange = targetAttackRange;
            if(targetGameObject == ultimateTarget){
                attackRange += 1;
            }
            if(Vector2.Distance(targetGameObject.transform.position, transform.position) < attackRange){
                if(!attacking){
                    startAttackCycle();
                }
            } else if(Vector2.Distance(currentTarget.gameObject.transform.position, transform.position) > targetFollowRange){
                disengageObject();
            } else if(attacking){
                stopAttackCycle();
            }
        }
    }

    void disengageObject(){
        engaged = false;
        stopAttackCycle();
        CancelInvoke("trackObject");
        setPath(ultimateTarget.transform.position);
        checkAttack();
    }

    void attackCycle(){
        if(currentTarget.gameObject == null){
            disengageObject();
        } else {
            bool destroyed = currentTarget.dealDamage(damage);
            if(destroyed){
                disengageObject();
            }
        }
    }

    void moveOnPath(){
        Vector2 intendedDirection = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        if(checkForObstacles(intendedDirection)){
            intendedDirection = avoidDirection(intendedDirection);
        }

        Vector2 intendedForce = intendedDirection * currentSpeed * Time.deltaTime;
        rb.AddForce(intendedForce);

        float currentWaypointDistance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if(currentWaypointDistance < nextWaypointDistance){
            currentWaypoint ++;
        }
    }

    bool checkForObstacles(Vector2 intendedDirection){
        RaycastHit2D hit = Physics2D.Raycast(transform.position, intendedDirection, avoidDistance, avoidable);

        if(hit.collider != null){
            return true;
        }

        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Quaternion.Euler(15, 0, 0) * intendedDirection, avoidDistance, avoidable);
        if(hitLeft.collider != null){
            return true;
        }

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Quaternion.Euler(-15, 0, 0) * intendedDirection, avoidDistance, avoidable);
        if(hitRight.collider != null){
            return true;
        }

        return false;

    }

    Vector2 avoidDirection(Vector2 intendedDirection){
        Vector2 angleLeft = Quaternion.Euler(70, 0, 0) * intendedDirection;
        Vector2 angleRight = Quaternion.Euler(-70, 0, 0) * intendedDirection;
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, angleLeft, avoidDistance, avoidable);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, angleRight, avoidDistance, avoidable);

        if(hitRight.collider == null){
            return Quaternion.Euler(-20, 0, 0) * angleRight;
        }

        if(hitLeft.collider == null){
            return Quaternion.Euler(20, 0, 0) * angleLeft;
        }

        return intendedDirection;
    }

    // Update is called once per frame
    void Update()
    {
        if(!(path == null)){
            if(!(currentWaypoint >= path.vectorPath.Count)){
                moveOnPath();
            }
        }

        if(rb.velocity.magnitude > 0.1){
            animator.SetBool("isMoving", true);
        } else {
            animator.SetBool("isMoving", false);
            if(!engaged){
                checkAttack();
            }
        }
    }
}
