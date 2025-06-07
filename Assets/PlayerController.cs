using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float speed = 1.0f;
    public Tilemap map;
    public TileBase trailTile;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateTrail();
    }

    void UpdateMovement()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(inputX, inputY, 0).normalized;
        transform.Translate(speed * Time.deltaTime * movement);
    }

    void UpdateTrail()
    {
        Vector3Int currentTile = map.WorldToCell(transform.position);
        map.SetTile(currentTile, trailTile);
    }
}
