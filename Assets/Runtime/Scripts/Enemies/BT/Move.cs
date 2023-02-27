using UnityEngine;
using UnityEngine.AI;

namespace Final_Survivors.Enemies
{
    public class Move : Node
    {
        private Enemy instance;
        NavMeshPath navMeshPath = new NavMeshPath();

        public Move(Transform transform)
        {
            instance = transform.GetComponent<Enemy>();
        }
        public override NodeState Evaluate()
        {
            if (!instance.isAttacking)
            {
                if (Vector3.Distance(instance.transform.position, instance.player.position) > instance.attackRange)
                {
                    ChooseNewPath();
                    if (navMeshPath.status == NavMeshPathStatus.PathComplete || navMeshPath.status == NavMeshPathStatus.PathPartial)
                    {
                        if (!instance.animator.GetBool("isMoving"))
                        {
                            foreach (AnimatorControllerParameter param in instance.animator.parameters)
                            {
                                instance.animator.SetBool(param.name, false);
                            }
                            instance.animator.SetBool("isMoving", true);
                        }

                        instance.Agent.path = navMeshPath;
                        instance.Agent.speed = instance.moveSpeed;
                        instance.Agent.isStopped = false;

                        return NodeState.SUCCESS;
                    }
                    return NodeState.RUNNING;
                }
            }
            return NodeState.FAILURE;
        }
        private void ChooseNewPath()
        {
            instance.Agent.CalculatePath(instance.player.position, navMeshPath);
        }
    }
}
