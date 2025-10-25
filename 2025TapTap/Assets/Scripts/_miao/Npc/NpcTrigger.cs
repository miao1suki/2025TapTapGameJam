using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    public class NpcTrigger : MonoBehaviour
    {
        private Animator animator;
        private AudioSource audioSource;
        [SerializeField] int attack = 50;
        void Start()
        {
            animator = gameObject.GetComponent<Animator>();
            audioSource = gameObject.GetComponent<AudioSource>();

        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && animator)
            {
                animator.SetTrigger("PlayerEnter");
                ScoreTrigger.Instance.AddScore("Äã¾ªÈÅÁË" + gameObject.name, 500);
                Player.Instance.ChangePlayerHealth(-attack);
                Player.Instance.GetComponent<PlayerMoving>().CheckPlayerHealth();
            }

            if(other.CompareTag("Player") && audioSource)
            {
                audioSource.Play();
            }


        }
    }

}

