using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketControllerLRFB : MonoBehaviour
{
    public Rigidbody rb; // 물리엔진
    public AgentControllerLRFB ac; // 에이전트 컨트롤 연동

    public float mainEngineForce = 20f; // 메인엔진 힘 받기
    public float sideEngineForce = 1f; // 사이드엔진 힘 받기

    public bool mainEngineOn = false; // 메인엔진 비활성화
    public bool leftEngineOn = false; // 왼쪽엔진 비활성화
    public bool rightEngineOn = false; // 오른쪽엔진 비활성화
    public bool forwardEngineOn = false; // 앞쪽엔진 비활성화
    public bool backwardEngineOn = false; // 뒤쪽엔진 비활성화

    public GameObject mainEngineFx; // 메인엔진 게임오브젝트 받기
    public GameObject leftEngineFx; // 왼쪽엔진 게임오브젝트 받기
    public GameObject rightEngineFx; // 오른쪽엔진 게임오브젝트 받기
    public GameObject forwardEngineFx; // 앞쪽엔진 게임오브젝트 받기
    public GameObject backwardEngineFx; // 뒤쪽엔진 게임오브젝트 받기

    public bool reset = false;
    public bool stop = false;
    public Renderer floorRenderer;

    public float initHeight = 10; // 초기화 높이
    public float rotaionRange = 0; // 회전 범위

    void Start()
    {
        ac = GetComponent<AgentControllerLRFB>(); // 에이전트 구성요소 받기
        rb = GetComponent<Rigidbody>(); // 물리엔진 구성요소 받기
    }

    public void ResetRocket()
    {
        reset = true; // 리셋 활성화
    } 

    public void SetMainEngine(int value)
    {
        if (value == 1)
        {
            mainEngineOn = true; // 메인엔진 활성화
        }
        else
        {
            mainEngineOn = false; // 메인엔진 비활성화
        }
    }

    public void SetLeftEngine(int value)
    {
        if (value == 1)
        {
            leftEngineOn = true; // 왼쪽엔진 활성화
        }
        else
        {
            leftEngineOn = false; // 왼졲엔진 비활성화
        }
    }

    public void SetRightEngine(int value)
    {
        if (value == 1)
        {
            rightEngineOn = true; // 오른쪽엔진 활성화
        }
        else
        {
            rightEngineOn = false; // 오른쪽엔진 비활성화
        }
    }

    public void SetForwardEngine(int value)
    {
        forwardEngineOn = (value == 1); // 앞쪽엔진 활,비활성화
    }

    public void SetBackwardEngine(int value)
    {
        backwardEngineOn = (value == 1); // 뒤쪽엔진 활,비활성화
    }

    private void FixedUpdate()
    {
         // 만약 한 에피소드가 끝나면
        if (ac.episodeFinished)
        {
            return;
        }

        // 만약 리셋이 활성화 된다면
        if (reset)
        {
             // 범위 : (0, 초기값높이, 0)
            transform.localPosition = new Vector3(0, initHeight, 0);
            transform.localRotation = Quaternion.Euler(
                new Vector3(
                    Random.Range(-rotaionRange, rotaionRange),
                    Random.Range(-rotaionRange, rotaionRange),
                    Random.Range(-rotaionRange, rotaionRange)
                )
            );
            rb.velocity = Vector3.zero; // 속도
            rb.angularVelocity = Vector3.zero; // 각속도.

            reset = false; // 리셋 비활성화
            stop = false; // stop 비활성화
            floorRenderer.material.color = Color.white; // 바닥색 바꾸기
            return;
        }

        // 로켓의 고도가 180을 넘어가면 0점 처리
        // 로켓의 고도가 음수가 되면 0점 처리
        if (transform.localPosition.y > initHeight * 1.2f || transform.localPosition.y < 0)
        {
            floorRenderer.material.color = Color.red; // 바닥샏 바꾸기
            ac.EndEpisode(0); // 점수 0점
            return;
        }

        // ##(목표 달성 보상)##
        if (rb.IsSleeping())
        {
            // 착륙 후 로켓의 속도가 1 이상이며, 왼쪽, 오른쪽, 앞, 뒤 각도가 0.1 보다 크면
            if (Mathf.Abs(Vector3.Dot(transform.up, Vector3.up)) > 0.9 &&
                Mathf.Abs(Vector3.Dot(transform.up, Vector3.right)) < 0.1 &&
                Mathf.Abs(Vector3.Dot(transform.up, Vector3.left)) < 0.1 &&
                Mathf.Abs(Vector3.Dot(transform.up, Vector3.backward)) < 0.1 &&
                Mathf.Abs(Vector3.Dot(transform.up, Vector3.forward)) < 0.1)
            {
                floorRenderer.material.color = Color.green;
                ac.EndEpisode(1); // 보상
            }
            else
            {
                floorRenderer.material.color = Color.red;
                ac.EndEpisode(0); // 실패
            }
        }

        // 엔진 조작 파트
        if (stop)
        {
            return;
        }

        if (mainEngineOn)
        {
             // up * 메인엔진
            rb.AddForceAtPosition(transform.up * mainEngineForce, transform.position);
            mainEngineFx.SetActive(true);
        }
        else
        {
            mainEngineFx.SetActive(false);
        }

        if (leftEngineOn) // 왼쪽엔진 활성화
        {
            rb.AddForceAtPosition(transform.right * sideEngineForce, leftEngineFx.transform.position);
        }
        leftEngineFx.SetActive(leftEngineOn);

        if (rightEngineOn) // 오른쪽엔진 활성화
        {
            rb.AddForceAtPosition(-transform.right * sideEngineForce, rightEngineFx.transform.position);
        }
        rightEngineFx.SetActive(rightEngineOn);

        if (forwardEngineOn) // 앞쪽엔진 활성화
        {
            rb.AddForceAtPosition(-transform.forward * sideEngineForce, forwardEngineFx.transform.position);
        }
        forwardEngineFx.SetActive(forwardEngineOn);

        if (backwardEngineOn) // 뒤쪽엔진 활성화
        {
            rb.AddForceAtPosition(transform.forward * sideEngineForce, backwardEngineFx.transform.position);
        }
        backwardEngineFx.SetActive(backwardEngineOn);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 에피소드가 끝나거나 멈추면 리턴
        if (ac.episodeFinished || stop)
        {
            return;
        }

        // 로켓 속도가 10 이상이면
        if (collision.relativeVelocity.y > 10f)
        {
            floorRenderer.material.color = Color.red;
            ac.EndEpisode(0f); // 0점
        }
        else
        {
            stop = true;
        }
    }
}
