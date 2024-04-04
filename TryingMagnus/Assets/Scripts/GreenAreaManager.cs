using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenAreaManager : MonoBehaviour
{
    Vector3 initpos;
    public void UpdatePosition()
    {
        transform.position = initpos + new Vector3(0, Random.Range(-0.6f, 0.6f), Random.Range(-3f, 3f));
    }
    // Start is called before the first frame update
    void Start()
    {
        initpos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
