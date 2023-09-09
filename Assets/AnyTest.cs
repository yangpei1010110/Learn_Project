using System;
using UnityEngine;

public class AnyTest : MonoBehaviour
{
    private void OnDrawGizmosSelected()
    {
        // get scene root
        var root = gameObject.scene.GetRootGameObjects();
        foreach (GameObject go in root)
        {
            ShowBounds(go);
        }
    }

    private void ShowBounds(GameObject go)
    {
        if (go.transform.childCount <= 0)
        {
            var bound = go.GetComponent<Mesh>().bounds;
            Gizmos.DrawWireCube(bound.center, bound.size);
        }
        else
        {
            foreach (Transform child in go.transform)
            {
                ShowBounds(child.gameObject);
            }
        }
    }
}