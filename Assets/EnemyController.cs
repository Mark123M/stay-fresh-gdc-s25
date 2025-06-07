using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 1;
    public float knockback_dist = 2;
    public float knockback_force = 2;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("Player found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player takes damage! " + collision.gameObject.name);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
       /* GameObject target = collision.gameObject;
        Rigidbody2D rigidBody = collision.attachedRigidbody;
        if (target.CompareTag("Player"))
        {
            
        } 
        else if (target.CompareTag("Enemy"))
        {
            Vector3 direction = (transform.position - target.transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < knockback_dist)
            {
                float kb_amt = Time.deltaTime * knockback_force * distance;
                transform.position += kb_amt * direction;
                target.transform.position += -kb_amt * direction;
            }
        }
        else
        {

        } */
    }
}
