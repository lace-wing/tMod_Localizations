if (ModLoader.HasMod("BossChecklist"))//如果有那个mod
            {
                Mod bossCheckList = ModLoader.GetMod("BossChecklist");//获取boss列表模组

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    14.5f,//时期 月总后
                    ModContent.NPCType<NPCs.Bosses.ApostleOfDeath.ApostleOfDeath>(),//npc
                    this,//本Mod
                    "$Mods.BloodErosion.NPCName.ApostleOfDeath",//获取命名
                    //() => BloodErosionSystem.downedApostleOfDeath,//检测击败
                    ModContent.ItemType<Items.InvertedCrossNecklace>(),//召唤物
                    new List<int>(0),//没有收藏品
                    new List<int>(0),//添加普通掉落物{ ModContent.ItemType<Items.BloodToothChaosFlesh>(), 
                    "$Mods.BloodErosion.BossSpawnInfo.ApostleOfDeath",//召唤条件
                    "$Mods.BloodErosion.Introduce.ApostleOfDeath",//介绍
                    "BloodErosion/Images/donwedApostleOfDeathImages"//图片
                    );
            }