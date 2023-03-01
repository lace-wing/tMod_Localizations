import hjson
import json
from collections import OrderedDict as o_dict

legal_keys = ('Mods.StarlightRiver', 'ItemName', 'ItemTooltip', 'ProjectileName', 'Containers', 'BiomeName', 'NPCName', \
            'BuffName', 'BuffDescription', 'Prefix', 'MapObject')

def flat_dict(dic:o_dict) -> o_dict:
    res = o_dict()
    def flat(prefix:str, dic:o_dict):
        for key, val in dic.items():
            if type(val) is o_dict:
                if key in legal_keys:
                    flat(prefix + '.' + key,val)
                else:
                    pass
            else:
                res[prefix + '.' + key] = val
    for key,val in dic.items():
        if type(val) is o_dict:
            flat(key,val)
    return res

def load_hjson(path:str, file:str) -> o_dict:
    with open(path + file, 'r', encoding='utf-8') as new:
        return hjson.load(new, encoding='utf-8')

# handle multiline
def hm(str:str) -> str:
    if '\n' not in str:
        return '\"' + str + '\"', False
    l = ['    ' + ss if ss != '' else '' for ss in  str.split('\n')]
    return '    \'\'\'\n' + '\n'.join(l) + '\n    \'\'\'', True

def is_translated(str:str) -> bool:
    for ch in str:
        if '\u4e00' <= ch <= '\u9fff:':
            return True
    return False