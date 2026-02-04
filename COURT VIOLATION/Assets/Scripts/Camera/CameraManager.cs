using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Cameras")]
    public Camera npcCamera;
    public Camera playerCamera;
    public Camera sceneCamera;

    public enum CamState { NPC, PLAYER, SCENE }

    CamState currentState;

    void Start()
    {
        SetCamera(CamState.SCENE); // start on wide shot
    }

    public void SetCamera(CamState state)
    {
      
        npcCamera.gameObject.SetActive(state == CamState.NPC);
        playerCamera.gameObject.SetActive(state == CamState.PLAYER);
        sceneCamera.gameObject.SetActive(state == CamState.SCENE);

    }
}
