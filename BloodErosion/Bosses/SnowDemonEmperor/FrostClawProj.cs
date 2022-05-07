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

namespace BloodErosion.NPCs.Bosses.SnowDemonEmperor
{
	public class FrostClawProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("FrostClawProj");
		}
		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 16;
			Projectile.aiStyle = 171;
			Projectile.penetrate = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 240;
			Projectile.DamageType = DamageClass.Ranged;
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
			Color color3 = new Color(102, 179, 255);
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

				Projectile.velocity *= MathHelper.Lerp(0.85f, 1f, Utils.GetLerpValue(0f, 90f, (float)Projectile.timeLeft, true)) / 1.15f;

			}
			Projectile.Opacity = Utils.GetLerpValue(240f, 220f, (float)Projectile.timeLeft, true);
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57079637f;
		}
		public override void OnHitPlayer(Player Target, int damage, bool crit)
		{
			Target.AddBuff(BuffID.Frostburn, 300);
		}
		public override bool PreDraw(ref Color lightColor)
		{
			BloodErosionWay.ProjectileDrawTail3(Projectile, Color.LightCyan);
			return false;
		}
	}
}
