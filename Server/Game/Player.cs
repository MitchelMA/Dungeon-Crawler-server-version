using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using Server.Game;

namespace Server.Game
{
    internal class Player
    {
        internal int[] position = new int[2];
        internal int InSceneIndex { get => position[1] * scene.width + position[1] + position[0]; }
        internal int currentHp;
        internal int maxHp;
        internal int[] damage = new int[2];
        internal int xpNecUp;
        internal int currentXp;
        internal int currentLvl = 1;

        internal Scene scene;
        internal Socket socket;

        internal Player(int maxHp, int[] position, int[] damage, int xpNecUp, Scene scene, Socket socket)
        {
            this.maxHp = maxHp;
            currentHp = maxHp;

            // copy the position
            this.position[0] = position[0];
            this.position[1] = position[1];

            // copy the damage
            this.damage[0] = damage[0];
            this.damage[1] = damage[1];

            this.xpNecUp = xpNecUp;

            this.scene = scene;

            this.socket = socket;
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

        internal void Transfer(Scene oldScene, Scene nextScene) {
            oldScene.RemovePlayerFromScene(this);
            nextScene.AddplayerToScene(this);
            this.scene = nextScene;
        }
    }
}
