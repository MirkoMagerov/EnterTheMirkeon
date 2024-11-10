using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeigth = 4;

    [SerializeField]
    private int dungeonWidth = 20, dungeonHeigth = 20;

    [SerializeField]
    [Range(0, 10)] private int offset = 1;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        List<BoundsInt> roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPos,
            new Vector3Int(dungeonWidth, dungeonHeigth, 0)), minRoomWidth + offset + 1, minRoomHeigth + offset + 1);

        HashSet<Vector2Int> floor = CreateSimpleRooms(roomsList);

        List<Vector2Int> roomCenters = new();
        foreach(BoundsInt room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new();
        List<Vector2Int> connectedRooms = new() { roomCenters[Random.Range(0, roomCenters.Count)] };
        roomCenters.Remove(connectedRooms[0]);

        while (roomCenters.Count > 0)
        {
            float shortestDistance = float.MaxValue;
            Vector2Int closestRoom = Vector2Int.zero;
            Vector2Int currentRoom = Vector2Int.zero;

            foreach (var connectedRoom in connectedRooms)
            {
                foreach (var room in roomCenters)
                {
                    float distance = Vector2.Distance(connectedRoom, room);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        closestRoom = room;
                        currentRoom = connectedRoom;
                    }
                }
            }

            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoom, closestRoom);
            corridors.UnionWith(newCorridor);

            // Agrega la habitación recién conectada al conjunto de habitaciones conectadas
            connectedRooms.Add(closestRoom);
            roomCenters.Remove(closestRoom);
        }

        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new();
        var position = currentRoomCenter;
        corridor.Add(position);

        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }

        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }

        return corridor;
    }

    private Vector2Int FindClosestPoint(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = new();
        float distance = float.MaxValue;

        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }

        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new();

        foreach(var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    // Guardar esto en otra coleccion a futuro para poder procesar cada habitacion individualmente
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }

        return floor;
    }
}
