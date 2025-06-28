using UnityEditor;
using UnityEngine;

public class EditorUtilities : MonoBehaviour
{
    [MenuItem("Tools/Rename Cube Pieces")]
    public static void RenameCubePieces()
    {
        // Get all selected objects
        GameObject selectedObject = Selection.activeGameObject;
        
        if (selectedObject == null)
        {
            Debug.LogWarning("No object selected!");
            return;
        }

        int index = 0;
        for (int i = 0; i < selectedObject.transform.childCount; i++)
        {
            Transform child = selectedObject.transform.GetChild(i);
            
            // Calculate x, y, z coordinates based on index
            int x = index % 3;
            int y = index / 9;
            int z = (index / 3) % 3;
            
            // Rename the child object
            child.name = $"Cube_{x}_{y}_{z}";
            index++;
        }
        
        Debug.Log("Cube pieces renamed successfully!");
    }

    [MenuItem("Tools/Change Cubelet Positions")]
    public static void ChangeCubeletPositions()
    {
        // Get all selected objects
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null)
        {
            foreach (Transform child in selectedObject.transform)
            {
                child.position = new Vector3(child.position.x / 2, child.position.y / 2, child.position.z / 2);
            }
        }
    }
}
