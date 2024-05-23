using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CreateColliderAround : MonoBehaviour 
{
    private void Awake() 
    {
        if(!EditorApplication.isPlaying)
        {
            CreateCollider();
            EditorApplication.update += DestroyThisComponent;
        }
    }

    private void CreateCollider()
    {
        //Get all the MeshRenderers and create a bound using them
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("No MeshRenderers found in children.");
            return;
        }

        Bounds bound = new Bounds(renderers[0].bounds.center, Vector3.zero);
        for(int i = 0; i < renderers.Length; i++)
        {
            bound.Encapsulate(renderers[i].bounds);
        }

        //Create a new GameObject at the center of the bound with BoxCollider
        GameObject center = new GameObject(gameObject.name);
        center.transform.position = bound.center;
        center.transform.SetParent(transform.parent);
        BoxCollider boxCollider = center.AddComponent<BoxCollider>();
        boxCollider.size = bound.size;

        //Reparent all children in the new GameObject
        Transform[] children = new Transform[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        foreach(Transform child in children)
        {
            child.SetParent(center.transform, true);
        }
    }

    //Remove the old GameObject
    private void DestroyThisComponent()
    {
        EditorApplication.update -= DestroyThisComponent;
        DestroyImmediate(gameObject);
    }
}
