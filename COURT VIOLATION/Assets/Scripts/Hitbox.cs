using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public bool dynamicHitbox;

    public float baseVel,
        launchVel,
        maxVel;
    public Transform hitArea;

    public string user;

    void Start()
    {
        user = transform.root.gameObject.name;
    }

    void Update() { }

    public void Launch(Rigidbody rigidbody, Vector3 launchAngle, float launchVel)
    {
        rigidbody.linearVelocity = launchAngle.normalized * launchVel;
    }

    public Vector3 PredictLandingPoint(Rigidbody rb)
    {
        Vector3 p0 = rb.position;
        Vector3 v0 = transform.forward * launchVel;
        Vector3 g = Physics.gravity;

        // Solve for time when y = 0
        float a = 0.5f * g.y;
        float b = v0.y;
        float c = p0.y;

        float discriminant = b * b - 4f * a * c;
        if (discriminant < 0f)
            return Vector3.positiveInfinity; // never lands

        float t = (-b - Mathf.Sqrt(discriminant)) / (2f * a);
        if (t < 0f)
            t = (-b + Mathf.Sqrt(discriminant)) / (2f * a);

        return p0 + v0 * t + 0.5f * t * t * g;
    }

    public void AdjustLaunchAngle(
        Rigidbody rb,
        Transform hitbox,
        float xMin,
        float xMax,
        float zMin,
        float zMax,
        float pitchStep = 1f,
        float yawStep = 1f,
        int maxIterations = 90
    )
    {
        for (int i = 0; i < maxIterations; i++)
        {
            Vector3 landing = PredictLandingPoint(rb);

            // Check bounds
            if (xMin <= landing.x && landing.x <= xMax && zMin <= landing.z && landing.z <= zMax)
            {
                Debug.Log($"Valid landing found after {i} iterations at {landing}");
                return;
            }

            // Adjust pitch (up/down) based on Z distance (forward axis)
            if (landing.z < zMin)
                hitbox.Rotate(Vector3.right, -pitchStep, Space.Self); // tilt up (increase range)
            else if (landing.z > zMax)
                hitbox.Rotate(Vector3.right, pitchStep, Space.Self); // tilt down (reduce range)

            // Adjust yaw (left/right) based on X distance
            if (landing.x < xMin)
                hitbox.Rotate(Vector3.up, yawStep, Space.Self); // rotate left
            else if (landing.x > xMax)
                hitbox.Rotate(Vector3.up, -yawStep, Space.Self); // rotate right

            // Update velocity to match new direction
            Vector3 newDir = hitbox.forward;
            float speed = rb.linearVelocity.magnitude;
            rb.linearVelocity = newDir * speed;
        }

        Debug.LogWarning("Could not find valid landing within limits.");
    }
}
