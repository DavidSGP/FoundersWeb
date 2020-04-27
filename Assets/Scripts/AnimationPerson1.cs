using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPerson1 : MonoBehaviour
{

    [SerializeField] private AnimationStruct[] RandomAnimations;

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        PlayRandomAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayRandomAnimation()
    {
        float r = Random.Range(0.0f, 1.0f);
        float lastProb = 0;
        for (int i = 0; i < RandomAnimations.Length; i++)
        {
            if (r < RandomAnimations[i].probability + lastProb)
            {
                anim.SetTrigger(RandomAnimations[i].triggerName);
                break;
            }
            else
            {
                lastProb += RandomAnimations[i].probability;
            }
        }
    }

    public void EndRandomAnimation()
    {
        PlayRandomAnimation();
    }
}
