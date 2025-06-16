using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    public float scrollSpeed;
    public float resetPositionX;
    public float startPositionX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if(transform.position.x <= resetPositionX)
        {
            Vector3 newPos = transform.position;
            newPos.x = startPositionX;
            transform.position = newPos;
        }
    }
}
