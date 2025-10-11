
using UnityEngine;
//����������ʱ���ʴζ�����ü�һ��Null���жϣ���ֹ�����˳������屻����
public class MonoSingleton<T>:MonoBehaviour where T : MonoSingleton<T> 
{
    private static T instance;
    public static T Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<T>();
                if (instance == null)
                {
                    var go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();
                }
            }
            return instance; 
        }
    }
    protected virtual void Awake()
    {
        //Ĭ�Ϸŵ�DontDestroyScene��
        if (instance == null)
        {
            instance = (T)this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(instance);
        }
    }
}
