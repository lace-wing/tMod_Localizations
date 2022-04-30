using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using BloodSoul.MyUtils;
using BloodSoul.Projectiles.Bosses;
using System.IO;
using BloodSoul.NPCs.Bosses.HolyLightSwords;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Audio;
using BloodSoul.Items.BossBag;
using Terraria.GameContent.ItemDropRules;
using BloodSoul.Items.Weapons.Melee.BloodDevilEyeTooth;
using BloodSoul.Items.Weapons.Ranged.Bow;
using BloodSoul.Items.Weapons.Magic;

namespace BloodSoul.NPCs.Bosses.BloodCrystalEyes
{
    [AutoloadBossHead]
    public class BloodCrystalEye : FSMnpc
    {
        private float colorop = 0;
        //private int frameTime = 0;
        private int EatNPCcd = 0;
        private enum BloodEyeAI
        {
            Show,//开幕ai
            ShootBomb,//血弹散射
            Sprint,//冲刺
            Sprint2,//预判冲刺
            SpermEjection,//血精喷射
            BloodPressureJet,//血牙喷射（血压喷射）
            EjectionSpread,//射血扩散
            SummonLittleBloodEye,//召唤小弟
            PowerOfGorefiend,//血魔之力
            StrongestKill//最强杀戮（
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Crystal Eye");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "血晶魔眼");
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 10500;
            NPC.damage = 145 / 3;
            NPC.defense = 32;
            NPC.friendly = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 260;
            NPC.height = 260;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 16, 66, 66);
            NPC.lavaImmune = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath11;
            DrawOffsetY = 20;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BloodCrystalEye");
            }
            BossBag = ModContent.ItemType<BloodEyeBossBag>();
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                base.SendExtraAI(writer);
                writer.Write(colorop);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                base.ReceiveExtraAI(reader);
                colorop = reader.ReadSingle();
            }
        }
        public override void AI()
        {
            if (NPC.target <= 0||NPC.target == 255||!Target.active|| Target.dead)//寻找npc的目标
            {
                NPC.TargetClosest();
            }
            if (colorop < 1) colorop += 0.1f / 6;
            if (Main.dayTime) Main.dayTime = false;//强制修改到黑夜
            Vector2 ToTarget = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);//获取npc到目标的向量
            NPC.rotation = ToTarget.ToRotation();//默认选择为对着玩家
            if (!Filters.Scene["ScreenShader"].Active)//开启shader
            {
                Filters.Scene.Activate("ScreenShader");
            }
            #region 切换状态
            if (NPC.life < NPC.lifeMax * 0.7f && State4 == 0)
            {
                State1 = (int)BloodEyeAI.PowerOfGorefiend;
                Timer1 = Timer2 = Timer3 = 0;
                NPC.alpha = 0;
                State4 = 1;
                CombatText.NewText(NPC.Hitbox, Color.Purple, "能活到现在就是你的运气！",true);
            }
            else if (NPC.life < NPC.lifeMax * 0.3f && State4 == 1)
            {
                State1 = (int)BloodEyeAI.PowerOfGorefiend;
                Timer1 = Timer2 = Timer3 = 0;
                NPC.alpha = 0;
                State4 = 2;
                CombatText.NewText(NPC.Hitbox, Color.Purple, "我要让你看看什么是真正的实力！", true);
            }
            else if (Target.dead)
            {
                Timer4++;
                if (Timer4 == 20)
                {
                    NPC.velocity *= 0;
                    CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height)
                        , Color.Purple, "哈哈哈哈哈哈哈，就这！", true, false);
                }
                else if (Timer4 > 25)
                {
                    NPC.life = 0;
                }
                return;
            }
            #endregion  
            #region 撕咬npc
            if (EatNPCcd > 0) EatNPCcd--;
            else
            {
                foreach(NPC npc in Main.npc)
                {
                    if((!npc.boss || npc.type == NPCID.EyeofCthulhu) && npc.active && 
                        npc.Hitbox.Intersects(NPC.Hitbox))
                    {
                        NPC.life += npc.life;
                        CombatText.NewText(npc.Hitbox, Color.Green,npc.life);
                        npc.active = false;
                        EatNPCcd += 240;
                        if(EatNPCcd > 420)//吃了3个npc以上
                        {
                            break;
                        }
                    }
                }
            }
            if(NPC.life > NPC.lifeMax)
            {
                NPC.lifeMax = NPC.life;
            }
            #endregion
            #region 状态机
            switch ((BloodEyeAI)State1)
            {
                case BloodEyeAI.Show://开幕
                    {
                        Timer1++;
                        if (!Main.dedServ)
                        {
                            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Headt");
                        }
                        NPC.dontTakeDamage = true;
                        Color textColor = Color.Purple;
                        switch (Timer1)
                        {
                            case 50:
                                {
                                    CombatText.NewText(NPC.Hitbox, textColor, "圣光剑...", true, false);
                                    break;
                                }
                            case 150:
                                {
                                    CombatText.NewText(NPC.Hitbox, textColor, "我要看看你新主的力量", true, false);
                                    break;
                                }
                            case 250:
                                {
                                    CombatText.NewText(NPC.Hitbox, textColor, "你真的以为你能打过我？", true, false);
                                    break;
                                }
                            case 350:
                                {
                                    CombatText.NewText(NPC.Hitbox, textColor, "你的下场也是它的前主人一样，被我撕碎！哈哈哈哈哈！", true, false);
                                    break;
                                }
                            default:
                                {
                                    if (Timer1 > 450)
                                    {
                                        Timer1 = 0;
                                        if (!Main.dedServ)
                                        {
                                            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BloodCrystalEye");
                                        }
                                        NPC.dontTakeDamage = false;
                                        SwitchState1((int)BloodEyeAI.Sprint2);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case BloodEyeAI.ShootBomb://血弹散射
                    {
                        float X = NPC.Center.X - Target.Center.X;
                        int Cen = (X > 0) ? 1 : -1;
                        switch (State2)
                        {
                            case 0://对应位置
                                {
                                    Vector2 Head = new Vector2(Target.Center.X + (500 * Cen), Target.Center.Y);
                                    float ToHead = Vector2.Distance(NPC.Center, Head);
                                    NPC.velocity = (NPC.velocity * 10 + ((Head - NPC.Center)
                                        .SafeNormalize(Vector2.UnitX) * (ToHead < 300 ? 15 : (float)Math.Sqrt(ToHead)))) / 11;
                                    if (ToHead < 10)
                                    {
                                        Timer1++;
                                        if (Timer1 > 10)
                                        {
                                            Timer1 = 0;
                                            SwitchState2(1);
                                        }
                                    }
                                    break;
                                }
                            case 1://发射弹幕
                                {
                                    Timer1++;
                                    if(Timer1 > 10)
                                    {
                                        for (int i = 0; i < Main.rand.Next(15,21); i++)
                                        {
                                            Vector2 vector1 = ToTarget * 10 + new Vector2(Main.rand.NextFloatDirection() * 2, Main.rand.NextFloatDirection() * 2);
                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                            {
                                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, vector1,
                                                    ModContent.ProjectileType<BloodBomb>(),46, 1.2f, Main.myPlayer); ;
                                            }
                                            Timer2++;
                                        }
                                    }
                                    if(Timer2 > 15)
                                    {
                                        Timer1 = 0;
                                        Timer2 = 0;
                                        SwitchState2(0);
                                        BloodEyeSwichState(BloodEyeAI.ShootBomb);
                                    }
                                    break;
                                }
                        }
                                break;
                    }
                case BloodEyeAI.Sprint://冲刺
                    {
                        switch(State2)
                        {
                            case 0://飞行到玩家头顶
                                {
                                    Vector2 TargetHead = Target.Center + new Vector2(0, -400);
                                    NPC.velocity = (NPC.velocity * 5 + (TargetHead - NPC.position).SafeNormalize(default) * 15)/ 6;//改变速度 //6
                                    if (NPC.Distance(TargetHead) < NPC.width)//小于宽度
                                    {
                                        Timer1++;
                                        if (Timer1 > 10)
                                        {
                                            Timer1 = 0;
                                            NPC.velocity *= 0.3f;//使其减速
                                            SwitchState2(1);//切换ai
                                        }
                                    }
                                    break;
                                }
                            case 1://冲刺
                                {
                                    Timer3++;
                                    if(Timer3 > 4)
                                    {
                                        Timer2 = Timer1 = Timer3 = 0;
                                        SwitchState2(0);
                                        BloodEyeSwichState(BloodEyeAI.Sprint);
                                        break;
                                    }
                                    NPC.velocity = ToTarget * 20;
                                    NPC.rotation = NPC.velocity.ToRotation();
                                    SoundEngine.PlaySound(SoundID.ForceRoar, (int)NPC.position.X, (int)NPC.position.Y, 0, 2f, -0.35f);
                                    SwitchState2(2);
                                    break;
                                }
                            case 2://等待同时转移位置
                                {
                                    Timer1++;
                                    NPC.rotation = NPC.velocity.ToRotation();
                                    if (Timer1 > 40)
                                    {
                                        if (NPC.alpha > 255)
                                        {
                                            NPC.alpha = 255;
                                        }//使npc逐渐隐身
                                        else if(NPC.alpha == 255)
                                        {
                                            NPC.damage = 0;
                                            Timer2++;
                                            if(Timer2 > 20)//切换ai
                                            {
                                                Timer1 = 0;
                                                Timer2 = 0;
                                                NPC.alpha = 0;
                                                NPC.damage = NPC.defDamage;
                                                SwitchState2(1);
                                            }
                                            else//移动
                                            {
                                                if(NPC.Distance(Target.position) > 600)
                                                {
                                                    Timer2--;
                                                    NPC.velocity = (NPC.velocity * 5 + (Target.position - NPC.position) * 0.3f) / 6;
                                                }
                                                else
                                                {
                                                    NPC.velocity *= 0.3f;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            NPC.alpha += 20;
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case BloodEyeAI.Sprint2://预判冲刺
                    {
                        switch(State2)
                        {
                            case 0://到达头顶
                                {
                                    if (NPC.alpha > 255)
                                    {
                                        NPC.alpha = 255;
                                    }//使npc逐渐隐身
                                    else if(NPC.alpha < 255)
                                    {
                                        NPC.alpha += 20;
                                    }
                                    NPC.damage = 0;
                                    Vector2 TargetHead = Target.Center + new Vector2(0, -400);
                                    NPC.velocity = (NPC.velocity * 5 + (TargetHead - NPC.position).SafeNormalize(default) * 15) / 6;//改变速度
                                    if (NPC.Distance(TargetHead) < NPC.width)//小于宽度
                                    {
                                        Timer1++;
                                        if (Timer1 > 10)
                                        {
                                            NPC.damage = NPC.defDamage;
                                            Timer1 = 0;
                                            NPC.alpha = 0;//显身
                                            NPC.velocity *= 0.3f;//使其减速
                                            SwitchState2(1);//切换ai
                                        }
                                    }
                                    break;
                                }
                            case 1://预判冲刺
                                {
                                    Timer1++;
                                    Vector2 ToPlayer = (ToTarget * 30) + Target.velocity;
                                    NPC.rotation = ToPlayer.ToRotation();
                                    if(Timer1 > 45)
                                    {
                                        NPC.velocity = ToPlayer;
                                        Timer1 = 0;
                                        SoundEngine.PlaySound(SoundID.ForceRoar, (int)NPC.position.X, (int)NPC.position.Y, 0, 2f, -0.45f);
                                        SwitchState2(2);
                                    }
                                    break;
                                }
                            case 2://保持速度
                                {
                                    Timer1++;
                                    NPC.velocity *= 1f;
                                    NPC.rotation = NPC.velocity.ToRotation();
                                    if(Timer1 > 60)
                                    {
                                        Timer1 = 0;
                                        SwitchState2(0);
                                        BloodEyeSwichState(BloodEyeAI.Sprint2);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case BloodEyeAI.SpermEjection://血牙散射
                    {
                        Timer1++;
                        NPC.velocity = (NPC.velocity * 10 + ToTarget * 8) / 11;
                        if (Timer1 == 5)
                        {
                            SoundEngine.PlaySound(SoundID.ForceRoar, (int)NPC.position.X, (int)NPC.position.Y,0,2f,-0.35f);
                        }
                        if (Timer1 % 90 == 0 && Timer1 != 0)
                        {
                            for (int i = -1; i <= 1; i++)
                            {
                                for (int j = -2; j <= 2; j++)
                                {
                                    Vector2 r = (ToTarget.ToRotation() + (j * MathHelper.Pi / 6)).ToRotationVector2() * 15;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(),NPC.Center,r,
                                            ModContent.ProjectileType<BloodTooth>(),53,2.3f, Main.myPlayer,i);
                                    }
                                }
                            }
                        }
                        if(Timer1 > 520)
                        {
                            Timer1 = 0;
                            BloodEyeSwichState(BloodEyeAI.SpermEjection);
                        }
                        break;
                    }
                case BloodEyeAI.BloodPressureJet://血晶散射
                    {
                        Timer1++;
                        NPC.velocity = (NPC.velocity * 10 + ToTarget) / 11;
                        if (Timer1 > 420)
                        {
                            Timer1 = Timer2 = 0;
                            BloodEyeSwichState(BloodEyeAI.BloodPressureJet);
                        }
                        else if (Timer1 % 15 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Timer2++;
                            if (Timer2 > 3) Timer2 = 0;
                            for (int i = (int)(-2 - Timer2); i <= 2 + Timer2; i++)
                            {
                                Vector2 vel = ToTarget.RotatedBy(0.1f / Timer2 * i) * 15;
                                Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, vel,
                                    ModContent.ProjectileType<BloodCrystal>(), 30, 1.2f, Main.myPlayer);

                            }
                        }
                        break;
                    }
                case BloodEyeAI.EjectionSpread://射血扩散
                    {
                        if (Timer1 <= 0)
                        {
                            Timer1 = 300;
                            Timer2 = 0;
                            State2++;
                        }
                        else Timer1--;
                        switch(State2)
                        {
                            case 1:
                                {
                                    NPC.velocity = (NPC.velocity * 5 + ToTarget) / 6;
                                    NPC.rotation = NPC.velocity.ToRotation();
                                    float alp;
                                    if (Timer1 > 255) alp = 255;
                                    else alp = Timer1;
                                    for(int i = -1;i<=1;i++)
                                    {
                                        float r = NPC.rotation + i * MathHelper.PiOver4;
                                        Vector2 ves = r.ToRotationVector2();
                                        if(Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ves,
                                                ModContent.ProjectileType<BloodLine>(), 84, 2f, Main.myPlayer, alp);
                                        }
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    NPC.rotation = NPC.velocity.ToRotation();
                                    if (Timer1 < 250)
                                    {
                                        if (Timer2 < 30) Timer2++;
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            for (int i = -1; i <= 1; i++)
                                            {
                                                float r = NPC.rotation + (i * (MathHelper.PiOver4 - Timer2.DegToRad()));
                                                Vector2 ves = r.ToRotationVector2();
                                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                                {
                                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ves,
                                                        ModContent.ProjectileType<BloodLine>(),84, 2f, Main.myPlayer);
                                                }
                                            }
                                        }

                                    }
                                    break;
                                }
                            case 3:
                                {
                                    Timer2 = 0;
                                    Timer1 = 0;
                                    SwitchState2(0);
                                    BloodEyeSwichState(BloodEyeAI.EjectionSpread);
                                    break;
                                }
                        }
                        break;
                    }
                case BloodEyeAI.SummonLittleBloodEye://召唤小血眼
                    {
                        Timer1++;
                        NPC.velocity = ToTarget * 8;
                        Vector2[] center = new Vector2[2]
                        {
                            (NPC.rotation - MathHelper.PiOver2).ToRotationVector2() * 30,
                            (NPC.rotation + MathHelper.PiOver2).ToRotationVector2() * 30,
                        };
                        if(Timer1 > 50)
                        {
                            Timer1 = 0;
                            for(int i = 0;i<6;i++)
                            {
                                int j = (i<3)? 0:1;
                                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)center[j].X, (int)center[j].Y, 
                                    ModContent.NPCType<BloodEye>());
                            }
                            SoundEngine.PlaySound(SoundID.NPCDeath10, NPC.position);
                            BloodEyeSwichState(BloodEyeAI.SummonLittleBloodEye);
                        }
                        break;
                    }
                case BloodEyeAI.PowerOfGorefiend://最终血魔的吼叫（
                    {
                        Timer1++;
                        if(State4 == 1)
                        {
                            NPC.damage = NPC.defDamage * 2;
                        }
                        switch(Timer1)
                        {
                            case <= 60://吼叫1次,记录位置
                                {
                                    if (Timer1 == 60)
                                    {
                                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("BloodSoul/Sounds/Custom/BloodEyeRun"),
                                            NPC.position);
                                    }
                                    Timer2 = Target.position.X;
                                    Timer3 = Target.position.Y;//记录位置
                                    break;
                                }
                            case 160://第二次冲刺
                                {
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("BloodSoul/Sounds/Custom/BloodEyeRun"),
                                          NPC.position);
                                    NPC.velocity = (new Vector2(Timer2,Timer3) - NPC.Center).RealSafeNormalize() * 30;
                                    break;
                                }
                            default:
                                {
                                    if(Timer1 < 60)
                                    {
                                        NPC.velocity *= 0.1f;
                                    }
                                    else if(Timer1 > 90)
                                    {
                                        Target.velocity *= 0f;//让玩家停止
                                    }
                                    if(Timer1 > 160)
                                    {
                                        if(Timer1 > 200)
                                        {
                                            Timer1 = Timer2 = Timer3 = 0;
                                            NPC.damage = NPC.defDamage;
                                            BloodEyeSwichState(BloodEyeAI.PowerOfGorefiend);
                                            break;
                                        }
                                        NPC.rotation = NPC.velocity.ToRotation();
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case BloodEyeAI.StrongestKill://尾杀
                    {
                        Timer1++;
                        #region 限制圈
                        float Dis = Vector2.Distance(NPC.Center, Target.Center);
                        for(float r =0;r<MathHelper.TwoPi;r+= MathHelper.Pi/36)
                        {
                            var dust = Dust.NewDustDirect(NPC.Center + (r.ToRotationVector2() * 1000),
                                1, 1, MyDustId.RedBlood);
                            dust.noGravity = true;
                            dust.noLight = false;
                            dust.velocity *= 0;
                            dust.scale = 1.1f;
                        }
                        if(Dis > 1000)
                        {
                            Target.velocity /= 5;
                            Target.velocity += Vector2.Normalize(NPC.Center - Target.Center) * 10;
                        }
                        if(Dis > 1010)
                        {
                            Target.position = NPC.position + ToTarget * 1000;
                        }
                        #endregion
                        #region 血晶喷射阶段
                        if (Timer1 < 300)
                        {
                            NPC.alpha = 0;
                            if(Timer1 == 10)
                            {
                                CombatText.NewText(NPC.Hitbox, Color.Purple, "就算是死也不会让你好过！", true);
                            }
                            Timer1++;
                            NPC.velocity = (NPC.velocity * 10 + ToTarget) / 11;
                            if (Timer1 % 30 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Timer2++;
                                if (Timer2 > 10) Timer2 = 0;
                                for (int i = (int)(-2 - Timer2 * 3); i <= 2 + Timer2 * 3; i++)
                                {
                                    Vector2 vel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi / (Timer2 * 3) * i) * 10;
                                    Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, vel,
                                        ModContent.ProjectileType<BloodCrystal>(), 30, 1.2f, Main.myPlayer);

                                }
                            }
                        }
                        #endregion
                        #region 摸鱼
                        else if (Timer1 >= 300 && Timer1 < 600)
                        {
                            Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 500);
                            NPC.velocity = (NPC.velocity * 20 + (Head - NPC.Center).SafeNormalize(Vector2.UnitX) * 10) / 21;
                            if (Timer1 % 15 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, NPC.rotation.ToRotationVector2() * 10,
                                    ModContent.ProjectileType<BloodBomb>(), 165 / 3, 1.2f, Main.myPlayer);
                            }
                        }
                        #endregion
                        #region 召唤小弟
                        else if (Timer1 >= 600 && Timer1 < 650)
                        {
                            NPC.velocity = ToTarget * 8;
                            if (Timer1 % 20 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2[] center = new Vector2[2]
                                {
                                    NPC.Center + (NPC.rotation - MathHelper.PiOver2).ToRotationVector2() * 30,
                                    NPC.Center + (NPC.rotation + MathHelper.PiOver2).ToRotationVector2() * 30,
                                };
                                for (int i = 0; i < 6; i++)
                                {
                                    int j = (i < 3) ? 0 : 1;
                                    NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)center[j].X, (int)center[j].Y,
                                        ModContent.NPCType<BloodEye>());
                                }
                            }
                        }
                        #endregion
                        #region 冲刺
                        else if (Timer1 >= 650 && Timer1 < 1000)
                        {
                            switch (State2)
                            {
                                case 0://冲刺
                                    {
                                        NPC.velocity = ToTarget * 20;
                                        NPC.rotation = NPC.velocity.ToRotation();
                                        SoundEngine.PlaySound(SoundID.ForceRoar, (int)NPC.position.X, (int)NPC.position.Y, 0, 2f, -0.35f);
                                        SwitchState2(1);
                                        break;
                                    }
                                case 1://冲刺
                                    {
                                        Timer2++;
                                        NPC.rotation = NPC.velocity.ToRotation();
                                        if (Timer2 > 30)
                                        {
                                            if (NPC.alpha > 255)
                                            {
                                                NPC.alpha = 255;
                                            }//使npc逐渐隐身
                                            else if (NPC.alpha == 255)
                                            {
                                                NPC.damage = 0;
                                                Timer3++;
                                                if (Timer3 > 40)//恢复CD
                                                {
                                                    Timer2 = 0;
                                                    NPC.alpha = 0;
                                                    NPC.damage = NPC.defDamage;
                                                    SwitchState2(0);
                                                }
                                                else//移动
                                                {
                                                    if (NPC.Distance(Target.position) > 1000)
                                                    {
                                                        Timer3--;
                                                        NPC.velocity = (NPC.velocity * 5 + (Target.position - NPC.position) * 0.3f) / 6;
                                                    }
                                                    else
                                                    {
                                                        NPC.velocity *= 0.3f;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                NPC.alpha += 20;
                                            }
                                        }
                                        break;
                                    }
                            }
                        }
                        #endregion
                        #region 最后的死亡
                        else if (Timer1 >= 1000)
                        {
                            Timer2++;
                            NPC.velocity *= 0.9f;
                            NPC.rotation = NPC.velocity.ToRotation() + Timer2.DegToRad() * 15;
                            Color textColor = Color.Purple;
                            switch (Timer2)
                            {
                                case 100:
                                    {
                                        //#region 纯属娱乐
                                        //Time1 = 0;
                                        //Time2 = 0;
                                        //#endregion
                                        CombatText.NewText(NPC.Hitbox, textColor, "不！这不可能！", true);
                                        break;
                                    }
                                case 200:
                                    {
                                        CombatText.NewText(NPC.Hitbox, textColor, "我怎么可能会被你击败！", true);
                                        break;
                                    }
                                case 300:
                                    {
                                        CombatText.NewText(NPC.Hitbox, textColor, "不！！！", true);
                                        NPC.life = 0;
                                        NPC.checkDead();
                                        break;
                                    }
                            }
                        }
                        #endregion
                        break;
                    }
                default:
                    {
                        Timer1++;
                        Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 500);
                        NPC.velocity = (NPC.velocity * 20 + (Head - NPC.Center).SafeNormalize(Vector2.UnitX) * 15) / 21;
                        if (Timer1 % 15 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, NPC.rotation.ToRotationVector2() * 17,
                                ModContent.ProjectileType<BloodBomb>(), 180 /3, 1.2f, Main.myPlayer);
                        }
                        if (Timer1 > 300)
                        {
                            Timer1 = 0;
                            BloodEyeSwichState(BloodEyeAI.Show);
                        }
                        break;
                    }
            }
            #endregion
        }
        public override bool CheckActive()
        {
            return Target.dead;
        }
        public override bool CheckDead()
        {
            if(State1 != (int)BloodEyeAI.StrongestKill)
            {
                SoundEngine.PlaySound(SoundID.ForceRoar, (int)NPC.position.X, (int)NPC.position.Y, 0, 2f, -0.35f);
                Timer1 = 0;
                Timer2 = 0;
                SwitchState2(0);
                NPC.life = 4000;
                NPC.dontTakeDamage = true;
                SwitchState1((int)BloodEyeAI.StrongestKill);
                return false;
            }
            return true;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5;
            NPC.frameCounter += NPC.velocity.Length() * 0.001f;
            if (NPC.frameCounter > 10)
            {
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
        public override void OnKill()
        {
            BloodSoulSystem.downedBloodCrystalEye = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color color = Color.White;
            int frame = (int)(Main.GlobalTimeWrappedHourly * 10) % 13;
            #region 黑洞
            if (State1 == (int)BloodEyeAI.Show)
            {
                Texture2D Portal = (Texture2D)BloodSoulUtils.GetTexture("NPCs/Bosses/BloodCrystalEyes/Portal");
                Main.spriteBatch.Draw(Portal, NPC.Center - Main.screenPosition, null, new Color(255, 0, 0, 0),
                    Main.GlobalTimeWrappedHourly, Portal.Size() * 0.5f, 2f, SpriteEffects.None, 0);
            }
            #endregion
            #region 预判线
            if (State1 == 3 && State2 == 1)
            {
                color = Color.DarkRed;
                Vector2 vector = NPC.Center - Main.screenPosition;
                Texture2D value2 = (Texture2D)BloodSoulUtils.GetTexture("Images/Extra_178");
                Vector2 origin = value2.Frame(1, 1, 0, 0).Size() * new Vector2(0f, 0.5f);
                Main.spriteBatch.Draw(value2, vector, null, new Color(255, 0, 0, 100), NPC.rotation, origin, new Vector2(100, 0.5f), SpriteEffects.None, 0f);
            }
            #endregion
            #region 残影
            Texture2D NPCTexture = TextureAssets.Npc[NPC.type].Value;
            SpriteEffects spriteEffects = 0;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            int frameCount = Main.npcFrameCount[NPC.type];
            Vector2 DrawOrigin;
            DrawOrigin = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / frameCount / 2));

            for (int i = 0; i < NPC.oldPos.Length; i += 1)
            {
                Color Taiolcolor = Color.Lerp(drawColor, color, 0.5f);
                Taiolcolor = NPC.GetAlpha(Taiolcolor);
                Taiolcolor *= (float)(NPC.oldPos.Length - i) / NPC.oldPos.Length;
                Vector2 DrawPosition = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - Main.screenPosition;
                DrawPosition -= new Vector2((float)NPCTexture.Width, (float)(NPCTexture.Height / frameCount)) * NPC.scale / 2f;
                DrawPosition += DrawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY - DrawOffsetY * 2.5f);
                Main.spriteBatch.Draw(NPCTexture, DrawPosition, new Rectangle?(NPC.frame), Taiolcolor, NPC.rotation, DrawOrigin, NPC.scale, spriteEffects, 0f);
            }
            #endregion
            #region 四角
            Texture2D Cs = (Texture2D)BloodSoulUtils.GetTexture("Images/触手");//13
            Texture2D Cs2 = (Texture2D)BloodSoulUtils.GetTexture("Images/触手2");//13
            //zuoxia
            Main.spriteBatch.Draw(Cs2, new Vector2(160, Main.screenHeight), new Rectangle?(new Rectangle(0, 160 * frame, 160, 160)), Color.White * colorop, 0, new Vector2(80, 0), new Vector2(-2, -2), SpriteEffects.None, 0f);
            //youshang
            Main.spriteBatch.Draw(Cs2, new Vector2(Main.screenWidth, 160), new Rectangle?(new Rectangle(0, 160 * frame, 160, 160)), Color.White * colorop, 0, new Vector2(160, 80), new Vector2(2, 2), SpriteEffects.None, 0f);
            //左上
            Main.spriteBatch.Draw(Cs, new Vector2(160, 160), new Rectangle?(new Rectangle(0, 160 * frame, 160, 160)), Color.White * colorop, 0, new Vector2(80, 80), new Vector2(2, 2), SpriteEffects.None, 0f);
            //右下
            Main.spriteBatch.Draw(Cs, new Vector2(Main.screenWidth, Main.screenHeight), new Rectangle?(new Rectangle(0, 160 * frame, 160, 160)), Color.White * colorop, 0, new Vector2(0, 0), new Vector2(-2, -2), SpriteEffects.None, 0f);
            #endregion
            return true;
        }
        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if(damage > 50 + NPC.defense /2)
            {
                damage = 50 + NPC.defense / 2;
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (damage > 50 + NPC.defense / 2)
            {
                damage = 50 + NPC.defense / 2;
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (damage != 0)
            {
                NPC.life += 300;
                CombatText.NewText(NPC.Hitbox, Color.LightGreen, 300, false, false);
            }
        }
        public override void BossHeadSlot(ref int index)
        {
            if(NPC.alpha > 200)
            {
                index = -1;
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => NPC.alpha > 200 ? false : null;
        private void BloodEyeSwichState(BloodEyeAI EyeAI)
        {
            int ai = (int)EyeAI;
            int rand;
            if (State4 == 0) rand = Main.rand.Next(1, 4);
            else if (State4 == 1) rand = Main.rand.Next(1, (int)BloodEyeAI.BloodPressureJet + 1);
            else rand = Main.rand.Next(1, (int)BloodEyeAI.PowerOfGorefiend);
            if (rand == ai && rand != 1) SwitchState1(rand - 1);
            else if (rand == ai && rand == 1) SwitchState1(rand + 1);
            else SwitchState1(rand);
            if (Main.rand.Next(6) < 2) SwitchState1(10);
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = (Texture2D)BloodSoulUtils.GetTexture("NPCs/Bosses/BloodCrystalEyes/BloodCrystalEye_Glow");
            SpriteEffects spriteEffects = 0;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            int frameCount = Main.npcFrameCount[NPC.type];
            Vector2 DrawOrigin;
            DrawOrigin = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / frameCount / 2));
            Color Taiolcolor = Color.Lerp(drawColor, Color.White, 0.5f);
            Taiolcolor = NPC.GetAlpha(Taiolcolor);
            Vector2 DrawPosition = NPC.position + new Vector2((float)NPC.width, (float)NPC.height) / 2f - Main.screenPosition;
            DrawPosition -= new Vector2((float)Glow.Width, (float)(Glow.Height / frameCount)) * NPC.scale / 2f;
            DrawPosition += DrawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY - DrawOffsetY * 2.5f);
            Main.spriteBatch.Draw(Glow, DrawPosition, new Rectangle?(NPC.frame), Color.White * 0.3f, NPC.rotation, DrawOrigin, NPC.scale, spriteEffects, 0f);
        }
    }
}

