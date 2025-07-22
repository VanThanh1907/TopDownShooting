using Spine.Unity;
using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    private SkeletonAnimation skeletonAnim;

    void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
    }

    public void PlayAnimation(string animationName, bool loop)
    {
        if (skeletonAnim != null)
        {
            skeletonAnim.AnimationState.SetAnimation(0, animationName, loop);
        }
    }
}