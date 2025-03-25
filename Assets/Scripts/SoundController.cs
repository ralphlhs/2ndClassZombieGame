using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void PlaySfx(string name, Vector3 position, float spatialBlend_2d3d)
    {

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
