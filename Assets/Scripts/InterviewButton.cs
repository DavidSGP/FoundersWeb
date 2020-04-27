using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterviewButton : MonoBehaviour
{
    [SerializeField] private bool isDiligent;
    [SerializeField] private AudioManager manager;

    public void Click() {
        manager.SetType(isDiligent);
    }
}
