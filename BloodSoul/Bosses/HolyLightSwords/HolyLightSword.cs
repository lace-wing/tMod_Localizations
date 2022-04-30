using BloodSoul.Items.Weapons.SwordSoul;
using BloodSoul.MyUtils;
using BloodSoul.Projectiles.Bosses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;

namespace BloodSoul.NPCs.Bosses.HolyLightSwords
{
    [AutoloadBossHead]
    class HolyLightSword : FSMnpc
    {
        enum HolySword
        {
            St1,
            St2,
            St3,
            St4,
            St5,
            St6,
            St7,
            St8,
        }
        private int interval = 0;
        private bool CanDraw = false;
        //以下是特效部分的变量
        private float alpha = 0;

        private Vector2 targetOldPos;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("远古英灵剑");
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 7500;
            NPC.defense = NPC.defDefense = 20;
            NPC.damage = 95 / 3;
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
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/HolyLightSword");
            }
            BossBag = ModContent.ItemType<Items.BossBag.HolySwordBossBag>();
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            var parameters = new DropOneByOne.Parameters()
            {
                ChanceNumerator = 1,
                ChanceDenominator = 1,
                MinimumStackPerChunkBase = 1,
                MaximumStackPerChunkBase = 1,
                MinimumItemDropsCount = 1,
                MaximumItemDropsCount = 1,
            };
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            int itemType = 0;
            int o = Main.rand.Next(4);
            switch (o)
            {
                case 0:
                    {
                        itemType = ModContent.ItemType<HolySoulStaff>();
                        notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));
                        npcLoot.Add(notExpertRule);
                        break;
                    }
                case 1:
                    {
                        itemType = ModContent.ItemType<HolySoulSwordWand>();
                        notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));
                        npcLoot.Add(notExpertRule);
                        break;
                    }
                case 2:
                    {
                        itemType = ModContent.ItemType<SoulMagicBow>();
                        notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));
                        npcLoot.Add(notExpertRule);
                        break;
                    }
                case 3:
                    {
                        itemType = ModContent.ItemType<HolySwordSword>();
                        notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));
                        npcLoot.Add(notExpertRule);
                        break;
                    }
                default:
                    break;
            }
            npcLoot.Add(ItemDropRule.BossBag(BossBag));
        }
        public override bool CheckActive()
        {
            if (!NPC.HasValidTarget)
            {
                return true;
            }
            return false;
        }
        public override bool CheckDead()
        {
            if (State1 != (int)HolySword.St8)
            {
                State1 = (int)HolySword.St8;
                Timer1 = 0;
                Timer2 = 0;
                interval = 0;
                NPC.dontTakeDamage = true;
                NPC.life = 5;
                return false;
            }
            return true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Main.netMode == 2)
            {
                writer.Write(CanDraw);
                writer.Write(alpha);
                writer.Write(interval);
                writer.Write(State3);
                writer.Write(State4);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode != 1)
            {
                CanDraw = reader.ReadBoolean();
                alpha = reader.ReadSingle();
                interval = reader.ReadInt32();
                State3 = reader.ReadSingle();
                State4 = reader.ReadSingle();
            }
        }
        public override void BossHeadRotation(ref float rotation) => rotation = NPC.rotation;
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }//获取敌人

            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.Pi / 4;
            if (NPC.life < NPC.lifeMax * 0.7f)
            {
                SwitchState2(1);
            }//切换阶段

            if(NPC.life < NPC.lifeMax * 0.5f && State3 != 1)
            {
                State3 = 1;
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + 50), (int)NPC.Center.Y,ModContent.NPCType<HolySwordFS>());
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)(NPC.Center.X - 50), (int)NPC.Center.Y, ModContent.NPCType<HolySwordFS>());
            }//召唤分身

            if(Target.dead)
            {
                Timer4++;
                if(Timer4 == 20)
                {
                    NPC.velocity *= 0;
                    CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height)
                        , Color.Gold, "不，你的实力还不够，下次再来找我吧", true, false);
                }
                else if(Timer4 > 25)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDustDirect(NPC.position, 30, 30, DustID.GlowingSnail, Main.rand.NextFloat(), Main.rand.NextFloat());
                    }
                    NPC.life = 0;
                }
                foreach (Projectile p in Main.projectile)
                {
                    if (p.type == ModContent.ProjectileType<SwordQi1>() && p.active)
                    {
                        p.active = false;
                        p.netUpdate = true;
                    }
                }
                return;
            }//玩家死亡部分

            Send(2000, Target.Center.X, Target.Center.Y + 1000);//距离过远传送

            switch ((HolySword)State1)
            {
                case HolySword.St1://开幕
                    {
                        NPC.rotation = NPC.velocity.ToRotation() - MathHelper.Pi / 4;
                        if (Timer1 < 400)
                        {
                            alpha += 0.05f;
                            if (alpha > 1)
                            {
                                alpha = 1;
                            }
                        }
                        else
                        {
                            alpha -= 0.05f;
                            if (alpha < 0)
                            {
                                alpha = 0;
                            }
                            NPC.netUpdate = true;
                        }
                        NPC.velocity *= 0;
                        NPC.dontTakeDamage = true;
                        Timer1++;
                        Color messageColor = Color.Gold;
                        switch (Timer1)
                        {
                            case 100:
                                {
                                    CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y,
                                        NPC.width, NPC.height), messageColor, "吸引吾之前来", true, false);
                                    break;
                                }
                            case 200:
                                {
                                    CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y,
                                        NPC.width, NPC.height), messageColor, "是否准备好与吾战斗", true, false);
                                    break;
                                }
                            case 300:
                                {
                                    CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y,
                                        NPC.width, NPC.height), messageColor, "呵...", true, false);
                                    break;
                                }
                            case 400:
                                {
                                    CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y,
                                        NPC.width, NPC.height), messageColor, "当心命陨此地！", true, false);
                                    break;
                                }
                        }
                        if (Timer1 > 480)
                        {
                            SwitchState1((int)HolySword.St2);
                            NPC.dontTakeDamage = false;
                            Timer1 = 0;
                        }
                        break;
                    }
                case HolySword.St2://旋转释放剑风
                    {
                        Timer1++;
                        if (Timer1 == 1)
                        {
                            Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                .ToRotationVector2() * 300;

                            Timer2 = center.X;
                            Timer3 = center.Y;//记录位置
                        }
                        else if (Timer1 > 10 && Timer1 <= 30)
                        {
                            Vector2 center = new(Timer2, Timer3);

                            if(Timer1 == 30)
                            {
                                NPC.position = center;//改变位置
                                NPC.velocity = ToTarget.SafeNormalize(default) * 5;//使npc具有动能
                                NPC.netUpdate = true;//更新npc
                            }

                        }
                        else if (Timer1 > 30)
                        {
                            if (interval >= 6 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Timer1 = Timer2 = Timer3 = interval = 0;//重置计时器

                                if (State2 == 0)
                                {
                                    SwitchState1((int)HolySword.St2, (int)HolySword.St5 + 1);
                                }
                                else if (State2 == 1)
                                {
                                    SwitchState1((int)HolySword.St2, (int)HolySword.St7 + 1);
                                }//切换ai

                                if (Main.rand.Next(5) == 1) SwitchState1(11);//切换到摸鱼ai
                                break;
                            }
                            NPC.velocity = NPC.velocity.RotatedBy(0.055f) * -1;
                            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi / 2;
                            if (Timer1 % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, (NPC.rotation - MathHelper.Pi / 2).ToRotationVector2() * 5
                                    , ModContent.ProjectileType<SwordWind2>(), 43, 2f, Main.myPlayer);
                                interval++;
                            }
                        }
                        break;
                    }
                case HolySword.St3://冲刺
                    {
                        if(Timer1 < 30) Timer1 = 80;
                        else Timer1--;//冲刺冷却时间减少
                        CanDraw = true;//改变拖尾颜色
                        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi / 4;//改变npc旋转方向

                        if(Timer1 % 6 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var proj = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, NPC.velocity
                                , ModContent.ProjectileType<SwordQi1>(), 45, 2f, Main.myPlayer);
                            proj.timeLeft = 114514;
                        }
                        if (Timer1 == 60)
                        {
                            int r = Main.rand.Next(6);//确定随机出现的位置
                            Vector2 center = Target.position + (MathHelper.TwoPi / 6 * r).ToRotationVector2() * 700;//确定点的位置
                            Timer2 = center.X;
                            Timer3 = center.Y;
                        }
                        else if (Timer1 == 30)
                        {
                            NPC.position = new(Timer2, Timer3);
                            NPC.netUpdate = true;//改变位置
                        }
                        else if (Timer1 == 80 && Main.netMode != NetmodeID.MultiplayerClient)//冲刺
                        {
                            NPC.velocity = ToTarget * 1.5f;
                            NPC.netUpdate = true;
                            interval++;
                        }
                        else
                        {
                            if (Timer2 != 0 && Timer3 != 0)
                            {
                                Vector2 center = new(Timer2, Timer3);
                                Dust dust = Dust.NewDustDirect(center, 5, 5, DustID.YellowStarDust);
                                dust.noGravity = true;
                            }//产生粒子
                        }
                        if(interval >= 6 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Timer1 = Timer2 = Timer3 = interval = 0;
                            foreach(Projectile projectile in Main.projectile)
                            {
                                if(projectile.type == ModContent.ProjectileType<SwordQi1>())
                                {
                                    projectile.Kill();
                                }
                            }
                            if (State2 == 0)
                            {
                                SwitchState1((int)HolySword.St2, (int)HolySword.St5 + 1);
                            }
                            else
                            {
                                SwitchState1((int)HolySword.St2, (int)HolySword.St7 + 1);
                            }
                            if (Main.rand.Next(5) == 1) SwitchState1(11);
                        }
                        break;
                    }
                case HolySword.St4://斜角冲1
                    {
                        if (Timer2 == 0)
                        {
                            Vector2 vector = new Vector2(Target.position.X - 1000, Target.position.Y - 400);
                            Vector2 ToTar = (vector - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
                            float dis = Vector2.Distance(vector, NPC.position);
                            if (dis < 50)
                                Timer2++;
                            else
                                NPC.velocity = (NPC.velocity * 10 + ToTar) / 11;
                        }
                        else if (Timer2 == 1)
                        {
                            Timer1++;
                            NPC.velocity = NPC.position - new Vector2(NPC.position.X - 30f, NPC.position.Y);
                            NPC.rotation = MathHelper.Pi / 1.2f - MathHelper.Pi / 4;
                            if (Timer1 >= 6 && Main.netMode != 1)
                            {
                                Projectile qi= Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center,
                                    (NPC.rotation - MathHelper.Pi / 4).ToRotationVector2() * 30, ModContent.ProjectileType<SwordQi1>(), 45, 2f, Main.myPlayer,0);
                                qi.timeLeft = 1000;
                                Timer1 = 0;
                                interval++;
                            }
                        }
                        if (interval >= 12 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            interval = 0;
                            Timer1 = 0;
                            Timer2 = 0;
                            ShootSword();
                            if (State2 == 0)
                            {
                                SwitchState1((int)HolySword.St2, (int)HolySword.St5 + 1);
                                if (Main.rand.Next(5) == 1) SwitchState1(11);
                            }
                            else
                            {
                                SwitchState1(4);
                            }
                        }
                        break;
                    }
                case HolySword.St5://斜角冲2
                    {
                        if (Timer2 == 0)
                        {
                            Vector2 vector = new Vector2(Target.position.X + 1000, Target.position.Y - 400);
                            Vector2 ToTar = (vector - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
                            float dis = Vector2.Distance(vector, NPC.position);
                            if(dis < 50)
                                Timer2++;
                            else
                                NPC.velocity = (NPC.velocity * 10 + ToTar) / 11;
                        }
                        else if(Timer2 == 1)
                        {
                            Timer1++;
                            NPC.velocity = NPC.position - new Vector2(NPC.position.X + 30f,NPC.position.Y);
                            NPC.rotation = MathHelper.Pi / 180 * 165;
                            if (Timer1 >= 6 && Main.netMode != 1)
                            {
                                var qi = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center,
                                    (NPC.rotation - MathHelper.Pi / 4).ToRotationVector2() * 30, ModContent.ProjectileType<SwordQi1>(),45, 2f, Main.myPlayer,0);
                                qi.timeLeft = 1000;
                                Timer1 = 0;
                                interval++;
                            }
                        }
                        if (interval >= 12 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            interval = 0;
                            Timer1 = 0;
                            Timer2 = 0;
                            ShootSword();
                            if (State2 == 0)
                            {
                                SwitchState1((int)HolySword.St2, (int)HolySword.St5 + 1);
                            }
                            else
                            {
                                SwitchState1((int)HolySword.St2, (int)HolySword.St7 + 1);
                            }
                            if (Main.rand.Next(5) == 1) SwitchState1(11);
                        }
                        break;
                    }
                case HolySword.St6://旋转发射近距离弹幕
                    {
                        Timer1++;
                        if(Timer1 < 5)
                        {
                            targetOldPos = Target.position;
                        }
                        else if(Timer1 > 5)
                        {
                            Timer2++;
                            NPC.velocity = CircularMotionForCenter(0.1f, Timer1, 250,targetOldPos);
                            NPC.velocity *= 5;//旋转
                            NPC.rotation += (NPC.position - targetOldPos).ToRotation() + MathHelper.Pi/4;//改变朝向
                            if (Timer2 == 8 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                interval++;
                                Timer2 = 0;
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center
                                    , (targetOldPos - NPC.Center).SafeNormalize(Vector2.UnitX) * 25, ModContent.ProjectileType<SwordQi1>(),50, 2.3f, Main.myPlayer,0);
                                NPC.netUpdate = true;
                            }//发射弹幕
                            else if (interval >= 8 && Main.netMode != NetmodeID.MultiplayerClient && Timer2 > 5)
                            {
                                interval = 0;
                                Timer1 = 0;
                                Timer2 = 0;
                                ShootSword();

                                SwitchState1((int)HolySword.St2, (int)HolySword.St7 + 1);
                            }
                        }
                        break;
                    }
                case HolySword.St7://散射
                    {
                        Timer1++;
                        float r = Timer1;
                        NPC.rotation += r.DegToRad();
                        NPC.velocity = NPC.rotation.ToRotationVector2();
                        if(Timer1 % 5 == 1)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center
                                    , (NPC.rotation - MathHelper.Pi / 4).ToRotationVector2() * 28, ModContent.ProjectileType<SwordQi1>(),50, 2.3f, Main.myPlayer, 0);
                            interval++;
                        }
                        if (interval >= 12 && Main.netMode != NetmodeID.MultiplayerClient && Timer1 % 5 == 2)
                        {
                            interval = 0;
                            Timer1 = 0;
                            Timer2 = 0;
                            ShootSword();

                            SwitchState1((int)HolySword.St2, (int)HolySword.St7 + 1);
                            if (Main.rand.Next(5) == 1) SwitchState1(11);
                        }
                        break;
                    }
                case HolySword.St8://尾杀
                    {
                        switch (State4)
                        {
                            case 0://旋转远离玩家
                                {
                                    Timer1++;
                                    float dis = (Timer1 * 2) + 150;//最低152的距离限制
                                    NPC.velocity = CircularMotion(0.5f, Timer1, dis, 0.15f);
                                    if(Timer1 > 100 && Timer1 % 7 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        if(Timer1 > 250)
                                        {
                                            ShootSword();
                                            Timer1 = 0;
                                            State4++;
                                            break;
                                        }
                                        var qi = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center,
                                            (Target.Center - NPC.Center).SafeNormalize(default) * 10f, ModContent.ProjectileType<SwordQi1>(), 45, 2f, Main.myPlayer, 0);
                                        qi.timeLeft = 1000;
                                    }
                                    break;
                                }
                            case 1://天下剑风
                                {
                                    NPC.velocity *= 0.9f;
                                    Timer1++;
                                    if(Timer1 > 60 && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        if(Timer2 > 10)
                                        {
                                            State4++;
                                            Timer1 = Timer2 = 0;
                                        }
                                        for(int i = -5; i<= 5;i++)
                                        {
                                            Vector2 center = Target.position;
                                            Vector2 vel;
                                            if(Timer2 % 2 == 0)//偶数情况
                                            {
                                                center += new Vector2(250 * i, -400);
                                                vel = Vector2.UnitY;
                                            }
                                            else//基数情况
                                            {
                                                center += new Vector2(100 - (250 * i), 400);
                                                vel = -Vector2.UnitY;
                                            }
                                            Projectile.NewProjectile(null, center,vel, ModContent.ProjectileType<SwordWind2>(),
                                                60, 1.3f, Main.myPlayer);
                                        }
                                        Timer2++;
                                        Timer1 = 0;
                                    }
                                    break;
                                }
                            case 2://结尾
                                {
                                    Timer1++;
                                    if(Timer1 >= 10)
                                    {
                                        CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y,
                                            NPC.width, NPC.height),Color.Gold, "你具有很强的潜力，你得到了我的认可", true, false);
                                        foreach(NPC npc in Main.npc)
                                        {
                                            if(npc.active && npc.type == ModContent.NPCType<HolySwordFS>())
                                            {
                                                npc.active = false;
                                            }
                                        }
                                        NPC.life = 0;
                                        NPC.checkDead();
                                        foreach (NPC n in Main.npc)
                                        {
                                            if (n.type == ModContent.NPCType<HolySwordFS>() && n.active)
                                            {
                                                n.active = false;
                                                n.netUpdate = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        NPC.position = new Vector2(Target.position.X, Target.position.Y - 90);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        Timer1++;
                        NPC.velocity = (NPC.velocity * 10 + ToTarget.SafeNormalize(default) * 5)/11;
                        if(Timer1 > 200)
                        {
                            Timer1 = 0;
                            if (State2 == 0)
                            {
                                SwitchState1((int)HolySword.St2, (int)HolySword.St5 + 1);
                            }
                            else
                            {
                                SwitchState1((int)HolySword.St2, (int)HolySword.St7 + 1);
                            }
                        }
                        break;
                    }
            }
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if(State1 > (int)HolySword.St7)
            {
                damage *= 2;
            }
        }
        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (State1 > (int)HolySword.St7)
            {
                damage *= 2;
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (State1 > (int)HolySword.St7)
            {
                damage *= 2;
            }
        }
        public override void OnKill()
        {
            BloodSoulSystem.downedHolyLightSword = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color Tail;//拖尾颜色
            switch ((HolySword)State1)
            {
                case HolySword.St1:
                    //开幕的Draw
                    {

                        Texture2D texture = (Texture2D)BloodSoulUtils.GetTexture("Images/Light");
                        Vector2 drawOrigin;
                        drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height);
                        Main.spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, new Color(255, 255, 0, 0) * alpha, 0, drawOrigin, new Vector2(2, 3), SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, new Color(255, 255, 0, 0) * alpha, (float)Math.PI / 2, drawOrigin, new Vector2(2, 3), SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, new Color(255, 255, 0, 0) * alpha, -(float)Math.PI / 2, drawOrigin, new Vector2(2, 3), SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, new Color(255, 255, 0, 0) * alpha, (float)Math.PI, drawOrigin, new Vector2(2, 3), SpriteEffects.None, 0);

                        Texture2D texture2 = (Texture2D)BloodSoulUtils.GetTexture("Images/Ray");
                        Vector2 drawOrigin2;
                        drawOrigin2 = new Vector2(texture2.Width * 0.5f, texture2.Height * 0.5f);
                        Main.spriteBatch.Draw(texture2, NPC.Center - Main.screenPosition, null, new Color(255, 255, 0, 0) * alpha, Main.GlobalTimeWrappedHourly, drawOrigin2, new Vector2(1.7f, 1.7f), SpriteEffects.None, 0);

                        Texture2D texture3 = (Texture2D)BloodSoulUtils.GetTexture("Images/Start");
                        Vector2 drawOrigin3;
                        drawOrigin3 = new Vector2(texture3.Width * 0.5f, texture3.Height * 0.5f);
                        Main.spriteBatch.Draw(texture3, NPC.Center - Main.screenPosition, null, new Color(255, 255, 0, 0) * alpha, -Main.GlobalTimeWrappedHourly, drawOrigin3, new Vector2(0.6f, 0.6f), SpriteEffects.None, 0);
                        break;
                    }

                case HolySword.St2:
                    //绘制传送门
                    {
                        if (Timer1 > 1 && Timer1 <= 30)
                        {
                            Vector2 center = new Vector2(Timer2, Timer3) - Main.screenPosition;
                            Main.spriteBatch.Draw(BloodSoulUtils.GetTexture("NPCs/Bosses/HolyLightSwords/Portal").Value,
                                center,
                                null,
                                Color.Yellow,
                                Main.GlobalTimeWrappedHourly,
                                Vector2.Zero,
                                1.5f,
                                SpriteEffects.None,
                                0
                                );//绘制传送门

                        }
                        break;
                    }
                case HolySword.St3:
                    //绘制箭头
                    {
                        if(Timer1 > 30 && Timer1 < 80 && interval < 5)
                        {
                            Vector2 center = new(Timer2, Timer3);
                            center = Target.Center + (center - Target.position).SafeNormalize(default) * 100;//获取绘制箭头的方位置
                            float rot = (center - new Vector2(Timer2, Timer3)).ToRotation() - MathHelper.PiOver2;//获取绘制箭头的朝向
                            center -= Main.screenPosition;
                            Main.spriteBatch.Draw(BloodSoulUtils.GetTexture("NPCs/Bosses/HolyLightSwords/MouseHead").Value,
                                center,
                                null,
                                Color.Yellow,
                                rot,
                                Vector2.Zero,
                                1.5f,
                                SpriteEffects.None,
                                0
                                );//绘制箭头
                        }
                        break;
                    }
            }
            if (CanDraw)
            {
                Dust dust = Dust.NewDustDirect(NPC.Center, 5, 5, DustID.GoldCoin);
                dust.noGravity = true;
                dust.velocity *= 0;
                Tail = Color.Gold;
            }
            else
            {
                Tail = Color.White;
            }//改变拖尾颜色

            BloodSoulWay.NewNpcDrawTail(NPC, Tail, Tail);
            return false;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax + 0.5f * bossLifeScale);
        }
        private void ShootSword()
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<SwordQi1>() && projectile.active && projectile.ai[0] != 2)
                {
                    projectile.ai[0] = 2;
                    projectile.timeLeft = 150;
                }
            }
        }
        #region 旧大招
        /*if(interval < 10)
                            {
                                Time1++;
                                NPC.rotation = ToTarget.ToRotation() + MathHelper.Pi / 4;
                                NPC.velocity *= 0.5f;
                                if(Time1 == 30)
                                {
                                    for(float r = 0;r<MathHelper.TwoPi;r+= MathHelper.TwoPi / 36)
                                    {
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center
                                        , r.ToRotationVector2() * 10, ModContent.ProjectileType<SwordQi1>(), 85, 2.3f, Main.myPlayer, 0);
                                    }
                                    interval++;
                                }
                                else if(Time1 == 60)
                                {
                                    for (float r = MathHelper.TwoPi / 36;
                                        r <= MathHelper.TwoPi + MathHelper.TwoPi / 36;
                                        r += MathHelper.TwoPi / 36)
                                    {
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center
                                        , r.ToRotationVector2() * 15, ModContent.ProjectileType<SwordQi1>(), 82, 2.3f, Main.myPlayer, 0);
                                    }
                                    interval++;
                                }
                                else if(Time1 >= 70)
                                {
                                    foreach (Projectile projectile in Main.projectile)
                                    {
                                        if (projectile.type == ModContent.ProjectileType<SwordQi1>())
                                        {
                                            projectile.ai[0] = 2;
                                        }
                                    }
                                    Time1 = 0;
                                }
                            }
                            else if(interval == 10)
                            {
                                NPC.velocity *= 0;
                                NPC.rotation = NPC.rotation = NPC.velocity.ToRotation() - MathHelper.Pi / 4;
                                Time2++;
                                if (Time2 == 20)
                                {
                                    CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height),
                                        Color.Gold, "这是最后的考验，接招吧！你这个企图想要征服我的人！", true, false);
                                    foreach (Projectile projectile in Main.projectile)
                                    {
                                        if (projectile.type == ModContent.ProjectileType<SwordQi1>())
                                        {
                                            projectile.ai[0] = 2;
                                        }
                                    }
                                }
                                else if(Time2 == 120)
                                {
                                    Time1 = 0;
                                    Time2 = 0;
                                    interval++;
                                }
                            }
                            else if(interval > 10 && interval <= 20)
                            {
                                Time1++;
                                if(Time1 % 30 == 0)
                                {
                                    interval++;
                                    NPC.velocity = (ToTarget * Time1 / 60)+ target.velocity;
                                }
                            }
                            else if(interval > 20)
                            {
                                NPC.position = new Vector2(target.position.X, target.position.Y - 200);
                                CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height),
                                        Color.Gold, "你的力量是存在无限可能的，你得到了我的认可", true, false);
                                NPC.life = 0;
                                NPC.checkDead();
                            }*/
        #endregion
    }
}
