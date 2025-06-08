using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Wellspring : MonoBehaviour
{
    public float corruption_rate = 5f; // 5 percent every second
    public float purify_rate = 10f;
    public float purify_range = 2f;
    public float purity;
    private MapManager map_manager;
    private PlayerController player;
    public TextMeshPro purity_display;
    public Sprite healthy_sprite;
    public Sprite hurt_sprite;
    public Sprite dead_sprite;

    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        map_manager = GameObject.FindGameObjectWithTag("Map").GetComponent<MapManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        purity = 100f;
    }

    private void FixedUpdate()
    {
        Vector3Int tile_coords = map_manager.map.WorldToCell(transform.position);
        Vector3 player_to_wellspring = transform.position - player.transform.position;
        if (player_to_wellspring.magnitude <= purify_range)
        {
            purity = Mathf.Clamp(purity + purify_rate * Time.fixedDeltaTime, 0f, 100f);
        }
        if (map_manager.map.GetTile(tile_coords) == map_manager.corrupt_tile)
        {
            purity = Mathf.Clamp(purity - corruption_rate * Time.fixedDeltaTime, 0f, 100f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int purity_int = (int)purity;
        if (purity_int > 50)
        {
            sr.sprite = healthy_sprite;
        }
        else if (purity_int > 0)
        {
            sr.sprite = hurt_sprite;
        }
        else
        {
            sr.sprite = dead_sprite;
        }
        purity_display.text = purity_int.ToString();
    }
}
