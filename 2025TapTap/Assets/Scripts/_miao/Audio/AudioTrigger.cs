using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace miao
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioTrigger : MonoBehaviour
    {
        [Header("��Ƶ����")]
        public bool isSinglePlay = true; // �Ƿ񵥴β���
        public DetectionShape detectionShape = DetectionShape.Sphere; // ��ⷶΧ��״
        public float detectionRadius = 5f; // �뾶�����������κͽ����Σ�
        public Vector3 detectionSize = new Vector3(5f, 5f, 5f); // �����εĴ�С
        public LayerMask detectionLayer; // ���ڼ��Ĳ�
        public bool isEnabledAtStart = false; // ��ʼʱ�Ƿ�����

        private AudioSource audioSource;
        private Collider[] detectedObjects;

        private HashSet<Collider> objectsInRange = new HashSet<Collider>();

        public enum DetectionShape
        {
            Sphere,
            Box,
            Capsule
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;

            // ���� 3D �ռ���Ч
            audioSource.spatialBlend = 1f; // 1 = ��ȫ3D��0 = ��ȫ2D
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic; // ʹ�ö���˥��
            audioSource.minDistance = 1f; // ������벻˥��
            audioSource.maxDistance = 20f; // ��Զ��������˥����0
        }

        private void Start()
        {
            objectsInRange.Clear();
            this.enabled = isEnabledAtStart;
        }

        private void FixedUpdate()
        {
            if (!gameObject.activeSelf || SceneManager.GetActiveScene().name == "Title") return;

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

            foreach (var obj in detectedObjects)
            {
                if (obj.gameObject == this.gameObject || obj.gameObject == this.gameObject.transform.parent) continue;

                // ���Ŀ������������Ƿ��� AudioTrigger
                AudioTrigger otherAudioTrigger = obj.GetComponentInChildren<AudioTrigger>();
                if (otherAudioTrigger == null) continue;

                if (!objectsInRange.Contains(obj))
                {
                    // �����Լ����ϵ� AudioSource
                    if (audioSource != null && !audioSource.isPlaying)
                    {
                        if (isSinglePlay)
                        {
                            audioSource.PlayOneShot(audioSource.clip);
                        }
                        else
                        {
                            audioSource.loop = true;
                            audioSource.Play();
                        }
                    }

                    objectsInRange.Add(obj);
                }
            }

            // �Ƴ��˳���Χ������
            HashSet<Collider> objectsToRemove = new HashSet<Collider>();
            foreach (var obj in objectsInRange)
            {
                bool isStillInRange = false;
                foreach (var detectedObj in detectedObjects)
                {
                    if (detectedObj == obj)
                    {
                        isStillInRange = true;
                        break;
                    }
                }

                if (!isStillInRange)
                {
                    objectsToRemove.Add(obj);
                }
            }

            foreach (var obj in objectsToRemove)
            {
                objectsInRange.Remove(obj);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

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
