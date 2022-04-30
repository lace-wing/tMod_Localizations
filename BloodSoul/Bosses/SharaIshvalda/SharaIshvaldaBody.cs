using BloodSoul.Items;
using BloodSoul.MyUtils;
using BloodSoul.NPCs.Bosses.TheStarGazer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.Mount;

namespace BloodSoul.NPCs.Bosses.SharaIshvalda
{
    [AutoloadBossHead]
    class SharaIshvaldaBody : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;
        private int interval = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        private static float gravity = 0.3f;
        public Vector2 PlayerOldPos = Vector2.Zero;
        private int leavl = 0;
        public int i = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shara·Ishvalda");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "天地煌啼龙");
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 88000;
            NPC.defense = 25;
            NPC.damage = 0;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 94;
            NPC.height = 108;
            NPC.value = 50000;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit44;
            NPC.DeathSound = SoundID.DD2_BetsyDeath;
            NPC.aiStyle = -1;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.Ichor] = true;
            NPC.scale = 1f;
            NPC.hide = true;
            BossBag = ModContent.ItemType<BenevolentEyeShell>();
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SharaIshvalda");
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
        public int Body = 0;
        private enum SharaIshvaldaAI
        {
            St0,//开幕
            St1,//沙暴
            St2,//真空射线
            St3,//12组合
            St4,//头部
            St5,//沙弹
            St6,//真空射线2
            St7,//激光
            R//最终
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
            }

            var player = Main.player[NPC.target];
            player.wingTimeMax = 0;

            Vector2 toTarget = Target.position - NPC.position;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 50)
            {
                Vector2 center = NPC.Center + (i.ToRotationVector2() * 650);
                int dust = Dust.NewDust(center, 1, 1, DustID.Sand);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
            if (NPC.position.Distance(Target.position) > 650)
            {
                Target.velocity = -toTarget.SafeNormalize(toTarget) * 5;
            }
            if (NPC.position.Distance(Target.position) > 700)
            {
                Target.Center = NPC.Center;
            }
            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            TargetVel *= 0f;
            float accX = 0.7f;
            float accY = 0.7f;
            NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
            NPC.velocity *= 0;
            NPC.scale = 2f;
            NPC.realLife = NPC.whoAmI;
            if (Body == 0)
            {
                int num1 = NPC.whoAmI;
                NPC.ai[3] = NPC.whoAmI;
                int SpawnBody1 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + NPC.width / 2), (int)(NPC.Center.Y + NPC.height), ModContent.NPCType<SharaIshvaldaHead>(), NPC.whoAmI);
                Main.npc[SpawnBody1].ai[3] = num1;
                Main.npc[SpawnBody1].ai[1] = num1;
                //Main.npc[num1].ai[0] = SpawnBody1;
                Main.npc[SpawnBody1].realLife = NPC.whoAmI;
                NPC.netUpdate = true;
                num1 = SpawnBody1;
                int SpawnWing1 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + NPC.width / 2), (int)(NPC.Center.Y + NPC.height), ModContent.NPCType<RightWingMain>(), NPC.whoAmI);
                Main.npc[SpawnWing1].ai[3] = num1;
                Main.npc[SpawnWing1].ai[1] = num1;
                Main.npc[num1].ai[0] = SpawnWing1;
                Main.npc[SpawnWing1].realLife = NPC.whoAmI;
                int SpawnWing2 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + NPC.width / 2), (int)(NPC.Center.Y + NPC.height), ModContent.NPCType<LeftWingMain>(), NPC.whoAmI);
                Main.npc[SpawnWing2].ai[3] = num1;
                Main.npc[SpawnWing2].ai[1] = num1;
                //Main.npc[num1].ai[0] = SpawnWing2;
                Main.npc[SpawnWing2].realLife = NPC.whoAmI;

                int RightArm2 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + NPC.width / 2), (int)(NPC.Center.Y + NPC.height), ModContent.NPCType<RightArm2>(), NPC.whoAmI);
                Main.npc[RightArm2].ai[3] = num1;
                Main.npc[RightArm2].ai[1] = num1;
                //Main.npc[num1].ai[0] = RightArm2;
                Main.npc[RightArm2].realLife = NPC.whoAmI;

                int LeftA2 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + NPC.width / 2), (int)(NPC.Center.Y + NPC.height), ModContent.NPCType<LeftArm2>(), NPC.whoAmI);
                Main.npc[LeftA2].ai[3] = num1;
                Main.npc[LeftA2].ai[1] = num1;
                //Main.npc[num1].ai[0] = LeftA2;
                Main.npc[LeftA2].realLife = NPC.whoAmI;
                Body = 1;
            }

            if (NPC.life < NPC.lifeMax * 0.1f && leavl == 0)
            {
                NPC.life = (int)(NPC.lifeMax * 0.1f);
                leavl = 1;
                Time1 = 0;
                Time2 = 0;
                Timer1 = 0;
                Timer = 0;
                Timer3 = 0;
                Timer2 = 0;
                SwitchState2(0);
                SwitchState1((int)SharaIshvaldaAI.R, (int)SharaIshvaldaAI.R + 1);
            }

            switch ((SharaIshvaldaAI)State1)
            {
                case SharaIshvaldaAI.St0:
                    {
                        Time1++;
                        Time2++;
                        //if (Main.netMode != NetmodeID.MultiplayerClient) BaseUtility.Chat("天地煌啼龙", new Color(102, 20, 48));
                        if (Time1 >= 10)
                        {
                            if (Time2 >= 30)
                            {
                                SoundEngine.PlaySound(SoundID.Item45, NPC.position);
                                Projectile.NewProjectile(projectileSource, NPC.Center + new Vector2(-300, +75), toTarget * 0f, ProjectileID.SandnadoHostileMark, 180 / 6, 0, Main.myPlayer, 0, 0);
                                Projectile.NewProjectile(projectileSource, NPC.Center + new Vector2(300, +75), toTarget * 0f, ProjectileID.SandnadoHostileMark, 180 / 6, 0, Main.myPlayer, 0, 0);
                                Time2 = 0;
                            }
                        }
                        if (Time1 >= 120)
                        {
                            var modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 12f, 7f, 180, 1000f);
                            Main.instance.CameraModifiers.Add(modifier);
                            for (int i = 0; i < 10; i++)
                            {
                                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("BloodSoul/Sounds/Custom/SharaIshvaldaRoar"), NPC.Center);
                            }
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState1((int)SharaIshvaldaAI.St1, (int)SharaIshvaldaAI.St1 + 1);
                        }
                        break;
                    }
                case SharaIshvaldaAI.St1:
                    {
                        Time1++;
                        if (Time1 == 20)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<RightArm2>() || n.type == ModContent.NPCType<LeftArm2>() && n.active)
                                {
                                    n.ai[0] = 1;
                                    n.netUpdate = true;
                                }
                            }
                        }
                        if (Time1 >= 120)
                        {
                            Time1 = 0;
                            SwitchState1((int)SharaIshvaldaAI.St2, (int)SharaIshvaldaAI.St7 + 1);
                        }
                        break;
                    }
                case SharaIshvaldaAI.St2:
                    {
                        Time1++;
                        if (Time1 == 20)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<RightWingMain>() || n.type == ModContent.NPCType<LeftWingMain>() && n.active)
                                {
                                    n.ai[0] = 1;
                                    n.netUpdate = true;
                                }
                            }
                        }
                        if (Time1 >= 300)
                        {
                            Time1 = 0;
                            SwitchState1((int)SharaIshvaldaAI.St1, (int)SharaIshvaldaAI.St7 + 1);
                        }
                        break;
                    }
                case SharaIshvaldaAI.St3:
                    {
                        Time1++;
                        if (Time1 == 60)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<RightWingMain>() || n.type == ModContent.NPCType<LeftWingMain>() || n.type == ModContent.NPCType<RightArm2>() || n.type == ModContent.NPCType<LeftArm2>() && n.active)
                                {
                                    n.ai[0] = 1;
                                    n.netUpdate = true;
                                }
                            }
                        }
                        if (Time1 >= 300)
                        {
                            Time1 = 0;
                            SwitchState1((int)SharaIshvaldaAI.St1, (int)SharaIshvaldaAI.St7 + 1);
                        }
                        break;
                    }
                case SharaIshvaldaAI.St4:
                    {
                        Time1++;
                        if (Time1 == 60)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<SharaIshvaldaHead>() && n.active)
                                {
                                    n.ai[0] = 1;
                                    n.netUpdate = true;
                                }
                            }
                        }
                        if (Time1 >= 240)
                        {
                            Time1 = 0;
                            SwitchState1((int)SharaIshvaldaAI.St1, (int)SharaIshvaldaAI.St7 + 1);
                        }
                        break;
                    }
                case SharaIshvaldaAI.St5:
                    {
                        Time1++;
                        if (Time1 == 20)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<RightArm2>() || n.type == ModContent.NPCType<LeftArm2>() && n.active)
                                {
                                    n.ai[0] = 2;
                                    n.netUpdate = true;
                                }
                            }
                        }
                        if (Time1 >= 240)
                        {
                            Time1 = 0;
                            SwitchState1((int)SharaIshvaldaAI.St1, (int)SharaIshvaldaAI.St7 + 1);
                        }
                        break;
                    }
                case SharaIshvaldaAI.St6:
                    {
                        Time1++;
                        if (Time1 == 20)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<RightWingMain>() || n.type == ModContent.NPCType<LeftWingMain>() && n.active)
                                {
                                    n.ai[0] = 2;
                                    n.netUpdate = true;
                                }
                            }
                        }
                        if (Time1 >= 360)
                        {
                            Time1 = 0;
                            SwitchState1((int)SharaIshvaldaAI.St1, (int)SharaIshvaldaAI.St7 + 1);
                        }
                        break;
                    }
                case SharaIshvaldaAI.St7:
                    {
                        Time1++;
                        if (Time1 == 20)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<SharaIshvaldaHead>() && n.active)
                                {
                                    n.ai[0] = 2;
                                    n.netUpdate = true;
                                }
                            }
                        }
                        if (Time1 >= 360)
                        {
                            Time1 = 0;
                            SwitchState1((int)SharaIshvaldaAI.St1, (int)SharaIshvaldaAI.St6 + 1);
                        }
                        break;
                    }
                case SharaIshvaldaAI.R:
                    {
                        NPC.dontTakeDamage = true;
                        foreach (NPC n in Main.npc)
                        {
                            if (n.type == ModContent.NPCType<RightWingMain>() || n.type == ModContent.NPCType<LeftWingMain>() || n.type == ModContent.NPCType<SharaIshvaldaHead>() && n.active)
                            {
                                n.dontTakeDamage = true;
                                n.netUpdate = true;
                            }
                        }
                        Vector2 ToTarget = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 15;
                        Time1++;
                        if(Time1 == 80)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<RightWingMain>() || n.type == ModContent.NPCType<LeftWingMain>() && n.active)
                                {
                                    n.ai[0] = 2;
                                    n.netUpdate = true;
                                }
                            }
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("BloodSoul/Sounds/Custom/AirRay"), NPC.Center);
                        }
                        if (Time1 == 120)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToTarget * 0f, ModContent.ProjectileType<VacuumBomb>(), 280 / 6, 0, Main.myPlayer, 0, 0);
                        }
                        if (Time1 >= 600)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<RightWingMain>() || n.type == ModContent.NPCType<LeftWingMain>() || n.type == ModContent.NPCType<SharaIshvaldaHead>() && n.active)
                                {
                                    n.dontTakeDamage = false;
                                    n.netUpdate = true;
                                }
                            }
                            NPC.dontTakeDamage = false;
                            Time1 = 0;
                            SwitchState1((int)SharaIshvaldaAI.St1, (int)SharaIshvaldaAI.St7 + 1);
                        }
                        break;
                    }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            // If the NPC dies, spawn gore and play a sound
            if (Main.netMode == NetmodeID.Server)
            {
                // We don't want Mod.Find<ModGore> to run on servers as it will crash because gores are not loaded on servers
                return;
            }

            /*if (NPC.life <= 0)
            {
                var modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 25f, 6f, 120, 1000f);
                Main.instance.CameraModifiers.Add(modifier);
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("BloodSoul/Sounds/Custom/SharaIshvaldaRoar"), NPC.Center);
                // These gores work by simply existing as a texture inside any folder which path contains "Gores/"
                int Gore1 = Mod.Find<ModGore>("LeftArm1").Type;
                int Gore2 = Mod.Find<ModGore>("RightArm1").Type;
                int Gore3 = Mod.Find<ModGore>("LeftArm2").Type;
                int Gore4 = Mod.Find<ModGore>("RightArm2").Type;
                int Gore5 = Mod.Find<ModGore>("SharaIshvaldaHead").Type;

                for (int i = 0; i < 1; i++)
                {
                    Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore1);
                    Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore2);
                    Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore3);
                    Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore4);
                    Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore5);
                }
            }*/
        }
        public override bool CheckDead()
        {
            var modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 25f, 6f, 120, 1000f);
            Main.instance.CameraModifiers.Add(modifier);
            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("BloodSoul/Sounds/Custom/SharaIshvaldaRoar"), NPC.Center);
            // These gores work by simply existing as a texture inside any folder which path contains "Gores/"
            int Gore1 = Mod.Find<ModGore>("LeftArm1-export").Type;
            int Gore2 = Mod.Find<ModGore>("RightArm1-export").Type;
            int Gore3 = Mod.Find<ModGore>("LeftArm2-export").Type;
            int Gore4 = Mod.Find<ModGore>("RightArm2-export").Type;
            int Gore5 = Mod.Find<ModGore>("SharaIshvaldaHead-export").Type;
            int Gore6 = Mod.Find<ModGore>("RightWing1-export").Type;
            int Gore7 = Mod.Find<ModGore>("RightWing2-export").Type;
            int Gore8 = Mod.Find<ModGore>("RightWing3-export").Type;
            int Gore9 = Mod.Find<ModGore>("RightWingMain-export").Type;
            int Gore10 = Mod.Find<ModGore>("LeftWing1-export").Type;
            int Gore11 = Mod.Find<ModGore>("LeftWing2-export").Type;
            int Gore12 = Mod.Find<ModGore>("LeftWing3-export").Type;
            int Gore13 = Mod.Find<ModGore>("LeftWingMain-export").Type;

            for (int i = 0; i < 1; i++)
            {
                Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore1);
                Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore2);
                Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore3);
                Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore4);
                Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore5);
                Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore6);
                Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore7);
                Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore8);
                Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore9);
                Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore10);
                Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore11);
                Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore12);
                Gore.NewGore(NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), Gore13);

            }
            return base.CheckDead();
        }
        public override void OnKill()
        {
            BloodSoulSystem.SharaIshvalda = true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Neck = (Texture2D)BloodSoulUtils.GetTexture("NPCs/Bosses/SharaIshvalda/SharaIshvalda_Neck");
            Vector2 drawOrigin2;
            drawOrigin2 = new Vector2(Neck.Width, Neck.Height);
            Vector2 NeckPos = new(NPC.Center.X + 46, NPC.Center.Y - 130);
            Main.spriteBatch.Draw(Neck, NeckPos - screenPos, null, Color.White, 0, drawOrigin2, new Vector2(2f, 2f), SpriteEffects.None, 0);

            Texture2D LeftArm1 = (Texture2D)BloodSoulUtils.GetTexture("NPCs/Bosses/SharaIshvalda/LeftArm1");
            drawOrigin2 = new Vector2(Neck.Width, Neck.Height);
            Vector2 LeftArm1Pos = new(NPC.Center.X - 95, NPC.Center.Y - 110);
            Main.spriteBatch.Draw(LeftArm1, LeftArm1Pos - screenPos, null, Color.White, 0, drawOrigin2, new Vector2(2f, 2f), SpriteEffects.None, 0);

            Texture2D RightArm1 = (Texture2D)BloodSoulUtils.GetTexture("NPCs/Bosses/SharaIshvalda/RightArm1");
            drawOrigin2 = new Vector2(Neck.Width, Neck.Height);
            Vector2 RightArm1Pos = new(NPC.Center.X + 175, NPC.Center.Y - 110);
            Main.spriteBatch.Draw(RightArm1, RightArm1Pos - screenPos, null, Color.White, 0, drawOrigin2, new Vector2(2f, 2f), SpriteEffects.None, 0);
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsMoonMoon.Add(index);
        }
        public EntitySource_ByProjectileSourceId projectileSource;
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            player = Main.player[NPC.target];
            if (!player.active || player.dead || Main.dayTime)
            {
                NPC.active = false;
            }
        }
    }

}