using System;
using System.Collections.Generic;

public class EventHandlerBase<T>
{
    //֧��һ���¼������ж���¼���ע�ᡢ���¼��Ĵ������ݸ����¼�������ѡ����߲�ʹ��
    private readonly Dictionary<T, List<Action<object, object>>> eventDic = new Dictionary<T, List<Action<object, object>>>();
   
    private readonly Dictionary<object,List<T>> targetEventDic = new Dictionary<object, List<T>>(); 

    public void AddEventHandler(T t, Action<object, object> action)
    {
        if (!eventDic.ContainsKey(t))
        {
            //eventDic.Add(t, new List<Action<object, object>>());ʹ����������
            eventDic[t]=new List<Action<object, object>>();
        }
        //������ظ�ע����ͬ���¼�
        List<Action<object,object>> eventList = eventDic[t];
        Action<object, object> checkAction = eventList.Find(i => i == action);
        if (checkAction != null)
        {
            //˵��List����
            return;
        }
        //�����ھ����������¼�
        eventList.Add(action);
        //���´�������Ϣ
        object target = action.Target; 
        if (!targetEventDic.ContainsKey(target))
        {
            //targetEventDic.Add(target, new List<T>());
            targetEventDic[target] = new List<T>();
        }
        //���¼�����ӵ�����������б���
        targetEventDic[target].Add(t);
    }

    public void RemoveEventByID(T t)
    {
        if (eventDic.ContainsKey(t))
        {
            //�Ȳ��ż��Ƴ�
            //����target
            List<Action<object,object>> actions = eventDic[t];
            foreach (var action in actions)
            {
                object target = action.Target;
                if (target != null && targetEventDic.ContainsKey(target))
                {
                    //������ܴ���һ��λ��ע����ͬ�����¼���list������ظ�Ԫ�أ�
                    List<T> idList = targetEventDic[target];
                    idList.RemoveAll(id => id.Equals(t));//��������ķ����ʽ
                    if (idList.Count == 0)
                    {
                        targetEventDic.Remove(target);
                    }
                }
            }
            eventDic.Remove(t);
        }
    }
    public void RemoveEventByTarget(object target)
    {
        if (targetEventDic.ContainsKey(target))
        {
            List<T> idList = targetEventDic[target];
            foreach (var id in idList)
            {
                if (eventDic.ContainsKey(id))
                {
                    List<Action<object, object>> actions = eventDic[id];
                    actions.RemoveAll(action => action.Target == target);
                    if (actions.Count == 0)
                    {
                        eventDic.Remove(id);
                    }
                }
            }
            targetEventDic.Remove(target);
        }
    }
    public List<Action<object, object>> GetEvent(T t)
    {
        if (eventDic.ContainsKey(t))
        {
            return eventDic[t];
        }
        return null;
    }
    
}
