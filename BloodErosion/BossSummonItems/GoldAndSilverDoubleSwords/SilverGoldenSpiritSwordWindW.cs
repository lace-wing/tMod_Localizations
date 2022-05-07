﻿using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Terraria.IO;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System;
using BloodSoul.MyUtils;
using Terraria.GameContent;
using BloodSoul.Projectiles;

namespace BloodErosion.Items.Boss.GoldAndSilverDoubleSwords
{
    class SilverGoldenSpiritSwordWindW : BaseProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silver Golden Spirit Sword Wind");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese,"金银剑风");
        }
        public override void SetDef()
        {
            Projectile.width = 22;
            Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 1000;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.light = 0.2f;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            float v = Projectile.velocity.ToRotation();
            Projectile.rotation = v;
            Timer++;
            if (Timer == 10)
            {
                Projectile.velocity *= 1.3f;
                Projectile.netUpdate = true;
                Timer = 0;
            }
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin);
            dust.noGravity = true;
            //草
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value; // 弹幕的贴图

            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type]; // 获取弹幕一帧的高度.

            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f); // 开始绘制的原点.

            for (int k = 0; k < Projectile.oldPos.Length; k++) //开启一个循环 , 用于决定残影的长度. 这里取了比较合适的值.
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY); //绘制位置.

                Color color = Projectile.GetAlpha(lightColor * 0.5f) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length); // 透明度决定.

                //残影绘制.
                Main.spriteBatch.Draw(projectileTexture, drawPos, new Rectangle(0, frameHeight * Projectile.frame, projectileTexture.Width, frameHeight), color, Projectile.rotation, drawOrigin, MathHelper.Lerp(Projectile.scale, 0.75f, (Projectile.oldPos.Length + k) / 15f), SpriteEffects.None, 0f);
            }
        }
        
        public override void ModifyHitPlayer(Player Target, ref int damage, ref bool crit)
        {
            damage = ModifyHitDamage(damage);
        }
    }
}
