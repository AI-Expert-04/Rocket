using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentController : Agent
{
    // MLAgents 이벤트 파라미터 불러오기
    public EnvironmentParameters m_ResetParams;
    //로켓 컨트롤 연동하는  코드
    public RocketController rc;

    // 에피소드는 비활성화로 실행.
    public bool episodeFinished = false;

    public override void Initialize()
    {
        // 로켓런처 구성 요소 가져오기.
        rc = GetComponent<RocketController2>();
    }

    public override void OnEpisodeBegin()
    {
        rc.ResetRocket();
        // 에피소드 완료시 비활성화
        episodeFinished = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 로켓의 로컬 위치 월드 좌표
        Vector3 rocketPosition = rc.transform.localPosition;
        Vector3 rocketVelocity = rc.rb.velocity; // 로켓.물리엔진.속도

        sensor.AddObservation(rocketPosition.x); // 로켓 로컬 x 위치
        sensor.AddObservation(rocketPosition.y); // 로켓 로컬 y 위치
        sensor.AddObservation(rocketPosition.z); // 로켓 로컬 z 위치

        sensor.AddObservation(rocketVelocity.x); // 로켓 로컬 x 속도
        sensor.AddObservation(rocketVelocity.y); // 로켓 로컬 y 속도
        sensor.AddObservation(rocketVelocity.z); // 로켓 로컬 z 속도
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
         // 엔진(킨다, 끈다)
        if (actionBuffers.DiscreteActions[0] == 1)
        {
            rc.OnEngine();
        }
        else
        {
            rc.OffEngine();
        }
    }

    // 끝난 에피소드
    public void EndEpisode(float reward)
    {
        SetReward(reward);
        episodeFinished = true;
        StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        // 학습과정에서 성공 여부를 보려고 1초 딜레이를 줌
        yield return new WaitForSeconds(1f);
        EndEpisode();
    }

    // 키로 입력받아 로켓 조정하기( 학습과는 상관 X)
    public override void Heuristic(in ActionBuffers actionsBuffers)
    {
        var actionsOut = actionsBuffers.DiscreteActions;
        if (Input.GetKey(KeyCode.Space))
        {
            actionsOut[0] = 1;
        }
        else
        {
            actionsOut[0] = 0;
        }
    }
}
