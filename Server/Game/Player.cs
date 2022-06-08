using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using Server.Game;
using Server.Game.Items;

namespace Server.Game
{
    internal class Player
    {
        private int[] position = new int[2];
        private int currentHp;
        private int maxHp;
        private int[] damage = new int[2];
        private int xpNecUp;
        private int currentXp;
        private int currentLvl = 1;

        private Scene scene;
        private readonly Socket socket;

        // internal Properties for the player info
        internal int InSceneIndex { get => position[1] * scene.Width + position[1] + position[0]; }
        internal int[] Position { get => new int[2] { position[0], position[1] }; }
        internal int CurrentHp { get => currentHp; }
        internal int MaxHp { get => maxHp; }
        internal int[] Damage { get => new int[2] { damage[0], damage[1] }; }
        internal int XpNecUp { get => xpNecUp; }
        internal int CurrentXp { get => currentXp; }
        internal int CurrentLvl { get => currentLvl; }

        internal Scene Scene { get => scene; }
        internal Socket Socket { get => socket; }


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
            LvlUp();
            XpUp(0);
        }

        private void LvlUp()
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

        internal void TakeDamage(int amount, Monster from)
        {
            currentHp -= amount;
            int rnd = new Random(DateTime.Now.Millisecond).Next(damage[0], damage[1]+1);
            from.TakeDamage(rnd);
        }

        internal void Heal(int amount)
        {
            currentHp = Math.Min(maxHp, currentHp + amount);
        }

        internal void Transfer(Scene oldScene, Scene nextScene)
        {
            // If the player was already present in the next scene,
            // do nothing
            if (nextScene.AddplayerToScene(this))
            {
                oldScene.RemovePlayerFromScene(this);
                // update the old scene to refresh all the other players that are still in that scene
                oldScene.Update();
                scene = nextScene;
            }
        }
    }
}
