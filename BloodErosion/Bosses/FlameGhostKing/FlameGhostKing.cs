using BloodErosion.Items.Boss.FlameGhostKings;
using BloodSoul;
using BloodSoul.MyUtils;
using BloodSoul.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.FlameGhostKing
{
    [AutoloadBossHead]
    class FlameGhostKing : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;
        private int interval = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;
        private int leavl = 0;
        private enum FlameGhostKingAI
        {
            ScatteringBouncingProjectile,//散射弹跳火弹
            Spike2,//连续突刺
            ExplosiveBurst,//爆炸连射
            Soldier,//士兵
            RotatingFlame,//旋转火焰
            VoidFlame,//虚空火焰
            ShotgunFire,//散弹连射
            FlameRay,//王炎射线
            Claw1,//爪1
            Claw2,//爪2
            Death,//死亡
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame Ghost King");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "炎鬼王");
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 78000 / 3;
            NPC.defense = 20;
            NPC.damage = 145 / 3;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 298;
            NPC.height = 202;
            NPC.value = 70000;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.aiStyle = -1;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.scale = 1f;
            BossBag = ModContent.ItemType<FlameGhostKingBossBag>();
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BurningGhostKing");
            }

        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));
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
        public int E = 0;
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
            bool forceChange = false;

            Vector2 toTarget = Target.position - NPC.position;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 50)
            {
                Vector2 center = NPC.Center + (i.ToRotationVector2() * 1200);
                int dust = Dust.NewDust(center, 1, 1, DustID.FlameBurst);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
            if (NPC.position.Distance(Target.position) > 1200)
            {
                Target.velocity = -toTarget.SafeNormalize(toTarget) * 3;
            }

            if (NPC.life < NPC.lifeMax * 0.7f && leavl == 0)
            {
                leavl = 1;
                Time1 = 0;
                Time2 = 0;
                Timer = 0;
                SwitchState1((int)FlameGhostKingAI.Claw1, (int)FlameGhostKingAI.Claw2 + 1);
                E = 1;
                int num1 = NPC.whoAmI;
                int SpawnBody1 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + NPC.width / 2), (int)(NPC.Center.Y + NPC.height), ModContent.NPCType<FlameGhostKingLeftClaw>(), NPC.whoAmI);
                Main.npc[SpawnBody1].ai[3] = num1;
                //Main.npc[SpawnBody1].ai[1] = num1;
                NPC.netUpdate = true;
                num1 = NPC.whoAmI;
                int SpawnWing1 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + NPC.width / 2), (int)(NPC.Center.Y + NPC.height), ModContent.NPCType<FlameGhostKingRightClaw>(), NPC.whoAmI);
                Main.npc[SpawnWing1].ai[3] = num1;
                //Main.npc[SpawnWing1].ai[1] = num1;
            }

            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            TargetVel *= 8f;
            float accX = 0.1f;
            float accY = 0.1f;
            NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;

            switch ((FlameGhostKingAI)State1)
            {
                case FlameGhostKingAI.ScatteringBouncingProjectile:
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
                        if (Timer % 50 == 0)
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
                                        258, 65 / 3, 0f, Main.myPlayer);
                                        interval++;
                                        SoundEngine.PlaySound(SoundID.Item20, NPC.position);
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
                            if (E == 0)
                            {
                                SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.FlameRay + 1);
                            }
                            else if (E == 1)
                            {
                                SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.Claw2 + 1);
                            }//切换ai

                        }
                        break;
                    }
                case FlameGhostKingAI.Spike2:
                    {
                        switch (State2)
                        {
                            case 0://冲刺
                                {
                                    Time1++;
                                    if (Time1 > 45)
                                    {
                                        Time2++;
                                        SoundEngine.PlaySound(SoundID.ForceRoar, (int)NPC.position.X, (int)NPC.position.Y, 0, 2f, -0.75f);
                                        var modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 20f, 6f, 30, 1000f);
                                        Main.instance.CameraModifiers.Add(modifier);
                                        NPC.velocity = ToTarget * 0.8f;
                                        Time1 = 0;
                                        SwitchState2(1);
                                    }
                                    break;
                                }
                            case 1://保持速度
                                {
                                    Time1++;

                                    if (Time1 > 30)
                                    {
                                        Time1 = 0;
                                        Time2++;
                                        if (Time2 > 4)
                                        {
                                            NPC.velocity *= 1f;
                                            var player = Main.player[NPC.target];
                                            Vector2 ToPlayer = player.Center - NPC.Center;
                                            for (int i = 0; i < 12; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 6)).ToRotationVector2() * 9;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r,
                                                328, 65 / 3, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            for (int i = 0; i < 6; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 3)).ToRotationVector2() * 9;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r,
                                                292, 65 / 3, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            //SwitchState2(1);
                                            Time1 = 0;
                                            Time2 = 0;
                                            Timer = 0;
                                            if (E == 0)
                                            {
                                                SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.ShotgunFire + 1);
                                            }
                                            else if (E == 1)
                                            {
                                                SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.Claw2 + 1);
                                            }//切换ai
                                        }
                                        else
                                        {
                                            NPC.velocity *= 1f;
                                            var player = Main.player[NPC.target];
                                            Vector2 ToPlayer = player.Center - NPC.Center;
                                            for (int i = 0; i < 8; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 4)).ToRotationVector2() * 11;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r,
                                                328, 145 / 6, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            for (int i = 0; i < 6; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 3)).ToRotationVector2() * 9;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r,
                                                292, 145 / 6, 0f, Main.myPlayer);
                                            }
                                            SwitchState2(0);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case FlameGhostKingAI.ExplosiveBurst:
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
                        if (Timer % 25 == 0)
                        {

                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];

                            {
                                for (int i = 1; i <= 1; i++)
                                {
                                    for (int h = -0; h <= 0; h++)
                                    {
                                        Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 8)).ToRotationVector2() * 5;
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r,
                                        292, 145 / 6, 0f, Main.myPlayer);
                                        interval++;
                                        SoundEngine.PlaySound(SoundID.Item20, NPC.position);
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
                            if (E == 0)
                            {
                                SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.FlameRay + 1);
                            }
                            else if (E == 1)
                            {
                                SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.Claw2 + 1);
                            }//切换ai

                        }
                        break;
                    }
                case FlameGhostKingAI.Soldier:
                    {
                        Time1++;
                        if (Time1 > 50)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<HellRockGhostS>(), NPC.whoAmI);
                            SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.ShotgunFire + 1);
                        }
                        break;
                    }
                case FlameGhostKingAI.RotatingFlame:
                    {
                        float V = NPC.Center.X - Target.Center.X;
                        int VAN = (V > 0) ? 1 : -1;
                        switch (State2)
                        {

                            case 0://发射弹幕
                                {
                                    Time1++;
                                    if (Time1 == 25)
                                    {
                                        Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                            .ToRotationVector2() * 300;
                                        Dust.NewDustDirect(center, 10, 10, MyDustId.YellowGoldenFire);
                                        NPC.netUpdate = true;
                                    }
                                    if (Time1 > 25)
                                    {
                                        if (Time1 % 20 == 0 && Main.netMode != 1)
                                        {
                                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, (NPC.rotation - MathHelper.Pi / 2).ToRotationVector2() * 12
                                                , 258, 145 / 6, 2f, Main.myPlayer);
                                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, (NPC.rotation - MathHelper.Pi / -2).ToRotationVector2() * 12
                                                , 258, 145 / 6, 2f, Main.myPlayer);
                                            SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                                            interval++;
                                            Time2++;
                                        }
                                    }
                                    if (Time2 > 20)
                                    {
                                        Time1 = 0;
                                        Time2 = 0;
                                        Timer = 0;
                                        SwitchState2(0);
                                        if (E == 0)
                                        {
                                            SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.FlameRay + 1);
                                        }
                                        else if (E == 1)
                                        {
                                            SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.Claw2 + 1);
                                        }//切换ai
                                    }
                                    break;
                                }

                        }
                        break;
                    }
                case FlameGhostKingAI.VoidFlame:
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
                                        Dust.NewDustDirect(center, 10, 10, MyDustId.YellowGoldenFire);
                                        NPC.position = center;
                                        NPC.rotation = ToTarget.ToRotation();
                                        NPC.velocity = NPC.rotation.ToRotationVector2();
                                        NPC.netUpdate = true;
                                    }
                                    if (Time1 > 25)
                                    {
                                        Time1++;
                                        Vector2 ToPlayer = (ToTarget * 0.8f);
                                        for (int i = 0; i < 1; i++)
                                        {
                                            Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 3)).ToRotationVector2() * 9;
                                            Projectile.NewProjectile(Source_NPC, NPC.Center, r,
                                            292, 65 / 3, 0f, Main.myPlayer);
                                            interval++;
                                        }
                                        SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                                        if (Time1 > 45)
                                        {
                                            SoundEngine.PlaySound(SoundID.ForceRoar, (int)NPC.position.X, (int)NPC.position.Y, 0, 2f, -0.75f);
                                            var modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 20f, 6f, 30, 1000f);
                                            Main.instance.CameraModifiers.Add(modifier);
                                            NPC.velocity = ToPlayer;
                                            Time1 = 0;
                                            Time2 = 0;
                                            Timer = 0;
                                            SwitchState2(0);
                                            if (E == 0)
                                            {
                                                SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.FlameRay + 1);
                                            }
                                            else if (E == 1)
                                            {
                                                SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.Claw2 + 1);
                                            }//切换ai
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case FlameGhostKingAI.ShotgunFire:
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
                        if (Timer % 50 == 0)
                        {
                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];
                            Vector2 plrToMouse = player.Center - NPC.Center;
                            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                            for (int i = -3; i <= 3; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 60f;
                                Vector2 shootVel = r2.ToRotationVector2() * 13.5f;
                                Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, 258, 145 / 6, 2, player.whoAmI);
                            }
                            return;
                        }
                        if (Time2 > 100)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            if (E == 0)
                            {
                                SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.FlameRay + 1);
                            }
                            else if (E == 1)
                            {
                                SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.Claw2 + 1);
                            }//切换ai

                        }
                        break;
                    }
                case FlameGhostKingAI.FlameRay:
                    {
                        NPC.velocity *= 0.01f;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = (ToTarget * 2f) + Target.velocity;
                        Time2++;
                        Timer++;
                        if (Timer == 1000)
                        {
                            // 重置计时器
                            Timer = 0;
                        }
                        if (Timer % 25 == 0)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToPlayer,
                            ModContent.ProjectileType<BurningRay2>(), 165 / 6, 0f, Main.myPlayer);
                            interval++;
                            SoundEngine.PlaySound(SoundID.Item117, NPC.position);
                            return;
                        }
                        if (Time2 > 200)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            if (E == 0)
                            {
                                SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.ShotgunFire + 1);
                            }
                            else if (E == 1)
                            {
                                SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.Claw2 + 1);
                            }//切换ai
                        }
                        break;
                    }
                case FlameGhostKingAI.Claw1:
                    {
                        Time1++;
                        if (Time1 == 20)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<FlameGhostKingLeftClaw>() || n.type == ModContent.NPCType<FlameGhostKingRightClaw>() && n.active)
                                {
                                    n.ai[0] = 1;
                                    n.netUpdate = true;
                                }
                            }
                        }
                        if (Time1 >= 300)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.ShotgunFire + 1);
                        }
                        break;
                    }
                case FlameGhostKingAI.Claw2:
                    {
                        Time1++;
                        if (Time1 == 20)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<FlameGhostKingLeftClaw>() || n.type == ModContent.NPCType<FlameGhostKingRightClaw>() && n.active)
                                {
                                    n.ai[0] = 2;
                                    n.netUpdate = true;
                                }
                            }
                        }
                        if (Time1 >= 300)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState1((int)FlameGhostKingAI.ScatteringBouncingProjectile, (int)FlameGhostKingAI.ShotgunFire + 1);
                        }
                        break;
                    }

                case FlameGhostKingAI.Death:
                    {
                        foreach (NPC n in Main.npc)
                        {
                            if (n.type == ModContent.NPCType<FlameGhostKingLeftClaw>() || n.type == ModContent.NPCType<FlameGhostKingRightClaw>() && n.active)
                            {
                                n.ai[0] = 0;
                                n.netUpdate = true;
                            }
                        }
                        NPC.velocity *= 0;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        // 角度随机增加变化
                        rad += Main.rand.NextFloatDirection() * 0.5f;
                        rad2 += Main.rand.NextFloatDirection() * 0.7f;
                        rad3 += Main.rand.NextFloatDirection() * 0.3f;
                        // 半径100，圆上的点
                        var pos = rad.ToRotationVector2() * 100;
                        var pos2 = rad2.ToRotationVector2() * 100;
                        var pos3 = rad3.ToRotationVector2() * 100;
                        Time2++;
                        Timer++;
                        if (Timer >= 20)
                        {

                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, pos,
                            ModContent.ProjectileType<BurningRay2>(), 0, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, pos2,
                            ModContent.ProjectileType<BurningRay2>(), 0, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, pos3,
                            ModContent.ProjectileType<BurningRay2>(), 0, 0f, Main.myPlayer);

                            var modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 20f, 8f, 30, 1200f);
                            Main.instance.CameraModifiers.Add(modifier);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, pos3 * 0f,
                            ModContent.ProjectileType<BurningBoom2>(), 0, 0f, Main.myPlayer);
                            interval++;
                            SoundEngine.PlaySound(SoundID.ForceRoar, (int)NPC.Center.X, (int)NPC.Center.Y, 0, 2f, -0.75f);
                            SoundEngine.PlaySound(SoundID.Item117, NPC.Center);
                            Timer = 0;
                        }
                        if (Time2 > 420)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<FlameGhostKingLeftClaw>() || n.type == ModContent.NPCType<FlameGhostKingRightClaw>() && n.active)
                                {
                                    n.ai[0] = 0;
                                    n.active = false;
                                    n.netUpdate = true;
                                }
                            }
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, pos * 0,
                            ModContent.ProjectileType<BurningBoom>(), 0, 0f, Main.myPlayer);
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
                            NPC.life = 0;
                            NPC.checkDead();
                        }
                        break;
                    }
            }
        }
        private float rad;
        private float rad2;
        private float rad3;
        public override void OnHitPlayer(Player Target, int damage, bool crit)
        {
            Target.AddBuff(BuffID.OnFire3, 300);
        }
        public override bool CheckDead()
        {
            if (State1 != (int)FlameGhostKingAI.Death)
            {
                SoundEngine.PlaySound(SoundID.ForceRoar, (int)NPC.Center.X, (int)NPC.Center.Y, 0, 2f, -0.75f);
                Timer1 = 0;
                Timer2 = 0;
                SwitchState2(0);
                NPC.life = 4000;
                NPC.dontTakeDamage = true;
                SwitchState1((int)FlameGhostKingAI.Death);
                return false;
            }
            return true;
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