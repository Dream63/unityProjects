using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class Arms : MonoBehaviour
{
    public static Arms instance;

    [SerializeField] public GameObject armTop, armBot;
    public float armSize = 1f;
    public Vector3 startPos, endPos;
    public bool invertY;
    float angle;

    void Update()
{
        startPos = armTop.transform.position;
        Vector3 diffVec = endPos - startPos;
        float diffVecLength = diffVec.magnitude;

        if (diffVecLength >= armSize * 1.9999)
        {
            endPos = Utils.ClampVector2D(startPos, endPos, 0, (float)(armSize * 1.999));
            diffVec = endPos - startPos;
            diffVecLength = diffVec.magnitude;
        }

        angle = 90 - Utils.AsinE(diffVecLength / (2 * armSize), diffVec);

        if (invertY)
            angle = -angle;

        armTop.transform.rotation = Quaternion.Euler(0, 0, Utils.AngleBetweenPositions2D(startPos, endPos) + angle);
        armBot.transform.rotation = Quaternion.Euler(0, 0, Utils.AngleBetweenPositions2D(endPos, startPos) - angle);
        armBot.transform.position = endPos;
    }
}
