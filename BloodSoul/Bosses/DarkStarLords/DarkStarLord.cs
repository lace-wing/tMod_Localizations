using BloodSoul.Items.BossBag;
using BloodSoul.MyUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.DarkStarLords
{
    [AutoloadBossHead]
    class DarkStarLord : FSMnpc
    {
        private int leavl = 0;
        public EntitySource_ByProjectileSourceId Source_NPC;
        private Vector2 targetOldPos;
        private int frameTime = 0;
        private int interval = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;
        private enum DarkStarLordAI
        {
            St1,//加速弹幕
            St2,//连续暗星弹
            St3,//连续冲刺
            St4,//环绕弹幕
            St5,//加速弹幕2
            St6,//暗星冲刺
            St7,//爆炸
            StE,//特殊攻击
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Star Lord");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "暗星领主");
            Main.npcFrameCount[NPC.type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 98000 / 3;
            NPC.defense = 45;
            NPC.damage = 165 / 3;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 78;
            NPC.height = 84;
            NPC.value = 30000;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit3;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.aiStyle = -1;
            NPC.scale = 1.5f;
            BossBag = ModContent.ItemType<DarkStarLordBossBag>();
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/DSLord");
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1.0;
            int num155 = 8;
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
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));
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
            bool forceChange = false;

            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.Pi / 2;

            Vector2 toTarget = Target.position - NPC.position;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 50)
            {
                Vector2 center = NPC.Center + (i.ToRotationVector2() * 1500);
                int dust = Dust.NewDust(center, 1, 1, DustID.Gold);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
            if (NPC.position.Distance(Target.position) > 1500)
            {
                Target.velocity = -toTarget.SafeNormalize(toTarget) * 3;
            }

            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            int Cen = (X > 0) ? 1 : -1;

            if (NPC.life < NPC.lifeMax * 0.5f && leavl == 0)
            {
                leavl = 1;
                Time1 = 0;
                Time2 = 0;
                Timer1 = 0;
                Timer = 0;
                Timer3 = 0;
                Timer2 = 0;
                SwitchState1((int)DarkStarLordAI.StE);
            }

            int DarkStar = ModContent.ProjectileType<DarkStar>();
            int DSL = ModContent.ProjectileType<DarkStarLordProj>();
            int DSP = ModContent.ProjectileType<DarkStarProj2>();
            int DSP2 = ModContent.ProjectileType<DarkStarProj3>();
            int Boom = ModContent.ProjectileType<DarkStarLordProj2>();
            float accX = 0.51f;
            float accY = 0.51f;
            NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;

            switch ((DarkStarLordAI)State1)
            {
                case DarkStarLordAI.St1:
                    {
                        Time1++;
                        if (Time1 < 220)
                        {
                            Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 350);
                            float ToHead = Vector2.Distance(NPC.Center, Head);
                            NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                                .SafeNormalize(Vector2.UnitX) * 15) / 11;
                            var player = Main.player[NPC.target];
                            if (ToHead < 2)
                            {
                                Time1++;
                                if (Time1 > 5)
                                {
                                    Time1 = 0;
                                }
                            }
                            Time2++;
                            Timer++;
                            if (Timer == 1000)
                            {
                                // 重置计时器
                                Timer = 0;
                            }
                            if (Timer % 80 == 0)
                            {
                                Player p = Main.player[NPC.target];
                                Vector2 plrToMouse = player.Center - NPC.Center;
                                float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                                for (int i = -1; i <= 1; i++)
                                {
                                    float r2 = r + i * MathHelper.Pi / 36;
                                    Vector2 shootVel = r2.ToRotationVector2() * 13.5f;
                                    Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, DarkStar, 135 / 3, 2, player.whoAmI);
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
                            SwitchState1((int)DarkStarLordAI.St1, (int)DarkStarLordAI.St7 + 1);

                        }
                        break;
                    }
                case DarkStarLordAI.St2:
                    {
                        Time1++;
                        Time2++;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = TargetVel * 5.5f;
                        if (Time1 > 5)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToTarget * 1.1f, DarkStar, 135 / 3, 0, Main.myPlayer, 0, 0);
                            Time1 = 0;
                        }
                        if (Time2 > 120)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)DarkStarLordAI.St3, (int)DarkStarLordAI.St7 + 1);
                        }
                        break;
                    }
                case DarkStarLordAI.St3:
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
                                            Time2++;
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
                                        if (Time2 > 4)
                                        {
                                            Time1 = 0;
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)DarkStarLordAI.St1, (int)DarkStarLordAI.St7 + 1);
                                        }
                                        else
                                        {
                                            NPC.velocity *= 0f;
                                            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, NPC.position);
                                            for (int i = 0; i < 8; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 4)).ToRotationVector2() * 8;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, DarkStar, 135 / 3, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            SwitchState2(0);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case DarkStarLordAI.St4:
                    {
                        Timer1++;
                        if (Timer1 < 5)
                        {
                            targetOldPos = Target.position;
                        }
                        else if (Timer1 > 5)
                        {
                            Timer2++;
                            NPC.velocity = CircularMotionForCenter(0.1f, Timer1, 500, targetOldPos);
                            NPC.velocity *= 5;
                            NPC.rotation += (NPC.position - targetOldPos).ToRotation() + MathHelper.Pi / 2;
                            if (Timer2 == 8 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                interval++;
                                Timer2 = 0;
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center
                                    , (targetOldPos - NPC.Center).SafeNormalize(Vector2.UnitX) * 4, DarkStar, 135 / 3, 2.3f, Main.myPlayer, 0);
                                NPC.netUpdate = true;
                            }
                            else if (interval >= 12 && Main.netMode != NetmodeID.MultiplayerClient && Timer2 > 5)
                            {
                                interval = 0;
                                Timer1 = 0;
                                Timer2 = 0;

                                SwitchState1((int)DarkStarLordAI.St1, (int)DarkStarLordAI.St7 + 1);
                            }
                        }
                        break;
                    }
                case DarkStarLordAI.St5:
                    {
                        Time1++;
                        if (Time1 < 220)
                        {
                            Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 350);
                            float ToHead = Vector2.Distance(NPC.Center, Head);
                            NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                                .SafeNormalize(Vector2.UnitX) * 15) / 11;
                            var player = Main.player[NPC.target];
                            if (ToHead < 2)
                            {
                                Time1++;
                                if (Time1 > 5)
                                {
                                    Time1 = 0;
                                }
                            }
                            Time2++;
                            Timer++;
                            if (Timer == 1000)
                            {
                                // 重置计时器
                                Timer = 0;
                            }
                            if (Timer % 80 == 0)
                            {
                                Player p = Main.player[NPC.target];
                                Vector2 plrToMouse = player.Center - NPC.Center;
                                float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                                for (int i = -2; i <= 2; i++)
                                {
                                    float r2 = r + i * MathHelper.Pi / 12;
                                    Vector2 shootVel = r2.ToRotationVector2() * 13.5f;
                                    Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, DSL, 135 / 3, 2, player.whoAmI);
                                }
                                for (int i = -2; i <= 2; i++)
                                {
                                    float r2 = r + i * MathHelper.Pi / 18;
                                    Vector2 shootVel = r2.ToRotationVector2() * 16.5f;
                                    Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, DSL, 135 / 3, 2, player.whoAmI);
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
                            SwitchState1((int)DarkStarLordAI.St1, (int)DarkStarLordAI.St7 + 1);

                        }
                        break;
                    }
                case DarkStarLordAI.St6:
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
                                            Time2++;
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
                                    if (Time1 > 45)
                                    {
                                        Time1 = 0;
                                        if (Time2 > 4)
                                        {
                                            Time1 = 0;
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)DarkStarLordAI.St1, (int)DarkStarLordAI.St7 + 1);
                                        }
                                        else
                                        {
                                            NPC.velocity *= 0f;
                                            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, NPC.position);
                                            for (int i = 0; i < 10; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 5)).ToRotationVector2() * 7.5f;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, DSP, 135 / 3, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            Time1 = 0;
                                            SwitchState2(0);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case DarkStarLordAI.St7:
                    {
                        Time1++;
                        Time2++;
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = TargetVel * 5.5f;
                        if (Time1 > 15)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToTarget * 1.1f, Boom, 145 / 3, 0, Main.myPlayer, 0, 0);
                            Time1 = 0;
                        }
                        if (Time2 > 90)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)DarkStarLordAI.St3, (int)DarkStarLordAI.St6 + 1);
                        }
                        break;
                    }
                case DarkStarLordAI.StE:
                    {
                        Time1++;
                        if (Time1 < 220)
                        {
                            Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 400);
                            float ToHead = Vector2.Distance(NPC.Center, Head);
                            NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                                .SafeNormalize(Vector2.UnitX) * 15) / 11;
                            var player = Main.player[NPC.target];
                            if (ToHead < 2)
                            {
                                Time1++;
                                if (Time1 > 5)
                                {
                                    Time1 = 0;
                                }
                            }
                            Time2++;
                            Timer++;
                            if (Timer == 1000)
                            {
                                // 重置计时器
                                Timer = 0;
                            }
                            if (Timer % 80 == 0)
                            {
                                Player p = Main.player[NPC.target];
                                Vector2 plrToMouse = player.Center - NPC.Center;
                                float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                                for (int i = -2; i <= 2; i++)
                                {
                                    float r2 = r + i * MathHelper.Pi / 18;
                                    Vector2 shootVel = r2.ToRotationVector2() * 7f;
                                    Terraria.Projectile.NewProjectile(Source_NPC, NPC.Center, shootVel, DSP, 155 / 3, 2, player.whoAmI);
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
                            SwitchState1((int)DarkStarLordAI.St1, (int)DarkStarLordAI.St7 + 1);

                        }
                        break;
                    }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color color = Color.White;
            Texture2D NPCTexture = TextureAssets.Npc[NPC.type].Value;
            SpriteEffects spriteEffects = 0;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            int frameCount = Main.npcFrameCount[NPC.type];
            Vector2 DrawOrigin;
            DrawOrigin = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / frameCount / 2));

            for (int i = 0; i < NPC.oldPos.Length; i += 1)
            {
                Color Taiolcolor = Color.Lerp(drawColor, color, 0.5f);
                Taiolcolor = NPC.GetAlpha(Taiolcolor);
                Taiolcolor *= (float)(NPC.oldPos.Length - i) / NPC.oldPos.Length;
                Vector2 DrawPosition = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - Main.screenPosition;
                DrawPosition -= new Vector2((float)NPCTexture.Width, (float)(NPCTexture.Height / frameCount)) * NPC.scale / 2f;
                DrawPosition += DrawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY - DrawOffsetY * 2.5f);
                Main.spriteBatch.Draw(NPCTexture, DrawPosition, new Rectangle?(NPC.frame), Taiolcolor, NPC.rotation, DrawOrigin, NPC.scale, spriteEffects, 0f);
            }
            return true;
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
            var modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 40f, 6f, 30, 1000f);
            Main.instance.CameraModifiers.Add(modifier);
            Projectile.NewProjectile(projectileSource, NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<DarkStarBoom>(), 0, 0, 0);
            for (int i = 0; i < 10; i++)
            {
                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 5)).ToRotationVector2() * 7.5f;
                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, ModContent.ProjectileType<DarkStarProj2>(), 0, 0f, Main.myPlayer);
                interval++;
            }
            base.OnKill();
        }
        public EntitySource_ByProjectileSourceId projectileSource;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = (Texture2D)BloodSoulUtils.GetTexture("NPCs/Bosses/DarkStarLords/DarkStarLord_Gore");
            SpriteEffects spriteEffects = 0;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            int frameCount = Main.npcFrameCount[NPC.type];
            Vector2 DrawOrigin;
            DrawOrigin = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / frameCount / 2));
            Color Taiolcolor = Color.Lerp(drawColor, Color.White, 0.5f);
            Taiolcolor = NPC.GetAlpha(Taiolcolor);
            Vector2 DrawPosition = NPC.position + new Vector2((float)NPC.width, (float)NPC.height) / 2f - Main.screenPosition;
            DrawPosition -= new Vector2((float)Glow.Width, (float)(Glow.Height / frameCount)) * NPC.scale / 2f;
            DrawPosition += DrawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY - DrawOffsetY * 2.5f);
            Main.spriteBatch.Draw(Glow, DrawPosition + new Vector2(0, 2), new Rectangle?(NPC.frame), Color.White * 0.7f, NPC.rotation, DrawOrigin, NPC.scale, spriteEffects, 0f);
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
