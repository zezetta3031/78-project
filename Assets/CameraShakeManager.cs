using UnityEngine;
using Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager instance;
    [SerializeField] private float globalShakeForce = 1f;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void CameraShake(CinemachineImpulseSource source, float force)
    {
        source.GenerateImpulseWithForce(force);
    }
}
