using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    public MapManager map_manager;
    public TileBase trail_tile;
    public Material damage_flash;
    public Material default_material;
    public EnemySpawner enemy_spawner;
    public GameObject slash_left;
    public GameObject slash_right;
    public GameObject slash_up;
    public GameObject slash_down;

    private Vector3 velocity;
    private Vector3 inputs;
    private SpriteRenderer player_sprite;
    private int hp;
    private bool damage_taken;
    private bool stunned;
    public enum direction { up, down, left, right, none }
    private direction facing_direction;

    public bool slash_ready;
    public float slash_cooldown;
    public bool cleanse_ready;
    public float cleanse_range = 2f;
    public float cleanse_force = 100f;
    public float cleanse_cooldown = 2f;
    public bool trail_ready;
    public float trail_cooldown;

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector3(0f, 0f);
        hp = max_hp;
        damage_taken = false;
        stunned = false;
        slash_ready = true;
        cleanse_ready = true;
        trail_ready = true;
        facing_direction = direction.down;
        player_sprite = GetComponent<SpriteRenderer>();
        slash_left.SetActive(false);
        slash_right.SetActive(false);
        slash_up.SetActive(false);
        slash_down.SetActive(false);
        facing_direction = direction.right;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputs();
        //UpdateTrail();
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

            // default
            if (input_x > 0)
            {
                facing_direction = direction.right;
            }
            if (input_x < 0)
            {
                facing_direction = direction.left;
            }
            if (input_y < 0)
            {
                facing_direction = direction.down;
            }
            if (input_y > 0)
            {
                facing_direction = direction.up;
            }

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
        Vector3Int current_tile = map_manager.map.WorldToCell(transform.position);
        map_manager.map.SetTile(current_tile, trail_tile);
    }

    void UpdateAttacks()
    {
        if (Input.GetKeyDown(KeyCode.X) && slash_ready)
        {
            StartCoroutine(perform_slash());
            StartCoroutine(start_slash_cooldown());
        }
        else if (Input.GetKeyDown(KeyCode.C) && cleanse_ready)
        {
            map_manager.cleanse_tiles(map_manager.map.WorldToCell(transform.position));
            foreach (var enemy in FindObjectsOfType<EnemyController>())
            {
                Vector3 player_to_enemy = enemy.transform.position - transform.position;
                if (player_to_enemy.magnitude <= cleanse_range)
                {
                    enemy.ext_velocity += cleanse_force * player_to_enemy.normalized;
                }
            }
            StartCoroutine(start_cleanse_cooldown());
        }
        else if (Input.GetKeyDown(KeyCode.Z) && trail_ready)
        {
            StartCoroutine(start_trail_cooldown());
        }
    }
    IEnumerator perform_slash()
    {
        if (facing_direction == direction.left)
        {
            slash_left.SetActive(true);
        }
        else if (facing_direction == direction.right)
        {
            slash_right.SetActive(true);
        }
        else if (facing_direction == direction.up)
        {
            slash_up.SetActive(true);
        }
        else
        {
            slash_down.SetActive(true);
        }
        yield return new WaitForSeconds(0.1f);
        slash_left.SetActive(false);
        slash_right.SetActive(false);
        slash_up.SetActive(false);
        slash_down.SetActive(false);
    }
    IEnumerator start_slash_cooldown()
    {
        slash_ready = false;
        yield return new WaitForSeconds(slash_cooldown);
        slash_ready = true;
    }

    IEnumerator start_cleanse_cooldown()
    {
        cleanse_ready = false;
        yield return new WaitForSeconds(cleanse_cooldown);
        cleanse_ready = true;
    }

    IEnumerator start_trail_cooldown()
    {
        trail_ready = false;
        yield return new WaitForSeconds(trail_cooldown);
        trail_ready = true;
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
