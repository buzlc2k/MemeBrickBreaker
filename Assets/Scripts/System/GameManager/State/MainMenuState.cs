using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BullBrukBruker
{
    public class MainMenuState : BaseGameState
    {
        private readonly Predicate selectLevelPredicate;
        private readonly Predicate rankingPredicate;

        public MainMenuState(GameManager gameManager, ISMContext<GameStateID> context) : base(gameManager, context)
        {
            id = GameStateID.MainMenu;

            selectLevelPredicate = new EventPredicate(EventID.SelectLevelButton_Clicked);
            rankingPredicate = new EventPredicate(EventID.RankingButton_Clicked);
        }

        public override IEnumerator UpdateState()
        {
            yield return gameManager.StartCoroutine(base.UpdateState());

            while (true)
            {
                if (LoadingManager.Instance.IsLoading)
                {
                    context.ChangeState(GameStateID.Load);
                    yield break;
                }

                if (selectLevelPredicate.Evaluate())
                {
                    context.ChangeState(GameStateID.SelectLevel);
                    yield break;
                }
                
                if (rankingPredicate.Evaluate())
                {
                    context.ChangeState(GameStateID.Ranking);
                    yield break;
                }

                yield return null;
            }
        }
        
        public override void ExitState()
        {
            selectLevelPredicate.StopPredicate();
            rankingPredicate.StopPredicate();

            base.ExitState();
        }
    }
}