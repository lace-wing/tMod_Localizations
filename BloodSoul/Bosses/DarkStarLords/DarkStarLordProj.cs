using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.DarkStarLords
{
    public class DarkStarLordProj : ModProjectile
    {
        private float WaveState
        {
            get
            {
                return (int)Projectile.ai[0];
            }
            set
            {
                Projectile.ai[0] = value;
            }
        }
        private int WaveTimer
        {
            get
            {
                return (int)Projectile.ai[1];
            }
            set
            {
                Projectile.ai[1] = value;
            }
        }
        private int Amplitude
        {
            get
            {
                return (int)Projectile.localAI[0];
            }
        }
        private Vector2 tanDir = Vector2.Zero;
        private float colorlerp = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.ignoreWater = true;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {            
            Projectile.rotation += 1f;
            if (Projectile.timeLeft == 600)
            {
                tanDir = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Math.PI / 2);
                Projectile.velocity = tanDir * WaveState * -Amplitude + Projectile.velocity;
            }
            if (WaveTimer >= Amplitude * 2)
            {
                WaveState = -WaveState;
                WaveTimer = 0;
            }
            WaveTimer++;
            Projectile.velocity = tanDir * WaveState + Projectile.velocity;
            colorlerp += 0.05f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle = texture.Bounds;
            Vector2 origin = rectangle.Size() / 2f;
            Color color = Projectile.GetAlpha(lightColor);
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 offset = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
                int length = (int)offset.Length();
                float scale = Projectile.scale * (float)Math.Sin(i / MathHelper.Pi) * 0.5f;
                offset.Normalize();
                for (int j = 0; j < length; j += 3)
                {
                    Vector2 vector = Projectile.oldPos[i] + offset * j;
                    Main.spriteBatch.Draw(texture, vector + Projectile.Size / 2f - Main.screenPosition,
                        rectangle, color, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                }
            }
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {

            return Color.Lerp(Color.Gold, Color.DarkRed, 0.5f + (float)Math.Sin(colorlerp * 2) / 2);
        }
    }
}
