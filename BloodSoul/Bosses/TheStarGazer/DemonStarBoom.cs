using System;
using System.Collections.Generic;
using System.Linq;
using BloodSoul.Buffs;
using BloodSoul.Particle;
using BloodSoul.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.TheStarGazer
{
    public class DemonStarBoom : BaseProj
    {
        public int i = 0;
        public override void SetDef()
        {
            Projectile.width = 192;
            Projectile.height = 192;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("妖星爆破");
        }
        public Color color = new Color(153, 50, 204, 25);
        private bool BoomGlow;
        public override void AI()
        {
            if (!BoomGlow)
            {
                BoomGlowTimer += 2;
                if (BoomGlowTimer > 60)
                {
                    BoomGlow = true;
                    BoomGlowTimer = 0;
                }
            }
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 5)
                    Projectile.Kill();
            }
            Projectile.scale += 0.06f;
        }
        private float BoomGlowTimer;
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color?(color * Projectile.Opacity);
        }
        public override bool? CanHitNPC(NPC target) => Projectile.frame > 2 ? null : false;
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 60;
            target.immune = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 5;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D teleportGlow = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect2 = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin2 = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position2 = Projectile.Center - Main.screenPosition;
            Color colour2 = Color.Lerp(new Color(153, 50, 204), new Color(153, 50, 204), 1f / BoomGlowTimer * 10f) * (1f / BoomGlowTimer * 10f);
            if (!BoomGlow)
            {
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2, Projectile.rotation, origin2, 6f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2 * 0.3f, Projectile.rotation, origin2, 16f, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
