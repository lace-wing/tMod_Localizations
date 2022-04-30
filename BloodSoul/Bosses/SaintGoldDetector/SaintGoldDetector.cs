using BloodSoul.MyUtils;
using BloodSoul.Projectiles.Bosses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.SaintGoldDetector
{
    public class SaintGoldDetector : FSMnpc
    {
        private int frameSwitch = 0;//切换时间大小
        private int bombFrame = 0;//每一帧的帧图的
        private int texFrame = 0;//切换轰炸图的
        private bool _fireBomb = false;//发射导弹
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Saint Gold Detector");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "圣金探测器");
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 2500;
            NPC.defense = 20;
            NPC.damage = 85;
            NPC.boss = true;
            NPC.knockBackResist = 0f;
            NPC.width = 60;
            NPC.height = 60;
            NPC.value = Item.buyPrice(0, 11, 45, 14);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath10;
            NPC.aiStyle = -1;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
        }
        public override void AI()
        {
            GetNPCTarget();
            Vector2 toTarget = Target.position - NPC.position;
            frameSwitch++;
            if (frameSwitch == 60)
            {
                if (!_fireBomb && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    _fireBomb = true;
                    Vector2[] vectors = new Vector2[4]
                    {
                        Main.screenPosition,
                        Main.screenPosition + new Vector2(Main.screenWidth,0),
                        Main.screenPosition + new Vector2(Main.screenWidth,Main.screenHeight),
                        Main.screenPosition + new Vector2(0,Main.screenHeight),
                    };
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), vectors[i], Vector2.One, ModContent.ProjectileType<SaintGoldSighting>(), 210, 1.2f, Main.myPlayer, Target.whoAmI);
                    }
                }
            }
            switch (State1)
            {
                case 0://召唤两个圣金近卫机
                    {
                        NPC.velocity = toTarget.SafeNormalize(toTarget);
                        for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 50)
                        {
                            Vector2 center = NPC.Center + (i.ToRotationVector2() * 500);
                            int dust = Dust.NewDust(center, 1, 1, DustID.Enchanted_Gold);
                            Main.dust[dust].velocity *= 0;
                            Main.dust[dust].noGravity = true;
                        }
                        if (NPC.position.Distance(Target.position) > 500)
                        {
                            Target.velocity = -toTarget.SafeNormalize(toTarget) * 3;
                        }
                        if (State2 == 0)
                        {
                            State2++;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    int whoAmi = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<SaintGoldGuards>(), 0, 0, 0, 0, NPC.whoAmI);
                                    Main.npc[whoAmi].velocity = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 3;
                                    if (Main.netMode == NetmodeID.Server) NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, whoAmi);
                                }
                            }
                            NPC.dontTakeDamage = true;
                        }
                        else
                        {
                            if (State2 > 2)
                            {
                                //NPC.dontTakeDamage = false;
                            }
                        }
                        break;
                    }
                case 1://一阶段死亡ai
                    {
                        break;
                    }
            }
        }
        public override bool CheckDead()
        {
            return false;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > frameHeight * 3)
                {
                    NPC.frame.Y = 0;
                    _fireBomb = false;
                }
            }
        }
        public override bool PreKill()
        {
            return base.PreKill();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            BloodSoulWay.NpcDrawTail(NPC, drawColor, Color.White);
            if (frameSwitch > 60)
            {
                Texture2D texture = BloodSoulUtils.GetTexture("Images/SJBomb" + (texFrame + 1).ToString()).Value;
                //左上
                Main.spriteBatch.Draw(texture, Vector2.Zero, new Rectangle(0, (int)(texture.Height / 8 * bombFrame), texture.Width, texture.Height / 8), Color.White,
                    0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                //左下
                Main.spriteBatch.Draw(texture, new Vector2(0, Main.screenHeight), new Rectangle(0, (int)(texture.Height / 8 * bombFrame), texture.Width, texture.Height / 8), Color.White,
                    MathHelper.PiOver2 + MathHelper.Pi, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                //右上
                Main.spriteBatch.Draw(texture, new Vector2(Main.screenWidth, 0), new Rectangle(0, (int)(texture.Height / 8 * bombFrame), texture.Width, texture.Height / 8), Color.White,
                    MathHelper.PiOver2, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                //右下
                Main.spriteBatch.Draw(texture, new Vector2(Main.screenWidth, Main.screenHeight), new Rectangle(0, (int)(texture.Height / 8 * bombFrame), texture.Width, texture.Height / 8), Color.White,
                    MathHelper.Pi, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                bombFrame++;
                if (bombFrame > 7)
                {
                    bombFrame = 0;
                    texFrame++;
                    if (texFrame >= 5)
                    {
                        texFrame = 0;
                        frameSwitch = 0;
                        foreach (Projectile projectile in Main.projectile)
                        {
                            if (projectile.type == ModContent.ProjectileType<SaintGoldSighting>() && projectile.active)
                            {
                                projectile.Kill();
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
