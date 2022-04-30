if (ModLoader.HasMod("BossChecklist"))//如果有那个mod
            {
                Mod bossCheckList = ModLoader.GetMod("BossChecklist");//获取boss列表模组

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    1.5f,//时期 史莱姆王后
                    ModContent.NPCType<GelSpider>(),//npc
                    this,//本Mod
                    "$Mods.BloodSoul.NPCName.GelSpider",//获取命名
                    () => BloodSoulSystem.downedGelSpider,//检测击败
                    ModContent.ItemType<Items.EvidenceOfKingSlime>(),//召唤物
                    new List<int>(0),//没有收藏品
                    new List<int> { ModContent.ItemType<Items.GelGold>(), ModContent.ItemType<Items.BossBag.GelSpiderBossBag>() },//添加普通掉落物
                    "$Mods.BloodSoul.BossSpawnInfo.GelSpider",//召唤条件
                    "$Mods.BloodSoul.Introduce.GelSpider",//介绍
                    "BloodSoul/Images/donwedGelSpiderImages"//图片
                    );

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    5.5f,//时期 骷髅王后
                    ModContent.NPCType<NPCs.Bosses.HolyLightSwords.HolyLightSword>(),//npc
                    this,//本Mod
                    "$Mods.BloodSoul.NPCName.HolyLightSword",//获取阴凌剑的命名
                    () => BloodSoulSystem.downedHolyLightSword,//检测击败
                    ModContent.ItemType<Items.HolySword>(),//召唤物
                    new List<int>(0),//没有收藏品
                    new List<int> { ModContent.ItemType<Items.HolyFragment>(), ModContent.ItemType<Items.BossBag.HolySwordBossBag>() },//添加普通掉落物
                    "$Mods.BloodSoul.BossSpawnInfo.HolyLightSword",//召唤条件
                    "$Mods.BloodSoul.Introduce.HolyLightSword",//介绍
                    "BloodSoul/Images/donwedHolyLightSwordImages"//图片
                    );

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    5.6f,//时期 阴凌剑后
                    ModContent.NPCType<NPCs.Bosses.BloodCrystalEyes.BloodCrystalEye>(),//npc
                    this,//本Mod
                    "$Mods.BloodSoul.NPCName.BloodCrystalEye",//获取血眼的命名
                    () => BloodSoulSystem.downedBloodCrystalEye,//检测击败
                    ModContent.ItemType<Items.SumBloodEyeBoss>(),//召唤物
                    new List<int>(0),//没有收藏品
                    new List<int> { ModContent.ItemType<Items.BloodToothChaosFlesh>(), ModContent.ItemType<Items.BossBag.BloodEyeBossBag>() },//添加普通掉落物
                    "$Mods.BloodSoul.BossSpawnInfo.BloodCrystalEye",//召唤条件
                    "$Mods.BloodSoul.Introduce.BloodCrystalEye",//介绍
                    "BloodSoul/Images/donwedBloodCrystalEyeImages"//图片
                    );

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    5.7f,//时期 血眼后
                    ModContent.NPCType<NPCs.Bosses.SharaIshvalda.SharaIshvaldaBody>(),//npc
                    this,//本Mod
                    "$Mods.BloodSoul.NPCName.SharaIshvaldaBody",//获取血眼的命名
                    () => BloodSoulSystem.SharaIshvalda,//检测击败
                    ModContent.ItemType<Items.DifferentWorld2>(),//召唤物
                    new List<int>(0),//没有收藏品
                                     //new List<int> { ModContent.ItemType<Items.BloodToothChaosFlesh>(), ModContent.ItemType<Items.BossBag.BloodEyeBossBag>() },//添加普通掉落物
                    "$Mods.BloodSoul.BossSpawnInfo.SharaIshvaldaBody",//召唤条件
                    "$Mods.BloodSoul.Introduce.SharaIshvaldaBody",//介绍
                    "BloodSoul/Images/SharaIshvaldaImages"//图片
                    );

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    13.5f,//时期 月总前
                    ModContent.NPCType<NPCs.Bosses.TheStarGazer.StarGazerBoss>(),//npc
                    this,//本Mod
                    "$Mods.BloodSoul.NPCName.StarGazerBoss",//获取命名
                    () => BloodSoulSystem.downedStarGazer,//检测击败
                    ModContent.ItemType<Items.StarProof>(),//召唤物
                    new List<int>(0),//没有收藏品
                    new List<int>(0),//添加普通掉落物
                    "$Mods.BloodSoul.BossSpawnInfo.StarGazerBoss",//召唤条件
                    "$Mods.BloodSoul.Introduce.StarGazerBoss",//介绍
                    "BloodSoul/Images/donwedStarGazerBossImages"//图片
                    );
            }