using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBall : MonoBehaviour
{
    public Transform goalPos;
    public Transform crossBar;
    void Start()
    {

    }
    public void UpdatePosition(Vector3 ballPos)
    {
        transform.position = crossBar.position + 1.2f * (ballPos - goalPos.position);
        transform.LookAt((ballPos*1.5f + crossBar.position) / 2.5f);
    }
}
