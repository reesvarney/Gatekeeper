using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform targetLocation;
    public float speed = 5f;
    public float attackRate = 3f;
    public int damage = 5;
    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint;
    bool finishedPath;

    Seeker seeker;
    Rigidbody2D rb;
    public Transform player;

    private bool engageDefences = false;
    public bool canTargetHigh = false;
    private bool engaged = false;
    public float targetAttackRange = 1f;
    public float targetFollowRange = 2f;

    private dynamic currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        seeker.StartPath(rb.position, targetLocation.position, OnPathComplete);
        
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        if(Random.value < 0.5f){
            // 50% chance that this enemy will target non-blocking defences
            engageDefences = true;
        }
    }

    void OnPathComplete(Path newPath){
        if(!newPath.error){
            path = newPath;
            currentWaypoint = 0;
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        // if is Player, 50% chance to target player and attack
        // Should then check player distance on update and switch back to primary objective if its over x distance
        if(other.gameObject.tag == "Defence"){
            var defence = other.transform.parent.gameObject.GetComponent<Defence>();
            Vector3 defencePos = other.transform.position;
            var defenceInstance = defence.gameInstances[defencePos];

            if(!engaged && (!defenceInstance.isOnWall || canTargetHigh)){
                if(rb.velocity.magnitude < 0.05f){
                    // If blocked by defence, always attack
                    attackObject(defenceInstance);
                } else {
                    // Attack according to attack chance
                    if(engageDefences){
                        attackObject(defenceInstance);
                    }
                }
            }
        }
    }

    void attackObject(dynamic newTarget){
        engaged = true;
        currentTarget = newTarget;
        InvokeRepeating("attackCycle", attackRate, attackRate);
    }

    void stopAttackCycle(){
        CancelInvoke("attackCycle");
        engaged = false;
    }

    void attackCycle(){
        // Check is in range of target
        var distanceToTarget = Vector3.Distance(currentTarget.gameObject.transform.position, transform.position);
        if(distanceToTarget < targetAttackRange){
            bool destroyed = currentTarget.dealDamage(damage);
            if(destroyed){
                stopAttackCycle();
            }
        }

        // Play attack animation
    }

    void moveOnPath(){
        Vector2 intendedDirection = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 intendedForce = intendedDirection * speed * Time.deltaTime;
        rb.AddForce(intendedForce);

        float currentWaypointDistance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if(currentWaypointDistance < nextWaypointDistance){
            currentWaypoint ++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!(path == null)){
            if(currentWaypoint >= path.vectorPath.Count){
                finishedPath = true;
            } else {
                finishedPath = false;
                moveOnPath();
            }
        }
    }
}
