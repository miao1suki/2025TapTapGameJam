

using UnityEngine;
/**************************************************************************
����: HuHu
����: 3112891874@qq.com
����: �����������࣬����InputSystem
**************************************************************************/
public class InputService : MonoSingleton<InputService>
{
    public InputMap inputMap;
    protected override void Awake()
    {
        base.Awake();
        if (inputMap == null)
        {
            inputMap = new InputMap();
        }
        inputMap.Enable();
    }
    private void OnDestroy()
    {
        inputMap.Disable();
    }

    /// <summary>
    /// ��װ�ߺ����Լ�PC�Ͱ�׿��������ƶ���
    /// </summary>
    public Vector2 GetMoveHorizontalValue
    {
        get
        {
            //��׿
#if UNITY_ANDROID
                return inputMap.Player.Move.ReadValue<Vector2>();
            //�ǰ�׿
#elif !UNITY_ANDROID
            Vector2 dir = inputMap.Player.Move.ReadValue<Vector2>();
            bool isShift = inputMap.Player.Shift.ReadValue<float>()!=0;

            if (dir != Vector2.zero && isShift)
            {
                dir.y = 0;
                return dir.normalized;
            }
            else if (dir != Vector2.zero && !isShift)
            {
                dir.y = 0;
                return dir.normalized;
            }
            else
            {
                return Vector2.zero;
            }
#else
                return 0f; // Ĭ��ֵ
#endif
        }
    }
    public Vector2 GetMoveVerticalValue
    {
        get
        {
            Vector2 dir = inputMap.Player.Move.ReadValue<Vector2>();

            if (dir != Vector2.zero)
            {
                dir.x = 0;
                return dir.normalized;
            }
            else
            {
                return Vector2.zero;
            }
        }
    }

    public bool Interactive => inputMap.Player.Interactive.ReadValue<float>()!= 0;

    public bool Shift
    {
       get
        {
           return inputMap.Player.Shift.ReadValue<float>() != 0;
        }
    }

    public Vector2 Move
    {
        get
        {
            Vector2 vector2 = inputMap.Player.Move.ReadValue<Vector2>();
            if (vector2.x > 0)
            {
                vector2.x = 1;
            }
            else if (vector2.x < 0)
            {
                vector2.x = -1;
            }
            else
            {
                vector2.x = 0;
            }
            if (vector2.y > 0)
            {
                vector2.y = 1;
            }
            else if (vector2.y < 0)
            {
                vector2.y = -1;
            }
            else
            {
                vector2.y = 0;
            }
            return vector2;
        }
    }
    public Vector2 Scroll =>inputMap.Player.Scroll.ReadValue<Vector2>();

}