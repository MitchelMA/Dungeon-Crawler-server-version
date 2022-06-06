using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Game;

namespace Server.Game
{
    internal class Player
    {
        internal int[] position;
        internal int InSceneIndex { get => position[1] * scene.width + position[1] + position[0]; }
        internal int currentHp;
        internal int maxHp;
        internal int[] damage;
        internal int xpNecUp;
        internal int currentXp;
        internal int currentLvl = 1;

        internal Scene scene;

        internal Player(int maxHp, int[] position, int[] damage, int xpNecUp, Scene scene)
        {
            this.maxHp = maxHp;
            currentHp = maxHp;

            this.position = position;

            this.damage = damage;

            this.xpNecUp = xpNecUp;

            this.scene = scene;
        }

        internal void XpUp(int amount)
        {
            currentXp += amount;
            if (currentXp < xpNecUp * currentLvl)
                return;
            XpUp(0);
        }

        internal void LvlUp()
        {
            damage[0] += 5;
            damage[1] += 5;

            currentHp = (int)(currentHp / (double)maxHp * (maxHp + 10));
            maxHp += 10;
            currentXp -= xpNecUp * currentLvl;
            currentLvl++;
        }

        internal void Move(int x, int y)
        {
            position[0] += x;
            position[1] += y;
            Console.WriteLine(InSceneIndex);
        }
        internal void MoveTo(int x, int y)
        {
            position[0] = x;
            position[1] = y;
            Console.WriteLine(InSceneIndex);
        }

        internal void CheckMove(int x, int y)
        {
            int nextPos = (position[1] + y) * scene.width + position[1] + y + position[0] + x;
            int[] listen;
            switch(scene.GameField[nextPos])
            {
                default:
                    break;
            }
        }
    }
}
