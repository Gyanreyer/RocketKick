using UnityEngine;
using System.Collections;

public class Feet : MonoBehaviour {

    public GameObject playerBody;


    void OnDrawGizmos()
    {
        BoxCollider2D bc = GetComponent<BoxCollider2D>();

        Vector3 offset = new Vector3(bc.offset.x,bc.offset.y,0);

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + offset/2, new Vector3(bc.size.x/2,bc.size.y/2,1));
    }

}
