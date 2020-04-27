using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebXR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer cursorL;
    [SerializeField] private SpriteRenderer cursor;
    [SerializeField] private SpriteRenderer cursorCenter;
    [SerializeField] private Color detectButtonColor;
    [SerializeField] private Transform centerEye;
    public string currentButtonTag;
    [SerializeField] private bool isGo;
    [SerializeField] private WebXRController left;
    [SerializeField] private WebXRController right;
    [SerializeField] private CardboardRaycast cardboardObject;
    public bool isFirst;
    private SkinnedMeshRenderer skin;
    [SerializeField] private Color initialColor;
    private GameObject currentButton;
    private Transform cursorTransform;
    private bool buttonClicked;
    private Vector3 startPos;

    private GameObject currentButtonL;
    private Transform cursorTransformL;
    private bool buttonClickedL;
    private Vector3 startPosL;
    private bool isVr;
   
    // Start is called before the first frame update
    void Start()
    {
        skin = left.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();

        if (skin.enabled)
        {
            isVr = true;
            cursorCenter.gameObject.SetActive(false);
            //initialColor = cursor.color;
            currentButton = null;
            cursorTransform = cursor.transform;
            startPos = cursorTransform.localPosition;

            currentButtonL = null;
            cursorTransformL = cursorL.transform;
            startPosL = cursorTransformL.localPosition;
            cardboardObject.enabled = false;
        }
        else {
            isVr = false;
            cursor.gameObject.SetActive(false);
            cursorL.gameObject.SetActive(false);

            //initialColor = cursorCenter.color;
            currentButton = null;
            cursorTransform = cursorCenter.transform;
            startPos = cursorTransform.localPosition;
            cardboardObject.enabled = true;
            
        }
        

    }

    // Update is called once per frame
    void Update()
    {

        if (skin.enabled)
        {
            isVr = true;
            cursorCenter.gameObject.SetActive(false);
            cursor.gameObject.SetActive(true);
            cursorL.gameObject.SetActive(true);
            //initialColor = cursor.color;
            currentButton = null;
            cursorTransform = cursor.transform;
            startPos = cursorTransform.localPosition;

            currentButtonL = null;
            cursorTransformL = cursorL.transform;
            startPosL = cursorTransformL.localPosition;
            cardboardObject.enabled = false;
        }
        else
        {
            isVr = false;
            cursor.gameObject.SetActive(false);
            cursorL.gameObject.SetActive(false);
            cursorCenter.gameObject.SetActive(true);
           // initialColor = cursorCenter.color;
            currentButton = null;
            cursorTransform = cursorCenter.transform;
            startPos = cursorTransform.localPosition;
            cardboardObject.enabled = true;

        }

        if (!isVr) {
            return;
        }
        RaycastHit hit;

        
            if (Physics.Raycast(centerEye.position, cursorTransform.position - centerEye.position, out hit, 30))
            {
                if (hit.collider.CompareTag("InstructionsButton") || hit.collider.CompareTag("InterviewButton"))
                {

                    currentButton = hit.collider.gameObject;
                //currentButtonTag = hit.collider.tag;
                cursor.color = detectButtonColor;
                    cursorTransform.position = Vector3.Lerp(centerEye.position, hit.point, 0.7f);

                }
                else
                {
                    cursor.color = initialColor;
                    currentButton = null;
                    cursorTransform.localPosition = startPos;
                }
            }
            else
            {
                cursor.color = initialColor;
                currentButton = null;
                cursorTransform.localPosition = startPos;
            }

            if (Physics.Raycast(centerEye.position, cursorTransformL.position - centerEye.position, out hit, 30))
            {
                if (hit.collider.CompareTag("InstructionsButton") || hit.collider.CompareTag("InterviewButton"))
                {

                    currentButtonL = hit.collider.gameObject;
                //currentButtonTag = hit.collider.tag;
                cursorL.color = detectButtonColor;
                    cursorTransformL.position = Vector3.Lerp(centerEye.position, hit.point, 0.7f);
                }
                else
                {
                //cursorL.color = initialColor;
                    currentButtonL = null;
                    cursorTransformL.localPosition = startPosL;
                }
            }
            else
            {
                cursorL.color = initialColor;
                currentButtonL = null;
                cursorTransformL.localPosition = startPosL;
            }

        

        if (currentButtonL != null)
            {
            //if (OVRInput.GetDown(OVRInput.Button.One)) {
            if (left.GetButton("Trigger") && !buttonClickedL)
            {
                if (currentButtonTag == "InstructionsButton")
                {
                    currentButtonL.GetComponent<InstructionsButton>().ClickButton();
                }
                else if (currentButtonTag == "InterviewButton")
                {
                    currentButtonL.GetComponent<InterviewButton>().Click();
                }
                buttonClickedL = true;
                SceneManager.LoadScene("FounderPitch-1");
            }
            else if (!left.GetButton("Trigger")) {
                buttonClickedL = false;
            }
                
            }

            if (currentButton != null)
            {
            if (right.GetButton("Trigger") && !buttonClicked)
            {
                if (currentButtonTag == "InstructionsButton")
                {
                    currentButton.GetComponent<InstructionsButton>().ClickButton();
                }
                else if (currentButtonTag == "InterviewButton")
                {
                    currentButton.GetComponent<InterviewButton>().Click();
                }
                buttonClicked = true;
            }
            else if (!right.GetButton("Trigger")) {
                buttonClicked = false;
            }
               
            }
        

        
    }
}
