using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace miao
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioTrigger : MonoBehaviour
    {
        [Header("音频设置")]
        public string audioName; // 播放的音频名称
        public bool isSinglePlay = true; // 是否单次播放
        public DetectionShape detectionShape = DetectionShape.Sphere; // 检测范围形状
        public float detectionRadius = 5f; // 半径（适用于球形和胶囊形）
        public Vector3 detectionSize = new Vector3(5f, 5f, 5f); // 盒子形的大小
        public LayerMask detectionLayer; // 用于检测的层
        public bool isEnabledAtStart = false; // 初始时是否启用

        private AudioSource audioSource;
        private Collider[] detectedObjects;

        // 用于跟踪进入过范围的物体
        private HashSet<Collider> objectsInRange = new HashSet<Collider>();

        // 定义检测范围形状
        public enum DetectionShape
        {
            Sphere,
            Box,
            Capsule
        }

        private void Awake()
        {
            audioSource = gameObject.GetComponent<AudioSource>();  // 获取 AudioSource 组件
        }

        private void Start()
        {
            objectsInRange.Clear(); // 清空检测列表
            this.enabled = isEnabledAtStart; // 初始状态禁用
        }

        private void FixedUpdate()
        {
            if (!gameObject.activeSelf) return;

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

            // 遍历当前所有检测到的物体
            foreach (var obj in detectedObjects)
            {
                if (obj.gameObject == this.gameObject) continue;  // 排除自己

                // 检查物体是否有音频脚本
                AudioTrigger otherAudioTrigger = obj.GetComponent<AudioTrigger>();

                // 如果物体没被检测过并且不在 HashSet 中，播放音效
                if (otherAudioTrigger != null && !objectsInRange.Contains(obj) && !audioSource.isPlaying)
                {
                    if (isSinglePlay)
                    {
                        AudioManager.Instance.PlayAudio(audioName, audioSource, true); // 播放一次
                    }
                    else
                    {
                        AudioManager.Instance.PlayAudio(audioName, audioSource, false); // 循环播放
                    }

                    // 将物体加入 HashSet，标记它已经播放过音效
                    objectsInRange.Add(obj);
                }
            }

            // 遍历退出检测范围的物体
            HashSet<Collider> objectsToRemove = new HashSet<Collider>();
            foreach (var obj in objectsInRange)
            {
                bool isStillInRange = false;

                // 检查物体是否还在范围内
                foreach (var detectedObj in detectedObjects)
                {
                    if (detectedObj == obj)
                    {
                        isStillInRange = true;
                        break;
                    }
                }

                // 如果物体不在范围内，移除它
                if (!isStillInRange)
                {
                    objectsToRemove.Add(obj);
                }
            }

            // 从 HashSet 中移除那些退出范围的物体
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

            // 根据选择的形状绘制相应的检测范围
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
