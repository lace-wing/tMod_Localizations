using BloodSoul.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.ApostleOfDeath
{
    [AutoloadBossHead]
    class ApostleOfDeath : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;
        private int interval = 0;
        private static float gravity = 0.3f;
        public Vector2 PlayerOldPos = Vector2.Zero;
        private int Time1 = 0;
        private int Time3 = 0;
        private int Time2 = 0;
        private enum ApostleOfDeathAI
        {
            Chop1,//普通斩
            SprintChop,//冲刺斩
            DeadLightDiffusion,//死光扩散
            ScatteringChopper//散射斩
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apostle Of Death");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "死亡使者");
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 22000;
            NPC.defense = 35;
            NPC.damage = 145 / 3;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 70;
            NPC.height = 58;
            NPC.value = 70000;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.aiStyle = -1;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.scale = 1.1f;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Death");
            }

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
        public override void BossHeadRotation(ref float rotation) => rotation = NPC.rotation;
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
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                writer.Write(interval);
                writer.Write(State3);
                writer.Write(State4);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                interval = reader.ReadInt32();
                State3 = reader.ReadSingle();
                State4 = reader.ReadSingle();
            }
        }
        public override void AI()
        {
            NPC.TargetClosest();
            Player TargetPlayer = Main.player[NPC.target];
            if (Main.player[NPC.target].dead || Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) > 6000f || Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 6000f)
            {
                NPC.TargetClosest(false);
                if (Main.player[NPC.target].dead || Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) > 6000f || Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 6000f)
                {
                    DespawnHandler();
                }
                return;
            }
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) NPC.TargetClosest();

            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 12;
            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            TargetVel *= 7f;
            float accX = 0.05f;
            float accY = 0.05f;
            NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;

            Vector2 toTarget = Target.position - NPC.position;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 50)
            {
                Vector2 center = NPC.Center + (i.ToRotationVector2() * 1600);
                int dust = Dust.NewDust(center, 1, 1, DustID.Shadowflame);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
            if (NPC.position.Distance(Target.position) > 1600)
            {
                Target.velocity = -toTarget.SafeNormalize(toTarget) * 3;
            }

            int Chop = ModContent.ProjectileType<Chop>();
            int DeadChop = ModContent.ProjectileType<DeadChop>();
            int LD3 = ModContent.ProjectileType<DeadLight3>();
            int LD = ModContent.ProjectileType<DeadLight>();

            switch ((ApostleOfDeathAI)State1)
            {
                case ApostleOfDeathAI.Chop1:
                    {
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = TargetVel * 0.1f;
                        Time1--;
                        if (Time1 < 0)
                        {
                            Time1 = 30;
                            Vector2 ToPlayer = (ToTarget * 2.1f) + Target.velocity;
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToPlayer * 0, Chop, 135 / 6, 2f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToPlayer * 0.6f, DeadChop, 135 / 6, 2f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToTarget * 1.1f, DeadChop, 135 / 6, 2f, Main.myPlayer);
                            Time2++;
                            if (Time2 > 10)
                            {
                                Time1 = 0;
                                Time2 = 0;
                                SwitchState2(0);
                                SwitchState1((int)ApostleOfDeathAI.SprintChop, (int)ApostleOfDeathAI.ScatteringChopper + 1);
                            }
                        }
                        break;
                    }
                case ApostleOfDeathAI.SprintChop:
                    {
                        switch (State2)
                        {
                            case 0://冲刺
                                {
                                    Time1++;
                                    if (Time1 > 45)
                                    {
                                        Time2++;
                                        SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                        Vector2 ToPlayer = (ToTarget * 1.1f) + Target.velocity;
                                        NPC.velocity = ToPlayer;
                                        Time1 = 0;
                                        SwitchState2(1);
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    Time1++;
                                    if (Time1 > 45)
                                    {
                                        Time1 = 0;
                                        Time2++;
                                        if (Time2 > 6)
                                        {
                                            NPC.velocity *= 1f;
                                            var player = Main.player[NPC.target];
                                            Vector2 ToPlayer = player.Center - NPC.Center;
                                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToPlayer * 0, Chop, 145 / 6, 2f, Main.myPlayer);
                                            for (int i = 0; i < 8; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 4)).ToRotationVector2() * 16;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, DeadChop, 145 / 6, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            SwitchState2(0);
                                            SwitchState1((int)ApostleOfDeathAI.DeadLightDiffusion, (int)ApostleOfDeathAI.ScatteringChopper + 1);
                                            Time2 = 0;
                                        }
                                        else
                                        {
                                            NPC.velocity *= 1f;
                                            var player = Main.player[NPC.target];
                                            Vector2 ToPlayer = player.Center - NPC.Center;
                                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToPlayer * 0, Chop, 135 / 6, 2f, Main.myPlayer);
                                            for (int i = 0; i < 8; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 4)).ToRotationVector2() * 16;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f,
                                                DeadChop, 145 / 6, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            SwitchState2(0);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case ApostleOfDeathAI.DeadLightDiffusion:
                    {
                        accX = 0.02f;
                        accY = 0.02f;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = (ToTarget * 2.1f) + Target.velocity;
                        NPC.velocity *= 0.01f;
                        Time2++;
                        Timer++;
                        if (Timer == 1000)
                        {
                            // 重置计时器
                            Timer = 0;
                        }
                        if (Timer % 30 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];

                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    for (int h = 0; h <= 7; h++)
                                    {
                                        Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 4)).ToRotationVector2() * 12;
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r,
                                        ModContent.ProjectileType<DeadLight3>(), 145 / 6, 0f, Main.myPlayer);
                                        interval++;
                                    }
                                }
                                return;
                            }
                        }
                        if (Timer % 45 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];

                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    for (int h = -9; h <= 2; h++)
                                    {
                                        Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 6)).ToRotationVector2() * 10;
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r,
                                        ModContent.ProjectileType<DeadLight3>(), 145 / 6, 0f, Main.myPlayer);
                                        interval++;
                                    }
                                }
                                return;
                            }
                        }
                        if (Time2 > 180)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)ApostleOfDeathAI.Chop1, (int)ApostleOfDeathAI.ScatteringChopper + 1);
                        }
                        break;
                    }
                case ApostleOfDeathAI.ScatteringChopper:
                    {
                        NPC.velocity = TargetVel * 0.1f;
                        Time1++;
                        if (Time1 == 60)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, Target.Center, Chop, 135 / 6, 2f, Main.myPlayer);
                            Vector2 plrToMouse = Main.MouseWorld - NPC.Center;
                            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                            for (int i = 1; i <= 9; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 108f;
                                Vector2 shootVel = r2.ToRotationVector2() * 10;
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, shootVel, DeadChop, 145 / 6, 2, Main.myPlayer);
                            }
                        }
                        if (Time1 == 120)
                        {
                            Time1 = 0;
                            SwitchState2(0);
                            SwitchState1((int)ApostleOfDeathAI.Chop1, (int)ApostleOfDeathAI.ScatteringChopper + 1);
                        }
                        break;
                    }
            }



        }
        public override void OnKill()
        {
            NPC.active = false;
            NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<DeathAppearance>(), NPC.whoAmI);
        }
        public override void OnHitPlayer(Player Target, int damage, bool crit)
        {
            Target.AddBuff(BuffID.ShadowFlame, 300);
        }

        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.velocity.X = 0;
                NPC.velocity.Y -= 1;
            }
        }
    }

}