using UnityEngine;

public class SpriteBillboarding : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
