using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using BloodSoul.NPCs;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using BloodErosion.Items.MasterTrophy;
using BloodErosion.Items.Boss.SpearOfCanglanGods;

namespace BloodErosion.NPCs.Bosses.SpearOfCanglanGod.SpearOfCanglanGod2
{
    [AutoloadBossHead]
    class SpearOfCanglanGod_2 : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;
        private int interval = 0;
        private int Time1 = 0;
        private int Time3 = 0;
        private int Time2 = 0;
        private int leavl = 0;
        private Vector2 targetOldPos;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear Of Canglan God");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "苍岚神之矛");
            Main.npcFrameCount[NPC.type] = 4;
        }
        private enum SpearOfCanglanGodAI
        { 
            HeadLighting,//头顶电弹幕
            LightingSprint,//闪电突刺
            RotatingLightning,//旋转闪电
            ContinuousLightning,//连续闪电
            LightingSprint2,//闪电突刺2
            Shotgun,//散弹
            Lightning,//闪电
            Lightning2,//闪电2
            ThunderS,//雷矛
            FinalThunder,//最终雷霆
        }
        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 128;
            NPC.height = 128;
            NPC.value = 70000;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.aiStyle = -1;
            NPC.scale = 1f;
            NPC.lifeMax = 155000 / 3;
            NPC.defense = 55;
            NPC.damage = 185 / 3;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Spear");
            }
        }
        public override void FindFrame(int frameHeight)
        {
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
            if (Main.player[NPC.target].dead)
            {
                NPC.TargetClosest(false);
                if (Main.player[NPC.target].dead)
                {
                    DespawnHandler();
                }
                return;
            }
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) NPC.TargetClosest();
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.Pi / 4;

            Vector2 toTarget = Target.position - NPC.position;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 50)
            {
                Vector2 center = NPC.Center + (i.ToRotationVector2() * 1200);
                int dust = Dust.NewDust(center, 1, 1, DustID.Electric);
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
                SwitchState2(0);
                SwitchState1((int)SpearOfCanglanGodAI.ThunderS, (int)SpearOfCanglanGodAI.ThunderS + 1);
            }
            if (NPC.life < NPC.lifeMax * 0.5f && leavl == 1)
            {
                leavl = 2;
                Time1 = 0;
                Time2 = 0;
                SwitchState2(0);
                SwitchState1((int)SpearOfCanglanGodAI.FinalThunder, (int)SpearOfCanglanGodAI.FinalThunder + 1);
            }
            if (NPC.life < NPC.lifeMax * 0.3f && leavl == 2)
            {
                leavl = 3;
                Time1 = 0;
                Time2 = 0;
                SwitchState2(0);
                SwitchState1((int)SpearOfCanglanGodAI.ThunderS, (int)SpearOfCanglanGodAI.ThunderS + 1);
            }


            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            int Cen = (X > 0) ? 1 : -1;

            int LProj = ModContent.ProjectileType<LightingProj>();
            int LProj2 = ModContent.ProjectileType<LightingProj2>();
            int LProj3 = ModContent.ProjectileType<LightingProj3>();
            int TL2 = ModContent.ProjectileType<ThunderLightning2>();
            int TL4 = ModContent.ProjectileType<ThunderLightning4>();
            int L = ModContent.ProjectileType<LightningProjectile2>();
            int T = ModContent.ProjectileType<SpearOfCanglanGodProj>();

            switch ((SpearOfCanglanGodAI)State1)
            {
                case SpearOfCanglanGodAI.HeadLighting:
                {
                        Time1++;
                        Time2++;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 250);
                        float ToHead = Vector2.Distance(NPC.Center, Head);
                        NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                            .SafeNormalize(Vector2.UnitX) * 15) / 11;
                        if (Time1 >= 30)
                        {
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, (NPC.rotation - MathHelper.Pi / 5f).ToRotationVector2() * 22, LProj, 155 / 3, 2f, Main.myPlayer);
                                interval++;
                                Time3 = 0;
                            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, NPC.position);
                            Time1 = 0;
                        }
                        if (Time2 >= 300)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.Lightning2 + 1);
                        }
                        break;
                }
                case SpearOfCanglanGodAI.LightingSprint:
                    {
                        switch(State2)
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
                                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.Lightning2 + 1);
                                        }
                                        else
                                        {
                                            NPC.velocity *= 0f;
                                            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, NPC.position);
                                            for (int i = 0; i < 20; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 10)).ToRotationVector2() * 16;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, LProj, 155 / 3, 0f, Main.myPlayer);
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
                case SpearOfCanglanGodAI.RotatingLightning:
                    {
                        Time1++;
                        Time2++;
                        NPC.velocity *= 0.1f;
                        NPC.rotation += Time1;
                        if (Time1 > 5)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, NPC.position);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, NPC.rotation.ToRotationVector2() * 12, TL2, 145 / 3, 0, Main.myPlayer, 0, 0);
                        }
                        if(Time2 > 300)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.Lightning2 + 1);
                        }
                        break;
                    }
                case SpearOfCanglanGodAI.ContinuousLightning:
                    {
                        Time1++;
                        Time2++;
                        float accX = 0.5f;
                        float accY = 0.5f;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = TargetVel * 6.5f;
                        if (Time1 > 5)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToTarget * 1.2f, LProj3, 145 / 3, 0, Main.myPlayer, 0, 0);
                            Time1 = 0;
                        }
                        if (Time2 > 300)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.Lightning2 + 1);
                        }
                        break;
                    }
                case SpearOfCanglanGodAI.LightingSprint2:
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
                                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.Lightning2 + 1);
                                        }
                                        else
                                        {
                                            NPC.velocity *= 0f;
                                            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, NPC.position);
                                            for (int i = 0; i < 16; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 8)).ToRotationVector2() * 16;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, LProj2, 155 / 3, 0f, Main.myPlayer);
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
                case SpearOfCanglanGodAI.Shotgun:
                    {
                        float accX = 0.6f;
                        float accY = 0.6f;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = TargetVel * 6.5f;
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
                                Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, TL2, 155 / 3, 2, player.whoAmI);
                            }
                        }
                        if (Time2 > 100)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.Lightning2 + 1);

                        }
                        break;
                    }
                case SpearOfCanglanGodAI.Lightning:
                    {
                        Time1++;
                        Time2++;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        Vector2 Head = new Vector2(Target.Center.X - 200, Target.Center.Y + 5);
                        Vector2 Head2 = new Vector2(Target.Center.X + 200, Target.Center.Y + 5);
                        float accX = 0.5f;
                        float accY = 0.5f;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = TargetVel * 6.5f;
                        if (Time1 > 60)
                        {
                            SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), Head, ToTarget * 0f, L, 200 / 3, 0, Main.myPlayer, 0, 0);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), Head2, ToTarget * 0f, L, 200 / 3, 0, Main.myPlayer, 0, 0);
                            Time1 = 0;
                        }
                        if (Time2 > 130)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.Lightning2 + 1);
                        }
                        break;
                    }
                case SpearOfCanglanGodAI.Lightning2:
                    {
                        Time1++;
                        Time2++;
                        if (Timer1 < 50)
                        {
                            targetOldPos = Target.position;
                        }
                        var player = Main.player[NPC.target];
                        float accX = 0.5f;
                        float accY = 0.5f;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = TargetVel * 6.5f;
                        if (Time1 > 30)
                        {
                            SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), targetOldPos, ToTarget * 0f, L, 180 / 3, 0, Main.myPlayer, 0, 0);
                            Time1 = 0;
                        }
                        if (Time2 > 300)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.Lightning2 + 1);
                        }
                        break;
                    }
                case SpearOfCanglanGodAI.FinalThunder:
                    {
                        NPC.rotation = NPC.velocity.ToRotation() - MathHelper.Pi / 4;
                        NPC.velocity *= 0;
                        NPC.dontTakeDamage = true;
                        Timer1++;
                        switch (Timer1)
                        {
                            case 30:
                                {
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    break;
                                }
                            case 90:
                                {
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 200, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 200, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 200, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 200, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    break;
                                }
                            case 100:
                                {
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 300, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 300, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 300, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 300, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);

                                    break;
                                }
                            case 110:
                                {
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 400, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 400, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 400, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 400, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    break;
                                }
                            case 120:
                                {
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 500, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 500, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 500, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 500, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    break;
                                }
                            case 130:
                                {
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 600, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 600, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 600, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 600, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    break;
                                }
                            case 140:
                                {
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 700, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 700, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 700, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 700, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    break;
                                }
                            case 150:
                                {
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 800, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 800, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 800, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 800, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    break;
                                }
                            case 160:
                                {
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 900, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 900, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 900, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 900, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    break;
                                }
                            case 170:
                                {
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 1000, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 1000, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 1000, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 1000, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    break;
                                }
                            case 180:
                                {
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 1100, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 1100, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 1100, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 1100, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    break;
                                }
                            case 190:
                                {
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 1200, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 1200, NPC.Center.Y + 5), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X + 1200, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(NPC.Center.X - 1200, NPC.Center.Y + 350), ToTarget * 0f, L, 250 / 3, 0, Main.myPlayer, 0, 0);
                                    break;
                                }
                        }
                        if (Timer1 > 200)
                        {
                            SwitchState2(0);
                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.Lightning2 + 1);
                            NPC.dontTakeDamage = false;
                            Timer1 = 0;
                        }
                        break;
                    }
                case SpearOfCanglanGodAI.ThunderS:
                    {
                        Time1++;
                        Time2++;
                        if (Timer1 < 15)
                        {
                            targetOldPos = Target.position;
                        }
                        var player = Main.player[NPC.target];
                        float accX = 0.5f;
                        float accY = 0.5f;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = TargetVel * 6.5f;
                        if (Time1 > 30)
                        {
                            SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                            Vector2 plrToMouse = player.Center - NPC.Center;
                            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                            for (int i = 1; i <= 1; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 72f;
                                Vector2 shootVel = r2.ToRotationVector2() * 20.5f;
                                Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, T, 155 / 3, 2, player.whoAmI);
                            }
                            Time1 = 0;
                        }
                        if (Time2 > 60)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.Lightning2 + 1);
                        }
                        break;
                    }
            }
        }
        public override void OnKill()
        {
            var player = Main.player[NPC.target];
            Vector2 ToPlayer = player.Center - NPC.Center;
            for (int i = 0; i < 16; i++)
            {
                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 8)).ToRotationVector2() * 13;
                Projectile.NewProjectile(Source_NPC, NPC.position, r,
                ModContent.ProjectileType<ThunderLightning2>(), 185 / 3, 0f, Main.myPlayer);
                interval++;
            }
            NPC.active = false;
            SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
            Projectile.NewProjectile(projectileSource, NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<LightningProjectile3>(), 0, 0, 0);
            base.OnKill();
        }
        public EntitySource_ByProjectileSourceId projectileSource;
        public override void OnHitPlayer(Player Target, int damage, bool crit)
        {
            Target.AddBuff(144, 120);
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
