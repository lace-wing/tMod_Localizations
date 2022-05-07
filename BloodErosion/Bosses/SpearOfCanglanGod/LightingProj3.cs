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
    class LightingProj3 : BaseProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ElectricArcProj3");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "电弧弹幕3");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            Main.projFrames[Projectile.type] = 4;
        }
        public int i;
        public override void SetDef()
        {
            Projectile.width = 34;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
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
            if (Projectile.timeLeft % 30 == 0)
            {
                Projectile.velocity = Projectile.velocity.RotatedByRandom(2.14f) / 1.4f;
            }
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
