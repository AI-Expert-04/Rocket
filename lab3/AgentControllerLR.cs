using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class AgentControllerLR : Agent
{
    // MLAgents 이벤트 파라미터 불러오기
    public BehaviorParameters behaviorParameters;
    public EnvironmentParameters environmentParameters;
    public RocketControllerLR rc; //로켓 컨트롤 연동하는  코드
    public bool episodeFinished = false; // 에피소드는 비활성화로 실행.

    public override void Initialize()
    {
        // 로켓런처 구성 요소 가져오기.
        rc = GetComponent<RocketControllerLR>();
        // 파라미터 구성 요소 가져오기.
        behaviorParameters = GetComponent<BehaviorParameters>();
    }

    public override void OnEpisodeBegin()
    {
        if (behaviorParameters.Model == null)
        {
            // 파라미터 불러오기.
            environmentParameters = Academy.Instance.EnvironmentParameters;
            // 초기높이 10
            rc.initHeight = environmentParameters.GetWithDefault("init_height", 10);
            // 초기회전각도 0 
            rc.rotationRange = environmentParameters.GetWithDefault("rotation_range", 0);
        }
        rc.ResetRocket();
        // 에피소드 완료시 비활성화
        episodeFinished = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 rocketPosition = rc.transform.localPosition; //로켓 로컬위치
        Vector3 rocketRotation = rc.transform.localRotation.eulerAngles; // 코렛 로컬 각도

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
    }
}
