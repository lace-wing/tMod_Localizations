using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Terraria.IO;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System;
using BloodSoul.Buffs;
using BloodSoul.Projectiles;
using BloodSoul;

namespace BloodErosion.NPCs.Bosses.SpearOfCanglanGod
{
    class LightingProj2 : BaseProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ElectricArcProj2");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "电弧弹幕2");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            Main.projFrames[Projectile.type] = 4;
        }
        public int i;
        private float rad;

        public override void SetDef()
        {
            Projectile.width = 34;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.light = 0.5f;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            float v = Projectile.velocity.ToRotation();
            Projectile.rotation = v;
            i++;
            if (i % 10 == 0)
            {
                Projectile.velocity *= 1.2f;
                Projectile.netUpdate = true;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            var center = Projectile.Center + Projectile.velocity * 3;
            // 角度随机增加变化
            rad += Main.rand.NextFloatDirection() * 0.5f;
            // 半径50，圆上的点
            var pos = center + rad.ToRotationVector2() * 50;
            Projectile.velocity = Vector2.Normalize(pos - Projectile.Center) * 10f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitPlayer(Player Target, int damage, bool crit)
        {
            Target.AddBuff(144, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            BloodErosionWay.ProjectileDrawTail3(Projectile, Color.LightCyan);
            return false;
        }
    }
}
