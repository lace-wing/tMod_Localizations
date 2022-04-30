using BloodSoul.MyUtils;
using BloodSoul.NPCs.GlobalNPCs;
using BloodSoul.NPCs.TownNPC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.TheStarGazer
{
    [AutoloadBossHead]
    class StarGazerBoss2 : FSMnpc
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
        float r = 0;
        public override void UpdateLifeRegen(ref int damage)
        {
            r += 0.01f;
        }
        private enum StarGazerAI
        {
            S1,//星弹
            S2,//加速弹
            S3,//天星之束
            S4,//普通加速弹
            S5,//天星束`小
            S6,//精准天星束
            Z,//待机
            E1,//特殊
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Gazer");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "观星者");
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 95000 / 3;
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
            NPC.DeathSound = SoundID.NPCDeath59;
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
            if (Main.player[NPC.target].dead)
            {
                NPC.TargetClosest(false);
                if (Main.player[NPC.target].dead)
                {
                    NPC.active = false;
                }
                return;
            }
            bool forceChange = false;
            NPC.timeLeft = 999999;
            BSNPC.StarGazer = NPC.whoAmI;
            if (!SkyManager.Instance["BloodSoul:StarSky"].IsActive())//开启天空
            {
                SkyManager.Instance.Activate("BloodSoul:StarSky");
            }

            Vector2 toTarget = Target.position - NPC.position;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 50)
            {
                Vector2 center = NPC.Center + (i.ToRotationVector2() * 2200);
                int dust = Dust.NewDust(center, 1, 1, DustID.PurpleTorch);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
            if (NPC.position.Distance(Target.position) > 2200)
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
            NPC.velocity *= 0;

            if (Target.position.X - NPC.position.X > 0f)
            {
                NPC.spriteDirection = 1;
            }
            if (Target.position.X - NPC.position.X < 0f)
            {
                NPC.spriteDirection = -1;
            }

            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<DemonStar>() && n.active)
                {
                    SwitchState2(0);
                    SwitchState1((int)StarGazerAI.Z, (int)StarGazerAI.Z + 1);
                    n.netUpdate = true;
                }
            }

            if (NPC.life < NPC.lifeMax * 0.7f && leavl == 0)
            {
                leavl = 1;
                Time1 = 0;
                Time2 = 0;
                Timer1 = 0;
                Timer = 0;
                Timer3 = 0;
                Timer2 = 0;
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X), (int)(NPC.Center.Y - 400), ModContent.NPCType<DemonStar>(), NPC.whoAmI);
            }
            if (NPC.life < NPC.lifeMax * 0.5f && leavl == 1)
            {
                leavl = 2;
                Time1 = 0;
                Time2 = 0;
                Timer1 = 0;
                Timer = 0;
                Timer3 = 0;
                Timer2 = 0;
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X), (int)(NPC.Center.Y - 400), ModContent.NPCType<DemonStarDemonBlade>(), NPC.whoAmI);
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
                SwitchState2(0);
                SwitchState1((int)StarGazerAI.E1, (int)StarGazerAI.E1 + 1);
            }

            if (Target.dead)
            {
                Main.NewText("我会等着你的再次挑战", Color.Purple);
                CombatText.NewText(NPC.Hitbox, Color.Purple, "我会等着你的再次挑战", true, false);
            }

            int Boom = ModContent.ProjectileType<StarBoomProj2>();
            int Star = ModContent.ProjectileType<Star>();//星弹
            int Comet = ModContent.ProjectileType<Comet>();//星弹
            int Comet2 = ModContent.ProjectileType<Comet2>();//星弹
            int BeamOfStars = ModContent.ProjectileType<BeamOfStars>();//天星束
            int BeamOfStars2 = ModContent.ProjectileType<BeamOfStars2>();//天星束2

            switch ((StarGazerAI)State1)
            {
                case StarGazerAI.S1:
                    {
                        Time1++;
                        Time2++;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        if (Time1 > 20)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 3)).ToRotationVector2() * 11f;
                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, Star, 140 / 3, 0f, Main.myPlayer);
                                interval++;
                            }
                            Time1 = 0;
                        }
                        if (Time2 > 30)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)StarGazerAI.S2, (int)StarGazerAI.S6 + 1);
                        }
                        break;
                    }
                case StarGazerAI.S2:
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
                            for (int i = -1; i <= 1; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 12;
                                Vector2 shootVel = r2.ToRotationVector2() * 2f;
                                Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, Comet, 135 / 3, 2, player.whoAmI);
                            }
                            for (int i = -1; i <= 1; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 18;
                                Vector2 shootVel = r2.ToRotationVector2() * 3.5f;
                                Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, Comet, 135 / 3, 2, player.whoAmI);
                            }
                            Time1 = 0;
                        }
                        if (Time2 > 30)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)StarGazerAI.S1, (int)StarGazerAI.S6 + 1);
                        }
                        break;
                    }
                case StarGazerAI.S3:
                    {
                        Time1++;
                        Time2++;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        if (Time1 > 90)
                        {
                            for (int i = 1; i <= 1; i++)
                            {
                                NPC.Shoot(new Vector2(player.Center.X + (Main.rand.NextBool(2) ? 1000 : -1000), player.Center.Y - 800),
                                    BeamOfStars2, 165,
                                    Vector2.Zero, false, SoundID.Item162);
                            }
                            for (int i = 1; i <= 1; i++)
                            {
                                NPC.Shoot(new Vector2(player.Center.X + (Main.rand.Next(600, 900) *
                                        (Main.rand.NextBool() ? -1 : 1)), player.Center.Y - 600), BeamOfStars,
                                        165, new Vector2(Main.rand.NextFloat(-1, 1), 10), false, SoundID.Item162);
                            }
                            Time1 = 0;
                        }
                        if (Time2 > 120)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)StarGazerAI.S1, (int)StarGazerAI.S6 + 1);
                        }
                        break;
                    }
                case StarGazerAI.S4:
                    {
                        Time1++;
                        Time2++;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        if (Time1 > 10)
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 3)).ToRotationVector2() * 4f;
                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, Comet, 140 / 3, 0f, Main.myPlayer);
                                interval++;
                            }
                            Time1 = 0;
                        }
                        if (Time2 > 30)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)StarGazerAI.S1, (int)StarGazerAI.S6 + 1);
                        }
                        break;
                    }
                case StarGazerAI.S5:
                    {
                        Time1++;
                        Time2++;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        if (Time1 > 45)
                        {
                            for (int i = -1; i <= 2; i++)
                            {
                                NPC.Shoot(new Vector2(player.Center.X + (Main.rand.Next(600, 900) *
                                        (Main.rand.NextBool() ? -1 : 1)), player.Center.Y - 600), BeamOfStars,
                                        165, new Vector2(Main.rand.NextFloat(-1, 1), 10), false, SoundID.Item162);
                            }
                            Time1 = 0;
                        }
                        if (Time2 > 150)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)StarGazerAI.S1, (int)StarGazerAI.S6 + 1);
                        }
                        break;
                    }
                case StarGazerAI.S6:
                    {
                        Time1++;
                        Time2++;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        if (Time1 > 45)
                        {
                            for (int i = 1; i <= 2; i++)
                            {
                                NPC.Shoot(new Vector2(player.Center.X + (Main.rand.Next(6, 9) *
                                        (Main.rand.NextBool() ? -1 : 1)), player.Center.Y - 600), BeamOfStars,
                                        165, new Vector2(Main.rand.NextFloat(-1, 1), 10), false, SoundID.Item162);
                            }
                            Time1 = 0;
                        }
                        if (Time2 > 250)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)StarGazerAI.S1, (int)StarGazerAI.S5 + 1);
                        }
                        break;
                    }


                case StarGazerAI.Z:
                    {
                        Time1++;
                        if (Time1 >= 60)
                        {
                            SwitchState2(0);
                            SwitchState1((int)StarGazerAI.S1, (int)StarGazerAI.S3 + 1);
                        }
                        break;
                    }
                case StarGazerAI.E1:
                    {
                        Time1++;
                        Time2++;
                        Time3++;
                        Timer++;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        if (Timer > 60)
                        {
                            NPC.Shoot(new Vector2(player.Center.X + (Main.rand.Next(0, 0) * (Main.rand.NextBool() ? -1 : 1)), player.Center.Y - 600), BeamOfStars, 165, new Vector2(Main.rand.NextFloat(-1, 1), 10), false, SoundID.Item162);
                            Timer = 0;
                        }
                        if (Time3 > 200)
                        {
                            for (int i = 1; i <= 2; i++)
                            {
                                NPC.Shoot(new Vector2(player.Center.X + (Main.rand.NextBool(2) ? 1200 : -1200), player.Center.Y - 800),
                                    BeamOfStars2, 165,
                                    Vector2.Zero, false, SoundID.Item162);
                            }
                            Time3 = 0;
                        }
                        if (Time1 > 20)
                        {

                            for (int i = -2; i <= 2; i++)
                            {
                                NPC.Shoot(new Vector2(player.Center.X + (Main.rand.Next(600, 900) *
                                        (Main.rand.NextBool() ? -1 : 1)), player.Center.Y - 600), BeamOfStars,
                                        165, new Vector2(Main.rand.NextFloat(-1, 1), 10), false, SoundID.Item162);
                            }
                            Time1 = 0;
                        }
                        if (Time2 > 600)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)StarGazerAI.S1, (int)StarGazerAI.S4 + 1);
                        }
                        break;
                    }
            }
        }

        public override bool CheckDead()
        {
            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<DemonStar>() || n.type == ModContent.NPCType<DemonStarDemonBlade>() && n.active)
                {
                    n.active = false;
                    n.netUpdate = true;
                }
            }
            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<StarGazer>() && !n.active)
                {
                    NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<StarGazer>(), NPC.whoAmI);
                    n.netUpdate = true;
                }
            }
            return base.CheckDead();
        }
        public override void OnKill()
        {
            BloodSoulSystem.downedStarGazer = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2 = BloodSoulUtils.GetTexture("NPCs/Bosses/TheStarGazer/Effects/StarCircle").Value;
            Vector2 drawOrigin2;
            drawOrigin2 = new Vector2(texture2.Width * 0.5f, texture2.Height * 0.5f);
            Main.spriteBatch.Draw(texture2, NPC.Center - Main.screenPosition, null, new Color(153, 50, 204, 0), r, drawOrigin2, new Vector2(1.2f, 1.2f), SpriteEffects.None, 0);

            Texture2D texture3 = BloodSoulUtils.GetTexture("NPCs/Bosses/TheStarGazer/Effects/DemonStarAura").Value;
            Vector2 drawOrigin3;
            drawOrigin3 = new Vector2(texture3.Width * 0.5f, texture3.Height * 0.5f);
            Main.spriteBatch.Draw(texture3, NPC.Center - Main.screenPosition, null, new Color(153, 50, 204, 0), -r, drawOrigin3, new Vector2(2.2f, 2.2f), SpriteEffects.None, 0);

            return true;
        }
        public EntitySource_ByProjectileSourceId projectileSource;
    }

}