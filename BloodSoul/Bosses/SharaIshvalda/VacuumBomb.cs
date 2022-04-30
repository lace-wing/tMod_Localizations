using System;
using BloodSoul.Buffs;
using BloodSoul.MyUtils;
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
    public class VacuumBomb : BaseProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("真空振动佛珠弹");
        }
        public override void SetDef()
        {
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void AI()
        {
            Projectile.scale -= 0.005f;
            Projectile.velocity *= 0;
        }
        float r = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawSunOrigin;
            drawSunOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, drawSunOrigin, Projectile.scale * 1.5f, (Projectile.spriteDirection == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            r += 0.01f;
            Texture2D texture3 = BloodSoulUtils.GetTexture("NPCs/Bosses/SharaIshvalda/VacuumBomb3").Value;
            Vector2 drawOrigin3;
            drawOrigin3 = new Vector2(texture3.Width * 0.5f, texture3.Height * 0.5f);
            Main.spriteBatch.Draw(texture3, Projectile.Center - Main.screenPosition, null, new Color(240, 230, 140, 0), -r, drawOrigin3, new Vector2(4f, 4f), SpriteEffects.None, 0);
            BloodSoulWay.ProjectileDrawTail(Projectile, TextureAssets.Projectile[Projectile.type].Value, new Color(255, 215, 0, 100));
            Texture2D texture4 = BloodSoulUtils.GetTexture("NPCs/Bosses/SharaIshvalda/VacuumBomb4").Value;
            Vector2 drawOrigin4;
            drawOrigin4 = new Vector2(texture4.Width * 0.5f, texture4.Height * 0.5f);
            Main.spriteBatch.Draw(texture4, Projectile.Center - Main.screenPosition, null, new Color(240, 230, 140, 0), -r, drawOrigin4, Projectile.scale, SpriteEffects.None, 0);
            BloodSoulWay.ProjectileDrawTail(Projectile, TextureAssets.Projectile[Projectile.type].Value, new Color(255, 215, 0, 100));
            return false;
        }
        public  EntitySource_ByProjectileSourceId projectileSource;

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
            var modifier = new PunchCameraModifier(Projectile.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 70f, 46f, 70, 1000f);
            Main.instance.CameraModifiers.Add(modifier);
            Projectile.NewProjectile(projectileSource,Projectile.Center, new Vector2(0, 0), ModContent.ProjectileType<VacuumBombBoom>(), 250 / 6, 0, 0);
        }
    }
}
