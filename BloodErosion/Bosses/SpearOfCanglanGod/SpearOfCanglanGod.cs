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
using BloodErosion.NPCs.Bosses.SpearOfCanglanGod.SpearOfCanglanGod2;

namespace BloodErosion.NPCs.Bosses.SpearOfCanglanGod
{
    [AutoloadBossHead]
    class SpearOfCanglanGod : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;
        private int interval = 0;
        private int Time1 = 0;
        private int Time3 = 0;
        private int Time2 = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear Of Canglan God·Phantom");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "苍岚神之矛·幻影");
            Main.npcFrameCount[NPC.type] = 1;
        }
        private enum SpearOfCanglanGodAI
        { 
            HeadLighting,//头顶电弹幕
            LightingSprint,//闪电突刺
            ContinuousLightning,//连续闪电
            LightingSprint2,//闪电突刺2
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
            NPC.lifeMax = 52500 / 3;
            NPC.defense = 55;
            NPC.damage = 160 / 3;
            NPC.alpha = 100;
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
                int dust = Dust.NewDust(center, 1, 1, DustID.FlameBurst);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
            if (NPC.position.Distance(Target.position) > 1200)
            {
                Target.velocity = -toTarget.SafeNormalize(toTarget) * 3;
            }

            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            int Cen = (X > 0) ? 1 : -1;

            int LProj = ModContent.ProjectileType<LightingProj>();
            int LProj2 = ModContent.ProjectileType<LightingProj2>();
            int LProj3 = ModContent.ProjectileType<LightingProj3>();

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
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, (NPC.rotation - MathHelper.Pi / 5f).ToRotationVector2() * 22, LProj, 185 / 3, 2f, Main.myPlayer);
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
                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.LightingSprint2 + 1);
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
                                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.LightingSprint2 + 1);
                                        }
                                        else
                                        {
                                            NPC.velocity *= 0f;
                                            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, NPC.position);
                                            for (int i = 0; i < 20; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 10)).ToRotationVector2() * 16;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, LProj, 185 / 3, 0f, Main.myPlayer);
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
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToTarget * 1.2f, LProj3, 165 / 3, 0, Main.myPlayer, 0, 0);
                            Time1 = 0;
                        }
                        if (Time2 > 300)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.LightingSprint2 + 1);
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
                                            SwitchState1((int)SpearOfCanglanGodAI.HeadLighting, (int)SpearOfCanglanGodAI.LightingSprint2 + 1);
                                        }
                                        else
                                        {
                                            NPC.velocity *= 0f;
                                            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, NPC.position);
                                            for (int i = 0; i < 16; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 8)).ToRotationVector2() * 16;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, LProj2, 185 / 3, 0f, Main.myPlayer);
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
        public override void OnKill()
        {
            var player = Main.player[NPC.target];
            Vector2 ToPlayer = player.Center - NPC.Center;
            for (int i = 0; i < 16; i++)
            {
                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 8)).ToRotationVector2() * 13;
                Projectile.NewProjectile(Source_NPC, NPC.position, r,
                ModContent.ProjectileType<LightingProj>(), 185 / 3, 0f, Main.myPlayer);
                interval++;
            }
            Main.NewText("那么就与你真正的战斗吧", Color.LightBlue);
            CombatText.NewText(NPC.Hitbox, Color.LightBlue, "那么就与你真正的战斗吧", true, false);
            NPC.active = false;
            SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
            Projectile.NewProjectile(projectileSource, NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<LightningProjectile>(), 0, 0, 0);
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
