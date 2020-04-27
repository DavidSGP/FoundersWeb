using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct AudioPool
{
    public string folderName;
    public string[] audios;
}

public class AudioManagerV1 : MonoBehaviour
{
    [SerializeField] private AudioPool Intros;
    [SerializeField] private AudioPool RandomQuestions;
    [SerializeField] private AudioPool ValuationQuestions;
    [SerializeField] private AudioPool GoToMarketQuestions;
    [SerializeField] private AudioPool Outros;
    [SerializeField] private AudioSource MicrophoneSource;
    [SerializeField] private TextMeshProUGUI speakerTurn;
    [SerializeField] private GameObject AnswerNotRecivedMessage;
    [SerializeField] private AnimationInterviewer[] interviewers;
    [SerializeField] private Image speakBar;
    [SerializeField] private GameObject parentSpeakBar;

    /*private List<string> randomQuestionsAsked;
    private List<string> valuationQuestionsAsked;
    private List<string> goToMarketQuestionsAsked;*/
    private List<int> randomQuestionsIds;
    private List<int> questionsPoolIds;
    private int valuationQuiestionsId;
    private int goToMarketQuestionsIds;
    private int numberOfQuestionPools;
    private int totalNumberOfQuestions;
    private int totalNumberOfQuestionsAsked;


    AudioSource audioSource;
    bool playedIntro;
    bool playedOutro;

    float waitTime = 3.0f;
    float currentWaitTime;
    bool isWaitingAnswer;
    float[] clipSampleData = new float[1024];
    string[] speakers = new string[2] { "Interviewer", "Founder" };
    bool microphoneConected;
    bool hasAnswered;

    private AnimationInterviewer currentInterviewer;
    private int lastInterviewer;
    private bool runningSpeakAnimation;
    private int lastAskAnimation;
    private int totalAskAnimations;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        randomQuestionsIds = new List<int>();
        questionsPoolIds = new List<int>();
        valuationQuiestionsId = 0;
        goToMarketQuestionsIds = 0;
        numberOfQuestionPools = 3;
        totalAskAnimations = 3;
        totalNumberOfQuestions = RandomQuestions.audios.Length + ValuationQuestions.audios.Length + GoToMarketQuestions.audios.Length;
        speakBar.fillAmount = 1;
        speakBar.gameObject.SetActive(false);
        for (int i = 0; i < numberOfQuestionPools; i++)
        {
            questionsPoolIds.Add(i);
        }
        for (int i = 0; i < RandomQuestions.audios.Length; i++)
        {
            randomQuestionsIds.Add(i);
        }

        currentWaitTime = 0;
        isWaitingAnswer = false;
        microphoneConected = false;
        hasAnswered = false;
        AnswerNotRecivedMessage.SetActive(false);
        DetectMicrophone(true);

        currentInterviewer = null;
        lastInterviewer = -1;
        lastAskAnimation = -1;
        runningSpeakAnimation = false;



    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }
        /* else if(Input.GetKeyDown(KeyCode.R)) {
             SceneManager.LoadScene(0);
         }*/

        if (!microphoneConected)
        {
            DetectMicrophone(false);
            return;
        }

        if (audioSource.isPlaying)
            return;

        if (!playedIntro)
        {
            int index = Random.Range(0, Intros.audios.Length);
            audioSource.clip = Resources.Load("Audios/" + Intros.folderName + "/" + Intros.audios[index]) as AudioClip;
            audioSource.Play();
            playedIntro = true;
            isWaitingAnswer = true;

            SelectAsker();
            SetSpeaker(0);
            //SelectAsker();
        }
        else if (isWaitingAnswer)
        {
            if (runningSpeakAnimation)
            {
                runningSpeakAnimation = false;
                currentInterviewer.StopAsking();
                SetSpeaker(1);
            }


            MicrophoneSource.GetSpectrumData(clipSampleData, 0, FFTWindow.Rectangular);

            float sumVolumes = 0;
            for (int i = 0; i < clipSampleData.Length; i++)
            {
                sumVolumes += clipSampleData[i];
            }
            float currentAverageVolume = (sumVolumes / clipSampleData.Length) * 1000;
            Debug.Log(currentAverageVolume);
            if (currentAverageVolume > 0.01f)
            {
                //Is speaking
                currentWaitTime = 0;
                speakBar.fillAmount = 1;
                hasAnswered = true;
                AnswerNotRecivedMessage.SetActive(false);

            }
            else
            {
                currentWaitTime += Time.deltaTime;
                speakBar.fillAmount = 1 - (currentWaitTime / waitTime);
                if (currentWaitTime >= waitTime)
                {
                    if (!hasAnswered)
                    {
                        AnswerNotRecivedMessage.SetActive(true);
                        hasAnswered = true;
                        currentWaitTime = 0;
                    }
                    else
                    {
                        AnswerNotRecivedMessage.SetActive(false);
                        isWaitingAnswer = false;
                        currentWaitTime = 0;
                        hasAnswered = false;
                    }
                }
            }
        }
        else if (totalNumberOfQuestionsAsked < totalNumberOfQuestions)
        {
            int questionPoolIndex = Random.Range(0, questionsPoolIds.Count);
            int index = questionsPoolIds[questionPoolIndex];

            SelectAsker();

            switch (index)
            {
                case 0:
                    askQuestion(ValuationQuestions.audios, valuationQuiestionsId, ValuationQuestions.folderName, questionPoolIndex);
                    valuationQuiestionsId++;
                    break;
                case 1:
                    askQuestion(GoToMarketQuestions.audios, goToMarketQuestionsIds, GoToMarketQuestions.folderName, questionPoolIndex);
                    goToMarketQuestionsIds++;
                    break;
                case 2:
                    askRandomQuestion(questionPoolIndex);
                    break;
            }
            isWaitingAnswer = true;
            speakBar.fillAmount = 1;

            SetSpeaker(0);
        }
        else if (!playedOutro)
        {
            SelectAsker();
            SetSpeaker(0);
            int index = Random.Range(0, Outros.audios.Length);
            audioSource.clip = Resources.Load("Audios/" + Outros.folderName + "/" + Outros.audios[index]) as AudioClip;
            audioSource.Play();
            playedOutro = true;

        }
        else
        {
            speakerTurn.text = "The interview has finished.";

            StartCoroutine(ExitCorutine());
        }
    }


    private void askRandomQuestion(int questionPoolIndex)
    {


        int index = Random.Range(0, randomQuestionsIds.Count);
        totalNumberOfQuestionsAsked++;
        audioSource.clip = Resources.Load("Audios/" + RandomQuestions.folderName + "/" + RandomQuestions.audios[randomQuestionsIds[index]]) as AudioClip;
        randomQuestionsIds.RemoveAt(index);
        audioSource.Play();

        if (randomQuestionsIds.Count == 0)
        {
            //there are no more random questions
            questionsPoolIds.RemoveAt(questionPoolIndex);
        }

    }

    private void askQuestion(string[] questions, int index, string folderName, int questionPoolIndex)
    {


        audioSource.clip = Resources.Load("Audios/" + folderName + "/" + questions[index]) as AudioClip;
        totalNumberOfQuestionsAsked++;
        audioSource.Play();

        if (index + 1 == questions.Length)
        {
            //there are no more questions in that array
            questionsPoolIds.RemoveAt(questionPoolIndex);
        }
    }

    public void SetSpeaker(int id)
    {
        speakerTurn.text = "Speaker: " + speakers[id];
        if (id == 1)
        {
            speakBar.gameObject.SetActive(true);
            parentSpeakBar.SetActive(true);
        }
        else
        {
            speakBar.gameObject.SetActive(false);
            parentSpeakBar.SetActive(false);
        }
    }

    public void DetectMicrophone(bool atStart)
    {
       /* if (Microphone.devices.Length > 0)
        {
            if (atStart)
            {
                MicrophoneSource.clip = Microphone.Start(Microphone.devices[0], true, 1, 44100);
                while (!(Microphone.GetPosition(null) > 0)) { }
                MicrophoneSource.Play();
                microphoneConected = true;
            }
            else
            {
                SceneManager.LoadScene("FounderPitch-1");
            }
        }
        else
        {
            speakerTurn.text = "There is microphone connected, please connect one";
        }*/
    }

    public void SelectAsker()
    {
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

        int randomAsk = Random.Range(1, totalAskAnimations + 1);

        if (randomAsk == lastAskAnimation)
        {
            randomAsk++;
            if (randomAsk > totalAskAnimations)
            {
                randomAsk = 1;
            }
        }
        lastAskAnimation = randomAsk;
        Debug.Log("Selected interviewer " + random);
        for (int i = 0; i < interviewers.Length; i++)
        {
            if (random == i)
            {
                currentInterviewer = interviewers[random];
                currentInterviewer.AskQuestion(true, randomAsk);
            }
            else
            {
                interviewers[i].AskQuestion(false, 0);
            }
        }
    }

    IEnumerator ExitCorutine()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("IntroScene");
    }
}
