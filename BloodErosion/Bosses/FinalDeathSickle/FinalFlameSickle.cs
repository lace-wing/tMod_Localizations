using BloodErosion.NPCs.Bosses.FinalDeathSickle.FinalDeathSickle2s;
using BloodSoul.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.FinalDeathSickle
{
    [AutoloadBossHead]
    class FinalFlameSickle : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;
        private Vector2 endPiont;
        private int frameTime = 0;
        private int interval = 0;
        private int TimeV = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;
        private enum FinalFrostSickleAI
        {
            Spike2//连续突刺
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Final Flame Sickle");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "最终烈焰镰");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 30000;
            NPC.defense = 30;
            NPC.damage = 215 / 3;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 68;
            NPC.height = 52;
            NPC.value = 0;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath10;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.aiStyle = -1;
            NPC.scale = 1f;
            NPC.dontTakeDamage = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SupremeSickle2");
            }
        }
        public override void BossHeadRotation(ref float rotation) => rotation = -NPC.rotation;
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
            if (Main.player[NPC.target].dead || Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) > 6000f || Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 6000f)
            {
                NPC.TargetClosest(false);
                if (Main.player[NPC.target].dead || Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) > 6000f || Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 6000f)
                {
                    DespawnHandler();
                }
                return;
            }
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) NPC.TargetClosest();
            TimeV++;
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = TimeV * 7;
            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float accX = 0.3f;
            float accY = 0.3f;
            NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
            NPC.velocity = TargetVel * 8.5f;


            switch ((FinalFrostSickleAI)State1)
            {
                case FinalFrostSickleAI.Spike2:
                    {
                        switch (State2)
                        {
                            case 0://冲刺
                                {
                                    Time1++;
                                    if (Time1 > 45)
                                    {
                                        Time2++;
                                        SoundEngine.PlaySound(SoundID.Item71, NPC.position);
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
                                            for (int i = 0; i < 3; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 6)).ToRotationVector2() * 9;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r,
                                                291, 60, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            //SwitchState2(1);
                                            Time2 = 0;
                                            SwitchState1((int)FinalFrostSickleAI.Spike2, (int)FinalFrostSickleAI.Spike2 + 1);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
            }

        }
        public override void OnHitPlayer(Player Target, int damage, bool crit)
        {
            Target.AddBuff(BuffID.OnFire, 90);
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
