using System;
using System.Collections.Generic;
using BloodSoul.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.GameContent.Shaders;
using Terraria.GameContent.Skies;
using BloodSoul.MyUtils;
using Terraria.Localization;
using BloodSoul.Projectiles;

namespace BloodSoul.NPCs.Bosses.TheStarGazer
{
    class DemonStarDemonBladeChop : BaseProj
    {
        private int i = 0;
        private int Time
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("DemonStarDemonBladeChop");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese,"妖星刃斩");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDef()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 400;
            Projectile.scale = 2; Projectile.light = 2f;
        }
        private float time = 0;
        public override void AI()
        {
            Time++;
            if(time < 1) { time += 0.05f; }if(time >= 1) { time -= 0.05f; }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi / 2;
            if(Projectile.ai[0] == 1)
            {
                Projectile.velocity *= 1.03f;
                
            }
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
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(153, 50, 204, 100) * Projectile.Opacity, Projectile.rotation, drawSunOrigin, Projectile.scale * 1.5f, (Projectile.spriteDirection == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            BloodSoulWay.ProjectileDrawTail(Projectile, TextureAssets.Projectile[Projectile.type].Value, new Color(153, 50, 204, 100));
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.expertMode)
            {
                damage /= 2;
            }
            if (Main.masterMode)
            {
                damage /= 2;
            }
        }
    }
}
