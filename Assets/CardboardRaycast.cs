using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardboardRaycast : MonoBehaviour
{
    [SerializeField] private SpriteRenderer cursor;
    [SerializeField] private Color initialColor;
    [SerializeField] private Color detectButtonColor;
    [SerializeField] private string currentButtonTag;


    private GameObject currentButton;
    private Transform cursorTransform;
    private bool buttonClicked;
    private Vector3 startPos;
    private float maxTimeClick = 1.0f;
    private float currentTime;

    // Start is called before the first frame update
    void Start()
    {

        //initialColor = cursor.color;
        currentButton = null;
        cursorTransform = cursor.transform.parent.transform;
        startPos = cursorTransform.localPosition;
        currentTime = 0;
        buttonClicked = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("No vr");
        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        //Debug.DrawRay(checkTransform.position, checkTransform.forward, Color.green);
        if (Physics.Raycast(this.transform.position, cursorTransform.position - this.transform.position, out hit, 30))
        {
            if (hit.collider.CompareTag("InstructionsButton") || hit.collider.CompareTag("InterviewButton"))
            {

                currentButton = hit.collider.gameObject;
                //currentButtonTag = hit.collider.tag;
                cursor.color = detectButtonColor;
                cursorTransform.position = Vector3.Lerp(this.transform.position, hit.point, 0.7f);
                /*currentTime = 0;
                cursor.fillAmount = 0;*/

            }
            else
            {
                cursor.color = initialColor;
                currentButton = null;
                cursorTransform.localPosition = startPos;
                buttonClicked = false;
                currentTime = 0;
            }
        }
        else
        {
            cursor.color = initialColor;
            currentButton = null;
            cursorTransform.localPosition = startPos;
            buttonClicked = false;
            currentTime = 0;
        }

        if (currentButton != null)
        {
            if (Input.touchCount != 0) {
                currentTime = maxTimeClick;
            }

            if (currentTime < maxTimeClick) {
                currentTime += Time.deltaTime;
                //Debug.Log(currentTime);

            }
            else if(!buttonClicked){
                //currentTime = 0;

                cursor.color = initialColor;
                buttonClicked = true;
                if (currentButtonTag == "InstructionsButton")
                {
                    currentButton.GetComponent<InstructionsButton>().ClickButton();
                }
                else if (currentButtonTag == "InterviewButton")
                {
                    cursorTransform.gameObject.SetActive(false);
                    this.enabled = false;
                    currentButton.GetComponent<InterviewButton>().Click();
                }
                
            }
            
        }
    }
}
