using BloodSoul.MyUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.ApostleOfDeath
{
    class DeathSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        public override void Update(GameTime gameTime)
        {
            /*if (BloodErosionGlobalNPC.AwakeningDeathApostles2 != -1)
            {
                int whoAmi = BloodErosionGlobalNPC.AwakeningDeathApostles2;
                if (Main.npc[whoAmi].active)
                {
                    if (intensity < 1f)
                    {
                        intensity += 0.03f;
                    }
                }
                else
                {
                    intensity -= 0.03f;
                    if (intensity < 0f)
                    {
                        intensity = 0f;
                        Deactivate();
                    }
                }
            }
            else
            {
                intensity -= 0.03f;
                if (intensity < 0f)
                {
                    intensity = 0f;
                    Deactivate();
                }
            }*/
        }
        public override void Reset()
        {
            isActive = false;
            intensity -= 0.03f;
        }
        public override bool IsActive()
        {
            if (Main.gameMenu)
            {
                intensity -= 0.03f;
                if (intensity < 0f)
                {
                    intensity = 0f;
                    Deactivate();
                }
            }
            return isActive;
        }
        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
            if (intensity >= 1) intensity = 1;
        }
        public static Asset<Texture2D> GetTexture(string path)
        {
            return ModContent.Request<Texture2D>("BloodErosion/" + path);
        }
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 3E29 && minDepth < 3E29)
            {
                Texture2D texture2 = GetTexture("NPCs/Bosses/ApostleOfDeath/DeathSky").Value;
                Main.spriteBatch.Draw(texture2,new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * intensity);
            }
            if (BloodErosionGlobalNPC.AwakeningDeathApostles2 != -1)
            {
                int whoAmi = BloodErosionGlobalNPC.AwakeningDeathApostles2;
                if (Main.npc[whoAmi].active)
                {
                    if (intensity < 1f)
                    {
                        intensity += 0.03f;
                    }
                }
                else
                {
                    intensity -= 0.03f;
                    if (intensity < 0f)
                    {
                        intensity = 0f;
                        Deactivate();
                    }
                }
            }
            else
            {
                intensity -= 0.03f;
                if (intensity < 0f)
                {
                    intensity = 0f;
                    Deactivate();
                }
            }
        }
        public override void Deactivate(params object[] args)
        {
            if (intensity <= 0)
            {
                isActive = false;
                BloodErosionGlobalNPC.AwakeningDeathApostles2 = -1;
            }
        }
        public override Color OnTileColor(Color inColor)
        {
            return new Color(Vector4.Lerp(new Vector4(0.6f, 0.9f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
        }
        public override float GetCloudAlpha()
        {
            return 1f - intensity;
        }
    }
}
