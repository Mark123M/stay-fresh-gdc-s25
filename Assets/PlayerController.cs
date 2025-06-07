using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float max_speed = 1.0f;
    public float acceleration = 2.0f;
    public float friction = 0.5f;
    public Tilemap map;
    public TileBase trailTile;

    private Vector3 velocity;
    private Vector3 inputs;

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector3(0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateMovement();
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        inputs = new Vector3(inputX, inputY, 0f);
        UpdateTrail();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        if (inputs.magnitude == 0f)
        {
            velocity -= friction * Time.fixedDeltaTime * velocity;
        }

        Vector3 direction = inputs.normalized;
        velocity += acceleration * Time.fixedDeltaTime * direction;
        velocity = Vector3.ClampMagnitude(velocity, max_speed);
        transform.Translate(Time.fixedDeltaTime * velocity);
    }

    void UpdateTrail()
    {
        Vector3Int currentTile = map.WorldToCell(transform.position);
        map.SetTile(currentTile, trailTile);
    }
}
