using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.SharaIshvalda
{
    class RightArm2 : FSMnpc
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
            NPC.defense = 25;
            NPC.damage = 0;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 78;
            NPC.height = 86;
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
            NPC.scale = 2f;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
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
        private Vector2 targetOldPos;
        private enum SharaIshvaldaHandAI
        {
            St0,//待机
            St1,//沙暴
            St2,//沙弹
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
            if (Main.player[NPC.target].dead)
            {
                NPC.timeLeft = 0;
            }

            NPC.timeLeft = 99999;
            NPC.Center = Main.npc[(int)NPC.ai[1]].Center + new Vector2(+155, +245);

            if (NPC.ai[0] == 1)
            {
                SwitchState1((int)SharaIshvaldaHandAI.St1, (int)SharaIshvaldaHandAI.St1 + 1);
            }

            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            switch ((SharaIshvaldaHandAI)State1)
            {
                case SharaIshvaldaHandAI.St0:
                    {
                        NPC.rotation = 0;
                        NPC.velocity *= 0;
                        break;
                    }
                case SharaIshvaldaHandAI.St1:
                    {
                        Time2++;
                        if (Timer2 < 5)
                        {
                            targetOldPos = Target.position;
                        }
                        Time1++;
                        if (NPC.rotation >= -0.9f)
                        {
                            NPC.rotation -= 0.02f;
                        }
                        if (Time1 >= 90)
                        {
                            for (int i = 0; i < 36; i++)
                            {
                                Vector2 vector8 = Utils.RotatedBy(new Vector2(18f, 18f), (double)((float)(i - 17) * 6.28318548f / 36f), default(Vector2)) + NPC.Center;
                                Vector2 vector3 = vector8 - NPC.Center + new Vector2(+10, +50);
                                int num2 = Dust.NewDust(vector8 + vector3, 0, 0, DustID.SpectreStaff, vector3.X * 1.1f, vector3.Y * 1.1f, 100, new Color(250, 250, 210, 30), 1.4f);
                                Main.dust[num2].noGravity = true;
                                Main.dust[num2].velocity = Vector2.Normalize(vector3);
                            }
                        }
                        if (Time1 == 120)
                        {
                            Time1 = 0;
                            NPC.rotation = 0f;
                            SoundEngine.PlaySound(SoundID.Item45, NPC.position);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), targetOldPos, ToTarget * 0f, ModContent.ProjectileType<Storm>(), 0, 0, Main.myPlayer, 0, 0);
                            SwitchState1((int)SharaIshvaldaHandAI.St0, (int)SharaIshvaldaHandAI.St0 + 1);
                        }
                        break;
                    }
                case SharaIshvaldaHandAI.St2:
                    {
                        Time2++;
                        if (Timer2 < 10)
                        {
                            targetOldPos = Target.position;
                        }
                        Time1++;
                        if (NPC.rotation >= -0.9f)
                        {
                            NPC.rotation -= 0.02f;
                        }
                        if (Time1 >= 90)
                        {
                            for (int i = 0; i < 36; i++)
                            {
                                Vector2 vector8 = Utils.RotatedBy(new Vector2(22f, 22f), (double)((float)(i - 17) * 6.28318548f / 36f), default(Vector2)) + NPC.Center;
                                Vector2 vector3 = vector8 - NPC.Center + new Vector2(+10, +50);
                                int num2 = Dust.NewDust(vector8 + vector3, 0, 0, DustID.SpectreStaff, vector3.X * 1.1f, vector3.Y * 1.1f, 100, new Color(218, 165, 32, 30), 1.4f);
                                Main.dust[num2].noGravity = true;
                                Main.dust[num2].velocity = Vector2.Normalize(vector3);
                            }
                        }
                        if (Time1 == 120)
                        {
                            var player = Main.player[NPC.target];
                            Vector2 ToPlayer = player.Center - NPC.Center;
                            Time1 = 0;
                            NPC.rotation = 0f;
                            SoundEngine.PlaySound(SoundID.Item45, NPC.position);
                            for (int i = 0; i < 8; i++)
                            {
                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 4)).ToRotationVector2() * 5.5f;
                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, ModContent.ProjectileType<AirProj>(), 145 / 6, 0f, Main.myPlayer);
                                interval++;
                            }
                            SwitchState1((int)SharaIshvaldaHandAI.St0, (int)SharaIshvaldaHandAI.St0 + 1);
                        }
                        break;
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
        }
        public EntitySource_ByProjectileSourceId projectileSource;
    }

}