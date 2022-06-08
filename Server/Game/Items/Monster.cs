using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Game.DataStructures;

namespace Server.Game.Items
{
    internal class Monster : AItem
    {
        private string difficulty;
        private int[] damage = new int[2];
        private int maxHp;
        private int currentHp;
        private int xpGain;

        internal string Difficulty { get => difficulty; }
        internal int[] Damage { get => new int[2] { damage[0], damage[1] }; }
        internal int MaxHp { get => maxHp; }
        internal int CurrentHp { get => currentHp; }
        internal int XpGain { get => xpGain; }

        internal Monster(int[] position, string difficulty, Dictionary<string, MonsterType> dataTable, int sceneWidth) : base(position, sceneWidth)
        {
            this.difficulty = difficulty;
            damage[0] = dataTable[difficulty].Damage[0];
            damage[1] = dataTable[difficulty].Damage[1];
            maxHp = dataTable[difficulty].Hp;
            currentHp = maxHp;
            xpGain = (int)(0.2 * (maxHp + damage[0] + damage[1]));
        }

        protected override void Interact(Player player)
        {
            int rnd = new Random(DateTime.Now.Millisecond).Next(damage[0], damage[1] + 1);
            player.TakeDamage(rnd, this);
            Console.WriteLine("Damage Taken");
            if(currentHp <= 0)
            {
                player.XpUp(xpGain);
                Console.WriteLine("Removed Monster: " + player.Scene.RemoveMonster(this));
            }
        }

        internal void TakeDamage(int amount)
        {
            currentHp -= amount;
            Console.WriteLine($"Monster took {amount} of damage: {currentHp}");
        }

        new internal static void CheckForPlayer(Player player, int x, int y)
        {
            player.Move(x, y);
            int playerPosIndex = player.InSceneIndex;
            player.Move(-x, -y);

            foreach (Monster monster in player.Scene.Monsters)
            {
                if (monster.positionIndex == playerPosIndex)
                {
                    monster.Interact(player);
                    break;
                }
            }
        }
    }
}
