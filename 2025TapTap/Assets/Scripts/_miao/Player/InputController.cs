using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace miao
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance;//����

#pragma warning disable CS0414
        private bool W_Flag;
        private bool A_Flag;
        private bool S_Flag;
        private bool D_Flag;
        private bool Left_Shift_Flag;
        private bool Esc_Flag;
        private bool Space_Flag;
#pragma warning disable CS0414

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
            Space_Flag = false;
        }
        void Update()
        {
            //ÿ֡��ⰴ������
            key_Reset();
            key_Down();
            key_Keep();
            //////////////////


        }

        public bool get_Key(string key)
        {
            // ��̬ƴ���ַ���
            string flagName = key + "_Flag"; 

            // ʹ�÷����ȡ��Ӧ�ĳ�Ա����
            FieldInfo field = this.GetType().GetField(flagName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                return (bool)field.GetValue(this);  // ��ȡ��Ӧ�ֶε�ֵ
            }
            else
            {
                Debug.LogError("δ�ҵ�" + "��" + key + "��");
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
            if (Input.GetKeyDown(KeyCode.Space)) Space_Flag = true;


        }

        private void key_Keep()
        {
            if (Input.GetKey(KeyCode.W)) W_Flag = true;
            if (Input.GetKey(KeyCode.A)) A_Flag = true;
            if (Input.GetKey(KeyCode.S)) S_Flag = true;
            if (Input.GetKey(KeyCode.D)) D_Flag = true;
            if (Input.GetKey(KeyCode.LeftShift)) Left_Shift_Flag = true;
            if (Input.GetKey(KeyCode.Escape)) Esc_Flag = true;
            if (Input.GetKey(KeyCode.Space)) Space_Flag = true;
        }

        private void key_Reset()
        {
            W_Flag = false;
            A_Flag = false;
            S_Flag = false;
            D_Flag = false;
            Left_Shift_Flag = false;
            Esc_Flag = false;
            Space_Flag = false;
        }
    }
}



