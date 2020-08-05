using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private float destroyTime;

    void Update()
    {
        GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.001f);
        if (destroyTime > 10)
        {
            Destroy(gameObject);
        }
        else
        {
            destroyTime += Time.deltaTime;
            Debug.Log(destroyTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Destroy(gameObject);
        }

    }

}
