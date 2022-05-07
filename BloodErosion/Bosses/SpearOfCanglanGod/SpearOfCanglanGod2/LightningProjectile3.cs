using BloodErosion.NPCs.Bosses.SpearOfCanglanGod.SpearOfCanglanGod2.SpearOfCanglanGod3;
using BloodSoul.MyUtils;
using BloodSoul.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.SpearOfCanglanGod.SpearOfCanglanGod2
{
	public class LightningProjectile3 : ModProjectile
	{
		public static float BeamProgress;
		public static float LightningProgress;
		public static float Timer;

		public override void SetDefaults()
		{
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 25;

			Projectile.width = 175;
			Projectile.height = Projectile.width;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI()
		{
			var progress = 1 - Projectile.timeLeft / 25f;
			Timer += 0.01f;
			BeamProgress = MyUtils.BloodSoulUtils.GradientValue<float>(MathHelper.Lerp, progress, new float[] { 0f, 0.1f, 0.2f, 0.9f, 0.45f, 0f });
			LightningProgress = MyUtils.BloodSoulUtils.GradientValue<float>(MathHelper.Lerp, progress, new float[] { 0f, 0f, 0f, 0.7f, 0.3f, 0f });

			Lighting.AddLight(Projectile.Center, new Color(255, 255, 0).ToVector3() * BeamProgress * 1.3f);

			if (Projectile.timeLeft == 16)
			{
				var modifier = new PunchCameraModifier(Projectile.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 20f, 6f, 30, 1000f);
				Main.instance.CameraModifiers.Add(modifier);
			}
			if (LightningProgress >= 0.5f && LightningProgress <= 0.55f)
			{
				NPC.NewNPC(Projectile.GetNPCSource_FromThis(), (int)(Projectile.Center.X), (int)(Projectile.Center.Y), ModContent.NPCType<SpearOfCanglanGod_3>(), Projectile.whoAmI);
			}
			// SoundEngine.PlaySound(SoundLoader.GetSoundSlot(Mod, ""), Projectile.Center);
		}

		public override bool? CanHitNPC(NPC target) => LightningProgress >= 0.5f && Vector2.Distance(target.Center, Projectile.Center) < Projectile.width;
		private static readonly Color _effectColor = new Color(0, 191, 255, 0);
		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			var drawPosition = Projectile.Center - Main.screenPosition;

			var scale = 2f;

			var texture = TextureAssets.Projectile[Type];
			var origin = new Vector2(texture.Width() * 0.5f, texture.Height());
			var offset = Vector2.UnitY * texture.Height() * scale;

			Main.EntitySpriteDraw(MyUtils.BloodSoulUtils.GetTexture("Tiles/Phantom/LightningProjectile").Value, drawPosition, null, _effectColor * BeamProgress, 0f, texture.Size() / 2 + new Vector2(0, texture.Height() / 2), 1, SpriteEffects.None, 0);

			texture = MyUtils.BloodSoulUtils.GetTexture("Images/Lightning1");
			Main.EntitySpriteDraw(texture.Value, drawPosition, null, _effectColor * LightningProgress * 2f, 0f, texture.Size() * 0.5f, scale, SpriteEffects.None, 0);

			texture = MyUtils.BloodSoulUtils.GetTexture("Images/Lightning2");
			Main.EntitySpriteDraw(texture.Value, drawPosition, null, _effectColor * LightningProgress * 0.5f, 0f, texture.Size() * 0.5f, scale * 0.4f, SpriteEffects.None, 0);

			texture = MyUtils.BloodSoulUtils.GetTexture("Images/LightningStar");
			Main.EntitySpriteDraw(texture.Value, drawPosition, null, _effectColor * LightningProgress * 0.5f, 0f, texture.Size() * 0.5f, scale * 0.6f, SpriteEffects.None, 0);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}
	}
}
