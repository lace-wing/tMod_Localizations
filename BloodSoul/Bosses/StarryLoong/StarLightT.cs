using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;

namespace BloodSoul.NPCs.Bosses.StarryLoong
{
    public class StarLightT : ModProjectile
    {
        public float SC = 1;
        public bool SCT = true;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        }
        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.light = 0.01f;
            Projectile.timeLeft = 240;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public Color StartColor(float alphaChannelMultiplier = 1f, float lerpToWhite = 0f, float? rawHueOverride = null)
        {
            Color color3 = new Color(75, 0, Main.DiscoB + 90);
            color3 *= Projectile.Opacity;
            if (lerpToWhite != 0f)
            {
                color3 = Color.Lerp(color3, Color.White, lerpToWhite);
            }
            color3.A = (byte)((float)color3.A * alphaChannelMultiplier);
            return color3;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            Color color = StartColor(0, 0, null);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Main.spriteBatch.Draw(projectileTexture, drawPos, new Rectangle(0, frameHeight * Projectile.frame, projectileTexture.Width, frameHeight), color, Projectile.rotation + 1.57f, drawOrigin, MathHelper.Lerp(Projectile.scale, 0.75f, (Projectile.oldPos.Length + k) / 15f), SpriteEffects.None, 0f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture = ModContent.Request<Texture2D>("BloodSoul/Images/Tail").Value;
            float num1 = Utils.GetLerpValue(15f, 30f, 240, true) * Utils.GetLerpValue(240, 200f, 120, true) * (1f + 0.2f * (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 30f / 0.5f * 6.28318548f * 3f))) * 0.8f;
            Vector2 vector = new Vector2(1f * Projectile.scale, 1.5f * Projectile.scale) * num1;
            Vector2 vector2 = new Vector2(1f * Projectile.scale, 1f * Projectile.scale) * num1;
            Vector2 vector3 = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Vector2 position = vector3 + Projectile.velocity.SafeNormalize(Vector2.Zero) * MathHelper.Lerp(0.5f, 1f, Projectile.localAI[0] / 60f) * 0;
            Rectangle rectangle = new Rectangle(0, TextureAssets.Projectile[Projectile.type].Value.Height, TextureAssets.Projectile[Projectile.type].Value.Width, TextureAssets.Projectile[Projectile.type].Value.Height);
            float alphaChannelMultiplier2 = 0f;
            float lerpToWhite2 = 0f;
            float? rawHueOverride = null;
            Color color = StartColor(alphaChannelMultiplier2, lerpToWhite2, rawHueOverride);

            Main.spriteBatch.Draw(texture, position, null, color, 1.57079637f + Main.GlobalTimeWrappedHourly, texture.Size() * 0.5f, vector, spriteEffects, 0);
            Main.spriteBatch.Draw(texture, position, null, color, 0 + Main.GlobalTimeWrappedHourly, texture.Size() * 0.5f, vector2, spriteEffects, 0);
            Main.spriteBatch.Draw(texture, position, null, color, 1.57079637f + Main.GlobalTimeWrappedHourly, texture.Size() * 0.5f, vector, spriteEffects, 0);
            Main.spriteBatch.Draw(texture, position, null, color, 0 + Main.GlobalTimeWrappedHourly, texture.Size() * 0.5f, vector2, spriteEffects, 0);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            for(int i = 0; i <= 30; i ++)
            {
                int num1 = Dust.NewDust(Projectile.Center, 0, 0, 272, 0,0);
                Dust dust = Main.dust[num1];
                dust.scale = 1.5f;
                dust.noGravity = true;

            }
        }
    }
}
