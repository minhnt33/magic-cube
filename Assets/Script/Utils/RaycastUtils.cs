using UnityEngine;
using System.Collections;

public static class RaycastUtils
{

    public static GameObject GetOnlyGrid(Vector3 pos)
    {
        return GetAllGameObject(pos, Vector3.down, 4f, "Grid");
    }


    public static GameObject GetGrid(Vector3 pos)
    {
        return GetGameObject(pos, Vector3.down, 2.5f, "Grid");
    }

    public static GameObject PlayerGetCube(Vector3 playerPos)
    {
        return GetGameObject(playerPos, Vector3.down, 1f, "Cube");
    }

    public static GameObject PlayerGetCubeForward(Vector3 playerPos, Vector3 dir)
    {
        return GetGameObject(playerPos + Vector3.up / 2, dir, 1f, "Cube");
    }

    public static GameObject GridGetCube(Vector3 gridPos)
    {
        return GetGameObject(gridPos, Vector3.up, 1f, "Cube");
    }

    public static GameObject ValidateCubeMove(Direction dir, Vector3 currentCubePos)
    {
        Vector3 direction = Vector3.zero;

        if (dir == Direction.FORWARD)
            direction = Vector3.forward;
        else if (dir == Direction.BACKWARD)
            direction = Vector3.back;
        else if (dir == Direction.LEFT)
            direction = Vector3.left;
        else if (dir == Direction.RIGHT)
            direction = Vector3.right;

        return GetGameObject(currentCubePos, direction, 1f, "Cube");
    }

    public static bool ValidatePlayerMove(Vector3 playerPosition, Vector3 dir)
    {
        return !CheckGameObject(playerPosition, dir, 1f, "Boundary");
    }

    public static bool CheckGameObject(Vector3 startingPos, Vector3 direction, float maxLength, string layerName)
    {
        RaycastHit hit;

        if (Physics.Raycast(startingPos, direction, out hit, maxLength))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(layerName))
                return true;
        }

        return false;
    }

    public static GameObject GetGameObject(Vector3 startingPos, Vector3 direction, float maxLength, string layerName)
    {
        RaycastHit hit;

        if (Physics.Raycast(startingPos, direction, out hit, maxLength))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(layerName))
                return hit.collider.gameObject;
        }

        return null;
    }

    public static GameObject GetAllGameObject(Vector3 startingPos, Vector3 direction, float maxLength, string layerName)
    {
        RaycastHit[] hits;
        startingPos.y = startingPos.y + 0.5f;
        hits = Physics.RaycastAll(startingPos, direction, maxLength);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.layer == 11)
                return hits[i].collider.gameObject;
        }

        return null;
    }
}