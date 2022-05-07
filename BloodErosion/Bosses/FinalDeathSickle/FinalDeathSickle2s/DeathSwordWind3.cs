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
    class DeathSwordWind3 : BaseProj
    {
        private Vector2 OldVec = Vector2.Zero;
        private int TimeV = 0;
        private int Time1 = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("FinalDeathSwordWind");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "最终死神剑风");
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
        }
        private float rad;
        public override void AI()
        {
            TimeV++;
            Time1++;
            Projectile.rotation = TimeV * 7;
            float r = (float)Math.Sin(TimeV) * 0.5f;
            // 哈哈，这是个内置的写法，能减少你的代码量（不用写Atan2和Cos，Sin了）
            if(Time1 >= 10)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(0.1);
                Time1 = 0;
            }
            
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
