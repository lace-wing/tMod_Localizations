using System;
using BloodSoul.Buffs;
using BloodSoul.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.SharaIshvalda
{
    public class VacuumBomb21 : BaseProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("真空佛珠弹");
        }
        public override void SetDef()
        {
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 50;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void AI()
        {
            if (Projectile.timeLeft > 20)
            {
                Projectile.Opacity = 1;
            }
            else
            {
                Projectile.Opacity = Projectile.timeLeft / 15f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawSunOrigin;
            drawSunOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(250, 250, 210, 100) * Projectile.Opacity, Projectile.rotation, drawSunOrigin, Projectile.scale * 1.5f, (Projectile.spriteDirection == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            BloodSoulWay.ProjectileDrawTail(Projectile, TextureAssets.Projectile[Projectile.type].Value, new Color(250, 250, 210, 100));
            return false;
        }
        public  EntitySource_ByProjectileSourceId projectileSource;

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
            var modifier = new PunchCameraModifier(Projectile.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 20f, 6f, 30, 1000f);
            Main.instance.CameraModifiers.Add(modifier);
            Projectile.NewProjectile(projectileSource,Projectile.Center, new Vector2(0, 0), ModContent.ProjectileType<VacuumBoom2>(), 150 / 6, 0, 0);  
        }
    }
}
