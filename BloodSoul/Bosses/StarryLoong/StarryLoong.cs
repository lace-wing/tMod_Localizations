using BloodSoul.MyUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.StarryLoong
{
	[AutoloadBossHead]
	public class StarryLoongHead : ModNPC
	{
		public int Timer;
		public override void BossHeadRotation(ref float rotation)
		{
			rotation = NPC.rotation;
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("繁星龙");
			NPCID.Sets.TrailCacheLength[NPC.type] = 12;
			NPCID.Sets.TrailingMode[NPC.type] = 1;
		}
		public override void SetDefaults()
		{
			NPC.width = 50;
			NPC.height = 58;
			NPC.scale = 1.1f;
			NPC.netAlways = true;
			NPC.damage = 100;
			NPC.aiStyle = -1;
			NPC.defense = 100;
			NPC.boss = true;
			NPC.lifeMax = 51000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.behindTiles = true;
			NPC.trapImmune = true;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.knockBackResist = 0f;
			NPC.behindTiles = true;
			NPC.dontCountMe = true;
			//Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/StarryLoong");
		}
		public override void AI()
		{
			Player player = Main.player[NPC.target];

			NPC.TargetClosest(true);
			NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);
			if (player.dead)
			{
				NPC.TargetClosest(false);
				NPC.active = false;
				NPC.noTileCollide = true;
				NPC.velocity.Y = -100;
				NPC.timeLeft = 0;
			}
			if (NPC.ai[0] == 0f)
			{
				NPC.ai[3] = NPC.whoAmI;
				NPC.realLife = NPC.whoAmI;
				int num1 = NPC.whoAmI;
				int SpawnBody1 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + NPC.width / 2), (int)(NPC.Center.Y + NPC.height / 2), ModContent.NPCType<StarryLoongJunction>(), NPC.whoAmI);
				Main.npc[SpawnBody1].ai[3] = num1;
				Main.npc[SpawnBody1].ai[1] = num1;
				Main.npc[num1].ai[0] = SpawnBody1;
				num1 = SpawnBody1;
				Main.npc[SpawnBody1].realLife = NPC.whoAmI;
				NPC.netUpdate = true;
				for (int l = 0; l < 40; l++)
				{
					int SpawnBody2 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + NPC.width / 2), (int)(NPC.Center.Y + NPC.height / 2), ModContent.NPCType<StarryLoongBody>(), NPC.whoAmI);
					Main.npc[SpawnBody2].ai[3] = num1;
					Main.npc[SpawnBody2].ai[1] = num1;
					Main.npc[num1].ai[0] = SpawnBody1;
					num1 = SpawnBody2;
					Main.npc[SpawnBody2].realLife = NPC.whoAmI;
					NPC.netUpdate = true;
				}
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int SpawnBody3 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)(NPC.Center.X / 2), (int)(NPC.Center.Y / 2), ModContent.NPCType<StarryLoongTail>(), NPC.whoAmI);
					if (Main.netMode == NetmodeID.Server && SpawnBody3 < 200) NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, SpawnBody3);
					Main.npc[SpawnBody3].ai[3] = NPC.whoAmI;
					Main.npc[SpawnBody3].realLife = NPC.whoAmI;
					Main.npc[SpawnBody3].ai[1] = num1;
					Main.npc[SpawnBody3].ai[0] = num1;
				}
			}
			NPC.spriteDirection = 1;

			float speed;
			float turnspeed;
			float distance = Vector2.Distance(player.Center, NPC.Center);
			Vector2 NPCposition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
			float PlayerpositionX = player.position.X + (float)(player.width / 2);
			float PlayerPositionY = player.position.Y + (float)(player.height / 2);

			speed = 15 + 5 * (distance * 0.004f);
			turnspeed = (distance * 0.0003f);

			PlayerpositionX -= NPCposition.X;
			PlayerPositionY -= NPCposition.Y;
			float num193;
			if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length)
			{
				try
				{
					NPCposition = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
					PlayerpositionX = Main.npc[(int)NPC.ai[1]].position.X + (Main.npc[(int)NPC.ai[1]].width / 2) - NPCposition.X;
					PlayerPositionY = Main.npc[(int)NPC.ai[1]].position.Y + (Main.npc[(int)NPC.ai[1]].height / 2) - NPCposition.Y;
				}
				catch
				{
				}
				NPC.rotation = (float)Math.Atan2(PlayerPositionY, PlayerpositionX) + 1.57f;
				num193 = (float)Math.Sqrt((PlayerpositionX * PlayerpositionX + PlayerPositionY * PlayerPositionY));
				int num194 = NPC.width;
				num193 = (num193 - num194) / num193;
				PlayerpositionX *= num193 * 0.5f;
				PlayerPositionY *= num193 * 0.5f;
				NPC.velocity = Vector2.Zero;
				NPC.position.X = NPC.position.X + PlayerpositionX;
				NPC.position.Y = NPC.position.Y + PlayerPositionY;
				if (PlayerpositionX < 0f)
					NPC.spriteDirection = 1;
				if (PlayerpositionX > 0f)
					NPC.spriteDirection = -1;
			}
			else
			{
				float AbsPlayerpositionX = Math.Abs(PlayerpositionX);
				float AbsPlayerPositionY = Math.Abs(PlayerPositionY);
				num193 = (float)Math.Sqrt((PlayerpositionX * PlayerpositionX + PlayerPositionY * PlayerPositionY));
				float num198 = speed / num193;
				PlayerpositionX *= num198;
				PlayerPositionY *= num198;

				if ((NPC.velocity.X > 0f && PlayerpositionX > 0f) || (NPC.velocity.X < 0f && PlayerpositionX < 0f) || (NPC.velocity.Y > 0f && PlayerPositionY > 0f) || (NPC.velocity.Y < 0f && PlayerPositionY < 0f))
				{
					if (NPC.velocity.X < PlayerpositionX)
						NPC.velocity.X = NPC.velocity.X + turnspeed;
					else if (NPC.velocity.X > PlayerpositionX)
						NPC.velocity.X = NPC.velocity.X - turnspeed;

					if (NPC.velocity.Y < PlayerPositionY)
						NPC.velocity.Y = NPC.velocity.Y + turnspeed;
					else if (NPC.velocity.Y > PlayerPositionY)
						NPC.velocity.Y = NPC.velocity.Y - turnspeed;

					if (Math.Abs(PlayerPositionY) < speed * 0.2 && ((NPC.velocity.X > 0f && PlayerpositionX < 0f) || (NPC.velocity.X < 0f && PlayerpositionX > 0f)))
					{
						if (NPC.velocity.Y > 0f)
							NPC.velocity.Y = NPC.velocity.Y + turnspeed;
						else
							NPC.velocity.Y = NPC.velocity.Y - turnspeed;
					}

					if (Math.Abs(PlayerpositionX) < speed * 0.2 && ((NPC.velocity.Y > 0f && PlayerPositionY < 0f) || (NPC.velocity.Y < 0f && PlayerPositionY > 0f)))
					{
						if (NPC.velocity.X > 0f)
							NPC.velocity.X = NPC.velocity.X + turnspeed;
						else
							NPC.velocity.X = NPC.velocity.X - turnspeed;
					}
				}
				else
				{
					if (AbsPlayerpositionX > AbsPlayerPositionY)
					{
						if (NPC.velocity.X < PlayerpositionX)
							NPC.velocity.X = NPC.velocity.X + turnspeed;
						else if (NPC.velocity.X > PlayerpositionX)
							NPC.velocity.X = NPC.velocity.X - turnspeed;

						if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < speed * 0.5)
						{
							if (NPC.velocity.Y > 0f)
								NPC.velocity.Y = NPC.velocity.Y + turnspeed;
							else
								NPC.velocity.Y = NPC.velocity.Y - turnspeed;
						}
					}
					else
					{
						if (NPC.velocity.Y < PlayerPositionY)
							NPC.velocity.Y = NPC.velocity.Y + turnspeed;
						else if (NPC.velocity.Y > PlayerPositionY)
							NPC.velocity.Y = NPC.velocity.Y - turnspeed;

						if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < speed * 0.5)
						{
							if (NPC.velocity.X > 0f)
								NPC.velocity.X = NPC.velocity.X + turnspeed;
							else
								NPC.velocity.X = NPC.velocity.X - turnspeed;
						}
					}
				}
			}
			Timer++;
			if(Timer == 1)
            {
				Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<StarryLoongProj>(), 0, 0, player.whoAmI);
			}

			if (NPC.life > NPC.lifeMax * 0.8f)
			{
				if (Timer == 60 && Main.rand.Next(10) == 1)
				{
					Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(player.Center.X + Main.rand.Next(-200, -200), player.Center.Y - Main.rand.Next(-300, -300)), Vector2.Zero, ModContent.ProjectileType<Proj>(), 35, 0, player.whoAmI);
				}
				if (Timer == 60)
				{
					Timer = 2;
				}
			}
			else if (NPC.life < NPC.lifeMax * 0.8f &&	NPC.life > NPC.lifeMax * 0.5f)
            {
				if (Timer == 60 && Main.rand.Next(7) == 1)
				{
					Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(player.Center.X + Main.rand.Next(-150, -150), player.Center.Y - Main.rand.Next(-200, -200)), Vector2.Zero, ModContent.ProjectileType<Proj>(), 35, 0, player.whoAmI);
				}
				if (Timer == 60)
				{
					Timer = 2;
				}
			}
			else if (NPC.life < NPC.lifeMax * 0.5f)
			{
				if (Timer == 60 && Main.rand.Next(5) == 1)
				{
					Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), new Vector2(player.Center.X + Main.rand.Next(-300, -300), player.Center.Y - Main.rand.Next(-300, -300)), Vector2.Zero, ModContent.ProjectileType<Proj>(), 35, 0, player.whoAmI);
				}
				if (Timer == 60)
				{
					Timer = 2;
				}
			}
		}
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
		public override bool CheckActive()
        {
			return false;
        }
	}
	public class StarryLoongTail : ModNPC
	{
		public int Timer;
		public static Random random = new Random();
		public override void SetDefaults()
		{
			NPC.behindTiles = true;
			NPC.trapImmune = true;
			NPC.chaseable = true;
			NPC.canGhostHeal = true;
			NPC.npcSlots = 0f;
			NPC.width = 45;
			NPC.height = 32;
			NPC.scale = 1.2f;
			NPC.netAlways = true;
			NPC.dontCountMe = true;
			NPC.damage = 40;
			NPC.lifeMax = 30;
			NPC.defense = 30;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.knockBackResist = 0f;
			NPC.behindTiles = true;
		}
		public override void AI()
		{
			Player player = Main.player[NPC.target];
			if (player.dead)
			{
				NPC.TargetClosest(false);
				NPC.active = false;
				NPC.noTileCollide = true;
				NPC.velocity.Y = -100;
				NPC.timeLeft = 0;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (!Main.npc[(int)NPC.ai[1]].active)
				{
					NPC.life = 0;
					NPC.HitEffect(0, 10.0);
					NPC.active = false;
					NPC.netUpdate = true;
				}
			}
			if (Main.player[NPC.target].dead && NPC.timeLeft > 300)
				NPC.timeLeft = 0;
			if (NPC.ai[1] < (double)Main.npc.Length)
			{
				Vector2 npcCenter = new Vector2(NPC.position.X + NPC.width / 2, NPC.position.Y + NPC.height / 2);
				float dirX = Main.npc[(int)NPC.ai[1]].position.X + Main.npc[(int)NPC.ai[1]].width / 2 - npcCenter.X;
				float dirY = Main.npc[(int)NPC.ai[1]].position.Y + Main.npc[(int)NPC.ai[1]].height / 2 - npcCenter.Y;
				float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
				NPC.rotation = (float)Math.Atan2(dirY, dirX);
				float dist = (length - NPC.width) / length;
				float posX = dirX * dist;
				float posY = dirY * dist;
				NPC.position.X = NPC.position.X + posX;
				NPC.position.Y = NPC.position.Y + posY;
				NPC.direction = 0;
			}
			NPC.spriteDirection = 1;
			Timer++;
			if (Timer == 40)
			{
				if (random.Next(40) == 0)
				{
					Vector2 vector = player.Center - NPC.Center;
					vector.Normalize();
					vector *= 0.1f;
				}
				Timer = 0;
			}
		}
		public override bool CheckActive()
		{
			return false;
		}
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
	}
	public class StarryLoongBody : ModNPC
	{
		public int Timer;
		public bool Line = true;
		public static Random random = new Random();
		public override void SetDefaults()
		{
			NPC.behindTiles = true;
			NPC.trapImmune = true;
			NPC.chaseable = true;
			NPC.canGhostHeal = true;
			NPC.npcSlots = 0f;
			NPC.width = 25;
			NPC.height = 44;
			NPC.scale = 1.2f;
			NPC.dontCountMe = true;
			NPC.netAlways = true;
			NPC.damage = 40;
			NPC.lifeMax = 30;
			NPC.defense = 30;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.knockBackResist = 0f;
			NPC.behindTiles = true;
		}
		public override void AI()
		{
			Player player = Main.player[NPC.target];
			if (player.dead)
			{
				NPC.TargetClosest(false);
				NPC.active = false;
				NPC.noTileCollide = true;
				NPC.velocity.Y = -100;
				NPC.timeLeft = 0;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (!Main.npc[(int)NPC.ai[1]].active)
				{
					NPC.life = 0;
					NPC.HitEffect(0, 10.0);
					NPC.active = false;
					NPC.netUpdate = true;
				}
			}
			if (Main.player[NPC.target].dead && NPC.timeLeft > 300)
				NPC.timeLeft = 0;
			if (NPC.ai[1] < (double)Main.npc.Length)
			{
				Vector2 npcCenter = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
				float dirX = Main.npc[(int)NPC.ai[1]].position.X + Main.npc[(int)NPC.ai[1]].width / 2 - npcCenter.X;
				float dirY = Main.npc[(int)NPC.ai[1]].position.Y + Main.npc[(int)NPC.ai[1]].height / 2 - npcCenter.Y;
				NPC.rotation = (float)Math.Atan2(dirY, dirX);
				float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
				float dist = (length - NPC.width) / length;
				float posX = dirX * dist;
				float posY = dirY * dist;
				NPC.position.X = NPC.position.X + posX;
				NPC.position.Y = NPC.position.Y + posY;
			}
			NPC.spriteDirection = 1;

			Timer++;
			if (Timer == 400 && random.Next(8) == 1)
            {
				Line = false;
            }
			if(Timer == 460 && !Line)
            {
				Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<StarLight>(), 30, 0, player.whoAmI);
				Line = true;
			}
			if(Timer > 460)
            {
				Timer = 0;
			}
		}
		public override bool CheckActive()
		{
			return false;
		}
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if(!Line)
            {
				Player player = new Player();
				Vector2 vector = NPC.Center - Main.screenPosition;
				Texture2D value2 = (Texture2D)BloodSoulUtils.GetTexture("NPCs/Bosses/StarryLoong/StarLightBall");
				Vector2 origin = value2.Frame(1, 1, 0, 0).Size() * new Vector2(0f, 0.5f);
				Main.spriteBatch.Draw(value2, vector, null, new Color(75, 0, Main.DiscoB + 130), NPC.rotation, origin, 1, SpriteEffects.None, 0f);
			}
			return true;
		}
	}
	public class StarryLoongJunction : ModNPC
	{
		public override void SetDefaults()
		{
			NPC.behindTiles = true;
			NPC.trapImmune = true;
			NPC.boss = true;
			NPC.chaseable = true;
			NPC.canGhostHeal = true;
			NPC.npcSlots = 0f;
			NPC.width = 35;
			NPC.height = 60;
			NPC.scale = 1.2f;
			NPC.netAlways = true;
			NPC.damage = 20;
			NPC.lifeMax = 30;
			NPC.defense = 20;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.knockBackResist = 0f;
			NPC.behindTiles = true;
		}
		public override void AI()
		{
			Player player = Main.player[NPC.target];
			if (player.dead)
			{
				NPC.TargetClosest(false);
				NPC.active = false;
				NPC.noTileCollide = true;
				NPC.velocity.Y = -100;
				NPC.timeLeft = 0;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (!Main.npc[(int)NPC.ai[1]].active)
				{
					NPC.life = 0;
					NPC.HitEffect(0, 10.0);
					NPC.active = false;
					NPC.netUpdate = true;
				}
			}
			if (Main.player[NPC.target].dead && NPC.timeLeft > 300)
				NPC.timeLeft = 0;
			if (NPC.ai[1] < (double)Main.npc.Length)
			{
				Vector2 npcCenter = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
				float dirX = Main.npc[(int)NPC.ai[1]].position.X + Main.npc[(int)NPC.ai[1]].width / 2 - npcCenter.X;
				float dirY = Main.npc[(int)NPC.ai[1]].position.Y + Main.npc[(int)NPC.ai[1]].height / 2 - npcCenter.Y;
				NPC.rotation = (float)Math.Atan2(dirY, dirX);
				float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
				float dist = (length - NPC.width) / length;
				float posX = dirX * dist;
				float posY = dirY * dist;
				NPC.position.X = NPC.position.X + posX;
				NPC.position.Y = NPC.position.Y + posY;
			}
			NPC.spriteDirection = 1;
		}
		public override bool CheckActive()
		{
			return false;
		}
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
	}
}
