using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace miao
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioTrigger : MonoBehaviour
    {
        [Header("��Ƶ����")]
        public string audioName; // ���ŵ���Ƶ����
        public bool isSinglePlay = true; // �Ƿ񵥴β���
        public DetectionShape detectionShape = DetectionShape.Sphere; // ��ⷶΧ��״
        public float detectionRadius = 5f; // �뾶�����������κͽ����Σ�
        public Vector3 detectionSize = new Vector3(5f, 5f, 5f); // �����εĴ�С
        public LayerMask detectionLayer; // ���ڼ��Ĳ�
        public bool isEnabledAtStart = false; // ��ʼʱ�Ƿ�����

        private AudioSource audioSource;
        private Collider[] detectedObjects;

        // ���ڸ��ٽ������Χ������
        private HashSet<Collider> objectsInRange = new HashSet<Collider>();

        // �����ⷶΧ��״
        public enum DetectionShape
        {
            Sphere,
            Box,
            Capsule
        }

        private void Awake()
        {
            audioSource = gameObject.GetComponent<AudioSource>();  // ��ȡ AudioSource ���
        }

        private void Start()
        {
            objectsInRange.Clear(); // ��ռ���б�
            this.enabled = isEnabledAtStart; // ��ʼ״̬����
        }

        private void FixedUpdate()
        {
            if (!gameObject.activeSelf) return;

            // ����ѡ�����״���м��
            switch (detectionShape)
            {
                case DetectionShape.Sphere:
                    detectedObjects = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
                    break;

                case DetectionShape.Box:
                    detectedObjects = Physics.OverlapBox(transform.position, detectionSize / 2f, Quaternion.identity, detectionLayer);
                    break;

                case DetectionShape.Capsule:
                    detectedObjects = Physics.OverlapCapsule(transform.position - Vector3.up * detectionRadius / 2f, transform.position + Vector3.up * detectionRadius / 2f, detectionRadius / 2f, detectionLayer);
                    break;
            }

            // ������ǰ���м�⵽������
            foreach (var obj in detectedObjects)
            {
                if (obj.gameObject == this.gameObject) continue;  // �ų��Լ�

                // ��������Ƿ�����Ƶ�ű�
                AudioTrigger otherAudioTrigger = obj.GetComponent<AudioTrigger>();

                // �������û���������Ҳ��� HashSet �У�������Ч
                if (otherAudioTrigger != null && !objectsInRange.Contains(obj) && !audioSource.isPlaying)
                {
                    if (isSinglePlay)
                    {
                        AudioManager.Instance.PlayAudio(audioName, audioSource, true); // ����һ��
                    }
                    else
                    {
                        AudioManager.Instance.PlayAudio(audioName, audioSource, false); // ѭ������
                    }

                    // ��������� HashSet��������Ѿ����Ź���Ч
                    objectsInRange.Add(obj);
                }
            }

            // �����˳���ⷶΧ������
            HashSet<Collider> objectsToRemove = new HashSet<Collider>();
            foreach (var obj in objectsInRange)
            {
                bool isStillInRange = false;

                // ��������Ƿ��ڷ�Χ��
                foreach (var detectedObj in detectedObjects)
                {
                    if (detectedObj == obj)
                    {
                        isStillInRange = true;
                        break;
                    }
                }

                // ������岻�ڷ�Χ�ڣ��Ƴ���
                if (!isStillInRange)
                {
                    objectsToRemove.Add(obj);
                }
            }

            // �� HashSet ���Ƴ���Щ�˳���Χ������
            foreach (var obj in objectsToRemove)
            {
                objectsInRange.Remove(obj);
            }
        }

        private IEnumerator DisableAfterPlay()
        {
            yield return new WaitForSeconds(audioSource.clip.length);
            gameObject.SetActive(false);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            // ����ѡ�����״������Ӧ�ļ�ⷶΧ
            switch (detectionShape)
            {
                case DetectionShape.Sphere:
                    Gizmos.DrawWireSphere(transform.position, detectionRadius);
                    break;

                case DetectionShape.Box:
                    Gizmos.DrawWireCube(transform.position, detectionSize);
                    break;

                case DetectionShape.Capsule:
                    Gizmos.DrawWireSphere(transform.position - Vector3.up * detectionRadius / 2f, detectionRadius / 2f);
                    Gizmos.DrawWireSphere(transform.position + Vector3.up * detectionRadius / 2f, detectionRadius / 2f);
                    Gizmos.DrawLine(transform.position - Vector3.up * detectionRadius / 2f, transform.position + Vector3.up * detectionRadius / 2f);
                    break;
            }
        }
    }
}
