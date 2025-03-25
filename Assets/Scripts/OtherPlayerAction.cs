using System.Security.Claims;
using UnityEngine;

public class OtherPlayerAction : MonoBehaviour
{
    private bool isPicking = false;
    private bool isAim = false;
    private bool isDying = false;
    private bool isPause = false;

    public Transform itemGetPos;
    public GameObject PauseObj;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Picking();
        ReloadDie();
    }



    private void Picking()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isPicking = !isPicking;
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
                animator.SetTrigger("IsReload");
            }
            else if (!isAim)
            {
                isAim = !isAim;
                animator.SetTrigger("IsReload");
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {

            isDying = !isDying;
            animator.SetTrigger("IsDie");
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


}
