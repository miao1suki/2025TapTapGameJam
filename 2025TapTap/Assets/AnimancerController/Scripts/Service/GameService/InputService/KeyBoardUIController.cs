using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyboardUIController : MonoBehaviour
{
    public EventSystem eventSystem;
    public Selectable firstSelected;

    void Start()
    {
        // ���ó�ʼѡ�е� UI Ԫ��
        if (firstSelected != null)
        {
            firstSelected.Select();
        }
    }
    void Update()
    {
        // ����Ƿ���ѡ�е� UI Ԫ��
        if (eventSystem.currentSelectedGameObject == null)
        {
            eventSystem.SetSelectedGameObject(firstSelected.gameObject);
        }

        // �����������
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = eventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                next.Select();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // ����ȷ�ϲ���
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
        }
    }
}
