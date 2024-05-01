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
    TrailRenderer trail;
    int power;
    public static Magnus magnus;
    public void Shoot()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForceAtPosition(shotDirection * shotPower, impactPosition.position);
        Invoke("StopForce", 0.5f);
    }
    public void Shoot(float straight, float up, float left, float right, float acc)
    {
        print("left: " + left.ToString());
        print("up: " + up.ToString());
        print("straight: " + straight.ToString());
        shotDirection = new Vector3(100, (acc-2)*3, 70 * left - 70 * right);
        Shoot();
    }
    void ResetTrail()
    {
        trail.emitting = true;
    }
    private void StopForce()
    {
        power = 0;
    }

    private int CalculateScore(Transform entryPos, Transform center)
    {
        float score = Mathf.Abs(entryPos.position.z - center.position.z)*15 + Mathf.Abs(entryPos.position.y-center.position.y)*30 + entryPos.position.y*10 + 10;
        return (int)score;
    }

    private void ResetBall()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.position = initPos;
        transform.position = new Vector3(Random.Range(25, 35f), initPos.y, Random.Range(-7.0f, 7.0f));
        transform.rotation = initRot;
        followBall.UpdatePosition(transform.position);
        trail.emitting = false;
        trail.Clear();
        power = 1;
        Invoke("ResetTrail", 0.2f);
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
            //power = -1;
            int score = CalculateScore(transform, other.transform);
            SucessTextManager.sucessTextManager.DisplayPoints(transform, score);
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
        magnus = this;
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
    }
    void Start() {
        Random.InitState(System.DateTime.Now.Millisecond);
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
        var magnitude = 4 / 3f * Mathf.PI * airDensity * Mathf.Pow(radius, 3) * power;
        rb.AddForce(magnitude * direction);
    }
}
