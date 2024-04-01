using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : MonoBehaviour
{
    public GameObject block2D;
    public GameObject block3D;

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePosition = block2D.transform.position - block3D.transform.position;
        if (relativePosition != new Vector3(0, 0, 0))
        {
            // Update the position of the parent object (block3D) without affecting the child (block2D)
            block3D.transform.position = new Vector3(block2D.transform.position.x, block2D.transform.position.y, block3D.transform.position.z);
            block2D.transform.localPosition = Vector3.zero;
        }

    }
}
