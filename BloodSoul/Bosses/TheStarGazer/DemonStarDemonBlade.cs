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
    class DemonStarDemonBlade : FSMnpc
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
            DisplayName.SetDefault("DemonStarDemonBlade");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "妖星魔刃");
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.friendly = false;
            NPC.width = 44;
            NPC.height = 54;
            NPC.aiStyle = -1;
            NPC.damage = 350;
            NPC.defense = 50;
            NPC.lifeMax = 20000 / 3;
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

            if (Target.position.X - NPC.position.X > 0f)
            {
                NPC.spriteDirection = 1;
            }
            if (Target.position.X - NPC.position.X < 0f)
            {
                NPC.spriteDirection = -1;
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
                            SwitchState1((int)NPCState.Attack, (int)NPCState.Attack + 1);
                        }
                        break;
                    }
                case NPCState.Attack:
                    {
                        Time1++;
                        Time2++;
                        if (Time2 >= 120)
                        {
                            var player = Main.player[NPC.target];
                            Vector2 ToPlayer = player.Center - NPC.Center;
                            for (int i = 0; i < 1; i++)
                            {
                                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 2)).ToRotationVector2() * 13;
                                Projectile.NewProjectile(Source_NPC, NPC.Center, r,
                                ModContent.ProjectileType<DemonStarDemonBladeChop>(), 155 / 3, 0f, Main.myPlayer);
                                interval++;
                            }
                            Time2 = 0;
                        }

                        if (Time1 >= 240)
                        {
                            SwitchState2(0);
                            SwitchState1((int)NPCState.Attack, (int)NPCState.Attack + 1);
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
            Projectile.NewProjectile(projectileSource, NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<StarBoom>(), 0, 0, 0);
            return true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2 = BloodSoulUtils.GetTexture("NPCs/Bosses/TheStarGazer/Effects/Sword").Value;
            Vector2 drawOrigin2;
            drawOrigin2 = new Vector2(texture2.Width * 0.5f, texture2.Height * 0.5f);
            Main.spriteBatch.Draw(texture2, NPC.Center - Main.screenPosition, null, new Color(153, 50, 204, 0), r, drawOrigin2, new Vector2(1f, 1f), SpriteEffects.None, 0);
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
