using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.ID;
using BloodSoul.Projectiles;
using BloodSoul;
using System;

namespace BloodErosion.NPCs.Bosses.FinalDeathSickle.FinalDeathSickle2s
{
    class DeathSickleProj : BaseProj
    {
        private Vector2 OldVec = Vector2.Zero;
        private int TimeV = 0;
        private int Time1 = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("DeathSickleProj");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "最终死神镰分身");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
        }
        public override void SetDef()
        {
            Projectile.width = 58;
            Projectile.height = 42;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.light = 0.3f;
            Projectile.alpha = 100;
        }
        public override void AI()
        {
            TimeV++;
            Projectile.rotation = TimeV * 7;
            var t = Main.time * 0.1f;
            var player = Main.player[Projectile.owner];
            // 要把弹幕速度归零，否则圆会有一个位移
            Projectile.velocity = Vector2.Zero;
            // 半径r = 50，以玩家中心为圆心
            Projectile.Center = player.Center + new Vector2((float)Math.Cos(t), (float)Math.Sin(t)) * 100f;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle TargetHitbox)
        {
            float r = 0;
            if (Collision.CheckAABBvLineCollision(TargetHitbox.TopLeft(), TargetHitbox.Size(),
                Projectile.Center + (Projectile.rotation - MathHelper.Pi / 4).ToRotationVector2() * 16,
                Projectile.Center + (Projectile.rotation - MathHelper.Pi / 4).ToRotationVector2() * -16, 10, ref r))
            {
                return true;
            }
            return false;
        }
        public override void ModifyHitPlayer(Player Target, ref int damage, ref bool crit)
        {
            damage = ModifyHitDamage(damage);
        }
        public override bool CanHitPlayer(Player Target)
        {
            return Projectile.Distance(Target.Center) < (Projectile.width / 2 + Projectile.height / 2);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            BloodErosionWay.ProjectileDrawTail3(Projectile, Color.LightCyan);
            return false;
        }
    }
}
