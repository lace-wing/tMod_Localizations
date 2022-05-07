using BloodSoul.NPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.ApostleOfDeath
{
    class EyeOfDeath2 : FSMnpc
    {
        public int r;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("EyeOfDeath");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "死神之眼");
            Main.npcFrameCount[NPC.type] = 8;
        }
        public int i;
        private int interval = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        private enum EyeOfDeathAI
        {
            ST1,
            ST2,
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 27000;
            NPC.defense = 20;
            NPC.damage = 180;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 136;
            NPC.height = 100;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.aiStyle = -1;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.scale = 0.8f;
            //NPC.dontTakeDamage = true;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.ai[0] == 2 || NPC.ai[0] == 3 || NPC.ai[0] == 4)
                {
                    if (NPC.frame.Y < 4 * frameHeight || NPC.frame.Y < 7 * frameHeight)
                    {
                        NPC.frame.Y = 4 * frameHeight;
                    }
                }
                else
                {
                    if (NPC.frame.Y > 4 * frameHeight)
                    {
                        NPC.frame.Y = 0;
                    }
                }
            }
        }
        protected int Timer
        {
            get { return (int)NPC.ai[1]; }
            set { NPC.ai[1] = value; }
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
            Vector2 Head = new Vector2(Target.Center.X + 300, Target.Center.Y + 300);
            float ToHead = Vector2.Distance(NPC.Center, Head);
            NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                .SafeNormalize(Vector2.UnitX) * 15) / 11;
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.Pi / -2;
            switch ((EyeOfDeathAI)State1)
            {
                case EyeOfDeathAI.ST1:
                    {
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        Time2++;
                        Timer++;
                        if (Timer == 1000)
                        {
                            // 重置计时器
                            Timer = 0;
                        }
                        if (Timer % 50 == 0)
                        {

                            // NPC的攻击目标
                            Player p = Main.player[NPC.target];

                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    Vector2 plrToMouse = player.Center - NPC.Center;
                                    float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                                    for (int s = 1; s <= 1; s++)
                                    {
                                        float r2 = r + s * MathHelper.Pi / 72;
                                        Vector2 shootVel = r2.ToRotationVector2() * 1.5f;
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, shootVel, ModContent.ProjectileType<DeathEnergyBomb>(), 185 / 6, 2, Main.myPlayer);
                                    }
                                }
                                return;
                            }
                        }
                        if (Time2 > 200)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)EyeOfDeathAI.ST2, (int)EyeOfDeathAI.ST2 + 1);
                        }
                        break;
                    }
                case EyeOfDeathAI.ST2:
                    {
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        Time2++;
                        Timer++;
                        if (Timer == 1000)
                        {
                            // 重置计时器
                            Timer = 0;
                        }
                        if (Timer % 50 == 0)
                        {
                            NPC.velocity *= 0.1f;
                            Player p = Main.player[NPC.target];
                            for (int i = 1; i <= 2; i++)
                            {
                                Vector2 plrToMouse = player.Center - NPC.Center;
                                float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                                for (int s = 1; s <= 1; s++)
                                {
                                    float r2 = r + s * MathHelper.Pi / 72;
                                    Vector2 shootVel = r2.ToRotationVector2() * 7;
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, shootVel, ModContent.ProjectileType<DeadLight4>(), 185 / 6, 2, Main.myPlayer);
                                }
                            }
                            return;

                        }
                        if (Time2 > 200)
                        {
                            NPC.velocity *= 10f;
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)EyeOfDeathAI.ST1, (int)EyeOfDeathAI.ST1 + 1);
                        }
                        break;
                    }
            }
        }
        public override bool CheckDead()
        {
            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<EyeOfDeath>() || n.type == ModContent.NPCType<EyeOfDeath3>() && n.active)
                {
                    n.active = false;
                    n.netUpdate = true;
                }
            }
            return base.CheckDead();
        }
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.active = false;
            }
        }
    }
}
