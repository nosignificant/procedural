using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Boid))]
public class BoidFlocking : MonoBehaviour
{
    private Boid boid;


    [Header("Settings")]
    public float neighborRadius = 5f; // 이웃 인식 범위
    public float separationRadius = 2.0f; // 충돌 회피 범위

    [Header("Weights")]
    public float alignmentWeight = 1.0f;
    public float cohesionWeight = 1.0f;
    public float separationWeight = 1.5f;
    public float boundaryWeight = 1.0f;

    private List<Boid> neighbors = new List<Boid>();

    void Start()
    {
        boid = GetComponent<Boid>();
    }

    void Update()
    {
        FindNeighbors();

        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();
        Vector3 separation = CalculateSeparation();
        Vector3 follow = followTarget();

        Vector3 steeringForce = (alignment * alignmentWeight) +
                                (cohesion * cohesionWeight) +
                                (separation * separationWeight) +
                                (follow * boundaryWeight);

        boid.velocity += steeringForce * Time.deltaTime;
    }

    void FindNeighbors()
    {
        neighbors.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, neighborRadius);

        foreach (Collider col in colliders)
        {
            if (col.transform == transform) return;

            Boid neighborBoid = col.GetComponent<Boid>();
            if (neighborBoid != null)
                neighbors.Add(neighborBoid);
        }
    }

    // 군집의 평균 속도(방향 * 속력)를 따라감 
    private Vector3 CalculateAlignment()
    {
        if (neighbors.Count == 0) return Vector3.zero;

        Vector3 averageVelocity = Vector3.zero;

        foreach (Boid neighbor in neighbors)
            averageVelocity += neighbor.velocity;

        averageVelocity /= neighbors.Count;
        //현재 속도 + 필요한 힘 = 목표 속도이므로 , 필요한 힘을 구하려면 목표 속도 - 현재 속도
        return (averageVelocity - boid.velocity).normalized;
    }

    //군집의 중심 위치로 모이려고 함: 중심 위치를 구하고 그 방향으로 다가감
    private Vector3 CalculateCohesion()
    {
        if (neighbors.Count == 0) return Vector3.zero;

        Vector3 centerOfMass = Vector3.zero;
        //중심 위치는 각 이웃 위치 합의 평균
        foreach (Boid neighbor in neighbors)
            centerOfMass += neighbor.transform.position;

        centerOfMass /= neighbors.Count;
        //중심으로 향하기
        //중심에서부터 멀리있으면 가까이 가려는 힘이 커질테니 그에 대한 영향을 받지 않기 위해 normalize 해줌 
        return (centerOfMass - transform.position).normalized;
    }

    //너무 많이 다가가면 멀어지게 함
    private Vector3 CalculateSeparation()
    {
        Vector3 separationForce = Vector3.zero;

        foreach (Boid neighbor in neighbors)
        {
            Vector3 awayDir = transform.position - neighbor.transform.position;
            //방향을 magnitude하면 거리가 나옴
            float dist = awayDir.magnitude;
            //separationRadius 보다 가까이 있으면 밀어내는 힘을 더함
            if (dist < separationRadius && dist > 0)
                // 멀어지는 방향 * 1/거리(거리에 반비례해서 힘이 강해짐)
                separationForce += awayDir.normalized / dist;
        }
        return separationForce;
    }

    private Vector3 followTarget()
    {
        if (boid.target == null) return Vector3.zero;
        Vector3 offset = boid.target.position - transform.position;
        float dist = offset.magnitude;

        if (dist > 10f)
            return offset.normalized;
        return Vector3.zero;
    }
}