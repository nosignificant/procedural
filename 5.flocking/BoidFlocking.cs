using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Boid))]
public class BoidFlocking : MonoBehaviour
{
    private Boid boid;

    public Transform target;

    [Header("Settings")]
    public float neighborRadius = 5f; // 이웃 인식 범위
    public float separationRadius = 2.0f; // 충돌 회피 범위

    public float boundaryRadius = 2.0f; // 감속 범위

    [Header("Weights")]
    public float alignmentWeight = 1.0f;
    public float cohesionWeight = 1.0f;
    public float separationWeight = 1.5f;
    public float boundaryWeight = 2.0f;
    public float noiseWeight = 0.5f;
    public float noiseScale = 1.0f;
    public float maxSteerForce = 10.0f;
    private List<Boid> neighbors = new List<Boid>();

    [Header("draw")]
    public LineRender line;


    void Start()
    {
        boid = GetComponent<Boid>();
        target = GameObject.Find("Target").transform;
        line = GetComponent<LineRender>();
    }

    void Update()
    {
        FindNeighbors();

        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();
        Vector3 separation = CalculateSeparation();
        Vector3 Arrive = KeepInBounds();
        Vector3 randomMove = CalculateNoise();

        Vector3 steeringForce = (alignment * alignmentWeight) +
                                (cohesion * cohesionWeight) +
                                (separation * separationWeight) +
                                (randomMove * noiseWeight) +
                                (Arrive * boundaryWeight);

        Vector3 clampedSteering = Vector3.ClampMagnitude(steeringForce, maxSteerForce);
        boid.velocity += clampedSteering * Time.deltaTime;

        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist > boundaryRadius)
                boid.velocity = Vector3.Lerp(boid.velocity, Vector3.zero, Time.deltaTime);
        }

    }

    void LateUpdate()
    {
        if (line == null) return;

        Transform[] neighborTransforms = new Transform[neighbors.Count];

        for (int i = 0; i < neighbors.Count; i++)
            neighborTransforms[i] = neighbors[i].transform;

        line.Draw(neighborTransforms);

    }

    void FindNeighbors()
    {
        neighbors.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, neighborRadius);

        foreach (Collider col in colliders)
        {
            if (col.transform == transform) continue;

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

    private Vector3 CalculateCohesion()
    {
        if (neighbors.Count == 0) return Vector3.zero;

        Vector3 centerOfMass = Vector3.zero;
        foreach (Boid neighbor in neighbors)
            centerOfMass += neighbor.transform.position;

        centerOfMass /= neighbors.Count;

        Vector3 targetDir = centerOfMass - transform.position;
        Vector3 desiredVelocity = targetDir.normalized * boid.maxVelocity;

        return desiredVelocity - boid.velocity;
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
            {
                if (dist > 0.01f)
                {
                    // 멀어지는 방향 * 1/거리(거리에 반비례해서 힘이 강해짐)
                    separationForce += awayDir.normalized / dist;
                }
            }

        }
        return separationForce;
    }
    private Vector3 KeepInBounds()
    {
        if (target == null) return Vector3.zero;

        Vector3 centerOffset = target.position - transform.position;
        float dist = centerOffset.magnitude;

        if (dist > boundaryRadius)
            return centerOffset.normalized * boid.maxVelocity;

        return -boid.velocity * dist * dist;
    }

    private Vector3 CalculateNoise()
    {
        float idOffset = boid.GetInstanceID() * 0.1f;

        float xNoise = Mathf.PerlinNoise(Time.time * noiseScale + idOffset, 0);
        float yNoise = Mathf.PerlinNoise(Time.time * noiseScale + idOffset, 100);
        float zNoise = Mathf.PerlinNoise(Time.time * noiseScale + idOffset, 200);

        Vector3 noiseDir = new Vector3(xNoise, yNoise, zNoise);

        return noiseDir.normalized;
    }
}