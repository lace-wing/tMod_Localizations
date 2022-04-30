using BloodSoul.Background;
using BloodSoul.Items.BossBag;
using BloodSoul.Mount.Uang;
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

namespace BloodSoul.NPCs.Bosses.PhantomUang
{
    [AutoloadBossHead]
    class Uang : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;
        private int interval = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        private static float gravity = 0.3f;
        public Vector2 PlayerOldPos = Vector2.Zero;
        private int leavl = 0;
        public int i = 0;
        private enum UangAI
        {
            Fish,//摸鱼
            PredictPproj,//预判光弹
            Bump,//冲撞
            BevelAngle1,//斜角1
            BevelAngle2,//斜角2
            UltimateSprint,//终极冲刺
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Uang");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "幻灵独角仙");
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 7500 / 3;
            NPC.defense = 25;
            NPC.damage = 110 / 3;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 166;
            NPC.height = 112;
            NPC.value = 50000;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit29;
            NPC.DeathSound = SoundID.NPCDeath26;
            NPC.aiStyle = -1;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.scale = 1f;
            BossBag = ModContent.ItemType<UangBossBag>();
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Uang");
            }

        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<UangEgg>()));
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
            bool forceChange = false;

            if(!Main.player[NPC.target].InModBiome(ModContent.GetInstance<ExampleUndergroundBiome>()))
            {
                NPC.active = false;
            }

            Vector2 toTarget = Target.position - NPC.position;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 50)
            {
                Vector2 center = NPC.Center + (i.ToRotationVector2() * 1500);
                int dust = Dust.NewDust(center, 1, 1, DustID.Ice);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
            if (NPC.position.Distance(Target.position) > 1500)
            {
                Target.velocity = -toTarget.SafeNormalize(toTarget) * 3;
            }
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 12;
            Vector2 targetVel = Vector2.Normalize(Target.position - NPC.Center);
            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            TargetVel *= 8f;
            float accX = 0.7f;
            float accY = 0.7f;
            NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;

            if (NPC.life < NPC.lifeMax * 0.5f && leavl == 0)
            {
                leavl = 1;
                Time1 = 0;
                Time2 = 0;
                Timer1 = 0;
                Timer = 0;
                Timer3 = 0;
                Timer2 = 0;
                SwitchState1((int)UangAI.UltimateSprint);
            }


            int Pproj = ModContent.ProjectileType<PhantomCarrierEyeProj>();

            switch ((UangAI)State1)
            {
                case UangAI.Fish:
                    {
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = targetVel * 3.5f;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        Time2++;
                        Timer++;
                        if (Timer == 800)
                        {
                            // 重置计时器
                            Timer = 0;
                        }
                        if (Timer % 50 == 0)
                        {
                            Player p = Main.player[NPC.target];
                            for (int i = 1; i <= 1; i++)
                            {
                                for (int h = -1; h <= 1; h++)
                                {
                                    Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 8)).ToRotationVector2() * 1.8f;
                                    Projectile.NewProjectile(Source_NPC, NPC.Center, r,
                                    Pproj, 55 / 3, 0f, Main.myPlayer);
                                    interval++;
                                    SoundEngine.PlaySound(SoundID.Item29, NPC.position);
                                }
                            }
                            return;
                        }
                        if (Time2 > 180)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)UangAI.Fish, (int)UangAI.BevelAngle2 + 1);

                        }
                        break;
                    }
                case UangAI.PredictPproj:
                    {
                        Timer1++;
                        Vector2 Toplayer = (ToTarget * 2.1f) + Target.velocity;
                        switch (Timer1)
                        {
                            case 120:
                                {
                                    SoundEngine.PlaySound(SoundID.Item29, NPC.position);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, Toplayer * 0.6f, Pproj, 65 / 3, 2f, Main.myPlayer);
                                    break;
                                }
                            default:
                                {
                                    if (Timer1 < 120)
                                    {
                                        NPC.velocity *= 0.9f;
                                    }
                                    if (Timer1 > 120)
                                    {
                                        Timer1 = 0;
                                        Timer2 = 0;
                                        Timer3 = 0;
                                        SwitchState2(0);
                                        SwitchState1((int)UangAI.Fish, (int)UangAI.BevelAngle2 + 1);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case UangAI.Bump:
                    {
                        switch (State2)
                        {
                            case 0://到达头顶
                                {
                                    Timer1++;
                                    Vector2 Head = new Vector2(Target.Center.X + 400, Target.Center.Y);
                                    float ToHead = Vector2.Distance(NPC.Center, Head);
                                    NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                                        .SafeNormalize(Vector2.UnitX) * 15) / 11;
                                    if (Timer1 > 90)
                                    {
                                        Timer1 = 0;
                                        SwitchState2(1);
                                    }
                                    break;
                                }
                            case 1://预判冲刺
                                {
                                    Timer1++;
                                    Vector2 ToPlayer = (ToTarget * 2) + Target.velocity;
                                    if (Timer1 > 30)
                                    {
                                        SoundEngine.PlaySound(SoundID.ForceRoar, (int)NPC.position.X, (int)NPC.position.Y, 0, 2f, +0.75f);
                                        NPC.velocity = ToTarget * 1.5f;
                                        Timer1 = 0;
                                        SwitchState2(2);
                                    }
                                    break;
                                }
                            case 2://保持速度
                                {
                                    Timer1++;
                                    NPC.velocity *= 1.05f;
                                    NPC.rotation = NPC.velocity.ToRotation();
                                    if (Timer1 > 30)
                                    {
                                        Timer1 = 0;
                                        SwitchState2(3);
                                    }
                                    break;
                                }
                            case 3://减速
                                {
                                    Timer1++;
                                    Timer2++;
                                    if (Timer2 >= 20)
                                    {
                                        NPC.velocity *= 0.1f;
                                        Timer2 = 0;
                                    }
                                    NPC.rotation = NPC.velocity.ToRotation();
                                    if (Timer1 > 60)
                                    {
                                        Timer1 = 0;
                                        Timer2 = 0;
                                        SwitchState2(4);
                                    }
                                    break;
                                }
                            case 4://到达头顶
                                {
                                    Timer1++;
                                    Vector2 Head = new Vector2(Target.Center.X - 400, Target.Center.Y);
                                    float ToHead = Vector2.Distance(NPC.Center, Head);
                                    NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                                        .SafeNormalize(Vector2.UnitX) * 15) / 11;
                                    if (Timer1 > 90)
                                    {
                                        Timer1 = 0;
                                        SwitchState2(5);
                                    }
                                    break;
                                }
                            case 5://预判冲刺
                                {
                                    Timer1++;
                                    Vector2 ToPlayer = (ToTarget * 2) + Target.velocity;
                                    if (Timer1 > 30)
                                    {
                                        SoundEngine.PlaySound(SoundID.ForceRoar, (int)NPC.position.X, (int)NPC.position.Y, 0, 2f, +0.75f);
                                        NPC.velocity = ToTarget * 1.5f;
                                        Timer1 = 0;
                                        SwitchState2(6);
                                    }
                                    break;
                                }
                            case 6://保持速度
                                {
                                    Timer1++;
                                    NPC.velocity *= 1.05f;
                                    NPC.rotation = NPC.velocity.ToRotation();
                                    if (Timer1 > 30)
                                    {
                                        Timer1 = 0;
                                        SwitchState2(7);
                                    }
                                    break;
                                }
                            case 7://减速
                                {
                                    Timer1++;
                                    Timer2++;
                                    if (Timer2 >= 20)
                                    {
                                        NPC.velocity *= 0.1f;
                                        Timer2 = 0;
                                    }
                                    NPC.rotation = NPC.velocity.ToRotation();
                                    if (Timer1 > 60)
                                    {
                                        Timer1 = 0;
                                        Timer2 = 0;
                                        SwitchState2(0);
                                        SwitchState1((int)UangAI.Fish, (int)UangAI.BevelAngle2 + 1);
                                    }
                                    break;
                                }

                        }
                        break;
                    }
                case UangAI.BevelAngle1:
                    {
                        if (Timer2 == 0)
                        {
                            Vector2 vector = new Vector2(Target.position.X - 300, Target.position.Y - 400);
                            Vector2 ToTar = (vector - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
                            float dis = Vector2.Distance(vector, NPC.position);
                            if (dis < 50)
                                Timer2++;
                            else
                                NPC.velocity = (NPC.velocity * 10 + ToTar) / 10.5f;
                        }
                        else if (Timer2 == 1)
                        {
                            Timer1++;
                            NPC.velocity = NPC.position - new Vector2(NPC.position.X - 30f, NPC.position.Y);
                            if (Timer1 >= 6 && Main.netMode != 1)
                            {
                                Projectile qi = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToTarget / 10f, Pproj, 55 / 3, 2f, Main.myPlayer, 0);
                                qi.timeLeft = 1000;
                                Timer1 = 0;
                                interval++;
                            }
                        }
                        if (interval >= 5 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            interval = 0;
                            Timer1 = 0;
                            Timer2 = 0;
                            SwitchState1((int)UangAI.Fish, (int)UangAI.BevelAngle2 + 1);
                        }
                        break;
                    }
                case UangAI.BevelAngle2:
                    {
                        if (Timer2 == 0)
                        {
                            Vector2 vector = new Vector2(Target.position.X + 300, Target.position.Y - 400);
                            Vector2 ToTar = (vector - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
                            float dis = Vector2.Distance(vector, NPC.position);
                            if (dis < 50)
                                Timer2++;
                            else
                                NPC.velocity = (NPC.velocity * 10 + ToTar) / 10.5f;
                        }
                        else if (Timer2 == 1)
                        {
                            Timer1++;
                            NPC.velocity = NPC.position - new Vector2(NPC.position.X + 30f, NPC.position.Y);
                            if (Timer1 >= 6 && Main.netMode != 1)
                            {
                                var qi = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center,
                                    ToTarget / 10f, Pproj, 55 / 3, 2f, Main.myPlayer, 0);
                                qi.timeLeft = 1000;
                                Timer1 = 0;
                                interval++;
                            }
                        }
                        if (interval >= 5 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            interval = 0;
                            Timer1 = 0;
                            Timer2 = 0;
                            SwitchState1((int)UangAI.Fish, (int)UangAI.BevelAngle2 + 1);
                        }
                        break;
                    }
                case UangAI.UltimateSprint:
                    {
                        switch (State2)
                        {
                            case 0:
                                {
                                    if (Timer2 == 0)
                                    {
                                        Vector2 vector = new Vector2(Target.position.X - 300, Target.position.Y - 400);
                                        Vector2 ToTar = (vector - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
                                        float dis = Vector2.Distance(vector, NPC.position);
                                        if (dis < 50)
                                            Timer2++;
                                        else
                                            NPC.velocity = (NPC.velocity * 10 + ToTar) / 10.5f;
                                    }
                                    else if (Timer2 == 1)
                                    {
                                        Timer1++;
                                        NPC.velocity = NPC.position - new Vector2(NPC.position.X - 30f, NPC.position.Y);
                                        if (Timer1 >= 7 && Main.netMode != 1)
                                        {
                                            Projectile qi = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToTarget / 10f, Pproj, 55 / 3, 2f, Main.myPlayer, 0);
                                            qi.timeLeft = 1000;
                                            Timer1 = 0;
                                            interval++;
                                        }
                                    }
                                    if (interval >= 5 && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        interval = 0;
                                        Timer1 = 0;
                                        Timer2 = 0;
                                        SwitchState2(1);
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    if (Timer2 == 0)
                                    {
                                        Vector2 vector = new Vector2(Target.position.X + 300, Target.position.Y + 400);
                                        Vector2 ToTar = (vector - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
                                        float dis = Vector2.Distance(vector, NPC.position);
                                        if (dis < 50)
                                            Timer2++;
                                        else
                                            NPC.velocity = (NPC.velocity * 10 + ToTar) / 10.2f;
                                    }
                                    else if (Timer2 == 1)
                                    {
                                        Timer1++;
                                        NPC.velocity = NPC.position - new Vector2(NPC.position.X + 30f, NPC.position.Y);
                                        if (Timer1 >= 7 && Main.netMode != 1)
                                        {
                                            var qi = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center,
                                                ToTarget / 10f, Pproj, 65 / 3, 2f, Main.myPlayer, 0);
                                            qi.timeLeft = 1000;
                                            Timer1 = 0;
                                            interval++;
                                        }
                                    }
                                    if (interval >= 5 && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        interval = 0;
                                        Timer1 = 0;
                                        Timer2 = 0;
                                        SwitchState1((int)UangAI.Bump, (int)UangAI.Bump + 1);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Lighting.AddLight(NPC.Center, new Vector3(30, 144, 255) * 0.003f);
            i++;
            if (i == 60)
            {
                for (int i = 0; i < Main.rand.Next(1, 3); i++)
                {
                    var particle = new LightParticle(Color.White,NPC.Center, Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(3));
                    ParticleSystem.NewParticle(particle);
                }
                i = 0;
            }
            return true;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            // If the NPC dies, spawn gore and play a sound
            if (Main.netMode == NetmodeID.Server)
            {
                // We don't want Mod.Find<ModGore> to run on servers as it will crash because gores are not loaded on servers
                return;
            }

            if (NPC.life <= 0)
            {
                // These gores work by simply existing as a texture inside any folder which path contains "Gores/"
                int UangGore1 = Mod.Find<ModGore>("UangGore1").Type;
                int UangGore2 = Mod.Find<ModGore>("UangGore2").Type;
                int UangGore3 = Mod.Find<ModGore>("UangGore3").Type;
                int UangGore4 = Mod.Find<ModGore>("UangGore4").Type;
                int UangGore5 = Mod.Find<ModGore>("UangGore5").Type;

                for (int i = 0; i < 1; i++)
                {
                    Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), UangGore1);
                    Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), UangGore2);
                    Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), UangGore3);
                    Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), UangGore4);
                    Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), UangGore5);
                }
            }
        }
        public override void OnKill()
        {
            var player = Main.player[NPC.target];
            Vector2 ToPlayer = player.Center - NPC.Center;
            NPC.active = false;
            for (int i = 0; i < 3; i++)
            {
                SoundEngine.PlaySound(SoundID.Item62, NPC.position);
            }    
            Projectile.NewProjectile(projectileSource, NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<UangBoom>(), 0, 0, 0);
            base.OnKill();
        }
        public  EntitySource_ByProjectileSourceId projectileSource;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = (Texture2D)BloodSoulUtils.GetTexture("NPCs/Bosses/PhantomUang/UangGlow");
            SpriteEffects spriteEffects = 0;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            int frameCount = Main.npcFrameCount[NPC.type];
            Vector2 DrawOrigin;
            DrawOrigin = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / frameCount / 2));
            Color Taiolcolor = Color.Lerp(drawColor, Color.White, 0.5f);
            Taiolcolor = NPC.GetAlpha(Taiolcolor);
            Vector2 DrawPosition = NPC.position + new Vector2((float)NPC.width, (float)NPC.height) / 2f - Main.screenPosition;
            DrawPosition -= new Vector2((float)Glow.Width, (float)(Glow.Height / frameCount)) * NPC.scale / 2f;
            DrawPosition += DrawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY - DrawOffsetY * 2.5f);
            Main.spriteBatch.Draw(Glow, DrawPosition + new Vector2(0,2), new Rectangle?(NPC.frame), Color.White * 0.7f, NPC.rotation, DrawOrigin, NPC.scale, spriteEffects, 0f);
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