using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.SharaIshvalda
{
    class LeftWingMain : FSMnpc
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
            NPC.lifeMax = 200000 / 3;
            NPC.defense = 50;
            NPC.damage = 355 / 3;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 64;
            NPC.height = 112;
            NPC.value = 50000;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit44;
            NPC.DeathSound = SoundID.DD2_BetsyDeath;
            NPC.aiStyle = -1;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.scale = 2f;
            //NPC.dontTakeDamage = true;
            NPC.hide = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SharaIshvalda");
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
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
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
        private enum WingAI
        {
            St0,//待机
            St1,//射线
            St2,//射线2
        }
        public override void AI()
        {
            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            TargetVel *= 0f;
            float accX = 0.7f;
            float accY = 0.7f;
            NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
            NPC.velocity *= 0;
            NPC.scale = 2f;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!Main.npc[(int)NPC.ai[1]].active)
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.active = false;
                    NPC.netUpdate = true;
                }
            }
            if (Body == 0)
            {
                int num1 = NPC.whoAmI;
                NPC.ai[3] = NPC.whoAmI;
                int SpawnBody1 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + NPC.width / 2), (int)(NPC.Center.Y + NPC.height), ModContent.NPCType<LeftWing1>(), NPC.whoAmI);
                Main.npc[SpawnBody1].ai[3] = num1;
                Main.npc[SpawnBody1].ai[1] = num1;
                Main.npc[SpawnBody1].realLife = NPC.whoAmI;
                Main.npc[SpawnBody1].rotation = NPC.rotation;
                NPC.netUpdate = true;
                num1 = SpawnBody1;
                int SpawnBody2 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + NPC.width / 2), (int)(NPC.Center.Y + NPC.height), ModContent.NPCType<LeftWing2>(), NPC.whoAmI);
                Main.npc[SpawnBody2].ai[3] = num1;
                Main.npc[SpawnBody2].ai[1] = num1;
                Main.npc[SpawnBody2].realLife = NPC.whoAmI;
                Main.npc[SpawnBody2].rotation = NPC.rotation;
                int SpawnBody3 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + NPC.width / 2), (int)(NPC.Center.Y + NPC.height), ModContent.NPCType<LeftWing3>(), NPC.whoAmI);
                Main.npc[SpawnBody3].ai[3] = num1;
                Main.npc[SpawnBody3].ai[1] = num1;
                Main.npc[SpawnBody3].realLife = NPC.whoAmI;
                Main.npc[SpawnBody3].rotation = NPC.rotation;
                Body = 1;
            }

            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<LeftWing2>() || n.type == ModContent.NPCType<LeftWing1>() || n.type == ModContent.NPCType<LeftWing3>() && n.active)
                {
                    n.rotation = NPC.rotation;
                    n.netUpdate = true;
                }
            }
            if (Main.player[NPC.target].dead)
            {
                NPC.timeLeft = 0;
            }

            NPC.timeLeft = 99999;
            NPC.Center = Main.npc[(int)NPC.ai[1]].Center + new Vector2(-90, -35);

            switch ((WingAI)State1)
            {
                case WingAI.St0:
                    {
                        NPC.rotation = 0;
                        NPC.velocity *= 0;
                        break;
                    }
                case WingAI.St1:
                    {
                        Time1++;

                        if (Time1 == 30)
                        {
                            for(int i = 0; i < 4; i++)
                            {
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("BloodSoul/Sounds/Custom/AirRay"), NPC.Center);
                            }
                        }
                        if (Time1 == 120)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<LeftWing2>() || n.type == ModContent.NPCType<LeftWing1>() || n.type == ModContent.NPCType<LeftWing3>() && n.active)
                                {
                                    var modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 12f, 6f, 60, 1000f);
                                    Main.instance.CameraModifiers.Add(modifier);
                                    n.ai[0] = 1;
                                    n.netUpdate = true;
                                }
                            }
                        }
                        if (Time1 >= 240)
                        {
                            Time1 = 0;
                            SwitchState1((int)WingAI.St0, (int)WingAI.St0 + 1);
                        }
                        break;
                    }
                case WingAI.St2:
                    {
                        Time1++;

                        if (Time1 == 30)
                        {
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("BloodSoul/Sounds/Custom/AirRay"), NPC.Center);
                        }
                        if (Time1 == 120)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (n.type == ModContent.NPCType<LeftWing2>() || n.type == ModContent.NPCType<LeftWing1>() || n.type == ModContent.NPCType<LeftWing3>() && n.active)
                                {
                                    n.ai[0] = 2;
                                    n.netUpdate = true;
                                }
                            }
                        }
                        if (Time1 >= 300)
                        {
                            Time1 = 0;
                            SwitchState1((int)WingAI.St0, (int)WingAI.St0 + 1);
                        }
                        break;
                    }
            }
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsMoonMoon.Add(index);
        }
        public override void OnKill()
        {
            NPC.active = false;
        }
        public EntitySource_ByProjectileSourceId projectileSource;
    }

}