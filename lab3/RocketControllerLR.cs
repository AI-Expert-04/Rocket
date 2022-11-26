using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketControllerLR : MonoBehaviour
{
    public Rigidbody rb; // 물리엔진
    public AgentControllerLR ac; // 에이전트 컨트롤 연동

    public float mainEngineForce = 10f; // 메인엔진 힘 받기
    public float leftEngineForce = 1f; // 왼쪽엔진 힘 받기
    public float rightEngineForce = 1f; // 오른쪽엔진 힘 받기

    public bool mainEngineOn = false; // 메인엔진 비활성화
    public bool leftEngineOn = false; // 왼쪽엔진 비활성화
    public bool rightEngineOn = false; // 오른쪽엔진 비활성화

    public bool reset = false;// 리셋 비활성화
    public bool stop = false; // 멈추기 비활성화
    public Renderer floorRenderer; // 렌더러 받기

    public GameObject mainEngineFx; // 메인엔진 게임오브젝트 받기
    public GameObject leftEngineFx; // 왼쪽엔진 게임오브젝트 받기
    public GameObject rightEngineFx; // 오른쪽엔진 게임오브젝트 받기

    public float initHeight = 10; // 초기화 높이
    public float rotationRange = 0; // 회전 범위

    void Start()
    {
        ac = GetComponent<AgentControllerLR>(); // 에이전트 구성요소 받기
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
            leftEngineOn = false; // 왼쪽엔진 비활성화
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
            transform.localRotation = Quaternion.Euler(new Vector3(Random.Range(
                -rotationRange, rotationRange), Random.Range(-rotationRange, rotationRange), 
                Random.Range(-rotationRange, rotationRange)));
            rb.velocity = Vector3.zero; // 속도
            // Vector3.zero = (0, 0, 0)
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
            // 착륙 후 로켓이 넘어지지 않는다면 
            if (Mathf.Abs(Vector3.Dot(transform.up, Vector3.right)) < 0.1 && Mathf.Abs(Vector3.Dot(transform.up, Vector3.forward)) < 0.1 && Vector3.Dot(transform.up, Vector3.up) > 0.9)
            {
                floorRenderer.material.color = Color.green;
                ac.EndEpisode(1); // 점수 1점
            }
            else
            {
                floorRenderer.material.color = Color.red;
                ac.EndEpisode(0); // 점수 0점
            }
        }

        if (stop)
        {
            return;
        }

        // 엔진 조작 파트
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
            rb.AddForceAtPosition(-transform.right * leftEngineForce, leftEnginePosition.transform.position);
            leftEngineFx.SetActive(true);
        }
        else
        {
            leftEngineFx.SetActive(false);
        }

        if (rightEngineOn) // 오른쪽엔진 활성화
        {
            rb.AddForceAtPosition(transform.right * rightEngineForce, rightEnginePosition.transform.position);
            rightEngineFx.SetActive(true);
        }
        else
        {
            rightEngineFx.SetActive(false);
        }
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
