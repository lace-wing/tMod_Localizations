using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.ID;
using BloodSoul.Projectiles;
using BloodSoul;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using BloodSoul.MyUtils;

namespace BloodErosion.NPCs.Bosses.SpearOfCanglanGod.SpearOfCanglanGod2
{
	public class ThunderLightning : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("ThunderLightning");
		}
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.aiStyle = 171;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 240;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.extraUpdates = 1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 60;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public Color StartColor(float alphaChannelMultiplier = 1f, float lerpToWhite = 0f, float? rawHueOverride = null)
		{
			Color color3 = new Color(0, 191, 255);
			color3 *= Projectile.Opacity;
			if (lerpToWhite != 0f)
			{
				color3 = Color.Lerp(color3, Color.White, lerpToWhite);
			}
			color3.A = (byte)((float)color3.A * alphaChannelMultiplier);
			return color3;
		}
		private int FindTargetWithLineOfSight(float maxRange = 800f)
		{
			float num = maxRange;
			int result = -1;
			for (int i = 0; i < 200; i++)
			{
				NPC npc = Main.npc[i];
				bool flag = npc.CanBeChasedBy(Projectile, false);
				if (Projectile.localNPCImmunity[i] != 0)
				{
					flag = false;
				}
				if (flag)
				{
					float num2 = Projectile.Distance(Main.npc[i].Center);
					if (num2 < num && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
					{
						num = num2;
						result = i;
					}
				}
			}
			return result;
		}
		public override void AI()
		{
			bool flag = false;
			bool flag2 = false;
			float num = 140f;
			float num2 = 30f;
			float scaleFactor = 0.98f;
			float value = 0.05f;
			float value2 = 0.1f;
			float scaleFactor2 = 30f;

			num = 180f;
			num2 = 20f;
			scaleFactor2 = 30f;
			scaleFactor = 0.97f;
			value = 0.075f;
			value2 = 0.125f;
			if (Projectile.timeLeft == 238)
			{
				int num3 = Projectile.alpha;
				Projectile.alpha = 0;
				Color fairyQueenWeaponsColor = StartColor(1f, 0f, null);
				Projectile.alpha = num3;
				for (int i = 0; i < 3; i++)
				{
					Dust dust = Dust.NewDustPerfect(Projectile.Center, 267, new Vector2?(Main.rand.NextVector2CircularEdge(3f, 3f) * (Main.rand.NextFloat() * 0.5f + 0.5f)), 0, fairyQueenWeaponsColor, 1f);
					dust.scale *= 1.2f;
					dust.noGravity = true;
				}
			}

			if ((float)Projectile.timeLeft > num)
			{
				flag = true;
			}
			else if ((float)Projectile.timeLeft > num2)
			{
				flag2 = true;
			}
			if (flag)
			{
				float num4 = (float)Math.Cos((double)((float)Projectile.whoAmI % 6f / 6f + Projectile.position.X / 320f + Projectile.position.Y / 160f));
				Projectile.velocity *= scaleFactor;
				Projectile.velocity = Projectile.velocity.RotatedBy((double)(num4 * 6.28318548f * 0.125f * 1f / 30f), default(Vector2));
			}
			if (Projectile.friendly)
			{
				int num5 = (int)Projectile.ai[0];
				if (Main.npc.IndexInRange(num5) && !Main.npc[num5].CanBeChasedBy(Projectile, false))
				{
					num5 = -1;
					Projectile.ai[0] = -1f;
					Projectile.netUpdate = true;
				}
				if (num5 == -1)
				{
					int num6 = FindTargetWithLineOfSight(800f);
					if (num6 != -1)
					{
						Projectile.ai[0] = (float)num6;
						Projectile.netUpdate = true;
					}
				}
			}
			if (flag2)
			{
				int num7 = (int)Projectile.ai[0];
				Vector2 value3 = Projectile.velocity;
				if (Projectile.hostile && Main.player.IndexInRange(num7))
				{
					Player player = Main.player[num7];
					value3 = Projectile.DirectionTo(player.Center) * scaleFactor2;
				}
				if (Projectile.friendly)
				{
					if (Main.npc.IndexInRange(num7))
					{
						NPC npc = Main.npc[num7];
						value3 = Projectile.DirectionTo(npc.Center) * scaleFactor2;
					}
					else
					{
						Projectile.timeLeft -= 2;
					}
				}
				float amount = MathHelper.Lerp(value, value2, Utils.GetLerpValue(num, 30f, (float)Projectile.timeLeft, true));
				Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, value3, amount);

				Projectile.velocity *= MathHelper.Lerp(0.85f, 1f, Utils.GetLerpValue(0f, 90f, (float)Projectile.timeLeft, true)) / 1.2f;

			}
			Projectile.Opacity = Utils.GetLerpValue(240f, 220f, (float)Projectile.timeLeft, true);
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57079637f;
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
