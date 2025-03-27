using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    private Dictionary<ParticleType, GameObject> particleSystemDic
    = new Dictionary<ParticleType, GameObject>();

    private Dictionary<ParticleType, Queue<GameObject>> particlePools
        = new Dictionary<ParticleType, Queue<GameObject>>();

    public GameObject weaponExplosionParticle;
    public GameObject weaponFireParticle;
    public GameObject weaponSmokeParticle;

    public int poolSize = 30;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        particleSystemDic.Add(ParticleType.DamageExplosion, weaponExplosionParticle);
        particleSystemDic.Add(ParticleType.WeaponFire, weaponFireParticle);
        particleSystemDic.Add(ParticleType.Smoke, weaponSmokeParticle);

        foreach (var type in particleSystemDic.Keys)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(particleSystemDic[type]);
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
            }
        }
    }
    public void ParticlePlay(ParticleType type, Vector3 position, Vector3 scale)
    {
        if (particlePools.ContainsKey(type))
        {
            GameObject particleObj = particlePools[type].Dequeue();
            if (particleObj != null)
            {
                particleObj.transform.position = position;
                ParticleSystem particleSystem = particleObj.GetComponentInChildren<ParticleSystem>();

                if (particleSystem.isPlaying)
                {
                    particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
                particleObj.transform.localScale = scale;
                particleObj.SetActive(true);
                particleSystem.Play();
                StartCoroutine(particleEnd(type, particleObj, particleSystem));
            }

        }

        IEnumerator particleEnd(ParticleType type, GameObject particleObj, ParticleSystem particleSystem)
        {
            while (particleSystem.isPlaying)
            {
                yield return null;
            }
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleObj.SetActive(false);
            particlePools[type].Enqueue(particleObj);
        }
    }

    public enum ParticleType
    {
        DamageExplosion,
        WeaponFire,
        Smoke,
    }
}