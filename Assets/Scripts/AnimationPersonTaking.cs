using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPersonTaking : MonoBehaviour
{
    [SerializeField] private float probabilityListen;

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    private void PlayRandomAnimation()
    {
        float r = Random.Range(0, 1);
        if (r < probabilityListen)
        {
            anim.SetTrigger("listen");
        }
        else {
            anim.SetTrigger("continue");
        }
    }

    public void EndRandomAnimation()
    {
        PlayRandomAnimation();
    }
}
