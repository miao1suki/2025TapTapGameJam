using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace miao
{
    public class GroundMarker : MonoBehaviour
    {
        [SerializeField]
        private float heightAboveObject = 0.1f; // 标记物体与物体表面之间的高度偏移

        void Update()
        {
            RaycastHit hit;
            // 使用射线从玩家位置向下检测
            if (Physics.Raycast(Player.Instance.transform.position, Vector3.down, out hit, Mathf.Infinity))
            {
                // 更新标记物体的位置，确保它位于物体上方一点
                Vector3 targetPosition = hit.point;
                targetPosition.y += heightAboveObject; // 将y坐标调高一点
                this.transform.position = targetPosition; // 更新标记物体位置
            }
        }
    }
}



