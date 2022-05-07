using BloodSoul.MyUtils;
using BloodSoul.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.ApostleOfDeath
{
    [AutoloadBossHead]
    class AwakeningDeathApostles : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;
        private int interval = 0;
        private int EyeTime = 0;
        private int leavl = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;
        private int Time1 = 0;
        private int Time2 = 0;
        private int Time3 = 0;
        private int Time4 = 0;
        private enum ApostleOfDeathAI
        {
            Chop1,//普通斩
            SprintChop,//冲刺斩
            DeadLightDiffusion,//死光扩散
            ScatteringChopper,//散射斩
            SkyDeadLight,//天降死光
            NPCDeadLight,//环绕死光
            SprintDL,//冲刺死光
            DLBurst,//死光连射
            Ball,//死神爆弹
            Void,//虚空
            BlaaDiffusion,//爆弹扩散
            Diffusion,//散弹预言
            DeathFire//死火
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Awakening·Death Apostles");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "觉醒·死亡使者");
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 230000 / 3;
            NPC.defense = 35;
            NPC.damage = 185 / 3;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 146;
            NPC.height = 174;
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
            NPC.scale = 1f;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Death2");
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
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) NPC.TargetClosest();
            //if (!SkyManager.Instance["DeathSky"].IsActive()) SkyManager.Instance.Activate("DeathSky");//开启天空
            //Vector2 toTarget = Target.position - NPC.position;
            //BloodErosionGlobalNPC.AwakeningDeathApostles2 = NPC.whoAmI;
            //if (Main.dayTime) Main.dayTime = false;
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
            bool forceChange = false;
            if (NPC.life < NPC.lifeMax * 0.5f && leavl == 0)
            {
                leavl = 1;
                Time1 = 0;
                Time2 = 0;
                Main.NewText("死神之眼，将他拦下!", Color.Purple);
                CombatText.NewText(NPC.Hitbox, Color.Purple, "死神之眼，将他拦下!", true, false);
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<EyeOfDeath>(), NPC.whoAmI);
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<EyeOfDeath2>(), NPC.whoAmI);
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<EyeOfDeath3>(), NPC.whoAmI);
            }
            Vector2 toTarget = Target.position - NPC.position;
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 12;
            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            TargetVel *= 7f;
            float accX = 0.05f;
            float accY = 0.05f;
            NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
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

            int Chop = ModContent.ProjectileType<Chop2>();
            int DeadChop = ModContent.ProjectileType<DeadChop2>();
            int LD3 = ModContent.ProjectileType<DeadLight3>();
            int LD4 = ModContent.ProjectileType<DeadLight4>();
            int DB = ModContent.ProjectileType<DeathBomb>();

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
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToPlayer * 0, Chop, 125 / 3, 2f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToPlayer * 0.8f, DeadChop, 125 / 3, 2f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToTarget * 1.3f, DeadChop, 125 / 3, 2f, Main.myPlayer);
                            Time2++;
                            if (Time2 > 7)
                            {
                                Time1 = 0;
                                Time2 = 0;
                                SwitchState2(0);
                                SwitchState1((int)ApostleOfDeathAI.SprintChop, (int)ApostleOfDeathAI.DeathFire + 1);
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
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, DeadChop, 125 / 3, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                            .ToRotationVector2() * 350;
                                            Dust.NewDustDirect(center, 10, 10, DustID.Shadowflame);
                                            NPC.position = center;
                                            NPC.velocity = NPC.rotation.ToRotationVector2();
                                            NPC.netUpdate = true;
                                            SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                                            SwitchState2(0);
                                            SwitchState1((int)ApostleOfDeathAI.DeadLightDiffusion, (int)ApostleOfDeathAI.DeathFire + 1);
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
                                                DeadChop, 135 / 3, 0f, Main.myPlayer);
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
                        if (Timer % 20 == 0)
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
                                        ModContent.ProjectileType<DeadLight3>(), 135 / 3, 0f, Main.myPlayer);
                                        interval++;
                                    }
                                }
                                return;
                            }
                        }
                        if (Timer % 30 == 0)
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
                                        ModContent.ProjectileType<DeadLight3>(), 160 / 6, 0f, Main.myPlayer);
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
                            SwitchState1((int)ApostleOfDeathAI.Chop1, (int)ApostleOfDeathAI.DeathFire + 1);
                        }
                        break;
                    }
                case ApostleOfDeathAI.ScatteringChopper:
                    {
                        NPC.velocity = TargetVel * 0.1f;
                        var player = Main.player[NPC.target];
                        Time1++;
                        if (Time1 == 60)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, Target.Center, Chop, 135 / 6, 2f, Main.myPlayer);
                            Vector2 plrToMouse = player.Center - NPC.Center;
                            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                            for (int i = 1; i <= 11; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 90f;
                                Vector2 shootVel = r2.ToRotationVector2() * 11;
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, shootVel, DeadChop, 125 / 3, 2, Main.myPlayer);
                            }
                        }
                        if (Time1 == 120)
                        {
                            Time1 = 0;
                            SwitchState2(0);
                            SwitchState1((int)ApostleOfDeathAI.Chop1, (int)ApostleOfDeathAI.DeathFire + 1);
                        }
                        break;
                    }
                case ApostleOfDeathAI.SkyDeadLight:
                    {
                        accX = 0.02f;
                        accY = 0.02f;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity *= 1f;
                        Time1++;
                        switch (Time1)
                        {
                            case 60:
                            case 120:
                                {
                                    if (Main.netMode != 1)
                                    {
                                        for (float r = 0; r < MathHelper.TwoPi; r += MathHelper.TwoPi / 12)
                                        {
                                            Vector2 center = Target.Center + (r.ToRotationVector2() * 650);
                                            var proj = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(),
                                                center, Vector2.Normalize(Target.Center - center) * 5, LD4,
                                                200 / 6, 2.3f, Main.myPlayer);
                                            SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                                            proj.timeLeft = 1000;
                                            proj.tileCollide = false;
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    if (Time1 > 210)
                                    {
                                        Time1 = 0;
                                        SwitchState2(0);
                                        SwitchState1((int)ApostleOfDeathAI.Chop1, (int)ApostleOfDeathAI.DeathFire + 1);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case ApostleOfDeathAI.NPCDeadLight:
                    {
                        accX = 0.02f;
                        accY = 0.02f;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = (ToTarget * 2.1f) + Target.velocity;
                        Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 250);
                        NPC.velocity *= 1f;
                        Time2++;
                        Timer++;
                        if (Timer == 1000)
                        {
                            // 重置计时器
                            Timer = 0;
                        }
                        if (Timer % 45 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                            Vector2 plrToMouse = player.Center - NPC.Center;
                            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                            for (int i = 1; i <= 1; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 180f;
                                Vector2 shootVel = r2.ToRotationVector2() * 10;
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X, NPC.Center.Y - 150), shootVel, LD3, 200 / 6, 2, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 50, NPC.Center.Y - 100), shootVel, LD3, 200 / 6, 2, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 50, NPC.Center.Y - 100), shootVel, LD3, 200 / 6, 2, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 100, NPC.Center.Y - 50), shootVel, LD3, 200 / 6, 2, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 100, NPC.Center.Y - 50), shootVel, LD3, 200 / 6, 2, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X, NPC.Center.Y + 150), shootVel, LD3, 200 / 6, 2, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 50, NPC.Center.Y + 100), shootVel, LD3, 200 / 6, 2, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 50, NPC.Center.Y + 100), shootVel, LD3, 200 / 6, 2, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 100, NPC.Center.Y + 50), shootVel, LD3, 200 / 6, 2, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 100, NPC.Center.Y + 50), shootVel, LD3, 200 / 6, 2, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 150, NPC.Center.Y), shootVel, LD3, 200 / 6, 2, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 150, NPC.Center.Y), shootVel, LD3, 200 / 6, 2, Main.myPlayer);
                            }
                            return;
                        }
                        if (Time2 > 180)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)ApostleOfDeathAI.Chop1, (int)ApostleOfDeathAI.DeathFire + 1);
                        }
                        break;
                    }
                case ApostleOfDeathAI.SprintDL:
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
                                            for (int i = 0; i < 12; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 6)).ToRotationVector2() * 11;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, LD4, 125 / 3, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                            .ToRotationVector2() * 350;
                                            Dust.NewDustDirect(center, 10, 10, DustID.Shadowflame);
                                            NPC.position = center;
                                            NPC.velocity = NPC.rotation.ToRotationVector2();
                                            NPC.netUpdate = true;
                                            SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                                            SwitchState2(0);
                                            SwitchState1((int)ApostleOfDeathAI.Chop1, (int)ApostleOfDeathAI.DeathFire + 1);
                                            Time2 = 0;
                                        }
                                        else
                                        {
                                            NPC.velocity *= 1f;
                                            var player = Main.player[NPC.target];
                                            Vector2 ToPlayer = player.Center - NPC.Center;
                                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToPlayer * 0, Chop, 135 / 6, 2f, Main.myPlayer);
                                            for (int i = 0; i < 16; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 8)).ToRotationVector2() * 11;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f,
                                                LD4, 185 / 6, 0f, Main.myPlayer);
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
                case ApostleOfDeathAI.DLBurst:
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
                        if (Timer % 20 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];

                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    for (int h = 0; h <= 11; h++)
                                    {
                                        Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 6)).ToRotationVector2() * 12;
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r,
                                        ModContent.ProjectileType<DeadLight4>(), 200 / 6, 0f, Main.myPlayer);
                                        interval++;
                                    }
                                }
                                return;
                            }
                        }
                        if (Timer % 25 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];

                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    for (int h = 1; h <= 16; h++)
                                    {
                                        Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 8)).ToRotationVector2() * 10;
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r,
                                        ModContent.ProjectileType<DeadLight4>(), 120 / 3, 0f, Main.myPlayer);
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
                            SwitchState1((int)ApostleOfDeathAI.Chop1, (int)ApostleOfDeathAI.DeathFire + 1);
                        }
                        break;
                    }
                case ApostleOfDeathAI.Ball:
                    {
                        NPC.velocity = TargetVel * 0.1f;
                        var player = Main.player[NPC.target];
                        Time1++;
                        if (Time1 == 60)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, Target.Center, Chop, 135 / 3, 2f, Main.myPlayer);
                            Vector2 plrToMouse = player.Center - NPC.Center;
                            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                            for (int i = 1; i <= 3; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 18f;
                                Vector2 shootVel = r2.ToRotationVector2() * 10;
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, shootVel, DB, 120 / 3, 2, Main.myPlayer);
                            }
                        }
                        if (Time1 == 120)
                        {
                            Time1 = 0;
                            SwitchState2(0);
                            SwitchState1((int)ApostleOfDeathAI.Chop1, (int)ApostleOfDeathAI.DeathFire + 1);
                        }
                        break;
                    }
                case ApostleOfDeathAI.Void:
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
                                        Dust.NewDustDirect(center, 10, 10, DustID.Shadowflame);
                                        NPC.position = center;
                                        NPC.velocity = NPC.rotation.ToRotationVector2();
                                        NPC.netUpdate = true;
                                        SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                                    }
                                    if (Time1 > 25)
                                    {
                                        Time1++;
                                        Vector2 ToPlayer = (ToTarget * 0.8f);
                                        SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                                        if (Time1 > 25)
                                        {
                                            Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                            .ToRotationVector2() * 650;
                                            Dust.NewDustDirect(center, 10, 10, MyDustId.DemonTorch);
                                            NPC.position = center;
                                            NPC.rotation = ToTarget.ToRotation();
                                            NPC.netUpdate = true;
                                            for (int i = 0; i < 1; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 3)).ToRotationVector2() * 9;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r / 2, 468, 135 / 3, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                        }
                                        if (Time1 > 90)
                                        {
                                            Time1 = 0;
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)ApostleOfDeathAI.Chop1, (int)ApostleOfDeathAI.Ball + 1);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case ApostleOfDeathAI.BlaaDiffusion:
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
                        if (Timer % 45 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];

                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    for (int h = 0; h <= 4; h++)
                                    {
                                        Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 2)).ToRotationVector2() * 12;
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r,
                                        DB, 135 / 3, 0f, Main.myPlayer);
                                        interval++;
                                    }
                                }
                                return;
                            }
                        }
                        if (Timer % 30 == 0)
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
                                        468, 120 / 3, 0f, Main.myPlayer);
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
                            SwitchState1((int)ApostleOfDeathAI.Chop1, (int)ApostleOfDeathAI.DeathFire + 1);
                        }
                        break;
                    }
                case ApostleOfDeathAI.Diffusion:
                    {
                        NPC.velocity = TargetVel * 0.1f;
                        var player = Main.player[NPC.target];
                        Time1++;
                        if (Time1 == 60)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, Target.Center, Chop, 135 / 6, 2f, Main.myPlayer);
                            Vector2 plrToMouse = player.Center - NPC.Center;
                            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                            for (int i = 1; i <= 9; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 9f;
                                Vector2 shootVel = r2.ToRotationVector2() * 10;
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, shootVel, 593, 125 / 3, 2, Main.myPlayer);
                            }
                        }
                        if (Time1 == 120)
                        {
                            Time1 = 0;
                            SwitchState2(0);
                            SwitchState1((int)ApostleOfDeathAI.Chop1, (int)ApostleOfDeathAI.DeathFire + 1);
                        }
                        break;
                    }
                case ApostleOfDeathAI.DeathFire:
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
                        if (Timer % 35 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];

                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    for (int h = 0; h <= 10; h++)
                                    {
                                        Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 5)).ToRotationVector2() * 12;
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r,
                                        596, 125 / 3, 0f, Main.myPlayer);
                                        interval++;
                                    }
                                }
                                return;
                            }
                        }
                        if (Timer % 50 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];

                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    for (int h = -9; h <= 10; h++)
                                    {
                                        Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 10)).ToRotationVector2() * 10;
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r,
                                        675, 125 / 3, 0f, Main.myPlayer);
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
                            SwitchState1((int)ApostleOfDeathAI.Chop1, (int)ApostleOfDeathAI.DeathFire + 1);
                        }
                        break;
                    }
            }
        }
        public override bool CheckDead()
        {
            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<EyeOfDeath>() || n.type == ModContent.NPCType<EyeOfDeath3>() || n.type == ModContent.NPCType<EyeOfDeath2>() && n.active)
                {
                    n.active = false;
                    n.netUpdate = true;
                }
            }
            return base.CheckDead();
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