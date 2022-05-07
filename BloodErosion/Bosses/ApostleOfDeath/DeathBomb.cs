using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Terraria.IO;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System;
using BloodSoul.MyUtils;
using Terraria.GameContent;
using BloodSoul.Projectiles;

namespace BloodErosion.NPCs.Bosses.ApostleOfDeath
{
    class DeathBomb : BaseProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("DeathBomb");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese,"死神爆弹");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDef()
        {
            Projectile.width = 40;
            Projectile.height = 42;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.light = 0.1f;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 7)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            Projectile.rotation = Utils.ToRotation(Projectile.velocity) + (float)(Math.PI / 2);
            Player player = Main.player[Projectile.owner];
            Player Target = null;
            float distanceMax = 250;
            foreach (Player player1 in Main.player)
            {
                if (player.active)
                {
                    float currentDistance = Vector2.Distance(player.Center, Projectile.Center);
                    if (currentDistance < distanceMax)
                    {
                        distanceMax = currentDistance;
                        Target = player;
                    }
                }
            }
            if (Target != null)
            {
                Vector2 TargetVec = Target.Center - Projectile.Center;
                TargetVec.Normalize();
                TargetVec *= ((int)Projectile.ai[0] == 1) ? 30f : 20f;
                Projectile.velocity = (Projectile.velocity * 30f + TargetVec) / 31f;
            }
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value; // 弹幕的贴图

            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type]; // 获取弹幕一帧的高度.

            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f); // 开始绘制的原点.

            for (int k = 0; k < Projectile.oldPos.Length; k++) //开启一个循环 , 用于决定残影的长度. 这里取了比较合适的值.
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY); //绘制位置.

                Color color = Projectile.GetAlpha(lightColor * 0.5f) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length); // 透明度决定.

                //残影绘制.
                Main.spriteBatch.Draw(projectileTexture, drawPos, new Rectangle(0, frameHeight * Projectile.frame, projectileTexture.Width, frameHeight), color, Projectile.rotation, drawOrigin, MathHelper.Lerp(Projectile.scale, 0.75f, (Projectile.oldPos.Length + k) / 15f), SpriteEffects.None, 0f);
            }
        }
        public override void Kill(int timeLeft)
        {
            var NPC = Main.npc[Projectile.owner];
            Vector2 plrToMouse = Main.MouseWorld - NPC.Center;
            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
            for (int i = -5; i <= 6; i++)
            {
                float r2 = r + i * MathHelper.Pi / 6f;
                Vector2 shootVel = r2.ToRotationVector2() * -10;
                Terraria.Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, shootVel, ModContent.ProjectileType<DeadLight4>(), 185 / 6, 10, NPC.whoAmI);
            }
            SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
            for (int i = 0; i < 25; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Shadowflame, Projectile.oldVelocity.X * 0.2f, Projectile.oldVelocity.Y * 0.2f);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle TargetHitbox)
        {
            float r = 0;
            if (Collision.CheckAABBvLineCollision(TargetHitbox.TopLeft(), TargetHitbox.Size(),
                Projectile.Center + (Projectile.rotation - MathHelper.Pi / 2).ToRotationVector2() * 16,
                Projectile.Center + (Projectile.rotation - MathHelper.Pi / 2).ToRotationVector2() * -16, 10, ref r))
            {
                return true;
            }
            return false;
        }
        public override void ModifyHitPlayer(Player Target, ref int damage, ref bool crit)
        {
            damage = ModifyHitDamage(damage);
        }
    }
}
