using BloodSoul.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.FinalDeathSickle.FinalDeathSickle2s
{
    [AutoloadBossHead]
    class FinalDeathSickle3 : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;
        private Vector2 endPiont;
        private int frameTime = 0;
        private int interval = 0;
        private int TimeV = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;
        private int leavl = 0;
        private int Time1 = 0;
        private int Time3 = 0;
        private int Time2 = 0;
        private int Time4 = 0;
        private enum FinalDeathSickleAI
        {
            S1,//摸鱼
            S2,//冲刺
            S3,//散射
            S4,//回旋冲刺
            S5,//散射回收
            E1,//死亡旋风
            E2,//旋风天下
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Final Death Sickle");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "最终死神镰");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 250000 / 3;
            NPC.defense = 55;
            NPC.damage = 315;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 72;
            NPC.height = 84;
            NPC.value = 100000;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath10;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.aiStyle = -1;
            NPC.scale = 1f;
            //BossBag = ModContent.ItemType<Item.Boss.SpiritOfSparks.PermanentCombustionSpark>();
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SupremeSickle4");
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));
        }
        public override void BossHeadRotation(ref float rotation) => rotation = -NPC.rotation;
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
            if (Main.netMode == 2)
            {
                base.SendExtraAI(writer);
                writer.Write(leavl);
                writer.WriteVector2(endPiont);
                writer.Write(interval);
                writer.Write(State3);
                writer.Write(State4);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode != 1)
            {
                base.ReceiveExtraAI(reader);
                leavl = reader.ReadInt32();
                endPiont = reader.ReadVector2();
                interval = reader.ReadInt32();
                State3 = reader.ReadSingle();
                State4 = reader.ReadSingle();
            }
        }

        public override void AI()
        {
            NPC.TargetClosest();
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
            TimeV++;
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = TimeV * 7;
            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float accX = 0.5f;
            float accY = 0.5f;

            Vector2 toTarget = Target.position - NPC.position;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 50)
            {
                Vector2 center = NPC.Center + (i.ToRotationVector2() * 2500);
                int dust = Dust.NewDust(center, 1, 1, DustID.DemonTorch);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
            if (NPC.position.Distance(Target.position) > 2500)
            {
                Target.velocity = -toTarget.SafeNormalize(toTarget) * 3;
            }

            if (NPC.life < NPC.lifeMax * 0.9f && leavl == 0)
            {
                leavl = 1;
            }

            if (NPC.life < NPC.lifeMax * 0.6f && leavl == 1)
            {
                leavl = 2;
                Time1 = 0;
                Time2 = 0;
                Timer1 = 0;
                Timer = 0;
                Timer3 = 0;
                Timer2 = 0;
                SwitchState1((int)FinalDeathSickleAI.E1);
            }

            if (NPC.life < NPC.lifeMax * 0.2f && leavl == 2)
            {
                leavl = 3;
                Time1 = 0;
                Time2 = 0;
                Timer1 = 0;
                Timer = 0;
                Timer3 = 0;
                Timer2 = 0;
                SwitchState1((int)FinalDeathSickleAI.E2);
            }

            //NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            //NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
            //NPC.velocity = TargetVel * 8f;
            int DeathSwordWind = ModContent.ProjectileType<FinalDeathSwordWind>();
            int DeathSwordWind2 = ModContent.ProjectileType<FinalDeathSwordWind2>();
            int DeathWind = ModContent.ProjectileType<DeathSwordWind>();
            int DeathWind2 = ModContent.ProjectileType<DeathSwordWind2>();//回旋
            int DeathStar = ModContent.ProjectileType<DeathStar>();
            switch ((FinalDeathSickleAI)State1)
            {
                case FinalDeathSickleAI.S1: 
                    {
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = TargetVel * 6f;
                        Time1--;
                        if (Time1 < 0)
                        {
                            Time1 = 60;
                            Vector2 ToPlayer = (ToTarget * 1.1f) + Target.velocity;
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToPlayer * 2, DeathWind, 45, 2f, Main.myPlayer);
                            Time2++;
                            if (Time2 > 4)
                            {
                                Time1 = 0;
                                Time2 = 0;
                                SwitchState2(0);
                                SwitchState1((int)FinalDeathSickleAI.S2, (int)FinalDeathSickleAI.S5 + 1);
                            }
                        }
                        break;
                    }
                case FinalDeathSickleAI.S2:
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
                                        NPC.velocity = ToPlayer * 1.1f;
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
                                            for (int i = 0; i < 8; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 4)).ToRotationVector2() * 17f;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, DeathStar, 45, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            //SwitchState2(1);
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)FinalDeathSickleAI.S1, (int)FinalDeathSickleAI.S5 + 1);
                                        }
                                        else
                                        {
                                            NPC.velocity *= 1f;
                                            var player = Main.player[NPC.target];
                                            Vector2 ToPlayer = player.Center - NPC.Center;
                                            for (int i = 0; i < 8; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 4)).ToRotationVector2() * 17f;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f,
                                                DeathStar, 45, 0f, Main.myPlayer);
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
                case FinalDeathSickleAI.S3:
                    {
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = TargetVel * 6f;
                        Time1--;
                        if (Time1 < 0)
                        {
                            Time1 = 60;
                            Vector2 ToPlayer = (ToTarget * 1.1f) + Target.velocity;
                            for (int i = 0; i < 20; i++)
                            {
                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 10)).ToRotationVector2() * 7;
                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, DeathSwordWind, 145 / 3, 0f, Main.myPlayer);
                                interval++;
                            }
                            Time2++;
                            if (Time2 > 3)
                            {
                                Time1 = 0;
                                Time2 = 0;
                                SwitchState2(0);
                                SwitchState1((int)FinalDeathSickleAI.S1, (int)FinalDeathSickleAI.S5 + 1);
                            }
                        }
                        break;
                    }
                case FinalDeathSickleAI.S4:
                    {
                        switch (State2)
                        {
                            case 0:
                                {
                                    Time1++;
                                    if (Time1 == 25)
                                    {
                                        Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                            .ToRotationVector2() * 350;
                                        Dust.NewDustDirect(center, 10, 10, DustID.Electric);
                                        NPC.position = center;
                                        NPC.rotation = ToTarget.ToRotation();
                                        NPC.velocity = NPC.rotation.ToRotationVector2();
                                        NPC.netUpdate = true;
                                    }
                                    if (Time1 > 25)
                                    {
                                        Time1++;
                                        Vector2 ToPlayer = (ToTarget * 1.5f) + Target.velocity;
                                        SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                        if (Time1 > 90)
                                        {
                                            Time2++;
                                            NPC.velocity = ToPlayer;
                                            Time1 = 0;
                                            SwitchState2(1);
                                        }
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    Time1++;
                                    NPC.velocity *= 1f;
                                    var player = Main.player[NPC.target];
                                    Vector2 ToPlayer = player.Center - NPC.Center;
                                    if (Time1 > 30)
                                    {
                                        Time1 = 0;
                                        if (Time2 > 4)
                                        {
                                            Time1 = 0;
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)FinalDeathSickleAI.S1, (int)FinalDeathSickleAI.S5 + 1);
                                        }
                                        else
                                        {
                                            NPC.velocity *= 0f;
                                            SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                            for (int i = 0; i < 8; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 4)).ToRotationVector2() * 15;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, DeathWind2, 145 / 3, 0f, Main.myPlayer);
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
                case FinalDeathSickleAI.S5:
                    {
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = TargetVel * 6f;
                        Time1--;
                        if (Time1 < 0)
                        {
                            Time1 = 60;
                            Vector2 ToPlayer = (ToTarget * 1.1f) + Target.velocity;
                            var player = Main.player[NPC.target];
                            Vector2 plrToMouse = player.Center - NPC.Center;
                            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                            for (int i = -2; i <= 2; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 12;
                                Vector2 shootVel = r2.ToRotationVector2() * 13.5f;
                                Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, DeathWind, 145 / 3, 2, player.whoAmI);
                            }
                            for (int i = -2; i <= 2; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 18;
                                Vector2 shootVel = r2.ToRotationVector2() * 16.5f;
                                Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, DeathWind, 145 / 3, 2, player.whoAmI);
                            }
                            Time2++;
                            if (Time2 > 2)
                            {
                                Time1 = 0;
                                Time2 = 0;
                                SwitchState2(0);
                                SwitchState1((int)FinalDeathSickleAI.S2, (int)FinalDeathSickleAI.S4 + 1);
                            }
                        }
                        break;
                    }

                    case FinalDeathSickleAI.E1:
                    {
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity *= 0;
                        Time1--;
                        if (Time1 < 0)
                        {
                            Time1 = 60;
                            Vector2 ToPlayer = (ToTarget * 1.1f) + Target.velocity;
                            for (int i = 0; i < 30; i++)
                            {
                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 15)).ToRotationVector2() * 7;
                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, ModContent.ProjectileType<DeathSwordWind>(), 145 / 3, 0f, Main.myPlayer);
                                interval++;
                            }
                            Time2++;
                            if (Time2 > 4)
                            {
                                Time1 = 0;
                                Time2 = 0;
                                SwitchState2(0);
                                SwitchState1((int)FinalDeathSickleAI.S1, (int)FinalDeathSickleAI.S5 + 1);
                            }
                        }
                        break;
                    }
                case FinalDeathSickleAI.E2:
                    {
                        switch (State2)
                        {
                            case 0:
                                {
                                    Time1++;
                                    if (Time1 > 15)
                                    {
                                        Time1++;
                                        Vector2 ToPlayer = (ToTarget * 1.5f) + Target.velocity;
                                        SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                        if (Time1 > 90)
                                        {
                                            Time2++;
                                            NPC.velocity = ToPlayer;
                                            Time1 = 0;
                                            SwitchState2(1);
                                        }
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    Time1++;
                                    NPC.velocity *= 1f;
                                    var player = Main.player[NPC.target];
                                    Vector2 ToPlayer = player.Center - NPC.Center;
                                    if (Time1 > 30)
                                    {
                                        Time1 = 0;
                                        if (Time2 > 7)
                                        {
                                            Time1 = 0;
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)FinalDeathSickleAI.S1, (int)FinalDeathSickleAI.S5 + 1);
                                        }
                                        else
                                        {
                                            NPC.velocity *= 0f;
                                            SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                            for (int i = 0; i < 12; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 6)).ToRotationVector2() * 17;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, DeathWind2, 145 / 3, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            for (int i = 0; i < 10; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 5)).ToRotationVector2() * 13;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, DeathWind2, 145 / 3, 0f, Main.myPlayer);
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
            }
        }
        public EntitySource_ByProjectileSourceId projectileSource;
        public override void OnHitPlayer(Player Target, int damage, bool crit)
        {
            Target.AddBuff(BuffID.ShadowFlame, 180);
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
