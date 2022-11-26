using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    public Rigidbody rb; // 물리엔진
    public AgentController ac; // 에이전트 컨트롤 연동
    public float force = 20f; // 힘 20

    // bool = False, True만 가능
    public bool engineOn = false; // 엔진 비활성화
    public bool reset = false; // 리셋 비활성화
    public Renderer floorRenderer; // 바닥 렌더러
    public GameObject engineFx; // 엔진 받기
    public float initHeight = 20; // 초기화 높이

    void Start()
    {
        ac = GetComponent<AgentController>(); // 에이전트 구성요소 받기
        rb = GetComponent<Rigidbody>(); // 물리엔진 구성요소 받기
    }

    public void ResetRocket()
    {
        reset = true; // 리셋이 활성화
    }

    public void OnEngine()
    {
        engineOn = true; // 엔진 활성화
    }

    public void OffEngine()
    {
        engineOn = false; // 엔진 비활성화
    }

    // fixed는 모든컴퓨터에서 사양 관계없이 똑같이 흐름.
    private void FixedUpdate()
    {
        // 한 에피소드가 끝나면
        if (ac.episodeFinished)
        {
            return;
        }

        // 만약 리셋이 활성화 된다면
        if (reset)
        {
            // 범위 (0, 초기값 위치, 0)
            transform.localPosition = new Vector3(0, initHeight, 0);
            // Vector3.zero = (0, 0, 0)
            rb.velocity = Vector3.zero; // 속도

            reset = false; // 리셋 비활성화
            floorRenderer.material.color = Color.white; // 바닥색깔 활성화
            return;
        }

        if (engineOn)
        {
            // up * 힘
            rb.AddForce(Vector3.up * force);
            engineFx.SetActive(true); // 엔진 활성화
        }
        else
        {
            engineFx.SetActive(false); // 엔진 비활성화
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 에피소드가 끝나면 리턴
        if (ac.episodeFinished)
        {
            return;
        }

        // 착륙 y속도가 10을 넘어가면 0점 처리 및 바닥색깔 바꾸기
        if (collision.relativeVelocity.y > 10f)
        {
            floorRenderer.material.color = Color.red;
            ac.EndEpisode(0f);
        }

        else // 1점 및 바닥색깔 바꾸기
        {
            floorRenderer.material.color = Color.green;
            ac.EndEpisode(1f);
        }
    }
}
