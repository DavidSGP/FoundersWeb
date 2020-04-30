using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InstructionsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] instructions;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject lastButton;
    [SerializeField] private TextMeshProUGUI nextText;
    [SerializeField] private Image circleLoader;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Transform head;
    [SerializeField] private GameObject camera;

    [SerializeField] private TextMeshProUGUI volumesText;


#if UNITY_WEBGL && !UNITY_EDITOR
        void Awake()
        {
            Microphone.Init();
            Microphone.QueryAudioInput();
        }
#endif

    private int currentinstruction;
    // Start is called before the first frame update
    void Start()
    {
        lastButton.SetActive(false);
        loadingPanel.SetActive(false);
        currentinstruction = 0;
        instructions[0].SetActive(true);
        nextText.gameObject.SetActive(true);
        for (int i = 1; i < instructions.Length; i++) {
            instructions[i].SetActive(false);
        }

        GameObject[] g = GameObject.FindGameObjectsWithTag("Head");
        Debug.Log(g.Length);
        if (g.Length == 1)
        {
            //g[0].SetActive(true);
            g[0].GetComponent<TouchManager>().isFirst = true;
            g[0].GetComponent<TouchManager>().currentButtonTag = "InstructionsButton";
        }
        else
        {
            /*for (int i = 0; i < g.Length; i++)
            {
                if (!g[i].GetComponent<TouchManager>().isFirst)
                {
                    Destroy(g[i]);
                }
                else {
                    head = g[i].transform;
                    g[i].GetComponent<TouchManager>().currentButtonTag = "InstructionsButton";
                }
            }*/

            GameObject c = Instantiate(camera) as GameObject;
            c.GetComponent<TouchManager>().isFirst = true;
            c.GetComponent<TouchManager>().currentButtonTag = "InstructionsButton";

        }
        

        /*head.position = new Vector3(-4.739f, 0f, -0.775f);
        head.rotation = Quaternion.Euler(new Vector3(0,90,0));
        head.GetComponent<TouchManager>().currentButtonTag = "InstructionsButton";*/
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("FounderPitch-1");
        }

#if UNITY_WEBGL && !UNITY_EDITOR
         Microphone.Update();
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
            float[] volumes = Microphone.volumes;
            volumesText.text = "Text ";
            for(int i = 0; i < volumes.Length; i++){
                volumesText.text += volumes[i]+"  ";
            }
            //currentAverageVolume =volumes[0]*1000;
#endif
    }

    public void ClickNext() {
        if (currentinstruction == instructions.Length-1)
        {
            instructions[currentinstruction].SetActive(false);
            loadingPanel.SetActive(true);
            lastButton.SetActive(false);
            nextText.transform.parent.gameObject.SetActive(false);
            StartCoroutine(ExitCorutine());
        }
        else {
            instructions[currentinstruction].SetActive(false);
            currentinstruction++;
            instructions[currentinstruction].SetActive(true);
            if (currentinstruction == 1)
            {
                lastButton.SetActive(true);
            }
            else if (currentinstruction == instructions.Length-1){
                nextText.text = "START";
            }
        }
       
    }

    public void ClickLast() {
        if (currentinstruction == 0) {
            return;
        }
        instructions[currentinstruction].SetActive(false);
        currentinstruction--;
        instructions[currentinstruction].SetActive(true);
        if (currentinstruction == 0)
        {
            lastButton.SetActive(false);
        }
        else if (currentinstruction == instructions.Length - 2) {
            nextText.text = "NEXT ";
        }
        
    }

    IEnumerator ExitCorutine()
    {
        /*yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("FounderPitch-1");*/
        yield return new WaitForEndOfFrame();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("FounderPitch-1");
        asyncLoad.allowSceneActivation = false;
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            progressText.text = Mathf.Round(asyncLoad.progress*100)+"%";
            circleLoader.fillAmount = asyncLoad.progress;
            if (asyncLoad.progress >= 0.9f)
            {
                progressText.text ="100%";
                circleLoader.fillAmount = 1;
                
                yield return new WaitForSeconds(0.2f);
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
