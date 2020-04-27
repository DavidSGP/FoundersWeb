using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMouseMovement : MonoBehaviour
{
   private float standUpHeight = 0.3f;

    Vector2 lastPos;
    float maxRotateX = 45f;
    float moveAttenuation = 0.5f;
    private float sitValue;
    private float deltaValue;
    private float speedMove = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        lastPos = new Vector2(-1, -1);
        deltaValue = DataController.instance.deltaValue;


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (lastPos.x != -1)
            {
                Vector2 delta = new Vector2(Input.mousePosition.x - lastPos.x, Input.mousePosition.y - lastPos.y) * moveAttenuation;
                //this.transform.Rotate(0,delta.x,0);

                float xRotation = this.transform.rotation.eulerAngles.x;
                if (xRotation > 180)
                {
                    xRotation = xRotation - 360;
                }
                if (!((xRotation < maxRotateX && delta.y < 0) || (xRotation > -maxRotateX && delta.y > 0)))
                {
                    delta.y = 0;
                }
                this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x - delta.y, this.transform.eulerAngles.y + delta.x, 0);

            }
            lastPos.x = Input.mousePosition.x;
            lastPos.y = Input.mousePosition.y;
        }
        else {
            lastPos.x = -1;
            lastPos.y = -1;
        }
    

    }
}
