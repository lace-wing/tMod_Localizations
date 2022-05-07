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
using BloodSoul;

namespace BloodErosion.NPCs.Bosses.DivineGlow
{
    class AwakeningHolySwordProj : BaseProj
    {
        private Vector2 OldVec = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("AwakeningHolySwordProj");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "真·圣剑弹幕");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
        }
        public override void SetDef()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.light = 0.2f;
        }
        public override void AI()
        {
            Projectile.rotation = OldVec.ToRotation() + MathHelper.Pi / 4;
            foreach (NPC npc in Main.npc)
            {
                if(npc.type == ModContent.NPCType<AwakeningHolySword>() && npc.type == ModContent.NPCType<AwakeningHolySword>() && !npc.active)
                {
                    Projectile.active = false;
                }
            }
            switch(State)
            {
                case 0://记录速度
                    {
                        OldVec = Projectile.velocity;
                        State = 1;
                        break;
                    }
                case 1://速度变0
                    {
                        Projectile.velocity *= 1;
                        break;
                    }
                case 2://速度恢复
                    {
                        Projectile.velocity = OldVec;
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi / 4;
                        break;
                    }
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
        public override bool PreDraw(ref Color lightColor)
        {
            BloodErosionWay.ProjectileDrawTail3(Projectile, Color.LightCyan);
            return false;
        }
    }
}
