﻿using System;
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
    public class Storm : BaseProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("沙尘暴预警");
        }
        public override void SetDef()
        {
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 75;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.alpha = 255;
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
            Projectile.velocity *= 0;
            Projectile.alpha--;
        }
        float r = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawSunOrigin;
            drawSunOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(255, 215, 0, 100) * Projectile.Opacity, Projectile.rotation, drawSunOrigin, new Vector2(1f, 2.5f), (Projectile.spriteDirection == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            return false;
        }
        public  EntitySource_ByProjectileSourceId projectileSource;

        public override void Kill(int timeLeft)
        {
            var modifier = new PunchCameraModifier(Projectile.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 10f, 6f, 30, 1000f);
            Main.instance.CameraModifiers.Add(modifier);
            Projectile.NewProjectile(projectileSource,Projectile.Center, new Vector2(0, 0), ProjectileID.SandnadoHostileMark, 145 / 6, 0, 0);
        }
    }
}
