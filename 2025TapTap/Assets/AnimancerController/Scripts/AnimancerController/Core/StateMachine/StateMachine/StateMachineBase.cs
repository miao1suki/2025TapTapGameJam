using UnityEngine.Rendering;

public class StateMachineBase
{
    public IState currentState;
    public IState lastState;

    /// <summary>
    /// ״̬�л���API
    /// </summary>
    /// <param name="targetState"></param>
    public virtual void ChangeState(IState targetState)
    {
        currentState?.OnExit();
        lastState = currentState;
        currentState = targetState;
        currentState?.OnEnter();
    }
    /// <summary>
    /// ����״̬�˳��Ľӿ�
    /// </summary>
    public void OnAnimationEnd()
    {
        currentState.OnAnimationEnd();
    }
    /// <summary>
    /// Update״̬API
    /// </summary>
    public void OnUpdate()
    {
        currentState?.OnUpdate();
    }
    /// <summary>
    /// ������֡������
    /// </summary>
    public void OnAnimationUpdate()
    {
        currentState?.OnAnimationUpdate();
    }

}
