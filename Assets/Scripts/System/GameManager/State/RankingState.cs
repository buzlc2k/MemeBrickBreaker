using System.Collections;
using UnityEngine;

namespace BullBrukBruker
{
    public class RankingState : BaseGameState
    {
        private readonly Predicate returnButtonPredicate;

        public RankingState(GameManager gameManager, ISMContext<GameStateID> context) : base(gameManager, context)
        {
            id = GameStateID.Ranking;

            returnButtonPredicate = new EventPredicate(EventID.ReturnMenuButton_Clicked);
        }

        public override IEnumerator UpdateState()
        {
            yield return gameManager.StartCoroutine(base.UpdateState());

            while (true)
            {
                if (returnButtonPredicate.Evaluate())
                {
                    context.ChangeState(GameStateID.MainMenu);
                    yield break;
                }

                yield return null;
            }
        }

        public override void ExitState()
        {
            returnButtonPredicate.StopPredicate();
            base.ExitState();
        }
    }
}