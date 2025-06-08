using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float max_speed = 1.0f;
    public float acceleration = 2.0f;
    public float friction = 0.5f;
    public float damage_time = 1f;
    public float damage_knockback = 10f;
    public int max_hp = 10;

    public Tilemap map;
    public TileBase trail_tile;
    public Material damage_flash;
    public Material default_material;

    private Vector3 velocity;
    private Vector3 inputs;
    private SpriteRenderer player_sprite;
    private int hp;
    private bool damage_taken;
    private bool stunned;

    public bool slashing;
    public bool cleansing;
    public float cleanse_range = 2f;
    public bool trailing;

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector3(0f, 0f);
        hp = max_hp;
        damage_taken = false;
        stunned = false;
        slashing = false;
        cleansing = false;
        trailing = false;
        player_sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputs();
        UpdateTrail();
        UpdateAttacks(); // idk if it should be here?
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    void UpdateInputs()
    {
        if (!stunned)
        {
            float input_x = Input.GetAxisRaw("Horizontal");
            float input_y = Input.GetAxisRaw("Vertical");
            inputs = new Vector3(input_x, input_y, 0f);
        }
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
        Vector3Int current_tile = map.WorldToCell(transform.position);
        map.SetTile(current_tile, trail_tile);
    }

    void UpdateAttacks()
    {
        bool is_attacking = slashing || cleansing || trailing;
        if (Input.GetKeyDown(KeyCode.X) && !is_attacking)
        {
            slashing = true;
        }
        else if (Input.GetKeyDown(KeyCode.Z) && !is_attacking)
        {
            cleansing = true;
        }
        else if (Input.GetKeyDown(KeyCode.C) && !is_attacking)
        {
            trailing = true;
        }
    }

    public void deal_damage(int damage, Vector3 position)
    {
        if (hp > 0)
        {
            Debug.Log("Player HP: " + hp);
            // player_sprite.material =
            if (!damage_taken)
            {
                Vector3 direction = (transform.position - position).normalized;
                velocity += direction * damage_knockback;
                hp -= damage;
                StartCoroutine(flash_player());
            }
            else
            {
                Debug.Log("INVINCIBLE");
            }
        } else
        {
            Debug.Log("Player is dead...");
        }
    }

    IEnumerator flash_player()
    {
        damage_taken = true;
        stunned = true;
        player_sprite.material = damage_flash;
        yield return new WaitForSeconds(damage_time * 0.75f);
        stunned = false;
        player_sprite.material = default_material;
        yield return new WaitForSeconds(damage_time * 0.25f);
        damage_taken = false;
    }
}
