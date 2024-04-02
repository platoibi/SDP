using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBall : MonoBehaviour
{
    [SerializeField] Transform ballTransform;
    Vector3 diff;
    int cnt = 0;
    // Start is called before the first frame update
    void Start()
    {
        diff = transform.position - ballTransform.position;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if ( cnt == 0 )
            diff *= 1.00f;
        cnt++;
        cnt %= 10;
        transform.position = ballTransform.position + diff;
    }
}
