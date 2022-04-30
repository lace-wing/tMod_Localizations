using BloodSoul.MyUtils;
using BloodSoul.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace BloodSoul.NPCs.Bosses.SharaIshvalda
{
    public class AirStar : BaseProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("空气弹");
        }
        public override void SetDef()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            //Projectile.aiStyle = 171;
            //Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public Color StartColor(float alphaChannelMultiplier = 1f, float lerpToWhite = 0f, float? rawHueOverride = null)
        {
            Color color3 = new Color(250, 250, 210);
            color3 *= Projectile.Opacity;
            if (lerpToWhite != 0f)
            {
                color3 = Color.Lerp(color3, Color.White, lerpToWhite);
            }
            color3.A = (byte)((float)color3.A * alphaChannelMultiplier);
            return color3;
        }
        public override void AI()
        {
            Projectile.rotation = Utils.ToRotation(Projectile.velocity) + (float)(Math.PI / 2);
            Player player = Main.player[Projectile.owner];
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D value11 = TextureAssets.Projectile[Projectile.type].Value;
            int num163 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y3 = num163 * Projectile.frame;
            Rectangle rectangle4 = new Rectangle(0, y3, value11.Width, num163);
            Vector2 origin4 = rectangle4.Size() / 2f;
            Vector2 zero = Vector2.Zero;
            float num164 = 0f;
            int num166;
            int num167;
            int num168;
            float value12;
            float num169;
            float num170 = 0f;
            Rectangle rectangle5 = rectangle4;

            num168 = 19;
            num169 = 20f;
            num166 = 0;
            num167 = -1;
            value12 = 0.7f;

            Color color30 = Lighting.GetColor((int)((double)Projectile.position.X + (double)Projectile.width * 0.5) / 16, (int)(((double)Projectile.position.Y + (double)Projectile.height * 0.5) / 16.0));

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            int num171 = num168;
            while ((num167 > 0 && num171 < num166) || (num167 < 0 && num171 > num166))
            {
                if (num171 < Projectile.oldPos.Length)
                {
                    Color color31 = color30;

                    Projectile projectile3 = Projectile;
                    float alphaChannelMultiplier = 0.5f;
                    float lerpToWhite = 0f;
                    color31 = StartColor(alphaChannelMultiplier, lerpToWhite, null);
                    color31 *= Utils.GetLerpValue(0f, 20f, (float)Projectile.timeLeft, true);

                    float num176 = (float)(num166 - num171);
                    if (num167 < 0)
                    {
                        num176 = (float)(num168 - num171);
                    }
                    color31 *= num176 / ((float)ProjectileID.Sets.TrailCacheLength[Projectile.type] * 1.5f);
                    Vector2 value14 = Projectile.oldPos[num171];
                    float num177 = Projectile.rotation;
                    SpriteEffects effects2 = spriteEffects;
                    if (ProjectileID.Sets.TrailingMode[Projectile.type] == 2 || ProjectileID.Sets.TrailingMode[Projectile.type] == 3 || ProjectileID.Sets.TrailingMode[Projectile.type] == 4)
                    {
                        num177 = Projectile.oldRot[num171];
                        effects2 = ((Projectile.oldSpriteDirection[num171] == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                    }
                    if (!(value14 == Vector2.Zero))
                    {
                        Vector2 position2 = value14 + zero + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                        Main.spriteBatch.Draw(value11, position2, new Rectangle?(rectangle5), color31, num177 + num164 + Projectile.rotation * num170 * (float)(num171 - 1) * (float)(-(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt()), origin4, MathHelper.Lerp(Projectile.scale, value12, (float)num171 / num169), effects2, 0);
                    }
                }
                num171 += num167;
            }
            Color color35 = Color.Gold * 0.5f;
            color35.A = 0;

            Projectile projectile4 = Projectile;
            float alphaChannelMultiplier2 = 0f;
            float lerpToWhite2 = 0f;
            float? rawHueOverride = null;
            color35 = StartColor(alphaChannelMultiplier2, lerpToWhite2, rawHueOverride);

            Vector2 vector29 = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Main.spriteBatch.Draw(value11, vector29, new Rectangle?(rectangle4), color35, Projectile.rotation, origin4, Projectile.scale * 0.9f, spriteEffects, 0);
            Texture2D value18 = BloodSoulUtils.GetTexture("Images/Tail").Value;
            Color color36 = color35;
            Vector2 origin5 = value18.Size() / 2f;
            Color color37 = color35 * 0.5f;
            float num182 = Utils.GetLerpValue(15f, 30f, (float)Projectile.timeLeft, true) * Utils.GetLerpValue(240f, 200f, (float)Projectile.timeLeft, true) * (1f + 0.2f * (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 30f / 0.5f * 6.28318548f * 3f))) * 0.8f;
            Vector2 vector30 = new Vector2(0.5f, 5f) * num182;
            Vector2 vector31 = new Vector2(0.5f, 2f) * num182;
            color36 *= num182;
            color37 *= num182;
            int num183 = 0;
            Vector2 position3 = vector29 + Projectile.velocity.SafeNormalize(Vector2.Zero) * MathHelper.Lerp(0.5f, 1f, Projectile.localAI[0] / 60f) * (float)num183;

            vector30 *= 0.4f;
            vector31 *= 0.4f;

            Main.spriteBatch.Draw(value18, position3, null, color36, 1.57079637f, origin5, vector30, spriteEffects, 0);
            Main.spriteBatch.Draw(value18, position3, null, color36, 0f, origin5, vector31, spriteEffects, 0);
            Main.spriteBatch.Draw(value18, position3, null, color37, 1.57079637f, origin5, vector30 * 0.6f, spriteEffects, 0);
            Main.spriteBatch.Draw(value18, position3, null, color37, 0f, origin5, vector31 * 0.6f, spriteEffects, 0);

            Color color41 = Projectile.GetAlpha(color30);
            float num189 = Projectile.scale;
            float rotation24 = Projectile.rotation + num164;

            color41.A /= 2;

            Main.spriteBatch.Draw(value11, Projectile.Center + zero - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle4), color41, rotation24, origin4, num189, spriteEffects, 0);
            return false;
        }
    }
}
