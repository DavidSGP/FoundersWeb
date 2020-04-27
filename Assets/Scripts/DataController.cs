using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataController : MonoBehaviour
{
    public static DataController instance;

    public float deltaValue;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        deltaValue = 0;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene("IntroScene");
    }

}
