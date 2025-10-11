using Animancer;
using Animancer.Units;
using UnityEngine;
/**************************************************************************
����: HuHu
����: 3112891874@qq.com
����: �Ų�IK
**************************************************************************/
public class RaycastFootIK : MonoBehaviour
{
  
    [SerializeField,Header("�Ƿ���")] private bool enable = true;
    [SerializeField] private AnimancerComponent _Animancer;
    [SerializeField,Header("������"), Meters] private float _RaycastOriginY = 0.1f;
    [SerializeField,Header("����յ�"), Meters] private float _RaycastEndY = -0.2f;
    [SerializeField] private float _ForwardOffset = 0;
    [SerializeField] LayerMask whatIsGround;
  

    private Transform _LeftFoot;
    private Transform _RightFoot;

    private AnimatedFloat _FootWeights;

  
    private BindableProperty<bool> Enable = new BindableProperty<bool>();
    /// <summary>Public property for a UI Toggle to enable or disable the IK.</summary>
    public bool ApplyAnimatorIK
    {
        get => _Animancer.Layers[0].ApplyAnimatorIK;
        set => _Animancer.Layers[0].ApplyAnimatorIK = value;
    }

  

    protected virtual void Awake()
    {
        _LeftFoot = _Animancer.Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        _RightFoot = _Animancer.Animator.GetBoneTransform(HumanBodyBones.RightFoot);

        _FootWeights = new(_Animancer, "LeftFootIK", "RightFootIK");

        Enable.ValueChanged += OnEnableChange;
        OnEnableChange(enable);
    }

    private void OnEnableChange(bool obj)
    {
        Debug.Log("�����Ų�IK" + obj);
        ApplyAnimatorIK = obj;
    }

  

    // Note that due to limitations in the Playables API,
    // Unity will always call this method with layerIndex = 0.
    protected virtual void OnAnimatorIK(int layerIndex)
    {
      
        // _FootWeights[0] is the first property we specified in Awake: "LeftFootIK".
        // _FootWeights[1] is the second property we specified in Awake: "RightFootIK".
        UpdateFootIK(
            _LeftFoot,
            AvatarIKGoal.LeftFoot,
            _FootWeights[0],
            _Animancer.Animator.leftFeetBottomHeight);
        UpdateFootIK(
            _RightFoot,
            AvatarIKGoal.RightFoot,
            _FootWeights[1],
            _Animancer.Animator.rightFeetBottomHeight);
    }


    private void Update()
    {
        Enable.Value = enable;
    }
    private void UpdateFootIK(
            Transform footTransform,
            AvatarIKGoal goal,
            float weight,
            float footBottomHeight)
    {
        Animator animator = _Animancer.Animator;
        animator.SetIKPositionWeight(goal, weight);
        animator.SetIKRotationWeight(goal, weight);
        if (weight == 0)
            return;

        Quaternion rotation = animator.GetIKRotation(goal);
        Vector3 localUp = rotation * Vector3.up;
        Vector3 LocalForward = rotation * Vector3.forward;
        Vector3 position = footTransform.position;
        position += localUp * _RaycastOriginY;
        position += LocalForward * _ForwardOffset;

        float distance = _RaycastOriginY - _RaycastEndY;

        // �������ߣ���Scene��ͼ�пɼ���
        Debug.DrawRay(position, -localUp * distance, Color.red);

        if (Physics.Raycast(position, -localUp, out RaycastHit hit, distance, whatIsGround))
        {
            Debug.DrawLine(hit.point, hit.point + hit.normal * 0.1f, Color.red);

            position = hit.point;
            position += localUp * footBottomHeight;
            position -= LocalForward * _ForwardOffset;
            animator.SetIKPosition(goal, position);

            Vector3 rotAxis = Vector3.Cross(localUp, hit.normal);
            float angle = Vector3.Angle(localUp, hit.normal);
            rotation = Quaternion.AngleAxis(angle, rotAxis) * rotation;
            animator.SetIKRotation(goal, rotation);
        }
        else
        {
            position += localUp * (footBottomHeight - distance);
            position -= LocalForward * _ForwardOffset;
            animator.SetIKPosition(goal, position);
        }
    }
}