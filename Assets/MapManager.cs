using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public Tilemap map;
    public Tile corrupt_tile;
    public Tile water_tile;
    public Tile trail_tile;
    public TileData[] tile_datas;
    public Vector3Int[] neighbour_offsets;
    public Vector3Int[] neighbour_neighbour_offset;
    public Wellspring wellspring1;
    public Wellspring wellspring2;
    public Wellspring wellspring3;
    public float spread_time;

    private Queue<Vector3Int> corrupt_tiles_queue;
    private Queue<Vector3Int> next_corrupt_tiles_queue; // child, parent
    private Dictionary<TileBase, TileData> tile_data_map;
    private HashSet<Vector3Int> last_cleaned_tiles;
    private Vector3Int min_coord_bound;
    private Vector3Int max_coord_bound;
    private bool spreading;

    // Start is called before the first frame update
    void Start()
    {
        corrupt_tiles_queue = new Queue<Vector3Int>();
        next_corrupt_tiles_queue = new Queue<Vector3Int>();
        tile_data_map = new Dictionary<TileBase, TileData>();
        last_cleaned_tiles = new HashSet<Vector3Int>();
        foreach (var tile_data in tile_datas)
        {
            tile_data_map.Add(tile_data.tile, tile_data);
        }
        Debug.Log(map.cellBounds);
        min_coord_bound = map.cellBounds.min;
        max_coord_bound = map.cellBounds.max;

        spreading = false;

        if (map.GetTile(Vector3Int.zero) != corrupt_tile)
        {
            map.SetTile(Vector3Int.zero, corrupt_tile);
        }
        corrupt_tiles_queue.Enqueue(Vector3Int.zero);
        StartCoroutine(spread_corruption());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool in_bounds(Vector3Int tile_coord)
    {
        return min_coord_bound.x <= tile_coord.x && min_coord_bound.y <= tile_coord.y && min_coord_bound.z <= tile_coord.z
                        && tile_coord.x < max_coord_bound.x && tile_coord.y < max_coord_bound.y && tile_coord.z < max_coord_bound.z;
    }

    public void cleanse_tiles(Vector3Int tile_coord)
    {
        map.SetTile(tile_coord, water_tile);
        foreach (var offset in neighbour_offsets)
        {
            Vector3Int neighbour_tile_coord = tile_coord + offset;
            if (in_bounds(neighbour_tile_coord) && map.GetTile(neighbour_tile_coord) == corrupt_tile)
            {
                map.SetTile(neighbour_tile_coord, water_tile);
                last_cleaned_tiles.Add(neighbour_tile_coord);
            }
        }
        foreach (var offset in neighbour_neighbour_offset)
        {
            Vector3Int neighbour_neighbour_tile_coord = tile_coord + offset;
            if (in_bounds(neighbour_neighbour_tile_coord) && map.GetTile(neighbour_neighbour_tile_coord) == corrupt_tile)
            {
                corrupt_tiles_queue.Enqueue(neighbour_neighbour_tile_coord);
            }
        }
        //yield return new WaitForSeconds(3f); // interval cleanse duration
        if (!spreading)
        {
            StartCoroutine(spread_corruption());
        }
    }

    public IEnumerator spread_corruption()
    {
        spreading = true;
        while (corrupt_tiles_queue.Count + next_corrupt_tiles_queue.Count > 0)
        {
            yield return new WaitForSeconds(spread_time);
            while (corrupt_tiles_queue.Count > 0)
            {
                Vector3Int cur_tile_coords = corrupt_tiles_queue.Dequeue();
                if (map.GetTile(cur_tile_coords) == corrupt_tile)
                {
                    foreach (var offset in neighbour_offsets)
                    {
                        Vector3Int new_tile_coords = cur_tile_coords + offset;
                        if (in_bounds(new_tile_coords))
                        {
                            if (map.GetTile(new_tile_coords) != corrupt_tile && !last_cleaned_tiles.Contains(new_tile_coords))
                            {
                                if (map.GetTile(new_tile_coords) != trail_tile)
                                {
                                    map.SetTile(new_tile_coords, corrupt_tile);
                                    //corrupt_tiles_queue.Enqueue(new_tile_coord);
                                    next_corrupt_tiles_queue.Enqueue(new_tile_coords);
                                }
                            }

                        }
                    }
                }
            } // OK THIS IS ACTUALLY WRONG BUT IT LOOKS FINE SO IM LEAVING IT HERE
            while (next_corrupt_tiles_queue.Count > 0)
            {
                /*Tuple<Vector3Int, Vector3Int> next_tile = next_corrupt_tiles_queue.Dequeue();
                if (map.GetTile(next_tile.Item2) == corrupt_tile) // Check again if parent is corrupted to prevent race conditions
                {
                    corrupt_tiles_queue.Enqueue(next_tile);
                }*/
                corrupt_tiles_queue.Enqueue(next_corrupt_tiles_queue.Dequeue());
            }
            last_cleaned_tiles.Clear();
        }
        spreading = false;
        //yield return null;
    }
}
