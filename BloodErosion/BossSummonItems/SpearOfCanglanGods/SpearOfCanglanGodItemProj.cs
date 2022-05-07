using System;
using System.Collections.Generic;
using System.IO;
using BloodErosion.Particle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BloodErosion.Items.Boss.SpearOfCanglanGods
{
    public class SpearOfCanglanGodItemProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("CanglanSpearProj");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.tileCollide = false;
        }
        public float LaserLength
        {
            get
            {
                return base.Projectile.localAI[1];
            }
            set
            {
                base.Projectile.localAI[1] = value;
            }
        }
        public int i = 0;
        public float scale = 0;
        public float LaserLengthMax = 1000f;
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 5)
                {
                    Projectile.frame = 0;
                }
            }
            if (scale < 1)
            {
                scale += 0.05f;
            }
            Player player = Main.player[Main.myPlayer];
            float w = (Main.MouseWorld - player.Center).ToRotation();
            float x2 = (float)Math.Cos(w) * 10;
            float y = (float)Math.Sin(w) * 10;
            Projectile.velocity = new Vector2(x2, y);
            Projectile.rotation = Projectile.velocity.ToRotation() + 3.14f / 2;
            Vector2 unit = Vector2.Normalize(Main.MouseWorld - player.Center);
            Projectile.Center = player.Center + unit * 14f;
            Projectile.spriteDirection = player.direction;
            player.ChangeDir(base.Projectile.direction);
            player.heldProj = base.Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(base.Projectile.velocity.Y * base.Projectile.direction), (double)(base.Projectile.velocity.X * base.Projectile.direction));
            if (Main.mouseLeft)
            {
                Projectile.timeLeft = 60;
                if (Main.mouseX > Main.screenWidth / 2)
                {
                    player.direction = 1;
                }
                else
                {
                    player.direction = -1;
                }
            }
            else
            {
                Projectile.Kill();
            }
            Projectile.velocity = Vector2.Normalize(Projectile.velocity);
                        i++;
            if (i == 10)
            {
                for (int i = 0; i < Main.rand.Next(1, 3); i++)
                {
                    var particle = new LightParticle(player.Center + unit * 70f + Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(20), Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(3));
                    ParticleSystem.NewParticle(particle);
                }
                i = 0;
            }
float[] array = new float[2];
            Collision.LaserScan(Projectile.Center, Projectile.velocity, 0f, LaserLengthMax, array);

            float num = 0f;
            for (int i = 0; i < array.Length; i++)
            {
                num += array[i];
            }
            num /= (float)array.Length;
            float amount = 0.75f;
            this.LaserLength = MathHelper.Lerp(this.LaserLength, num, amount);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float num = 0f;
            return new bool?(Collision.CheckAABBvLineCollision(Utils.TopLeft(targetHitbox), Utils.Size(targetHitbox), base.Projectile.Center, base.Projectile.Center + base.Projectile.velocity * this.LaserLength, (float)projHitbox.Width, ref num));
        }

        Vector2[] Vlaser = new Vector2[601];
        public override void PostDraw(Color lightColor)
        {
            float R = 0f;
            if (Projectile.spriteDirection == -1)
            {
                R = 4f;
            }
            else
            {
                R = -4f;
            }
            var effect = MyUtils.BloodSoulUtils.GetEffect("XNB/Content/Trail");
            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            float step = 4;
            int Count = 0;
            for (int m = 0; m < 600; ++m)
            {
                if (Collision.SolidCollision(Projectile.Center - new Vector2(0, step).RotatedBy(Projectile.rotation) * m, 1, 1))
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone,default, Main.GameViewMatrix.ZoomMatrix);

                    Texture2D texture = MyUtils.BloodSoulUtils.GetTexture("Images/Effects_Textures_840-1").Value;
                    Vector2 drawOrigin;
                    drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                    Main.spriteBatch.Draw(texture, Projectile.Center - new Vector2(0, step).RotatedBy(Projectile.rotation) * m - Main.screenPosition, null, Color.White, (float)(-Main.time * 0.03f), drawOrigin, Projectile.scale, 0, 0);

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
                    break;
                }
                Vlaser[m] = Projectile.Center - new Vector2(0, step).RotatedBy(Projectile.rotation) * m;
                ++Count;
            }
            for (int k = 1; k < Count; k++)
            {
                if (Vlaser[k] == Vector2.Zero) break;

                var normalDir = Vlaser[k - 1] - Vlaser[k];
                normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
                float alpha = 0.2f;
                var factor = k / Count;

                var color = Color.Lerp(Color.White, Color.Red, 0.2f);
                var w = MathHelper.Lerp(1f, 0.05f, alpha);
                float width = 25* scale;

                bars.Add(new CustomVertexInfo(Vlaser[k] + normalDir * width, new Color(255, 0, 0, 0), new Vector3((float)Math.Sqrt(factor), 1, w)));
                bars.Add(new CustomVertexInfo(Vlaser[k] + normalDir * -width, new Color(255, 0, 0, 0), new Vector3((float)Math.Sqrt(factor), 0, w)));
            }

            List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();
            if (bars.Count > 2)
            {
                // 按照顺序连接三角形
                triangleList.Add(bars[0]);
                triangleList.Add(bars[1]);
                triangleList.Add(bars[2]);
                for (int i = 0; i < bars.Count - 2; i += 2)
                {
                    triangleList.Add(bars[i]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 1]);

                    triangleList.Add(bars[i + 1]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 3]);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                // 干掉注释掉就可以只显示三角形栅格
                //RasterizerState rasterizerState = new RasterizerState();
                //rasterizerState.CullMode = CullMode.None;
                //rasterizerState.FillMode = FillMode.WireFrame;
                //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

                // 把变换和所需信息丢给shader
                effect.Parameters["uTransform"].SetValue(model * projection);
                effect.Parameters["uTime"].SetValue(-(float)Main.time * 0.03f);
                Main.graphics.GraphicsDevice.Textures[0] = MyUtils.BloodSoulUtils.GetTexture("Images/HeatMap_1").Value;
                Main.graphics.GraphicsDevice.Textures[1] = MyUtils.BloodSoulUtils.GetTexture("Images/Extra_197").Value;
                Main.graphics.GraphicsDevice.Textures[2] = MyUtils.BloodSoulUtils.GetTexture("Images/Style_8").Value; 
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
                //Main.graphics.GraphicsDevice.Textures[0] = Main.magicPixel;
                //Main.graphics.GraphicsDevice.Textures[1] = Main.magicPixel;
                //Main.graphics.GraphicsDevice.Textures[2] = Main.magicPixel;

                effect.CurrentTechnique.Passes[0].Apply();

                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                Main.graphics.GraphicsDevice.RasterizerState = originalState;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
            }
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D Tex2D = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 MydrawOrigin = new Vector2(Tex2D.Width * 0.5f, Tex2D.Height* 0.5f);
            int height = Tex2D.Height;
            int y = height;
            Rectangle rectangle = new Rectangle(0, y, Tex2D.Width, height);
            Main.spriteBatch.Draw(Tex2D, Projectile.Center - Main.screenPosition, rectangle, Color.White, Projectile.rotation + MathHelper.Pi / R, MydrawOrigin, Projectile.scale, spriteEffects, 0f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        // 自定义顶点数据结构，注意这个结构体里面的顺序需要和shader里面的数据相同
        private struct CustomVertexInfo : IVertexType
        {
            private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
            {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
            });
            public Vector2 Position;
            public Color Color;
            public Vector3 TexCoord;

            public CustomVertexInfo(Vector2 position, Color color, Vector3 texCoord)
            {
                this.Position = position;
                this.Color = color;
                this.TexCoord = texCoord;
            }
            public VertexDeclaration VertexDeclaration
            {
                get
                {
                    return _vertexDeclaration;
                }
            }
        }
    }
}
