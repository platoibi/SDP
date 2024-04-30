using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTextManager : MonoBehaviour
{
    public static TMP_Text text;
    public static ScoreTextManager scoreTextManager;
    int score = 0;
    // Start is called before the first frame update

    public void ChangeScore(int points)
    {
        score += points;
        text.text = "Score: " + score.ToString();
    }

    private void Awake()
    {
        scoreTextManager = this;
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
