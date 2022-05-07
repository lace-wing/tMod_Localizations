using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.IO;
using BloodSoul.Items;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using Terraria.Audio;
using BloodSoul.NPCs;

namespace BloodErosion.NPCs.Bosses.SnowDemonEmperor
{
    class FrostClaw : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;

        private int interval = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Claw");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "寒雪之爪");
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.friendly = false;
            NPC.width = 22;
            NPC.height = 16;
            NPC.aiStyle = -1;
            NPC.damage = 130 / 3;
            NPC.defense = 30;
            NPC.lifeMax = 110;
            NPC.HitSound = SoundID.NPCHit42;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.knockBackResist = 0.01f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 0.8f;
            NPC.alpha = 0;
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
        enum NPCState
        {
            Attack
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
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) NPC.TargetClosest();
            bool forceChange = false;

            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 12;
            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.Pi / 2;
            TargetVel *= 8.5f;
            float accX = 0.05f;
            float accY = 0.05f;
            NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
        }
        public override void OnHitPlayer(Player Target, int damage, bool crit)
        {
            Target.AddBuff(BuffID.Frozen, 15);
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
