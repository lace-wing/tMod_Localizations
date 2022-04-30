using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.ID;
using System;
using BloodSoul.Projectiles.Bosses.GelSpider;
using System.IO;
using BloodSoul.Items.BossBag;
using Terraria.GameContent.ItemDropRules;

namespace BloodSoul.NPCs.Bosses.GelSpider
{
    [AutoloadBossHead]
    public class GelSpider : FSMnpc
    {
        public bool HasGelCobweb = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gel Spider");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "凝膠毒蛛");
            Main.npcFrameCount[Type] = 19;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 6200 / 3;
            NPC.damage = 125 / 3;
            NPC.defense = 35;
            NPC.boss = true;
            NPC.friendly = false;
            NPC.width = 80;
            NPC.height = 80;
            NPC.DeathSound = SoundID.NPCDeath34;
            NPC.HitSound = SoundID.NPCHit23;
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            DrawOffsetY = 20;
            BossBag = ModContent.ItemType<GelSpiderBossBag>();
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Spider");
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(HasGelCobweb);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            HasGelCobweb = reader.ReadBoolean();
        }
        public override void AI()
        {
            if (!Target.active || Target.dead || NPC.target == 255 || NPC.target < 0)
            {
                NPC.TargetClosest();
            }//懒得调用基类
            NPC.spriteDirection = NPC.direction = ((Target.position - NPC.position).X > 0).ToDirectionInt();
            int damage = NPC.GetAttackDamage_ForProjectiles_MultiLerp(30, 20, 13);

            if(Vector2.Distance(Target.Center,NPC.Center) > 1000 || HasGelCobweb)//距离过远吐蛛丝
            {
                NPC.velocity.X *= 0.9f;
                if(!HasGelCobweb)
                {
                    HasGelCobweb = true;
                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.One, ModContent.ProjectileType<GelCobweb>(),
                        0, 0, Main.myPlayer, Target.whoAmI, NPC.whoAmI);
                    NPC.netUpdate = true;
                }
                return;
            }
            if(Target.dead)//死亡
            {
                NPC.velocity.X += NPC.velocity.X;//逃跑
                if (NPC.velocity.X < 0.1f) NPC.velocity.X = 0.1f;
                if (Math.Abs(NPC.velocity.X) > 10) NPC.active = false;
                return;
            }
            switch(State1)
            {
                case 0://所有玩家向我看齐，我宣布一件事（
                    {
                        //我是个傻逼（
                        Timer1++;
                        foreach(Player player in Main.player)
                        {
                            if(player.active && !player.dead)
                            {
                                player.GetModPlayer<BloodSoulPlayer>().GelSpiderControl = NPC.whoAmI;
                            }
                        }
                        if(Timer1 > 30)
                        {
                            string[] sayText = new string[3]
                            {
                                "是你杀了史莱姆王?",
                                "有意思......",
                                "那你做好被杀的觉悟了吗?"
                            };
                            CombatText.clearAll();
                            CombatText.NewText(NPC.Hitbox, Color.Blue, sayText[(int)Timer2], true);
                            Timer1 = 0;
                            Timer2++;
                            if(Timer2 >= 3)
                            {
                                Timer2 = 0;
                                State1++;//跳出说话ai
                                foreach (Player player in Main.player)
                                {
                                    if (player.active && !player.dead)
                                    {
                                        player.GetModPlayer<BloodSoulPlayer>().GelSpiderControl = -1;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 1://戳刺
                    {
                        Timer1++;
                        if (NPC.velocity.X > -8 && NPC.velocity.X < 8) NPC.velocity.X += NPC.spriteDirection == 1 ? 0.5f : -0.5f;
                        else NPC.velocity.X *= 0.8f;
                        if (Timer1 % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)//发射弹幕
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSourceForProjectileNPC(), NPC.Center, (Target.position - NPC.position).SafeNormalize(default) * 10,
                                ModContent.ProjectileType<GelWind>(), damage, 1.2f, Main.myPlayer);
                            if(Timer1 >= 20)
                            {
                                Timer1 = 0;
                                State1++;
                            }
                        }
                        break;
                    }
                case 2://正常走路跳跃
                    {
                        Timer1++;
                        if (Timer1 == 70)
                        {
                            NPC.velocity.Y = -12;
                        }
                        else if (Timer1 < 70 && Timer1 > 30)
                        {
                            if (NPC.velocity.X > -8 && NPC.velocity.X < 8) NPC.velocity.X += NPC.spriteDirection == 1 ? 0.5f : -0.5f;
                            else NPC.velocity.X *= 0.8f;
                        }
                        else if (Timer1 > 120 && NPC.collideY || Timer1 > 300)
                        {
                            Timer1 = 0;
                            NPC.velocity.X *= 0.6f;
                            State1++;
                        }
                        if (NPC.collideX)
                        {
                            NPC.position.Y -= 16;
                        }
                        break;
                    }
                case 5://高速蓄力旋转冲刺
                    {
                        Timer1++;
                        if (Timer1 >= 80)
                        {
                            Timer1 = 0;
                            Timer2++;
                        }
                        switch (Timer2)
                        {
                            case 0://缓慢加速(比索尼克草点)
                                {
                                    if (NPC.velocity.X > -10 && NPC.velocity.X < 10) NPC.velocity.X += NPC.spriteDirection == 1 ? 0.5f : -0.5f;
                                    else
                                    {
                                        Timer1 = 80;
                                    }
                                    if (NPC.collideX)
                                    {
                                        NPC.position.Y -= 16;
                                    }
                                    break;
                                }
                            case 1://高速冲刺
                            case 2:
                            case 3:
                                {
                                    if (Timer1 == 20 && Timer2 == 1)
                                    {
                                        if (NPC.collideY)
                                        {
                                            NPC.velocity.Y = -12;
                                        }
                                        else
                                        {
                                            NPC.velocity.Y *= 0.6f;
                                        }
                                        NPC.velocity.X = NPC.spriteDirection == 1 ? 15f : -15f;
                                    }
                                    if (NPC.collideY && NPC.velocity.Y > 1)
                                    {
                                        NPC.velocity.Y *= -0.9f;
                                    }
                                    if (NPC.collideX)
                                    {
                                        NPC.velocity.X = -NPC.velocity.X;
                                        NPC.velocity.Y -= 4f;
                                    }
                                    NPC.velocity.X += (Target.position.X - NPC.position.X > 0) ? 0.1f : -0.1f;
                                    if (Math.Abs(NPC.velocity.X) < 15) NPC.velocity.X *= 1.01f;
                                    break;
                                }
                            default://重置状态
                                {
                                    Timer1 = 0;
                                    Timer2 = 0;
                                    State1++;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        for (int i = 0; i < 15; i++)
                                        {
                                            Vector2 vel = Vector2.One.RotatedBy(MathHelper.TwoPi / 15 * i);
                                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center,
                                                vel * 5, ProjectileID.SpikedSlimeSpike, damage, 1.2f, Main.myPlayer);
                                        }
                                    }
                                }
                                break;

                        }
                        break;
                    }
                case 4://嘿~驮
                    {
                        if(NPC.collideY || Timer1 > 50)//Timer1 > 50 是防卡死机制
                        {
                            NPC.velocity *= 0.9f;
                            if (NPC.velocity.Length() < 1f)//速度小
                            {
                                Timer1++;
                                NPC.frame.Y = 0;
                                NPC.frameCounter = 0;
                                if (Timer1 > 30)//嘿 忒
                                {
                                    Timer1 = 0;
                                    State1++;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Main.projectile[Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, (Target.position - NPC.position).SafeNormalize(default) * 10,
                                            ModContent.ProjectileType<GelMissile>(), damage, 1.32f, Main.myPlayer)].alpha = 255;

                                    }
                                }
                            }
                            else
                            {
                                Timer1 = 0;
                            }
                        }
                        else
                        {
                            Timer1++;
                        }
                        break;
                    }
                case 3://高跳
                case 6://变成喷子
                    {
                        Timer1++;
                        if(Timer1 < 20)//减速开始蓄力
                        {
                            NPC.velocity.X *= 0.9f;
                            NPC.frame.Y = 0;
                            NPC.frameCounter = 0;
                        }
                        else if (Timer1 == 20 && State1 == 3)//芜湖起飞
                        {
                            NPC.velocity.Y = -20;
                            NPC.velocity.X = (Target.position.X - NPC.position.X) / 50; 
                        }
                        else if(Timer1 == 30 && State1 == 6)//喷!
                        {
                            for (int i = -3;i <= 3; i++)
                            {
                                Vector2 vel = (Target.position + new Vector2(0,-150) - NPC.position).RotatedBy(MathHelper.PiOver4 / 20 * i);
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center,
                                    vel.SafeNormalize(default) * 15, ProjectileID.SpikedSlimeSpike, damage, 1.2f, Main.myPlayer);
                            }
                        }
                        else if(Timer1 > 70)//准备落地
                        {
                            NPC.velocity.Y++;
                            if(NPC.collideY || NPC.collideX || Timer1 > 300)
                            {
                                Main.dust[Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.BlueCrystalShard)].velocity = Vector2.UnitY * -5;
                                NPC.velocity.X = 0;
                                Timer1 = 0;
                                State1++;
                                if(Main.netMode != NetmodeID.MultiplayerClient && State1 == 3)
                                {
                                    for (int i = 0; i < 15; i++)
                                    {
                                        Vector2 vel = Vector2.One.RotatedBy(MathHelper.TwoPi / 15 * i);
                                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center,
                                            vel * 5, ProjectileID.SpikedSlimeSpike, damage, 1.2f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        break;
                    }
                default:
                    State1 = 1;
                    break;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            switch (State1)
            {
                case 3://旋转冲刺
                case 5://蓄力
                    {
                        bool InChong = false;
                        if(State1 == 3)
                        {
                            InChong = NPC.velocity.Length() > 10;
                        }
                        else if(State1 == 5)
                        {
                            InChong = Timer1 < 70 && Timer1 > 10;
                        }
                        if(InChong)
                        {
                            Texture2D texture = MyUtils.BloodSoulUtils.GetTexture("NPCs/Bosses/GelSpider/GelSpider2").Value;
                            int frameY = (int)(Timer1 / 10) % 4;
                            spriteBatch.Draw(texture,NPC.Center - Main.screenPosition,new Rectangle?(new(0,texture.Height / 4 * frameY,texture.Width,texture.Height / 4)),drawColor,
                                0,new Vector2(texture.Width/2,texture.Height/8),1,SpriteEffects.None,0);
                            return false;
                        }
                        break;
                    }
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
        public override void OnKill()
        {
            BloodSoulSystem.downedGelSpider = true;
        }
        public override void FindFrame(int frameHeight)
        {
            if (State1 == 0) return;//如果是开幕
            //前4帧为走路帧
            //中间7帧是挥砍帧
            //后8帧是跳跃帧
            const int atk_frame = 4;
            const int jump_frame = 11;
            NPC.frameCounter += 0.2;
            NPC.frameCounter += Math.Abs(NPC.velocity.X * 0.03);
            if(NPC.frameCounter > 1.2)
            {
                bool AtkFrame = State1 == 1;
                NPC.frameCounter = 0;
                if (NPC.frame.Y < jump_frame * frameHeight && NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = jump_frame * frameHeight;
                    NPC.frameCounter = 1;
                }//处于刚刚离开地面的状态
                else if(NPC.frame.Y == (jump_frame + 4) * frameHeight && NPC.velocity.Y != 0)
                {
                    NPC.frame.Y = (jump_frame + 4) * frameHeight;
                }//空中飞行时
                else
                {
                    NPC.frame.Y += frameHeight;//其他时候
                }
                if (NPC.frame.Y >= atk_frame * frameHeight && NPC.velocity.Y == 0 && NPC.frame.Y < (jump_frame + 4) * frameHeight && !AtkFrame)
                {
                    NPC.frame.Y = 0;
                }//如果正在走路的帧图
                else if(NPC.frame.Y >= atk_frame * frameHeight && NPC.velocity.Y == 0 && NPC.frame.Y > (jump_frame + 4) * frameHeight && !AtkFrame)
                {
                    NPC.frameCounter = 1;
                }//如果落地
                else if(NPC.frame.Y < atk_frame * frameHeight && AtkFrame)
                {
                    NPC.frame.Y = atk_frame * frameHeight;
                }//处于攻击状态
                else if (NPC.frame.Y >= jump_frame * frameHeight && AtkFrame)
                {
                    NPC.frame.Y = atk_frame * frameHeight;
                }//如果到了跳跃帧,那么切换会攻击帧
                if (NPC.frame.Y > 18 * frameHeight) NPC.frame.Y = 0;//避免帧图切换过头
            }
        }
    }
}
