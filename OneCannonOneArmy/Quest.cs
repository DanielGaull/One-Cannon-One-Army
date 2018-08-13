using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCannonOneArmy
{
    public struct Quest
    {
        public QuestGoalType GoalType;
        public int TypeId;
        public int GoalNumber;
        public int RewardCoins;
        public Quest(QuestGoalType goal, int typeId, int goalNum, int rewardCoins)
        {
            GoalType = goal;
            TypeId = typeId;
            GoalNumber = goalNum;
            RewardCoins = rewardCoins;
        }
    }

    public class QuestInterface
    {
        MenuButton questButton;
    }
    public class QuestPopup
    {
        Quest quest;
        string questText = "";
        Vector2 questTextLoc;
    }

    public enum QuestGoalType
    {
        KillAliens,
        FireProjectiles,
        CraftProjectiles,
        BuyMaterials,
        SpendCoins,
        ObtainCoins,
        BeatLevels,
    }
}
