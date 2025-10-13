using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace miao
{
    public class GroundMarker : MonoBehaviour
    {
        [SerializeField]
        private float heightAboveObject = 0.1f; // ����������������֮��ĸ߶�ƫ��

        void Update()
        {
            RaycastHit hit;
            // ʹ�����ߴ����λ�����¼��
            if (Physics.Raycast(Player.Instance.transform.position, Vector3.down, out hit, Mathf.Infinity))
            {
                // ���±�������λ�ã�ȷ����λ�������Ϸ�һ��
                Vector3 targetPosition = hit.point;
                targetPosition.y += heightAboveObject; // ��y�������һ��
                this.transform.position = targetPosition; // ���±������λ��
            }
        }
    }
}



