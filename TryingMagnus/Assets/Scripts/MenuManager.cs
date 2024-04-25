using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public void LoadFreeKickScene()
    {
        SceneManager.LoadScene("FreeKickScene");
    }
    public void LoadPenaltyScene()
    {
        SceneManager.LoadScene("PenaltyScene");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
