using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    public class CamMoving : MonoBehaviour
    {
        [Header("玩家目标")]
        public Transform target; // 玩家Transform

        [Header("摄像机偏移量")]
        public Vector3 offset = new Vector3(0, 5, -8); // 距离玩家的相对偏移

        [Header("摄像机平滑系数")]
        public float smoothSpeed = 5f;

        [Header("固定Y高度")]
        public float fixedY = 6.9f; // 摄像机固定的竖直高度

        private void Awake()
        {
            target = Player.Instance.transform;
        }

        private void FixedUpdate()
        {
            if (target == null) return;

            // 计算目标位置（跟随X、Z）
            Vector3 desiredPos = new Vector3(
                target.position.x + offset.x,
                fixedY,  // 修正Y高度
                target.position.z + offset.z
            );

            // 平滑插值移动
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);
        }
    }
}



