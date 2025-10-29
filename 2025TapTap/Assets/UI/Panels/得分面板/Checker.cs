using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AchievementSystem;
using UnityEngine.InputSystem;
using miao;
using UnityEngine.SceneManagement;

public class Checker : MonoBehaviour
{
    [SerializeField] public Achievement achievement1;
    [SerializeField] public Achievement achievement2;
    [SerializeField] public Achievement achievement3;
    [SerializeField] public Achievement achievement4;
    [SerializeField] public Achievement achievement5;
    [SerializeField] public Achievement achievement6;
    [SerializeField] public Achievement achievement7;
    [SerializeField] public Achievement achievement8;
    [SerializeField] public Achievement achievement9;
    [SerializeField] public Achievement achievement10;
    public static Checker Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(Player.Instance.transform.position.y >= 100 && !AchievementManager.Ins.IsCompleted(achievement10))
        {
            Done(achievement10);
        }

        if(Input.GetKeyDown(KeyCode.Space) && !AchievementManager.Ins.IsCompleted(achievement2))
        {
            Done(achievement2);
        }
    }

    public void Done(Achievement achievement)
    {
        if (SceneManager.GetActiveScene().name == "Title") return;

        AchievementManager.Enable(achievement);
        AudioManager.Instance.PlayAudio("成就完成时的奖励音效（欢快）",Player.Instance.transform.position,false,0.9f);
    }
}
