using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SucessTextManager : MonoBehaviour
{
    TMP_Text text;
    public GameObject backPlane;
    public static SucessTextManager sucessTextManager;
    public float turnOffTime=1.5f;
    // Start is called before the first frame update
    private void Awake()
    {
        sucessTextManager = this;
    }

    public void DisplayPoints(Transform t, int score)
    {
        backPlane.SetActive(true);
        transform.position = t.position;
        text.text = score.ToString();
        Invoke("TurnOffPoints", turnOffTime);
        ScoreTextManager.scoreTextManager.ChangeScore(score);
    }

    void TurnOffPoints()
    {
        backPlane.SetActive(false);
        text.text = "";
    }

    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
