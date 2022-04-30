using BloodSoul.MyUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.TheStarGazer
{
    class DemonStar : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;

        private int interval = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        private int Time3 = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;
        float r = 0;
        public override void UpdateLifeRegen(ref int damage)
        {
            r += 0.01f;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("DemonStar");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "妖星");
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.friendly = false;
            NPC.width = 128;
            NPC.height = 128;
            NPC.aiStyle = -1;
            NPC.damage = 350;
            NPC.defense = 50;
            NPC.lifeMax = 60000 / 3;
            NPC.HitSound = SoundID.NPCHit3;
            NPC.DeathSound = SoundID.NPCHit55;
            NPC.knockBackResist = 0.5f;
            NPC.value = 80f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 0.8f;
            NPC.alpha = 255;
        }
        public override void FindFrame(int frameHeight)
        {
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
            Attack2,
            Attack3
        }
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
            Vector2 toTarget = Target.position - NPC.position;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 50)
            {
                Vector2 center = NPC.Center + (i.ToRotationVector2() * 900);
                int dust = Dust.NewDust(center, 1, 1, DustID.PurpleTorch);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
            if (NPC.Center.Distance(Target.position) > 900)
            {
                Target.velocity = -toTarget.SafeNormalize(toTarget) * 3;
            }

            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.Pi;
            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<StarGazerBoss2>())
                {
                    Vector2 Head = new Vector2(n.Center.X, n.Center.Y - 400);
                    float ToHead = Vector2.Distance(NPC.Center, Head);
                    NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                        .SafeNormalize(Vector2.UnitX) * 15) / 11;
                    n.dontTakeDamage = true;
                    n.netUpdate = true;
                }
            }
            switch ((NPCState)State)
            {
                case NPCState.Normal:
                    {
                        Time1++;
                        if (Time1 <= 60)
                        {
                            NPC.alpha = 255;
                        }
                        if (Time1 >= 61)
                        {
                            NPC.alpha--;
                        }
                        if (NPC.alpha == 0)
                        {
                            Time1 = 0;
                            NPC.velocity *= 0;
                            SwitchState2(0);
                            SwitchState1((int)NPCState.Attack, (int)NPCState.Attack3 + 1);
                        }
                        break;
                    }
                case NPCState.Attack:
                    {
                        Time1++;
                        Time2++;
                        if (Time2 >= 60)
                        {
                            var player = Main.player[NPC.target];
                            Vector2 ToPlayer = player.Center - NPC.Center;
                            for (int i = 0; i < 4; i++)
                            {
                                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 2)).ToRotationVector2() * 13;
                                Projectile.NewProjectile(Source_NPC, NPC.Center, r,
                                ModContent.ProjectileType<Star>(), 145 / 3, 0f, Main.myPlayer);
                                interval++;
                            }
                            Time2 = 0;
                        }

                        if (Time1 >= 180)
                        {
                            SwitchState2(0);
                            SwitchState1((int)NPCState.Attack2, (int)NPCState.Attack3 + 1);
                        }


                        break;
                    }
                case NPCState.Attack2:
                    {
                        Time1++;
                        Time2++;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        if (Time2 >= 20)
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 2)).ToRotationVector2() * 15;
                                Projectile.NewProjectile(Source_NPC, NPC.Center, r,
                                ModContent.ProjectileType<StarBoomProj2>(), 145 / 3, 0f, Main.myPlayer);
                                interval++;
                            }
                            Time2 = 0;
                        }

                        if (Time1 >= 180)
                        {
                            SwitchState2(0);
                            SwitchState1((int)NPCState.Attack2, (int)NPCState.Attack3 + 1);
                        }
                        break;
                    }
                case NPCState.Attack3:
                    {
                        Time1++;
                        Time2++;
                        var player = Main.player[NPC.target];
                        Vector2 ToPlayer = player.Center - NPC.Center;
                        if (Time1 > 60)
                        {
                            Player p = Main.player[NPC.target];
                            Vector2 plrToMouse = player.Center - NPC.Center;
                            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                            for (int i = -2; i <= 2; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 12;
                                Vector2 shootVel = r2.ToRotationVector2() * 2f;
                                Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, ModContent.ProjectileType<Comet>(), 135 / 3, 2, player.whoAmI);
                            }
                            for (int i = -2; i <= 2; i++)
                            {
                                float r2 = r + i * MathHelper.Pi / 18;
                                Vector2 shootVel = r2.ToRotationVector2() * 3.5f;
                                Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, ModContent.ProjectileType<Comet>(), 135 / 3, 2, player.whoAmI);
                            }
                            Time1 = 0;
                        }
                        if (Time2 > 180)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)NPCState.Attack2, (int)NPCState.Attack3 + 1);
                        }
                        break;
                    }
            }
        }
        public EntitySource_ByProjectileSourceId projectileSource;
        public override bool CheckDead()
        {
            NPC.active = false;
            for (int i = 0; i < 3; i++)
            {
                SoundEngine.PlaySound(SoundID.Item62, NPC.position);
            }
            Projectile.NewProjectile(projectileSource, NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<DemonStarBoom>(), 0, 0, 0);
            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<StarGazerBoss2>())
                {
                    n.dontTakeDamage = false;
                    n.netUpdate = true;
                }
            }
            return true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2 = BloodSoulUtils.GetTexture("NPCs/Bosses/TheStarGazer/Effects/DemonStar").Value;
            Vector2 drawOrigin2;
            drawOrigin2 = new Vector2(texture2.Width * 0.5f, texture2.Height * 0.5f);
            Main.spriteBatch.Draw(texture2, NPC.Center - Main.screenPosition, null, new Color(153, 50, 204, 0), r, drawOrigin2, new Vector2(1f, 1f), SpriteEffects.None, 0);

            Texture2D texture3 = BloodSoulUtils.GetTexture("NPCs/Bosses/TheStarGazer/Effects/StarRing").Value;
            Vector2 drawOrigin3;
            drawOrigin3 = new Vector2(texture3.Width * 0.5f, texture3.Height * 0.5f);
            Main.spriteBatch.Draw(texture3, NPC.Center - Main.screenPosition, null, new Color(153, 50, 204, 0), -r, drawOrigin3, new Vector2(3.2f, 3.2f), SpriteEffects.None, 0);
            return true;
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
