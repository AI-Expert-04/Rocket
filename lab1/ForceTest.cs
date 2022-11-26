using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceTest : MonoBehaviour
{
    public Rigidbody rb; // 물리엔진(강체) 받기
    public Transform left; // 위치(왼쪽) 받기
    public Transform right; // 위치(왼쪽) 받기
    public Transform engine; // 엔진 받기

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // 물리엔진 구성요소 받기
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // 엔진에 10 힘 주기
            rb.AddForceAtPosition(transform.up * 10, engine.transform.position);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            // 오른쪽 최대값(양수)
            rb.AddForceAtPosition(-transform.right, right.transform.position); 
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // 왼쪽 최대값(양수)
            rb.AddForceAtPosition(transform.right, left.transform.position);
        }
    }
}
