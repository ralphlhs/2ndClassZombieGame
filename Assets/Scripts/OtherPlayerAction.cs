using System.Security.Claims;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.PlayerSettings;

public class OtherPlayerAction : MonoBehaviour
{
    private bool isPicking = false;
    private bool isAim = false;
    private bool isDying = true;
    private bool isPause = false;

    public Transform itemGetPos;
    public GameObject PauseObj;
    public GameObject f_key;
    public GameObject AK;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        f_key.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Picking();
        ReloadDie();
    }



    private void Picking()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isPicking = !isPicking;
            animator.SetLayerWeight(1, 0); //첫번째 애니메이터 레이어를 활성화 해라.
            animator.SetTrigger("IsPick");
        }
    }



    private void ReloadDie()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isAim)
            {
                isAim = !isAim;
                animator.SetLayerWeight(1, 0); //첫번째 애니메이터 레이어를 활성화 해라.
                animator.SetTrigger("IsReload");
                SfxSound("Reload", transform.position, 1.0f);
            }
            else if (!isAim)
            {
                isAim = !isAim;
                animator.SetLayerWeight(1, 0); //첫번째 애니메이터 레이어를 활성화 해라.
                animator.SetTrigger("IsReload");
                SfxSound("Reload", transform.position, 1.0f);
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isDying)
            {
                isDying = !isDying;
                animator.SetLayerWeight(1, 0); //첫번째 애니메이터 레이어를 활성화 해라.
                animator.SetTrigger("IsDie");
                SfxSound("Dead", transform.position, 1.0f);
            }
            else isDying = !isDying;
        }
    }


    //=======================================
    public void ReGame()
    {
        //SfxSound("FootStep", transform.position, 0);
        PauseObj.SetActive(false);
        Time.timeScale = 1.0f; //게임시간 정지
        isPause = false;
    }

    private void Pause()
    {
        //SfxSound("FootStep", transform.position, 0f);
        PauseObj.SetActive(true);
        Time.timeScale = 0; //게임시간 정지
    }

    public void Exit()
    {
        //SfxSound("FootStep", transform.position, 0f);
        PauseObj.SetActive(false);
        Time.timeScale = 1;
        Application.Quit();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            f_key.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            f_key.SetActive(false);
        }
        
    }

    public void GKey()
    {
        f_key.SetActive(false);
    }

    private void SfxSound(string soundname, Vector3 pos, float spatial)
    {
        SoundController.Instance.PlaySfx(soundname, pos, spatial);
    }

}
