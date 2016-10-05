using UnityEngine;
using System.Collections;

public class Feet : MonoBehaviour {

    public GameObject playerBody;


    void OnDrawGizmos()
    {
        CircleCollider2D bc = GetComponent<CircleCollider2D>();

        Vector3 offset = new Vector3(bc.offset.x,bc.offset.y,0);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position + offset/2, bc.radius/2);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Feet")
        {
            Debug.Log("Hello");
            playerBody.GetComponent<PlayerController>().Deflect(other.transform.position);
        }
        else if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().Die();
        }

    }
}
