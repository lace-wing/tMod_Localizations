using BloodErosion.NPCs.Bosses.DivineGlow;
using BloodSoul.MyUtils;
using BloodSoul.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.DivineGlow
{
    class AwakeningEternalHolyLance : FSMnpc
    {
        float r = 0;
        public EntitySource_ByProjectileSourceId Source_NPC;
        enum SwordAI
        {
            Atk1,
            Atk2,
        }
        private int interval = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Awakening·Eternal Holy Lance");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "真·永恒圣枪");
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 10000;
            NPC.defense = 25;
            NPC.damage = 125/3;
            NPC.knockBackResist = 0f;
            NPC.width = 62;
            NPC.height = 62;
            NPC.value = 0;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Light");
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(interval);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            interval = reader.ReadInt32();
        }
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.Pi / 4;
            int Feng = ModContent.ProjectileType<Light>();
            if (Target.dead)
            {
                NPC.life = 0;
                return;
            }
            //因为ai变得很简单，所以不需要状态机
            #region 冲刺部分
            if (Time1 < 30) Time1 = 150;
            else Time1--;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi / 4;
            if (Time1 == 30)
            {
                Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                    .ToRotationVector2() * 700;
                Dust.NewDustDirect(center, 10, 10, MyDustId.YellowGoldenFire);
                NPC.position = center;
                NPC.netUpdate = true;
            }
            else if (Time1 == 150 && Main.netMode != 1)
            {
                SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                NPC.velocity = ToTarget;
                NPC.netUpdate = true;
                interval++;
            }
            #endregion
            #region 随机产生弹幕部分
            if (Main.rand.Next(200) == 0 && Main.netMode != 1)
            {
                Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                    .ToRotationVector2() * Main.rand.Next(300, 500);
                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), center, (Target.Center - center).SafeNormalize(Vector2.UnitX) * 5
                    , Feng, 60, 2f, Main.myPlayer);
            }
            #endregion
        }
        public override bool CheckActive()
        {
            Player player = Main.player[NPC.target];
            if (player.dead) return true;
            return false;
        }
        public override void OnKill()
        {
            var player = Main.player[NPC.target];
                Vector2 ToPlayer = player.Center - NPC.Center;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 4)).ToRotationVector2() * 16;
                    Projectile.NewProjectile(Source_NPC, NPC.position, r,
                    ModContent.ProjectileType<Light>(), 100 / 6, 0f, Main.myPlayer);
                    interval++;
                }
            base.OnKill();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2 = BloodSoulUtils.GetTexture("Images/Ray").Value;
            Vector2 drawOrigin2;
            drawOrigin2 = new Vector2(texture2.Width * 0.5f, texture2.Height * 0.5f);
            Main.spriteBatch.Draw(texture2, NPC.Center - Main.screenPosition, null, new Color(170, 170, 255, 0), r, drawOrigin2, new Vector2(1.7f, 1.7f), SpriteEffects.None, 0);
            return true;
        }
    }
}
