using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentControllerLRFB : Agent
{
    // MLAgents 이벤트 파라미터 불러오기
    public EnvironmentParameters environmentParameters;

    public RocketControllerLRFB rc; //로켓 컨트롤 연동하는  코드
    public bool episodeFinished = false; // 에피소드는 비활성화로 실행.

    public override void Initialize()
    {
         // 로켓런처 구성 요소 가져오기.
        rc = GetComponent<RocketControllerLRFB>();
    }

    public override void OnEpisodeBegin()
    {
        rc.ResetRocket();
        // 에피소드 완료시 비활성화
        episodeFinished = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 rocketPosition = rc.transform.localPosition; //로켓 로컬 위치
        Vector3 rocketRotation = rc.transform.localRotation.eulerAngles; // 로켓 로컬 각도

        Vector3 rocketVelocity = rc.rb.velocity; // 로켓.물리엔진.속도
        Vector3 rocketAngularVelocity = rc.rb.angularVelocity;
 
        sensor.AddObservation(rocketPosition.x); // 로켓 로컬 x 위치
        sensor.AddObservation(rocketPosition.y); // 로켓 로컬 y 위치
        sensor.AddObservation(rocketPosition.z); // 로켓 로컬 z 위치

        sensor.AddObservation(rocketRotation.x); // 로켓 로컬 x 회전
        sensor.AddObservation(rocketRotation.y); // 로켓 로컬 y 회전
        sensor.AddObservation(rocketRotation.z); // 로켓 로컬 z 회전

        sensor.AddObservation(rocketVelocity.x); // 로켓 로컬 x 속도
        sensor.AddObservation(rocketVelocity.y); // 로켓 로컬 y 속도
        sensor.AddObservation(rocketVelocity.z); // 로켓 로컬 z 속도

        sensor.AddObservation(rocketAngularVelocity.x); // 로켓 로컬 x 각도
        sensor.AddObservation(rocketAngularVelocity.y); // 로켓 로컬 y 각도
        sensor.AddObservation(rocketAngularVelocity.z); // 로켓 로컬 z 각도
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        rc.SetMainEngine(actionBuffers.DiscreteActions[0]); // 엔진(킨다, 끈다)
        rc.SetLeftEngine(actionBuffers.DiscreteActions[1]); // 엔진(킨다, 끈다)
        rc.SetRightEngine(actionBuffers.DiscreteActions[2]); // 엔진(킨다, 끈다)
        rc.SetForwardEngine(actionBuffers.DiscreteActions[3]); // 엔진(킨다, 끈다)
        rc.SetBackwardEngine(actionBuffers.DiscreteActions[4]); // 엔진(킨다, 끈다)
    }

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

        if (Input.GetKey(KeyCode.A))
        {
            actionsOut[1] = 1;
        }
        else
        {
            actionsOut[1] = 0;
        }

        if (Input.GetKey(KeyCode.D))
        {
            actionsOut[2] = 1;
        }
        else
        {
            actionsOut[2] = 0;
        }

        if (Input.GetKey(KeyCode.W))
        {
            actionsOut[3] = 1;
        }
        else
        {
            actionsOut[3] = 0;
        }

        if (Input.GetKey(KeyCode.S))
        {
            actionsOut[4] = 1;
        }
        else
        {
            actionsOut[4] = 0;
        }
    }
}
