using BloodSoul.MyUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BloodErosion.Items.Boss.FlameGhostKings
{
	public class BurningStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("炎星");
		}
        public override void SetDefaults()
		{
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 300;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void AI()
        {
			int rad = 45;
			if(Projectile.ai[0] < 0)
            {
				rad = -45;
			}
			Player player = Main.player[Projectile.owner];
			if (!player.active || player.dead /*|| !player.hasTitaniumStormBuff*/)
			{
				Projectile.Kill();
				return;
			}
			Projectile.rotation += 0.0157079641f;
			int num;
			int num2;
			AI_GetMyGroupIndexAndFillBlackList(null, out num, out num2);
			float f = ((float)num / (float)num2 + player.miscCounterNormalized * 6f) * MathHelper.TwoPi;
			float scaleFactor = 24f + (float)num2 * 6f;
			Vector2 value = player.position - player.oldPosition;
			Projectile.Center += value;
			Vector2 vector = f.ToRotationVector2();
			Projectile.localAI[0] = vector.Y;
			this._orbitInclination = (float)Math.Sin(Main.time * 0.034906584769487381) * MathHelper.ToRadians(rad);//改变rad可以改变倾斜度
			Vector2 value2 = player.Center + Utils.RotatedBy(vector * new Vector2(1f, 0.05f) * scaleFactor, (double)this._orbitInclination, default(Vector2)) - Projectile.Size / 2f;
			Projectile.Center = Vector2.Lerp(Projectile.Center, value2, 0.3f);
		}
		private float _orbitInclination;

		private void AI_GetMyGroupIndexAndFillBlackList(List<int> blackListedTargets, out int index, out int totalIndexesInGroup)
		{
			index = 0;
			totalIndexesInGroup = 0;
			for (int i = 0; i < 1000; i++)
			{
				Projectile projectile = Main.projectile[i];
				if (projectile.active && projectile.owner == Projectile.owner && projectile.type == Projectile.type)
				{
					if (Projectile.whoAmI > i)
					{
						index++;
					}
					totalIndexesInGroup++;
				}
			}
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (target.position.X + (float)(target.width / 2) < Projectile.position.X + (float)(Projectile.width / 2))
			{
				Projectile.direction = -1;
			}
			else
			{
				Projectile.direction = 1;
			}
		}
		public static void DrawWater(SpriteBatch spriteBatch)
		{
			foreach (Projectile d in Main.projectile)
			{
				if (d.type == ModContent.ProjectileType<BurningStar>() && d.active)
				{
					Texture2D tex = BloodSoulUtils.GetTexture("Niuqu").Value;
					spriteBatch.Draw(tex, d.position - Main.screenPosition, null, Color.White, 0, tex.Size() / 2, d.scale, SpriteEffects.None, 0);
				}
			}
			//float scale = (float)Math.Sin((float)DateTime.Now.TimeOfDay.TotalMilliseconds / 500f) / 2f + 0.5f;
			//Texture2D texture2D4 = TextureAssets.Projectile[Projectile.type].Value;
			//spriteBatch.Draw(BloodSoulUtils.GetTexture("Niuqu").Value, Projectile.Center - Main.screenPosition, null, Color.White, 0, Vector2.Zero, 5, 0, 0);
		}
		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

			ProjectileDrawTail3(Projectile, new Color(185, 0, 0));

			Main.spriteBatch.End();
			Main.spriteBatch.Begin();

			//Microsoft.Xna.Framework.Color color30 = Lighting.GetColor((int)((double)Projectile.position.X + (double)Projectile.width * 0.5) / 16, (int)(((double)Projectile.position.Y + (double)Projectile.height * 0.5) / 16.0));
			//         SpriteEffects spriteEffects = SpriteEffects.None;
			//         if (Projectile.spriteDirection == -1)
			//         {
			//             spriteEffects = SpriteEffects.FlipHorizontally;
			//         }
			//         Vector2 vector71 = Projectile.position + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
			//         Texture2D texture2D4 = TextureAssets.Projectile[Projectile.type].Value;
			//         //Microsoft.Xna.Framework.Rectangle rectangle29 = texture2D4.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame, 0, 0);
			//         Microsoft.Xna.Framework.Color color84 = Projectile.GetAlpha(color30);
			//         Vector2 origin21 = texture2D4.Size() / 2f;

			//         //PlayerTitaniumStormBuffTextureContent playerTitaniumStormBuff = TextureAssets.RenderTargets.PlayerTitaniumStormBuff;
			//         vector71 += (Main.GlobalTimeWrappedHourly * 8f + (float)Projectile.whoAmI).ToRotationVector2() * 4f;
			//         //playerTitaniumStormBuff.Request();
			//         //if (playerTitaniumStormBuff.IsReady)
			//         //{
			//         //    texture2D4 = playerTitaniumStormBuff.GetTarget();
			//         //}
			//         //rectangle29 = texture2D4.Frame(Main.projFrames[Projectile.type], 1, Projectile.frame, 0, 0, 0);

			//         origin21 = texture2D4.Size() / 2f;

			//         Main.EntitySpriteDraw(texture2D4, vector71, null, color84, Projectile.rotation, origin21, Projectile.scale, spriteEffects, 0);
			return false;
        }
		public static void ProjectileDrawTail3(Projectile projectile, Color TailColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Color LightColor = Lighting.GetColor((int)(projectile.position.X + projectile.width * 0.5) / 16, (int)((projectile.position.Y + projectile.height * 0.5) / 16.0));
			Texture2D Tex2D = BloodSoulUtils.GetTexture("StarRiverLoong/StarRunes" /*+ Math.Abs((int)projectile.ai[0])*/).Value;
			int height = TextureAssets.Projectile[projectile.type].Height() / Main.projFrames[projectile.type];
			int y = height * projectile.frame;
			Rectangle rectangle = new Rectangle(0, y, Tex2D.Width, height);
			Vector2 origin = rectangle.Size() / 2f;
			float scale = 1.4f;
			Color RealTailColor = LightColor;
			RealTailColor = projectile.GetAlpha(RealTailColor);
			RealTailColor = TailColor;
			RealTailColor.A /= 4;
			Vector2 position2 = projectile.position + Vector2.Zero + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
			Main.EntitySpriteDraw(Tex2D, position2, new Rectangle?(rectangle), RealTailColor, projectile.rotation, origin, MathHelper.Lerp(projectile.scale, scale, 1), spriteEffects, 0);
			Main.EntitySpriteDraw(Tex2D, position2, new Rectangle?(rectangle), new Color(185, 0, 0), projectile.rotation, origin, projectile.scale, spriteEffects, 0);
		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			if (Projectile.localAI[0] <= 0f)
			{
				behindProjectiles.Add(index);
			}
			if (Projectile.localAI[0] > 0f)
			{
				overPlayers.Add(index);
			}
		}
    }
}
