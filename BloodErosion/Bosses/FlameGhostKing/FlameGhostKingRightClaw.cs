using BloodSoul.NPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.FlameGhostKing
{
    class FlameGhostKingRightClaw : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;

        private int interval = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        private int Time3 = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("FlameGhostKingClaw");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "炎王之爪");
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.friendly = false;
            NPC.width = 84;
            NPC.height = 88;
            NPC.aiStyle = -1;
            NPC.damage = 130 / 3;
            NPC.defense = 30;
            NPC.lifeMax = 130;
            NPC.HitSound = SoundID.NPCHit42;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.knockBackResist = 0.5f;
            NPC.value = 80f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 0.8f;
            NPC.alpha = 0;
            NPC.dontTakeDamage = true;
        }
        public override void FindFrame(int frameHeight)
        {
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
        enum NPCState
        {
            Normal,
            Attack,
            Attack2
        }
        public int VE = 30;
        public override void AI()
        {
            if (Main.player[NPC.target].dead || Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) > 6000f || Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 6000f)
            {
                NPC.TargetClosest(false);
                if (Main.player[NPC.target].dead || Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) > 6000f || Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 6000f)
                {
                    DespawnHandler();
                }
                return;
            }
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            switch ((NPCState)State)
            {
                case NPCState.Normal:
                    {
                        foreach (NPC n in Main.npc)
                        {
                            if (n.type == ModContent.NPCType<FlameGhostKing>() && n.active)
                            {
                                Vector2 Head = new Vector2(n.Center.X - 150, n.Center.Y - 60);
                                float ToHead = Vector2.Distance(NPC.Center, Head);
                                NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                                    .SafeNormalize(Vector2.UnitX) * 15) / 11;
                            }
                        }
                        break;
                    }
                case NPCState.Attack:
                    {
                        Vector2 Head = new Vector2(Target.Center.X - 300, Target.Center.Y);
                        float ToHead = Vector2.Distance(NPC.Center, Head);
                        NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                            .SafeNormalize(Vector2.UnitX) * 15) / 11;
                        NPC.dontTakeDamage = true;
                        if (Time1 >= 210)
                        {
                            for (int i = 0; i < 36; i++)
                            {
                                Vector2 vector8 = Utils.RotatedBy(new Vector2(VE, VE), (double)((float)(i - 17) * 6.28318548f / 36f), default(Vector2)) + NPC.Center;
                                Vector2 vector3 = vector8 - NPC.Center;
                                int num2 = Dust.NewDust(vector8 + vector3, 0, 0, DustID.FlameBurst, vector3.X * 1.1f, vector3.Y * 1.1f, 50, Color.White, 1.2f);
                                Main.dust[num2].noGravity = true;
                                Main.dust[num2].velocity = Vector2.Normalize(vector3);
                            }
                            VE--;
                            if (VE <= 0)
                            {
                                VE = 0;
                            }
                        }



                        Time1++;
                        if (Time1 >= 240)
                        {
                            for (int h = -1; h <= 1; h++)
                            {
                                var player = Main.player[NPC.target];
                                Vector2 ToPlayer = player.Center - NPC.Center;
                                Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 8)).ToRotationVector2() * 15;
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r,
                                258, 65 / 3, 0f, Main.myPlayer);
                                interval++;
                                SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                            }
                            VE = 30;
                            Time1 = 0;
                            SwitchState1((int)NPCState.Normal, (int)NPCState.Normal + 1);
                        }
                        break;
                    }
                case NPCState.Attack2:
                    {
                        switch (State2)
                        {
                            case 0:
                                {
                                    Time1++;
                                    if (Time1 == 25)
                                    {
                                        NPC.rotation = ToTarget.ToRotation();
                                        NPC.velocity = NPC.rotation.ToRotationVector2();
                                        NPC.netUpdate = true;
                                    }
                                    if (Time1 > 25)
                                    {
                                        Time1++;
                                        Vector2 ToPlayer = (ToTarget * 1.5f) + Target.velocity;
                                        SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                        if (Time1 > 90)
                                        {
                                            NPC.velocity = ToPlayer;
                                            Time1 = 0;
                                            SwitchState2(1);
                                        }
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    Time1++;
                                    NPC.velocity *= 1f;
                                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi / 2;
                                    var player = Main.player[NPC.target];
                                    Vector2 ToPlayer = player.Center - NPC.Center;
                                    if (Time1 > 30)
                                    {
                                        Time1 = 0;
                                        if (Time2 >= 2)
                                        {
                                            Time1 = 0;
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)NPCState.Normal, (int)NPCState.Normal + 1);
                                        }
                                        else
                                        {
                                            Time2 += 1;
                                            NPC.velocity *= 0f;
                                            SwitchState2(0);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.velocity.X = 0;
                NPC.velocity.Y -= 1;
            }
        }
    }
}
