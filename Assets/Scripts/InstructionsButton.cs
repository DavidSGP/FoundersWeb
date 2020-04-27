using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsButton : MonoBehaviour
{
    [SerializeField] private bool isNext;
    private InstructionsManager manager;

    private void Start()
    {
        manager = transform.parent.GetComponent<InstructionsManager>();
    }

    public void ClickButton() {
        if (isNext)
        {
            manager.ClickNext();
        }
        else {
            manager.ClickLast();
        }
    }
}
