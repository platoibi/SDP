using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PenaltyBallManager : MonoBehaviour
{

    public float radius = 0.5f;
    public float airDensity = 0.01f;
    public float shotPower = 1;
    public Vector3 shotDirection = new Vector3(100, 50, 100);
    public Transform impactPosition;
    private Rigidbody rb;
    public GreenAreaManager greenAreaManager;
    Vector3 initPos;
    Vector3 initVel;
    Vector3 initAngVel;
    Quaternion initRot;
    public TMP_Text score;
    public void Shoot()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForceAtPosition(shotDirection * shotPower, impactPosition.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ( other.tag.Equals("GreenArea"))
        {
            score.text = (int.Parse(score.text) + 1).ToString();
        }
        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.position = initPos;
        transform.rotation = initRot;
        greenAreaManager.UpdatePosition();
        //rb.angularVelocity = initAngVel;
        //rb.velocity = initVel;
        //rb.angularDrag = 0;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        initVel = rb.velocity;
        initAngVel = rb.angularVelocity;
        initPos = transform.position;
        initRot = transform.rotation;
    }

    void FixedUpdate()
    {
        var direction = Vector3.Cross(rb.angularVelocity, rb.velocity);
        var magnitude = 4 / 3f * Mathf.PI * airDensity * Mathf.Pow(radius, 3);
        rb.AddForce(magnitude * direction);
    }
}
