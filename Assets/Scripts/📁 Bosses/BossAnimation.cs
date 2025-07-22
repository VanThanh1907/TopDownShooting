using UnityEngine;
using Spine.Unity;

public class BossAnimation : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    private string currentAnimation;

    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (skeletonAnimation == null)
        {
            Debug.LogError("SkeletonAnimation is not found on this GameObject!");
        }
    }

    public void PlayAnimation(string animationName, bool loop)
    {
        if (skeletonAnimation == null || skeletonAnimation.AnimationState == null)
        {
            Debug.LogError("SkeletonAnimation or AnimationState is null!");
            return;
        }

        if (currentAnimation != animationName)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
            currentAnimation = animationName;
            Debug.LogWarning($"Playing animation: {animationName}, loop: {loop}");
        }
    }
}