using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.TheStarGazer
{
    public class BeamOfStars2 : ModProjectile
    {
        public override string Texture => "BloodSoul/NPCs/Bosses/TheStarGazer/BeamOfStars";
        public float AITimer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public float Frame
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        public float LaserLength = 0;
        public float LaserScale = 2;
        public int LaserSegmentLength = 60;
        public int LaserWidth = 76;
        public int LaserEndSegmentLength = 60;

        //should be set to about half of the end length
        private const float FirstSegmentDrawDist = 30;

        public int MaxLaserLength = 1800;
        public int maxLaserFrames = 1;
        public int LaserFrameDelay = 5;
        public bool StopsOnTiles = false;
        // >
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ÌìÐÇÖ®Êø");
        }

        public override void SetDefaults()
        {
            Projectile.width = LaserWidth;
            Projectile.height = LaserWidth;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
            Projectile.alpha = 255;
        }

        public override bool CanHitPlayer(Player target) => AITimer >= 80;
        public override bool? CanHitNPC(NPC target) => target.friendly && AITimer >= 80 ? null : false;


        public override void AI()
        {
            Projectile.rotation = MathHelper.PiOver2;
            Player player = Main.player[BSHelper.GetNearestAlivePlayer(Projectile)];
            Projectile.MoveToVector2(player.Center + new Vector2(0, -800), 2);
            Projectile.position.Y = player.position.Y - 800;

            #region Beginning And End Effects
            if (AITimer == 80)
                SoundEngine.PlaySound(SoundID.Item122);
            if (AITimer >= 80)
            {
                Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 3;
                Projectile.alpha -= 10;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);
            }
            else
            {
                Projectile.alpha -= 10;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 200, 255);
            }
            if (Projectile.timeLeft < 10)
            {
                if (Projectile.timeLeft > 10)
                {
                    Projectile.timeLeft = 10;
                }
                Projectile.alpha += 20;
            }
            #endregion

            #region Length Setting
            if (StopsOnTiles)
            {
                EndpointTileCollision();
            }
            else
            {
                LaserLength = MaxLaserLength;
            }
            #endregion

            ++AITimer;

            #region misc
            //CutTiles();
            if (AITimer >= 80)
                CastLights();
            #endregion
        }

        #region Laser AI Submethods
        private void EndpointTileCollision()
        {
            for (LaserLength = FirstSegmentDrawDist; LaserLength < MaxLaserLength; LaserLength += LaserSegmentLength)
            {
                Vector2 start = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * LaserLength;
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, start, 1, 1))
                {
                    LaserLength -= LaserSegmentLength;
                    break;
                }
            }
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy(Projectile.rotation);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * LaserLength, (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
        }
        private void CastLights()
        {
            // Cast a light along the line of the Laser
            DelegateMethods.v3_1 = new Vector3(1f, 0.7f, 0f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserLength, 26, DelegateMethods.CastLight);
        }
        #endregion

        #region Drawcode
        // The core function of drawing a Laser, you shouldn't need to touch this
        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default, int transDist = 1)
        {

            float pulse = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 1.8f, 1.2f, 1.8f, 1.2f);
            float r = unit.ToRotation() + rotation;
            // Draws the Laser 'body'
            for (float i = transDist; i <= (maxDist * (1 / LaserScale)); i += LaserSegmentLength)
            {
                var origin = start + i * unit;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    new Rectangle((int)(80 * Frame), LaserEndSegmentLength, 80, LaserSegmentLength), color, r,
                    new Vector2(80 / 2, LaserSegmentLength / 2), scale, 0, 0);

                Main.EntitySpriteDraw(texture, origin - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    new Rectangle((int)(80 * Frame), LaserEndSegmentLength, 80, LaserSegmentLength), color * 0.5f, r,
                    new Vector2(80 / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            }
            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(80 * Frame), 0, 80, LaserEndSegmentLength), color, r, new Vector2(80 / 2, LaserSegmentLength / 2), scale, 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + maxDist * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(80 * Frame), LaserSegmentLength + LaserEndSegmentLength, 80, LaserEndSegmentLength), color, r, new Vector2(256 / 2, LaserSegmentLength / 2), scale, 0, 0);

            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(80 * Frame), 0, 80, LaserEndSegmentLength), color * 0.5f, r, new Vector2(80 / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + maxDist * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(80 * Frame), LaserSegmentLength + LaserEndSegmentLength, 80, LaserEndSegmentLength), color * 0.5f, r, new Vector2(80 / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1f, 0.8f, 1f, 0.8f, 1f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Projectile.GetAlpha(Color.White) * opacity, (int)FirstSegmentDrawDist);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        #endregion

        #region Collisions
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy(Projectile.rotation);
            float point = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + unit * LaserLength, 48 * LaserScale, ref point))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region MP Sync
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(LaserLength);
            writer.Write(LaserScale);
            writer.Write(LaserSegmentLength);
            writer.Write(LaserEndSegmentLength);
            writer.Write(LaserWidth);
            writer.Write(MaxLaserLength);
            //writer.Write(maxLaserFrames);
            //writer.Write(LaserFrameDelay);
            writer.Write(StopsOnTiles);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            LaserLength = reader.ReadSingle();
            LaserScale = reader.ReadSingle();
            LaserSegmentLength = reader.ReadInt32();
            LaserEndSegmentLength = reader.ReadInt32();
            LaserWidth = reader.ReadInt32();
            MaxLaserLength = reader.ReadInt32();
            StopsOnTiles = reader.ReadBoolean();
            //maxLaserFrames = reader.
        }
        #endregion
    }
    public static class BSHelper
    {
        public static int GetNearestAlivePlayer(this Projectile projectile)
        {
            float nearestPlayerDist = 4815162342f;
            int nearestPlayer = -1;
            foreach (Terraria.Player player in Main.player)
            {
                if (!(player.Distance(projectile.Center) < nearestPlayerDist) || !player.active)
                    continue;

                nearestPlayerDist = player.Distance(projectile.Center);
                nearestPlayer = player.whoAmI;
            }

            return nearestPlayer;
        }
        public static void MoveToVector2(this Projectile projectile, Vector2 p, float moveSpeed)
        {
            float velMultiplier = 1f;
            Vector2 dist = p - projectile.Center;
            float length = dist == Vector2.Zero ? 0f : dist.Length();
            if (length < moveSpeed)
            {
                velMultiplier = MathHelper.Lerp(0f, 1f, length / moveSpeed);
            }

            if (length < 100f)
            {
                moveSpeed *= 0.5f;
            }

            if (length < 50f)
            {
                moveSpeed *= 0.5f;
            }

            projectile.velocity = length == 0f ? Vector2.Zero : Vector2.Normalize(dist);
            projectile.velocity *= moveSpeed;
            projectile.velocity *= velMultiplier;
        }
        public static void Shoot(this Terraria.NPC npc, Vector2 position, int projType, int damage, Vector2 velocity,
            bool customSound, LegacySoundStyle sound, string soundString = "", float ai0 = 0, float ai1 = 0)
        {
            Mod mod = BloodSoul.Instance;
            if (customSound && !Main.dedServ)
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(mod, soundString), npc.position);
            else
                SoundEngine.PlaySound(sound, (int)npc.position.X, (int)npc.position.Y);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(null, position, velocity, projType, damage / 4, 0,
                    Main.myPlayer, ai0, ai1);
            }
        }
    }
    public class ScreenPlayer : ModPlayer
    {
        public float ScreenShakeIntensity;
        public override void ModifyScreenPosition()
        {
            if (ScreenShakeIntensity > 0.1f)
            {
                Main.screenPosition += new Vector2(Main.rand.NextFloat(ScreenShakeIntensity),
                    Main.rand.NextFloat(ScreenShakeIntensity));

                ScreenShakeIntensity *= 0.9f;
            }
        }
    }
}