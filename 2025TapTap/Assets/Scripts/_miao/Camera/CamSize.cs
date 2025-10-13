using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    public class CamSize : MonoBehaviour
    {
        [Header("���� GameManager �������")]
        public Camera targetCamera;
        private Camera main_Cam;

        [Header("���ֿ��Ʋ���")]
        [Tooltip("ÿ�ι���������Ӧ�� RT_Size �ı�ֵ")]
        public float scrollStep = 0.1f;

        [Header("�������")]
        public Transform player;  // ���Transform

        [Header("UIָ��")]
        public RectTransform offscreenIndicator; // UI��ͷ��Canvas�ڣ�

        [Header("��������")]
        public float autoAdjustSpeed = 2f; // �����RT_Size�����ٶ�
        public float maxRTSize = 24f;      // �����Ұ��С
        public float topThreshold = 0.95f; // �������Ļ��yֵ��������ֵ������Զ

        [Header("��Ļ��Եƫ��")]
        public float edgeOffset = 50f;

        // ������� Flag
        private bool wheelUp_Flag;
        private bool wheelDown_Flag;

        private void Start()
        {
            wheelUp_Flag = false;
            wheelDown_Flag = false;

            if (offscreenIndicator != null)
                offscreenIndicator.gameObject.SetActive(false);

            main_Cam = this.gameObject.GetComponent<Camera>();
        }

        void Update()
        {
            // ------------------ ���ֿ����߼� ------------------
            // ÿ֡���ñ�־
            ResetFlags();

            // ��ⰴ��
            DetectWheelDown();

            // ������
            DetectWheelKeep();
            if (GameManager.Instance == null || targetCamera == null) return;
            if (!GameManager.Instance.autoPixelControl) return;

            // ���� Flag ���� RT_Size
            if (wheelUp_Flag)
            {
                GameManager.Instance.Renderseter.RT_Size -= scrollStep;
            }
            if (wheelDown_Flag)
            {
                GameManager.Instance.Renderseter.RT_Size += scrollStep;
            }

            // ���Ʒ�Χ
            GameManager.Instance.Renderseter.RT_Size = Mathf.Clamp(GameManager.Instance.Renderseter.RT_Size, 0.1f, 24f);

            // ------------------ �Զ������߼� ------------------
            AutoAdjustSize();

            // Ӧ������
            GameManager.Instance.setCamSize();
        }

        private void AutoAdjustSize()
        {
            if (player == null)
            {
                Debug.LogWarning("δ��Player");
                return;
            }

            // �����λ��ת�����ӿڿռ䣨0~1��
            Vector3 viewPos = main_Cam.WorldToViewportPoint(player.position);
            float currentSize = GameManager.Instance.Renderseter.RT_Size;

            // �ж�����Ƿ񳬳���Ļ�����ⷽ��
            bool isOutOfView =
                viewPos.x < 0f || viewPos.x > 1f ||
                viewPos.y < 0f || viewPos.y > 1f ||
                viewPos.z < 0f; // �����������Ҳ�����

            if (isOutOfView)
            {
                //  �����Զ���Զ��Ұ�����ⷽ��
                if (currentSize < maxRTSize)
                {
                    GameManager.Instance.Renderseter.RT_Size = Mathf.Lerp(
                        currentSize,
                        Mathf.Min(maxRTSize, currentSize + 0.5f),
                        Time.deltaTime * autoAdjustSpeed
                    );
                }

                //  ��ʾUI��ͷ
                if (offscreenIndicator != null)
                {
                    offscreenIndicator.gameObject.SetActive(true);
                    UpdateOffscreenIndicator(viewPos);
                }
            }
            else
            {
                // ����ڻ�����������UI��ͷ
                if (offscreenIndicator != null)
                    offscreenIndicator.gameObject.SetActive(false);
            }
        }


        private void UpdateOffscreenIndicator(Vector3 viewPos)
        {
            if (offscreenIndicator == null) return;

            // �ӿ�����ת����Ļ����
            Vector2 screenPos = new Vector2(
                (viewPos.x - 0.5f) * Screen.width,
                (viewPos.y - 0.5f) * Screen.height
            );

            // ���Ƽ�ͷ����Ļ��Ե��Χ��
            float halfW = Screen.width * 0.5f - edgeOffset;
            float halfH = Screen.height * 0.5f - edgeOffset;
            Vector2 clampedPos = screenPos;
            clampedPos.x = Mathf.Clamp(clampedPos.x, -halfW, halfW);
            clampedPos.y = Mathf.Clamp(clampedPos.y, -halfH, halfH);

            offscreenIndicator.anchoredPosition = clampedPos;

            // ��ͷ��תָ����ҷ���
            Vector2 dir = screenPos.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            offscreenIndicator.rotation = Quaternion.Euler(0, 0, angle - 90);
        }



        #region �����ּ�ⷽ��

        private void ResetFlags()
        {
            wheelUp_Flag = false;
            wheelDown_Flag = false;
        }

        private void DetectWheelDown()
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll > 0f) wheelUp_Flag = true;
            if (scroll < 0f) wheelDown_Flag = true;
        }

        private void DetectWheelKeep()
        {
            // ���ֲ���Ҫ���������������ס���������ӿ��Է��Ժ���չ
        }

        #endregion
    }
}

