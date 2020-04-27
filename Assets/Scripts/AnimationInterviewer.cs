using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct AnimationStruct
{
    public string triggerName;
    public float probability;
}

public class AnimationInterviewer : MonoBehaviour
{

    [SerializeField] private AnimationStruct[] RandomAnimations;
    [SerializeField] private GameObject positionObject;
    [SerializeField] private Image arrow;
    [SerializeField] private Sprite arrowGray;
    [SerializeField] private Sprite arrowOrange;

    private Color initialColor;


    private Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        anim = this.GetComponent<Animator>();
    }

    void Start()
    {
        positionObject.SetActive(false);
        initialColor = arrow.color;
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    private void PlayRandomAnimation() {
        float r = Random.Range(0.0f, 1.0f);
        float lastProb = 0;
        for (int i = 0; i < RandomAnimations.Length; i++) {
            if (r < RandomAnimations[i].probability + lastProb)
            {
                anim.SetTrigger(RandomAnimations[i].triggerName);
                break;
            }
            else {
                lastProb += RandomAnimations[i].probability;
            }
        }
    }

    public void AskQuestion(bool isAsking, int askAnimation) {
        
        if (isAsking)
        {
            anim.SetTrigger("ask"+askAnimation);
            positionObject.SetActive(true);
            arrow.sprite = arrowOrange;
            arrow.color = Color.white;
        }
        else {
            PlayRandomAnimation();
        }
    }

    public void StopAsking() {
        PlayRandomAnimation();
        arrow.sprite = arrowGray;
        arrow.color = initialColor;
        positionObject.SetActive(false);
    }

    public void EndAnimationAsk() {
        PlayRandomAnimation();
    }

    public void EndRandomAnimation() {
        PlayRandomAnimation();
    }
}
