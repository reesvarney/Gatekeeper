using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAttack : MonoBehaviour
{

    public float attackRange = 5f;
    public int damage = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetMouseButtonDown(0) ){
            var mousePos = Input.mousePosition;
            mousePos.z = -(Camera.main.transform.position.z);
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

            Vector2 mouseWorldPos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos2D, Vector2.zero);
            
            if (hit.collider != null) {
                if(hit.collider.gameObject.tag == "Enemy" && Vector2.Distance(gameObject.transform.position, hit.collider.gameObject.transform.position) < attackRange){
                    hit.collider.gameObject.GetComponent<health>().dealDamage(damage);
                };
            }
        }
    }
}
