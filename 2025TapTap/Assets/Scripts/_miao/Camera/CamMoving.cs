using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    public class CamMoving : MonoBehaviour
    {
        [Header("���Ŀ��")]
        public Transform target; // ���Transform

        [Header("�����ƫ����")]
        public Vector3 offset = new Vector3(0, 5, -8); // ������ҵ����ƫ��

        [Header("�����ƽ��ϵ��")]
        public float smoothSpeed = 5f;

        [Header("�̶�Y�߶�")]
        public float fixedY = 6.9f; // ������̶�����ֱ�߶�

        private void Awake()
        {
            target = Player.Instance.transform;
        }

        private void FixedUpdate()
        {
            if (target == null) return;

            // ����Ŀ��λ�ã�����X��Z��
            Vector3 desiredPos = new Vector3(
                target.position.x + offset.x,
                fixedY,  // ����Y�߶�
                target.position.z + offset.z
            );

            // ƽ����ֵ�ƶ�
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);
        }
    }
}



