using BloodSoul.MyUtils;
using BloodSoul.Particle;
using BloodSoul.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.TheStarGazer
{
    [AutoloadBossHead]
    class StarGazerBoss : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;
        private int interval = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        private int Time3 = 0;
        private static float gravity = 0.3f;
        public Vector2 PlayerOldPos = Vector2.Zero;
        private int leavl = 0;
        public int i = 0;
        private enum StarGazerAI
        {
            S1,//开幕
            S2,//开场杀
            S3,//星弹
            S4,//加速弹
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Gazer");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "观星者");
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 25000 / 3;
            NPC.defense = 40;
            NPC.damage = 275;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 30;
            NPC.height = 50;
            NPC.value = 150000;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit3;
            NPC.DeathSound = SoundID.NPCHit55;
            NPC.aiStyle = -1;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.scale = 1f;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Star");
            }

        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossBag.>));
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.rotation = NPC.velocity.X * 0.05f;
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
                    NPC.active = false;
                }
                return;
            }
            bool forceChange = false;
            NPC.timeLeft = 999999;
            Vector2 toTarget = Target.position - NPC.position;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 50)
            {
                Vector2 center = NPC.Center + (i.ToRotationVector2() * 1700);
                int dust = Dust.NewDust(center, 1, 1, DustID.PurpleTorch);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
            if (NPC.position.Distance(Target.position) > 1700)
            {
                Target.velocity = -toTarget.SafeNormalize(toTarget) * 3;
            }
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 12;
            Vector2 targetVel = Vector2.Normalize(Target.position - NPC.Center);
            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            TargetVel *= 0f;
            float accX = 0.7f;
            float accY = 0.7f;
            NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
            

            if (Target.position.X - NPC.position.X > 0f)
            {
                NPC.spriteDirection = 1;
            }
            if (Target.position.X - NPC.position.X < 0f)
            {
                NPC.spriteDirection = -1;
            }

            if (NPC.life < NPC.lifeMax * 0.2f && leavl == 0)
            {
                leavl = 1;
                Time1 = 0;
                Time2 = 0;
                Timer1 = 0;
                Timer = 0;
                Timer3 = 0;
                Timer2 = 0;
                NPC.active = false;
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<StarGazerBoss2>(), NPC.whoAmI);
            }

            if (Target.dead)
            {
                Main.NewText("我会等着你的再次挑战", Color.Purple);
                CombatText.NewText(NPC.Hitbox, Color.Purple, "我会等着你的再次挑战", true, false);
            }

            int Boom = ModContent.ProjectileType<StarBoomProj2>();
            int Star = ModContent.ProjectileType<Star>();//星弹
            int Comet = ModContent.ProjectileType<Comet>();//星弹

            switch ((StarGazerAI)State1)
            {
                case StarGazerAI.S1:
                    {
                        Time3++;
                        Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 250);
                        float ToHead = Vector2.Distance(NPC.Center, Head);
                        NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                            .SafeNormalize(Vector2.UnitX) * 15) / 11;
                        NPC.dontTakeDamage = true;
                        Color textColor = Color.Purple;
                        if (ToHead < 10)
                        {
                            Time1++;
                            if (Time1 > 2)
                            {
                                Time1 = 0;
                            }
                        }
                        switch (Time3)
                        {
                            case 150:
                                {
                                    Main.NewText(".....", Color.Purple);
                                    CombatText.NewText(NPC.Hitbox, textColor, "......", true, false);
                                    break;
                                }
                            case 250:
                                {
                                    Main.NewText("你准备好了?", Color.Purple);
                                    CombatText.NewText(NPC.Hitbox, textColor, "你准备好了?", true, false);
                                    break;
                                }
                            case 400:
                                {
                                    Main.NewText("那么...开始吧", Color.Purple);
                                    CombatText.NewText(NPC.Hitbox, textColor, "那么...开始吧", true, false);
                                    break;
                                }
                            case 410:
                                {
                                    Time1 = 0;
                                    Time3 = 0;
                                    NPC.dontTakeDamage = false;
                                    NPC.velocity *= 0;
                                    SwitchState2(0);
                                    SwitchState1((int)StarGazerAI.S2, (int)StarGazerAI.S2 + 1);
                                    break;
                                }
                        }
                        break;
                    }
                    case StarGazerAI.S2 :
                    {
                        Time1++;
                        if (Time1 > 15)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToTarget * 1.1f, Boom, 145 / 3, 0, Main.myPlayer, 0, 0);
                            SwitchState2(0);
                            SwitchState1((int)StarGazerAI.S3, (int)StarGazerAI.S3 + 1);
                            Time1 = 0;
                        }
                        break;
                    }
                case StarGazerAI.S3:
                    {
                        Time1++;
                        Time2++;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        if (Time1 > 20)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 5)).ToRotationVector2() * 9f;
                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, Star, 140 / 3, 0f, Main.myPlayer);
                                interval++;
                            }
                            Time1 = 0;
                        }
                        if (Time2 > 60)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)StarGazerAI.S4, (int)StarGazerAI.S4 + 1);
                        }
                        break;
                    }
                case StarGazerAI.S4:
                    {
                        Time1++;
                        Time2++;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        if (Time1 > 20)
                        {
                            Player p = Main.player[NPC.target];
                            Vector2 plrToMouse = player.Center - NPC.Center;
                            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                            for (int i = -2; i <= 2; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 12;
                                Vector2 shootVel = r2.ToRotationVector2() * 2f;
                                Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, Comet, 135 / 3, 2, player.whoAmI);
                            }
                            for (int i = -2; i <= 2; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 18;
                                Vector2 shootVel = r2.ToRotationVector2() * 3.5f;
                                Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, Comet, 135 / 3, 2, player.whoAmI);
                            }
                            Time1 = 0;
                        }
                        if (Time2 > 50)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)StarGazerAI.S3, (int)StarGazerAI.S3 + 1);
                        }
                        break;
                    }
            }
        }
        public  EntitySource_ByProjectileSourceId projectileSource;
    }

}