using BloodSoul.MyUtils;
using BloodSoul.NPCs.Bosses.TheStarGazer;
using BloodSoul.Particle;
using BloodSoul.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    
    class SharaIshvaldaHead : FSMnpc
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
            NPC.damage = 355 / 3;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 82;
            NPC.height = 82;
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
            //NPC.dontTakeDamage = true;
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
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                rot = (float)reader.ReadDouble();
                repeat = reader.ReadInt32();
            }
        }
        public int repeat;
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
        private enum HeadAI
        {
            St0,//待机
            St1,//射线
            St2,//激光
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
            NPC.Center = Main.npc[(int)NPC.ai[1]].Center + new Vector2(0, -240);


            switch ((HeadAI)State1)
            {
                case HeadAI.St0:
                    {
                        NPC.rotation = 0;
                        NPC.velocity *= 0;
                        break;
                    }
                case HeadAI.St1:
                    {
                        var player = Main.player[NPC.target];
                        Vector2 ToTarget = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 15;
                        Time1++;
                        Time2++;
                        if(Time2 >= 25)
                        {
                            SoundEngine.PlaySound(SoundID.Item45, NPC.position);
                            Vector2 plrToMouse = player.Center - NPC.Center;
                            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                            for (int i = -1; i <= 0; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 60;
                                Vector2 shootVel = r2.ToRotationVector2() * 4.5f;
                                Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center + new Vector2(0,25), shootVel, ModContent.ProjectileType<AirStar>(), 135 / 6, 2, player.whoAmI);
                            }
                            Time2 = 0;
                        }
                        if (Time1 >= 240)
                        {
                            Time1 = 0;
                            SwitchState1((int)HeadAI.St0, (int)HeadAI.St0 + 1);
                        }
                        break;
                    }
                case HeadAI.St2:
                    {
                        var player = Main.player[NPC.target];
                        Vector2 ToTarget = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 15;
                        Time1++;
                        Time2++;
                        if (Time2 >= 70)
                        {
                            SoundEngine.PlaySound(SoundID.Item45, NPC.position);
                            Vector2 plrToMouse = player.Center - NPC.Center;
                            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                            for (int i = 0; i <= 0; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 72;
                                Vector2 shootVel = r2.ToRotationVector2() * 8.5f;
                                Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center + new Vector2(0, 30), shootVel, ModContent.ProjectileType<VacuumBomb21>(), 135 / 6, 2, player.whoAmI);
                            }
                            Time2 = 0;
                        }
                        if (Time1 >= 120)
                        {
                            Time1 = 0;
                            SwitchState1((int)HeadAI.St0, (int)HeadAI.St0 + 1);
                        }
                        break;
                    }
            }
        }

        public float rot;
        public ref float AITimer => ref NPC.ai[2];
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