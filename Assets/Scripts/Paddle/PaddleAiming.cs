using System.Collections;
using UnityEngine;

namespace BullBrukBruker
{
    public class PaddleAiming : MonoBehaviour
    {
        private Vector3 contactPoint;
        private Vector3 bouncingPoint;
        [SerializeField] private float boucingLength = 0.75f;
        [SerializeField] PredictionTrajectoryController predictionTrajectoryController;

        public void CalculatingPoints()
        {
            if (!predictionTrajectoryController.gameObject.activeInHierarchy)
                predictionTrajectoryController.gameObject.SetActive(true);

            var inputPos = InputManager.Instance.Position;
            var dir = (inputPos - transform.position).normalized;

            foreach (var bound in GridManager.Instance.LevelBounds)
            {
                if (PhysicsUtils.TryGetRayIntersectedInformation(transform.position,
                                                dir, Mathf.Infinity,
                                                bound.transform.position,
                                                bound.Collision.Config.Width,
                                                bound.Collision.Config.Height, out var collidedIn4)
                    && Mathf.Abs(collidedIn4.contactPoint.x) < ScreenManager.Instance.ScreenWidth
                    && ((collidedIn4.contactPoint.y > 0 && collidedIn4.contactPoint.y < ScreenManager.Instance.TopScreenHeight)
                    || (collidedIn4.contactPoint.y < 0 && collidedIn4.contactPoint.y > -ScreenManager.Instance.DownScreenHeight)))
                {
                    contactPoint = collidedIn4.contactPoint;

                    var bouncingDir = Vector3.Reflect(dir, collidedIn4.normal);

                    bouncingPoint = contactPoint + boucingLength * bouncingDir;
                    break;
                }
            }

            predictionTrajectoryController.UpdateLineRenderer(contactPoint, bouncingPoint);
        }

        public void Shooting()
        {
            var dir = (contactPoint - transform.position).normalized;
            BallSpawner.Instance.SpawnSingleBall(transform.position, dir);
            predictionTrajectoryController.gameObject.SetActive(false);
        }
    }
}