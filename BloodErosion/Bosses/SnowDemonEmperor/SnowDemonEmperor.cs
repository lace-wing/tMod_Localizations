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

namespace BloodErosion.NPCs.Bosses.SnowDemonEmperor
{
    [AutoloadBossHead]
    class SnowDemonEmperor : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;
        private int interval = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;
        private enum SnowDemonEmperorAI
        {
            SnowThorn,//雪斩
            SnowBlock,//血块
            SnowLight,//雪光
            FrostClawProj,//雪爪弹幕
            FrostClaw,//雪爪
            IcePhlegm,//冰痰
            BigIceBlock//大雪块
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snow Demon Emperor");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "雪妖皇");
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 44000 / 3;
            NPC.defense = 15;
            NPC.damage = 135 / 3;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 82;
            NPC.height = 56;
            NPC.value = 10000;
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
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SnowDemon");
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
            if (Main.netMode == 2)
            {
                writer.Write(interval);
                writer.Write(State3);
                writer.Write(State4);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode != 1)
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
            bool forceChange = false;

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
                Vector2 center = NPC.Center + (i.ToRotationVector2() * 1400);
                int dust = Dust.NewDust(center, 1, 1, DustID.IceTorch);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
            if (NPC.position.Distance(Target.position) > 1400)
            {
                Target.velocity = -toTarget.SafeNormalize(toTarget) * 3;
            }

            if (NPC.active)
            {
                Main.raining = true;
                Main.rainTime = 450;
                Main.maxRain = 1;
                Main.maxRaining = 1;
            }
            switch ((SnowDemonEmperorAI)State1)
            {
                case SnowDemonEmperorAI.SnowThorn:
                    {
                        accX = 0.02f;
                        accY = 0.02f;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        Time2++;
                        Timer++;
                        if (Timer == 1000)
                        {
                            // 重置计时器
                            Timer = 0;
                        }
                        if (Timer % 35 == 0)
                        {
                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    for (int h = -0; h <= 0; h++)
                                    {
                                        Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 8)).ToRotationVector2() * 13;
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r,
                                        348, 135 / 6, 0f, Main.myPlayer);
                                        interval++;
                                        SoundEngine.PlaySound(SoundID.Item101, NPC.position);
                                    }
                                }
                                return;
                            }
                        }
                        if (Time2 > 160)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)SnowDemonEmperorAI.SnowThorn, (int)SnowDemonEmperorAI.BigIceBlock + 1);

                        }
                        break;
                    }
                case SnowDemonEmperorAI.SnowBlock:
                    {
                        accX = 0.02f;
                        accY = 0.02f;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        Time2++;
                        Timer++;
                        if (Timer == 1000)
                        {
                            // 重置计时器
                            Timer = 0;
                        }
                        if (Timer % 35 == 0)
                        {

                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];

                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    for (int h = -1; h <= 1; h++)
                                    {
                                        Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 8)).ToRotationVector2() * 17;
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r,
                                        349, 135 / 6, 0f, Main.myPlayer);
                                        interval++;
                                        SoundEngine.PlaySound(SoundID.Item101, NPC.position);
                                    }
                                }
                                return;
                            }
                        }
                        if (Time2 > 160)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)SnowDemonEmperorAI.SnowThorn, (int)SnowDemonEmperorAI.BigIceBlock + 1);

                        }
                        break;
                    }
                case SnowDemonEmperorAI.SnowLight:
                    {
                        accX = 0.02f;
                        accY = 0.02f;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        Time2++;
                        Timer++;
                        if (Timer == 1000)
                        {
                            // 重置计时器
                            Timer = 0;
                        }
                        if (Timer % 35 == 0)
                        {

                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];

                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    for (int h = -1; h <= 1; h++)
                                    {
                                        Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 8)).ToRotationVector2() * 17;
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r,
                                        ModContent.ProjectileType<SnowLight>(), 135 / 6, 0f, Main.myPlayer);
                                        interval++;
                                        SoundEngine.PlaySound(SoundID.Item101, NPC.position);
                                    }
                                }
                                return;
                            }
                        }
                        if (Time2 > 160)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)SnowDemonEmperorAI.SnowThorn, (int)SnowDemonEmperorAI.BigIceBlock + 1);

                        }
                        break;
                    }
                case SnowDemonEmperorAI.FrostClawProj:
                    {
                        switch (State2)
                        {
                            case 0://冲刺
                                {
                                    Time1++;
                                    if (Time1 > 45)
                                    {
                                        Time2++;
                                        SoundEngine.PlaySound(SoundID.Item101, NPC.position);
                                        Vector2 ToPlayer = (ToTarget * 1.8f) + Target.velocity;
                                        NPC.velocity = ToPlayer / 2.5f;
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
                                        if (Time2 > 8)
                                        {
                                            NPC.velocity *= 0.9f;
                                            var player = Main.player[NPC.target];
                                            Vector2 ToPlayer = player.Center - NPC.Center;
                                            SoundEngine.PlaySound(SoundID.Item101, NPC.position);
                                            for (int i = 0; i < 6; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 3)).ToRotationVector2() * 12;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, ModContent.ProjectileType<FrostClawProj>(), 130 / 6, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            SwitchState2(0);
                                            SwitchState1((int)SnowDemonEmperorAI.SnowThorn, (int)SnowDemonEmperorAI.BigIceBlock + 1);
                                            Time2 = 0;
                                        }
                                        else
                                        {
                                            NPC.velocity *= 0.9f;
                                            var player = Main.player[NPC.target];
                                            Vector2 ToPlayer = player.Center - NPC.Center;
                                            SoundEngine.PlaySound(SoundID.Item101, NPC.position);
                                            for (int i = 0; i < 6; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 3)).ToRotationVector2() * 12;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f,
                                                ModContent.ProjectileType<FrostClawProj>(), 135 / 6, 0f, Main.myPlayer);
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
                case SnowDemonEmperorAI.FrostClaw:
                    {

                        NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<FrostClaw>(), NPC.whoAmI);
                        NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<FrostClaw>(), NPC.whoAmI);
                        SwitchState2(0);
                        SwitchState1((int)SnowDemonEmperorAI.SnowThorn, (int)SnowDemonEmperorAI.BigIceBlock + 1);
                        break;
                    }
                case SnowDemonEmperorAI.IcePhlegm:
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
                        if (Timer % 10 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item101, NPC.position);
                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];

                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    for (int h = -9; h <= 6; h++)
                                    {
                                        Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 8)).ToRotationVector2() * 17;
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r,
                                        ModContent.ProjectileType<IceThorn>(), 135 / 6, 0f, Main.myPlayer);
                                        interval++;
                                        SoundEngine.PlaySound(SoundID.Item101, NPC.position);
                                    }
                                }
                                return;
                            }
                        }
                        if (Time2 > 200)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)SnowDemonEmperorAI.SnowThorn, (int)SnowDemonEmperorAI.BigIceBlock + 1);
                        }
                        break;
                    }
                case SnowDemonEmperorAI.BigIceBlock:
                    {
                        Time1++;
                        if (Time1 < 220)
                        {
                            Time2++;
                            Timer++;
                            if (Timer == 1000)
                            {
                                Timer = 0;
                            }
                            if (Timer % 45 == 0)
                            {

                                // NPC的攻击目标
                                Player p = Main.player[NPC.target];
                                Vector2 ToPlayer = p.Center - NPC.Center;
                                //ToPlayer.Normalize();
                                {
                                    for (int i = 1; i <= 1; i++)
                                    {
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, (NPC.rotation - MathHelper.Pi / 2).ToRotationVector2() * 10
                                                        , ModContent.ProjectileType<IceBlock>(), 270 / 6, 2f, Main.myPlayer);
                                        SoundEngine.PlaySound(SoundID.Item73, NPC.position);
                                        interval++;
                                    }
                                    return;
                                }
                            }
                        }
                        if (Time2 > 200)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)SnowDemonEmperorAI.SnowThorn, (int)SnowDemonEmperorAI.BigIceBlock + 1);

                        }
                        break;
                    }
            }
        }
        public override void OnHitPlayer(Player Target, int damage, bool crit)
        {
            Target.AddBuff(BuffID.Frozen, 15);
        }
        public override void OnKill()
        {
            Main.raining = false;
            Main.rainTime = 0;
            Main.maxRain = 0;
            base.OnKill();
        }
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            player = Main.player[NPC.target];
            if (!player.active || player.dead || Main.dayTime)
            {
                NPC.velocity.X = 0;
                NPC.velocity.Y -= 1;
            }
        }
    }

}