import hjson
import collections
import time

def flat_dict(dic):
    res = {}
    def flat(prefix, dic):
        for key, val in dic.items():
            if type(val) is collections.OrderedDict:
                flat(prefix+'.'+key,val)
            else:
                res[prefix + '.' + key] = val
    for key,val in dic.items():
        if type(val) is collections.OrderedDict:
            flat(key,val)
    return res

def make_changelog(path, new_file, old_file, log_file):
    with open(path + new_file, 'r', encoding='utf-8') as new:
        hjson_dic_new = hjson.load(new, encoding='utf-8')

    with open(path + old_file, 'r', encoding='utf-8') as old:
        hjson_dic_old = hjson.load(old, encoding='utf-8')

    dic_new = flat_dict(hjson_dic_new)
    dic_old = flat_dict(hjson_dic_old)

    dic_edit = {}
    dic_add = {}
    dic_remove = {}
    for nk,nv in dic_new.items():
        o = dic_old.get(nk)
        if o is not None:
            if o != nv:
                dic_edit[nk] = o, nv
        else:
            dic_add[nk] = nv
    for o in dic_old.keys():
        if o not in dic_new:
            dic_remove[o] = dic_old[o]

    t = '_'.join(time.asctime().replace(':', ' ').split(' '))
    tlog_file = log_file + t + '_log.txt'

    with open(tlog_file, 'x', encoding='utf-8') as log:
        log.write('----------------   Changed:   ----------------\n\n')
        for k,v in dic_edit.items():
            log.write(f'{k}:\n    old:      "{v[0]}"\n    current:  "{v[1]}"\n\n')
        
        log.write('\n\n----------------   Added:   ----------------\n\n')
        for k,v in dic_add.items():
            log.write(f'{k}:\n    old:      ""\n\tcurrent:  "{v}"\n\n')
        
        log.write('\n\n----------------   Removed:   ----------------\n\n')
        for k,v in dic_remove.items():
            log.write(f'{k}:\n    old:      "{v}"\n\tcurrent:  ""\n\n')
