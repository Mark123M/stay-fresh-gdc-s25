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

    // Start is called before the first frame update
    void Start()
    {
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
        purity_display.text = ((int)(purity)).ToString();
    }
}
