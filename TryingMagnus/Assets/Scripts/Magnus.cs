using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnus : MonoBehaviour
{
    
    public float radius = 0.5f;
    public float airDensity = 0.01f;
    public float shotPower = 1;
    public Vector3 shotDirection = new Vector3(100, 50, 100);
    public Transform impactPosition;
    public FollowBall followBall;
    private Rigidbody rb;
    Vector3 initPos;
    Vector3 initVel;
    Vector3 initAngVel;
    Quaternion initRot;
    int power;
    public void Shoot()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForceAtPosition(shotDirection * shotPower, impactPosition.position);
        Invoke("StopForce", 0.5f);
    }

    private void StopForce()
    {
        power = 0;
    }

    private void ResetBall()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.position = initPos;
        transform.position = new Vector3(Random.Range(30, 35f), initPos.y, Random.Range(-6.0f, 6.0f));
        transform.rotation = initRot;
        followBall.UpdatePosition(transform.position);
        power = 1;
    }
    private void OnTriggerEnter(Collider other)
    {
        if ( other.tag == "GoalNet")
        {
            Invoke("ResetBall", 0.2f);
            //ResetBall();
        }
        else if( other.name == "GoalEntryTrigger" )
        {
            power = -1;
        }
        else
        {
            ResetBall();
        }
        //rb.angularVelocity = initAngVel;
        //rb.velocity = initVel;
        //rb.angularDrag = 0;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start() {
        initVel = rb.velocity;
        initAngVel = rb.angularVelocity;
        initPos = transform.position;
        initRot = transform.rotation;
        followBall.UpdatePosition(transform.position);
        power = 1;
    }
 
    void FixedUpdate()
    {
        var direction = Vector3.Cross(rb.angularVelocity, rb.velocity);
        var magnitude = 4 / 3f * Mathf.PI * airDensity * Mathf.Pow(radius, 3)*power;
        rb.AddForce(magnitude * direction);
    }
}
