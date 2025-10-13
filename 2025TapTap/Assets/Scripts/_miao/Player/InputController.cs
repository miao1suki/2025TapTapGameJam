using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace miao
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance;//单例

        private bool W_Flag;
        private bool A_Flag;
        private bool S_Flag;
        private bool D_Flag;
        private bool Left_Shift_Flag;
        private bool Esc_Flag;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            W_Flag = false;
            A_Flag = false;
            S_Flag = false;
            D_Flag = false;
            Left_Shift_Flag = false;
            Esc_Flag = false;

        }
        void Update()
        {
            //每帧检测按键部分
            key_Reset();
            key_Down();
            key_Keep();
            //////////////////


        }

        public bool get_Key(string key)
        {
            // 动态拼接字符串
            string flagName = key + "_Flag"; 

            // 使用反射获取对应的成员变量
            FieldInfo field = this.GetType().GetField(flagName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                return (bool)field.GetValue(this);  // 获取对应字段的值
            }
            else
            {
                return false; 
            }
        }

        public void OnEnable()
        {
            
        }
        public void Disable()
        {

        }

        private void key_Down()
        {
            if (Input.GetKeyDown(KeyCode.W)) W_Flag = true;
            if (Input.GetKeyDown(KeyCode.A)) A_Flag = true;
            if (Input.GetKeyDown(KeyCode.S)) S_Flag = true;
            if (Input.GetKeyDown(KeyCode.D)) D_Flag = true;
            if (Input.GetKeyDown(KeyCode.LeftShift)) Left_Shift_Flag = true;
            if (Input.GetKeyDown(KeyCode.Escape)) Esc_Flag = true;


        }

        private void key_Keep()
        {
            if (Input.GetKey(KeyCode.W)) W_Flag = true;
            if (Input.GetKey(KeyCode.A)) A_Flag = true;
            if (Input.GetKey(KeyCode.S)) S_Flag = true;
            if (Input.GetKey(KeyCode.D)) D_Flag = true;
            if (Input.GetKey(KeyCode.LeftShift)) Left_Shift_Flag = true;
            if (Input.GetKeyDown(KeyCode.D)) D_Flag = true;
        }

        private void key_Reset()
        {
            W_Flag = false;
            A_Flag = false;
            S_Flag = false;
            D_Flag = false;
            Left_Shift_Flag = false;
            Esc_Flag = false;
        }
    }
}



