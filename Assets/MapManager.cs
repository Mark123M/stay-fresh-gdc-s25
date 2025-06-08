using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public Tilemap map;
    public Tile corrupt_tile;
    public TileData[] tile_datas;
    public Vector3Int[] neighbour_offsets;
    public float spread_time;

    private Queue<Vector3Int> corrupt_tiles_queue;
    private Queue<Tuple<Vector3Int, Vector3Int>> next_corrupt_tiles_queue; // child, parent
    private Dictionary<TileBase, TileData> tile_data_map;
    private Vector3Int min_coord_bound;
    private Vector3Int max_coord_bound;

    // Start is called before the first frame update
    void Start()
    {
        corrupt_tiles_queue = new Queue<Vector3Int>();
        next_corrupt_tiles_queue = new Queue<Tuple<Vector3Int, Vector3Int>>();
        tile_data_map = new Dictionary<TileBase, TileData>();
        foreach (var tile_data in tile_datas)
        {
            tile_data_map.Add(tile_data.tile, tile_data);
        }
        Debug.Log(map.cellBounds);
        min_coord_bound = map.cellBounds.min;
        max_coord_bound = map.cellBounds.max;
        StartCoroutine(spread_corruption(new Vector3Int(0, 0)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator spread_corruption(Vector3Int tile_coord)
    {
        if (map.GetTile(tile_coord) != corrupt_tile)
        {
            //Debug.Log("Starting corruption " + tile_coord.x + " " + tile_coord.y);
            map.SetTile(tile_coord, corrupt_tile);
        }
        corrupt_tiles_queue.Enqueue(tile_coord);

        while (corrupt_tiles_queue.Count + next_corrupt_tiles_queue.Count > 0)
        {
            while (corrupt_tiles_queue.Count > 0)
            {
                Vector3Int cur_tile_coord = corrupt_tiles_queue.Dequeue();
                TileData cur_tile_data = tile_data_map[map.GetTile(cur_tile_coord)];

                foreach (var offset in neighbour_offsets)
                {
                    Vector3Int new_tile_coord = cur_tile_coord + offset;
                    if (min_coord_bound.x <= new_tile_coord.x && min_coord_bound.y <= new_tile_coord.y && min_coord_bound.z <= new_tile_coord.z
                        && new_tile_coord.x < max_coord_bound.x && new_tile_coord.y < max_coord_bound.y && new_tile_coord.z < max_coord_bound.z)
                    {
                        if (map.GetTile(new_tile_coord) != corrupt_tile)
                        {
                            map.SetTile(new_tile_coord, corrupt_tile);
                            //corrupt_tiles_queue.Enqueue(new_tile_coord);
                            next_corrupt_tiles_queue.Enqueue(new Tuple<Vector3Int, Vector3Int>(new_tile_coord, cur_tile_coord));
                        }

                    }
                }
            }
            yield return new WaitForSeconds(spread_time);
            while (next_corrupt_tiles_queue.Count > 0)
            {
                Tuple<Vector3Int, Vector3Int> next_tile = next_corrupt_tiles_queue.Dequeue();
                if (map.GetTile(next_tile.Item2) == corrupt_tile) // Check again if parent is corrupted to prevent race conditions
                {
                    corrupt_tiles_queue.Enqueue(next_tile.Item1);
                }
            }
        }
        
        yield return null;
    }
}
