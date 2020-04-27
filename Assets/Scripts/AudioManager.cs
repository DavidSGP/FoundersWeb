using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class AudioManager : MonoBehaviour
{

    [SerializeField] private string[] audioFolders;
    [SerializeField] private int totalQuestions;
    [SerializeField] private AudioSource MicrophoneSource;
    [SerializeField] private TextMeshProUGUI speakerTurn;

    [SerializeField] private GameObject AnswerNotRecivedMessage;
    [SerializeField] private AnimationInterviewer[] interviewers;
    [SerializeField] private Image speakBar;
    [SerializeField] private GameObject parentSpeakBar;
    [SerializeField] private string dialogsPath;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private int[] questionsExecutive;
    [SerializeField] private GameObject selectPanel;
    [SerializeField] private GameObject interviewPanel;
    [SerializeField] private TextMeshProUGUI volumesText;


    private int currentAudio;
    AudioSource audioSource;
    bool playedIntro;
    bool playedOutro;

    float waitTime = 3.0f;
    float currentWaitTime;
    bool isWaitingAnswer;
    float[] clipSampleData = new float[128];
    string[] speakers = new string[3] { "Virtual Investor", "Respond to Question", "Hmmm. . . " };
    string[] names = new string[3] { "Jesse", "Vincent", "Jennifer " };
    bool microphoneConected;
    bool hasAnswered;

    private AnimationInterviewer currentInterviewer;
    private int lastInterviewer;
    private bool runningSpeakAnimation;
    private int lastAskAnimation;
    private int totalAskAnimations;
    private string[] dialogs;

    private bool isDiligent;
    private int currentExecitiveQuestion;

    GameObject dialog = null;


#if UNITY_WEBGL && !UNITY_EDITOR
        void Awake()
        {
            Microphone.Init();
            Microphone.QueryAudioInput();
        }
#endif

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        /*randomQuestionsIds = new List<int>();
        questionsPoolIds = new List<int>();
        valuationQuiestionsId = 0;
        goToMarketQuestionsIds = 0;
        numberOfQuestionPools = 3;
        totalAskAnimations = 3;*/
        //totalNumberOfQuestions = RandomQuestions.audios.Length + ValuationQuestions.audios.Length + GoToMarketQuestions.audios.Length;
        speakBar.fillAmount = 1;
        speakBar.gameObject.SetActive(false);
        dialogText.gameObject.SetActive(false);
        /*for (int i = 0; i < numberOfQuestionPools; i++)
        {
            questionsPoolIds.Add(i);
        }*/
        /*for (int i = 0; i < RandomQuestions.audios.Length; i++)
        {
            randomQuestionsIds.Add(i);
        }*/
        TextAsset d = Resources.Load(dialogsPath) as TextAsset;
        dialogs = d.text.Split('\n');
        currentWaitTime = 0;
        isWaitingAnswer = false;
        microphoneConected = true;
        hasAnswered = false;
        AnswerNotRecivedMessage.SetActive(false);
        DetectMicrophone(true);

        currentInterviewer = null;
        lastInterviewer = -1;
        lastAskAnimation = -1;
        totalAskAnimations = 3;
        runningSpeakAnimation = false;

        currentAudio = 0;
        isDiligent = true;
        currentExecitiveQuestion = 0;

        selectPanel.SetActive(true);
        interviewPanel.SetActive(false);
        GameObject[] g = GameObject.FindGameObjectsWithTag("Head");
        Transform head = g[0].transform;
            
        /*head.position = new Vector3(0f, 0f, -0.775f);
        head.rotation = Quaternion.identity;*/
        head.GetComponent<TouchManager>().currentButtonTag = "InterviewButton";
    }

    /*void OnGUI()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            // The user denied permission to use the microphone.
            // Display a message explaining why you need it with Yes/No buttons.
            // If the user says yes then present the request again
            // Display a dialog here.
            dialog.AddComponent<PermissionsRationaleDialog>();
            return;
        }
        else if (dialog != null)
        {
            Destroy(dialog);
        }
#endif

        // Now you can do things with the microphone
    }*/


    void Update()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
         Microphone.Update();
#endif
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
            return;
        }

        if (!microphoneConected)
        {
            DetectMicrophone(false);
            return;
        }

        if (selectPanel.activeSelf) {
            return;
        }

        if (audioSource.isPlaying)
            return;

        if (!playedIntro)
        {

            playedIntro = true;
            isWaitingAnswer = true;

            SelectAsker("intro");
            SetSpeaker(0);
            //SelectAsker();
        }
        else if (isWaitingAnswer)
        {
            if (runningSpeakAnimation) {
                runningSpeakAnimation = false;
                currentInterviewer.StopAsking();
                SetSpeaker(1);
            }


            /*MicrophoneSource.GetSpectrumData(clipSampleData, 0, FFTWindow.Rectangular);

            float sumVolumes = 0;
            for (int i = 0; i < clipSampleData.Length; i++)
            {
                sumVolumes += clipSampleData[i];
            }
            float currentAverageVolume = (sumVolumes / clipSampleData.Length) * 1000;*/
            float currentAverageVolume = 0;
#if UNITY_WEBGL && !UNITY_EDITOR
            float[] volumes = Microphone.volumes;
            volumesText.text = "";
            for(int i = 0; i < volumes.Length; i++){
                volumesText.text += volumes[i]+"  ";
            }
            currentAverageVolume =volumes[0]*1000;
#endif
            // Debug.Log(currentAverageVolume);
            if (currentAverageVolume > 0.4f)
            {
                //Is speaking
                currentWaitTime = 0;
                speakBar.fillAmount = 1;
                hasAnswered = true;
                AnswerNotRecivedMessage.SetActive(false);
                dialogText.gameObject.SetActive(true);
                speakerTurn.text = "" + speakers[1];

            }
            else
            {
                currentWaitTime += Time.deltaTime;
                speakBar.fillAmount = 1- (currentWaitTime/waitTime);
                if (currentWaitTime >= waitTime)
                {
                    if (!hasAnswered) {
                        speakerTurn.text = "" + speakers[2];
                        AnswerNotRecivedMessage.SetActive(true);
                        dialogText.gameObject.SetActive(false);
                        hasAnswered = true;
                        currentWaitTime = 0;
                    }
                    else
                    {
                        //speakerTurn.text = "" + speakers[1];
                        //AnswerNotRecivedMessage.SetActive(false);
                        //dialogText.gameObject.SetActive(true);
                        isWaitingAnswer = false;
                        currentWaitTime = 0;
                        hasAnswered = false;
                    }
                }
            }
        }
        else if (currentAudio < totalQuestions && currentExecitiveQuestion < questionsExecutive.Length)
        {
            if (isDiligent)
            {
                currentAudio++;
            }
            else {
                currentAudio = questionsExecutive[currentExecitiveQuestion];
                currentExecitiveQuestion++;
            }
            SelectAsker("question"+currentAudio);
            AnswerNotRecivedMessage.SetActive(false);
            dialogText.gameObject.SetActive(true);

            isWaitingAnswer = true;
            speakBar.fillAmount = 1;

            SetSpeaker(0);
        }
        else if (!playedOutro)
        {
            AnswerNotRecivedMessage.SetActive(false);
            dialogText.gameObject.SetActive(true);
            currentAudio = totalQuestions+1;
            SelectAsker("out");
            SetSpeaker(0);
            
            playedOutro = true;

        }
        else {
            speakerTurn.text = "The interview has finished.";

            StartCoroutine(ExitCorutine());
        }
    }


    public void SetSpeaker(int id) {
        
        if (id == 1) {
            speakerTurn.text = "" + speakers[id];
            speakBar.gameObject.SetActive(true);
            parentSpeakBar.SetActive(true);
            //dialogText.gameObject.SetActive(false);
        }
        else {

            speakBar.gameObject.SetActive(false);
            parentSpeakBar.SetActive(false);
            dialogText.gameObject.SetActive(true);
            
        }
    }

    public void DetectMicrophone(bool atStart) {
        /*const string micPermissionName = "android.permission.RECORD_AUDIO";
        AndroidPermissionsManager.RequestPermission(micPermissionName)
            .ThenAction((grantResult) =>
            {
            });*/
#if UNITY_WEBGL && !UNITY_EDITOR
        if (Microphone.devices.Length > 0)
        {
            if (atStart)
            {
                //MicrophoneSource.clip = Microphone.Start(Microphone.devices[0], true, 1, 44100);
                //while (!(Microphone.GetPosition(null) > 0)) { }
                //MicrophoneSource.Play();
                microphoneConected = true;
            }
            else {
                SceneManager.LoadScene("FounderPitch-1");
            }
        }
        else {
            speakerTurn.text = "There is microphone connected, please connect one";
        }
#endif
    }

    public void SelectAsker(string audio) {
        
        runningSpeakAnimation = true;
        int random = Random.Range(0, interviewers.Length);
        if (random == lastInterviewer)
        {
            random++;
            if (random >= interviewers.Length)
            {
                random = 0;
            }
        }
        lastInterviewer = random;

        int randomAsk = Random.Range(1,totalAskAnimations+1);

        if (randomAsk == lastAskAnimation)
        {
            randomAsk++;
            if (randomAsk > totalAskAnimations)
            {
                randomAsk = 1;
            }
        }
        lastAskAnimation = randomAsk;

        speakerTurn.text = "" + names[random];
        for (int i = 0; i < interviewers.Length; i++)
        {
            if (random == i)
            {
                currentInterviewer = interviewers[random];
                currentInterviewer.AskQuestion(true,randomAsk);
            }
            else
            {
                interviewers[i].AskQuestion(false,0);
            }
        }

        audioSource.clip = Resources.Load("Audios/" + audioFolders[random] + "/"+audio) as AudioClip;
        audioSource.Play();

        if (audio == "intro")
        {
            dialogText.text = dialogs[0];
        }
        else {
            dialogText.text = dialogs[currentAudio];
        }
    }

    IEnumerator ExitCorutine() {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("IntroScene");
    }

    public void SetType(bool _isdDiligent) {
        isDiligent = _isdDiligent;
        selectPanel.SetActive(false);
        interviewPanel.SetActive(true);
    }
}
