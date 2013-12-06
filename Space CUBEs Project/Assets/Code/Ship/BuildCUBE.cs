// Steve Yeager
// 

using UnityEngine;

public class BuildCUBE
{
    private readonly Transform transform;
    private readonly Vector3 localTarget;
    private float time;
    private readonly float speed;
    public readonly Vector3 vector;
    private bool done;


    public BuildCUBE(Transform transform, Vector3 localTarget, float speed)
    {
        this.transform = transform;
        this.localTarget = localTarget;
        this.speed = speed;

        vector = localTarget - transform.localPosition;
        time = vector.magnitude / speed;
        vector.Normalize();
    }


    public void Update(float deltaTime)
    {
        if (done) return;
        time -= deltaTime;
        if (time <= 0f || Vector3.Distance(transform.localPosition, localTarget) <= 1f)
        {
            done = true;
            transform.localPosition = localTarget;
        }
        else
        {
            transform.localPosition += vector * speed * deltaTime;
        }
    }
}