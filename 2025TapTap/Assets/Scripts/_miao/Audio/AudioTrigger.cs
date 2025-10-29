using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace miao
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioTrigger : MonoBehaviour
    {
        [Header("音频设置")]
        public bool isSinglePlay = true; // 是否单次播放
        public DetectionShape detectionShape = DetectionShape.Sphere; // 检测范围形状
        public float detectionRadius = 5f; // 半径（适用于球形和胶囊形）
        public Vector3 detectionSize = new Vector3(5f, 5f, 5f); // 盒子形的大小
        public LayerMask detectionLayer; // 用于检测的层
        public bool isEnabledAtStart = false; // 初始时是否启用

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

            // 开启 3D 空间音效
            audioSource.spatialBlend = 1f; // 1 = 完全3D，0 = 完全2D
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic; // 使用对数衰减
            audioSource.minDistance = 1f; // 最近距离不衰减
            audioSource.maxDistance = 20f; // 最远距离声音衰减到0
        }

        private void Start()
        {
            objectsInRange.Clear();
            this.enabled = isEnabledAtStart;
        }

        private void FixedUpdate()
        {
            if (!gameObject.activeSelf || SceneManager.GetActiveScene().name == "Title") return;

            // 根据选择的形状进行检测
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

                // 检查目标或子物体上是否有 AudioTrigger
                AudioTrigger otherAudioTrigger = obj.GetComponentInChildren<AudioTrigger>();
                if (otherAudioTrigger == null) continue;

                if (!objectsInRange.Contains(obj))
                {
                    // 播放自己身上的 AudioSource
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

            // 移除退出范围的物体
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
