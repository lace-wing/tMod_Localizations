using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.IO;
using BloodSoul.Items;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using Terraria.Audio;
using BloodSoul.NPCs;

namespace BloodErosion.NPCs.Bosses.FlameGhostKing
{
    class HellRockGhostS : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;

        private int interval = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        private int Time3 = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hell Rock Ghost Soldier");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "地狱岩鬼兵");
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.friendly = false;
            NPC.width = 86;
            NPC.height = 62;
            NPC.aiStyle = -1;
            NPC.damage = 130 / 3;
            NPC.defense = 30;
            NPC.lifeMax = 130;
            NPC.HitSound = SoundID.NPCHit42;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.knockBackResist = 0.5f;
            NPC.value = 80f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 0.8f;
            NPC.alpha = 0;
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.X > 0f)
            {
                NPC.spriteDirection = 1;
            }
            if (NPC.velocity.X < 0f)
            {
                NPC.spriteDirection = -1;
            }
            NPC.rotation = NPC.velocity.X * 0.1f;
            NPC.frameCounter += 1.0;
            int num155 = 4;
            int num156 = Main.npcFrameCount[NPC.type];
            if (NPC.frameCounter >= (double)num155)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * num156)
            {
                NPC.frame.Y = 0;
            }
        }
        protected int State
        {
            get { return (int)NPC.ai[0]; }
            set { NPC.ai[0] = value; }
        }
        protected int Timer
        {
            get { return (int)NPC.ai[1]; }
            set { NPC.ai[1] = value; }
        }
        protected virtual void SwitchState(int state)
        {
            State = state;
        }
        enum NPCState
        {
            Normal,
            Attack,
        }
        public override void AI()
        {
            switch ((NPCState)State)
            {
                case NPCState.Normal:
                    {
                        {
                            NPC.TargetClosest();
                            Timer++;
                            Time1++;
                            var player = Main.player[NPC.target];
                            Vector2 ToPlayer = player.Center - NPC.Center;
                            ToPlayer.Normalize();
                            if (Timer == 2000)
                            {
                                // 重置计时器
                                Timer = 0;
                            }
                            if (Timer % 30 == 0)
                            {

                                // NPC的攻击目标
                                Player p = Main.player[NPC.target];

                                {
                                    for (int h = 0; h <= 0; h++)
                                    {
                                        Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 6)).ToRotationVector2() * 10;
                                        Projectile.NewProjectile(Source_NPC, NPC.Center, r,
                                        258, 30 / 2, 0f, Main.myPlayer);
                                        interval++;
                                        SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                                    }
                                    return;
                                }
                            }
                            float Xlimit = 4f;
                            float YLimit = 1.5f;
                            if (NPC.direction == -1 && NPC.velocity.X > -Xlimit)
                            {
                                NPC.velocity.X = NPC.velocity.X - 0.1f;
                                if (NPC.velocity.X > Xlimit)
                                {
                                    NPC.velocity.X = NPC.velocity.X - 0.1f;
                                }
                                else if (NPC.velocity.X > 0f)
                                {
                                    NPC.velocity.X = NPC.velocity.X + 0.05f;
                                }
                                if (NPC.velocity.X < -Xlimit)
                                {
                                    NPC.velocity.X = -Xlimit;
                                }
                            }
                            else if (NPC.direction == 1 && NPC.velocity.X < Xlimit)
                            {
                                NPC.velocity.X = NPC.velocity.X + 0.1f;
                                if (NPC.velocity.X < -Xlimit)
                                {
                                    NPC.velocity.X = NPC.velocity.X + 0.1f;
                                }
                                else if (NPC.velocity.X < 0f)
                                {
                                    NPC.velocity.X = NPC.velocity.X - 0.05f;
                                }
                                if (NPC.velocity.X > Xlimit)
                                {
                                    NPC.velocity.X = Xlimit;
                                }
                            }
                            if (NPC.directionY == -1 && NPC.velocity.Y > -YLimit)
                            {
                                NPC.velocity.Y = NPC.velocity.Y - 0.04f;
                                if (NPC.velocity.Y > YLimit)
                                {
                                    NPC.velocity.Y = NPC.velocity.Y - 0.05f;
                                }
                                else if (NPC.velocity.Y > 0f)
                                {
                                    NPC.velocity.Y = NPC.velocity.Y + 0.03f;
                                }
                                if (NPC.velocity.Y < -YLimit)
                                {
                                    NPC.velocity.Y = -YLimit;
                                }
                            }
                            else if (NPC.directionY == 1 && NPC.velocity.Y < YLimit)
                            {
                                NPC.velocity.Y = NPC.velocity.Y + 0.04f;
                                if (NPC.velocity.Y < -YLimit)
                                {
                                    NPC.velocity.Y = NPC.velocity.Y + 0.05f;
                                }
                                else if (NPC.velocity.Y < 0f)
                                {
                                    NPC.velocity.Y = NPC.velocity.Y - 0.03f;
                                }
                                if (NPC.velocity.Y > YLimit)
                                {
                                    NPC.velocity.Y = YLimit;
                                }
                            }
                            if (Main.player.Any(p =>
                             p.active && p.Distance(NPC.Center) < 300
                        ))
                            {
                                // 转换为攻击模式
                                SwitchState((int)NPCState.Attack);
                            }

                            break;
                        }
                    }
                // 攻击状态下
                case NPCState.Attack:
                    {
                        Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
                        TargetVel *= 7f;
                        float accX = 0.2f;
                        float accY = 0.2f;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        Timer++;
                        Time1++;
                        NPC.TargetClosest(true);
                        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        ToPlayer.Normalize();
                        if (Timer == 1200)
                        {
                            Timer = 0;
                        }
                        ToPlayer.Normalize();
                        if (Timer % 30 == 0)
                        {

                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];

                            {
                                for (int h = -1; h <= 1; h++)
                                {
                                    Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 6)).ToRotationVector2() * 16;
                                    Projectile.NewProjectile(Source_NPC, NPC.Center, r,
                                    326, 30 / 2, 0f, Main.myPlayer);
                                    interval++;
                                    SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                                }
                                return;
                            }
                        }
                        if (Time1 < 50 && Time1 > 20)
                        {
                            NPC.rotation = NPC.velocity.ToRotation();
                        }
                        if (Time1 >= 50)
                        {
                            NPC.velocity = ToPlayer;
                            Time1 = 0;
                        }
                        else if (Time1 > 40 && Time1 < 50)
                        {
                            NPC.velocity *= 0.6f;
                        }
                        if (Main.player.Any(p =>
                             p.active && p.Distance(NPC.Center) > 300
                        ))
                        {
                            // 转换为攻击模式
                            SwitchState((int)NPCState.Normal);
                        }
                        break;
                    }






            }
        }
    }
}
