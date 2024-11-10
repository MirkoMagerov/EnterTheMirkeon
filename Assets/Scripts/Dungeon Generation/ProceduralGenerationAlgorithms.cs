using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPos, int walkLength)
    {
        HashSet<Vector2Int> path = new();

        path.Add(startPos);
        Vector2Int previousPos = startPos;

        for (int i = 0; i < walkLength; i++)
        {
            Vector2Int newPosition = previousPos + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPos = newPosition;
        }

        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPos, int corridorLength)
    {
        List<Vector2Int> corridor = new();

        Vector2Int direction = Direction2D.GetRandomCardinalDirection();
        Vector2Int currentPos = startPos;

        corridor.Add(currentPos);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPos += direction;
            corridor.Add(currentPos);
        }

        return corridor;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new();
        List<BoundsInt> roomsList = new();

        roomsQueue.Enqueue(spaceToSplit);
        while (roomsQueue.Count > 0)
        {
            BoundsInt room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if(Random.value < 0.5f)
                {
                    if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else
                    {
                        roomsList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }

        return roomsList;
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        int xSplit = Random.Range(1, room.size.x);

        BoundsInt room1 = new(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        int ySplit = Random.Range(1, room.size.y);

        BoundsInt room1 = new(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}

public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new(0,1),
        new(1,0), 
        new(0,-1), 
        new(-1,0)
    };

    public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
    {
        new(1,1),
        new(1,-1),
        new(-1,-1),
        new(-1,1)
    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
    {
        new(0,1),
        new(1,1),
        new(1,0),
        new(1,-1),
        new(0,-1),
        new(-1,-1),
        new(-1,0),
        new(-1,1)
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0,cardinalDirectionsList.Count)];
    }
}
