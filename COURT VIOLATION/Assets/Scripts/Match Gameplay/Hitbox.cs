using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField]
    NewCharacterStats stats;
    public ShotType shotType;
    public float launchVel;
    public List<Collider> hitAreas;
    public delegate void Hit();
    public event Hit OnHit;

    public string user;

    void Awake()
    {
        user = transform.root.gameObject.name;
    }

    void Update() { }

    public Collider SetActiveHitArea()
    {
        switch (shotType)
        {
            case ShotType.BasicShot:
                return hitAreas[0];
            case ShotType.Lob:
                return hitAreas[1];
            case ShotType.DropShot:
                return hitAreas[2];
            default:
                return hitAreas[0];
        }
    }

    public void Launch(Rigidbody rigidbody, Vector3 launchAngle, float launchVel)
    {
        rigidbody.linearVelocity = launchAngle.normalized * launchVel;
        OnHit?.Invoke();
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
        Collider hitArea,
        float pitchStep = 1f,
        float yawStep = 1f,
        int maxIterations = 360
    )
    {
        float xMin = hitArea.bounds.min.x;
        float xMax = hitArea.bounds.max.x;
        float zMin = hitArea.bounds.min.z;
        float zMax = hitArea.bounds.max.z;

        Debug.Log("===== START AdjustLaunchAngle =====");

        Debug.Log($"Target Bounds | X: [{xMin}, {xMax}] | Z: [{zMin}, {zMax}]");

        Debug.Log(
            $"Initial Forward: {hitbox.forward} | " + $"Initial Rotation: {hitbox.eulerAngles}"
        );

        for (int i = 0; i < maxIterations; i++)
        {
            Vector3 landing = PredictLandingPoint(rb);

            Debug.Log(
                $"ITERATION {i}\n"
                    + $"Landing: {landing}\n"
                    + $"Forward: {hitbox.forward}\n"
                    + $"Rotation: {hitbox.eulerAngles}\n"
                    + $"Velocity: {rb.linearVelocity}\n"
            );

            int distanceSign = 0;

            bool insideBounds =
                xMin <= landing.x && landing.x <= xMax && zMin <= landing.z && landing.z <= zMax;

            if (insideBounds)
            {
                Debug.Log(
                    $"SUCCESS after {i} iterations\n"
                        + $"Final Landing: {landing}\n"
                        + $"Final Rotation: {hitbox.eulerAngles}"
                );

                return;
            }

            // DEPTH CHECK
            if (landing.z < zMin)
            {
                distanceSign = -1;

                Debug.Log($"Too SHORT | landing.z {landing.z} < zMin {zMin}");
            }
            else if (landing.z > zMax)
            {
                distanceSign = 1;

                Debug.Log($"Too FAR | landing.z {landing.z} > zMax {zMax}");
            }

            // PITCH ADJUSTMENT
            if (distanceSign != 0)
            {
                int shotSign = (shotType == ShotType.Lob) ? -1 : 1;

                float rotationAmount = distanceSign * shotSign * pitchStep;

                Debug.Log($"Applying PITCH rotation: {rotationAmount}");

                hitbox.Rotate(Vector3.right, rotationAmount, Space.Self);
            }

            // LEFT/RIGHT ADJUSTMENT
            if (landing.x < xMin)
            {
                Debug.Log($"Too LEFT | landing.x {landing.x} < xMin {xMin}");

                hitbox.Rotate(Vector3.up, yawStep, Space.Self);
            }
            else if (landing.x > xMax)
            {
                Debug.Log($"Too RIGHT | landing.x {landing.x} > xMax {xMax}");

                hitbox.Rotate(Vector3.up, -yawStep, Space.Self);
            }

            Vector3 newDir = hitbox.forward;
            float speed = rb.linearVelocity.magnitude;

            rb.linearVelocity = newDir * speed;
        }

        Debug.LogWarning("FAILED: Could not find valid landing within limits.");

        Debug.Log($"Final Rotation: {hitbox.eulerAngles}");
    }

    public ShotData CreateShotData(Transform hitboxTransform)
    {
        return new ShotData
        {
            shotType = shotType,
            direction = hitboxTransform.forward,
            velocity = launchVel,
            user = user,
        };
    }
}
