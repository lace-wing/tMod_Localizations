using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Terraria.IO;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using BloodSoul.MyUtils;
using System.Linq;
using BloodSoul.NPCs.Bosses;
using Terraria.GameContent;
using BloodSoul.NPCs.Bosses.HolyLightSwords;
using System;
using BloodSoul.Projectiles;
using Terraria.DataStructures;

namespace BloodErosion.NPCs.Bosses.SpearOfCanglanGod.SpearOfCanglanGod2
{
    class SpearOfCanglanGodProj : BaseProj
    {
        private Vector2 OldVec = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SpearOfCanglanGodProj");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "雷矛之影");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
        }
        public override void SetDef()
        {
            Projectile.width = 74;
            Projectile.height = 74;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.light = 0.2f;
            Projectile.alpha = 255;
        }
        public EntitySource_ByProjectileSourceId projectileSource;
        public override void AI()
        {
            Projectile.rotation = OldVec.ToRotation() + MathHelper.Pi / 4;
            Timer++;
            if(Timer >= 5)
            {
                Projectile.NewProjectile(projectileSource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<LightningProjectile2>(), 155 / 3, 0, 0);
                Timer = 0;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + (Projectile.rotation - MathHelper.Pi / 4).ToRotationVector2() * 16,
                Projectile.Center + (Projectile.rotation - MathHelper.Pi / 4).ToRotationVector2() * -16, 10, ref r))
            {
                return true;
            }
            return false;
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            damage = ModifyHitDamage(damage);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            BloodErosionWay.ProjectileDrawTail3(Projectile, Color.LightCyan);
            return false;
        }
    }
}
