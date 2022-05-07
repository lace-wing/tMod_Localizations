using BloodSoul.MyUtils;
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
using BloodErosion.NPCs.Bosses.FinalDeathSickle.FinalDeathSickle2s;

namespace BloodErosion.NPCs.Bosses.FinalDeathSickle
{
    [AutoloadBossHead]
    class FinalAwakeningSickle : FSMnpc
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
        private enum FinalAwakeningSickleAI
        {
            Spike2//连续突刺
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("FinalAwakeningSickle");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "最终觉醒镰");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 20000;
            NPC.defense = 40;
            NPC.damage = 235 / 3;
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
            //NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            //NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
            //NPC.velocity = TargetVel * 8f;

            int FrostSwordWind = ModContent.ProjectileType<FrostSwordWind>();

            switch ((FinalAwakeningSickleAI)State1)
            {
                case FinalAwakeningSickleAI.Spike2:
                    {
                        switch (State2)
                        {
                            case 0:
                                {
                                    Time1++;
                                    if (Time1 == 25)
                                    {
                                        Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                            .ToRotationVector2() * 250;
                                        Dust.NewDustDirect(center, 10, 10, MyDustId.PurpleGems);
                                        NPC.position = center;
                                        NPC.velocity = NPC.rotation.ToRotationVector2();
                                        NPC.netUpdate = true;
                                        SoundEngine.PlaySound(SoundID.Item21, NPC.position);
                                    }
                                    if (Time1 > 25)
                                    {
                                        Time1++;
                                        Vector2 ToPlayer = (ToTarget * 1f) + Target.velocity;
                                        SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                        if (Time1 > 200)
                                        {
                                            NPC.velocity = ToPlayer;
                                            Time1 = 0;
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)FinalAwakeningSickleAI.Spike2, (int)FinalAwakeningSickleAI.Spike2 + 1);
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                    break;
            }
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
