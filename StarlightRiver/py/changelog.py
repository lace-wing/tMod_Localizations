import utils
import time
import read_config_from_file as rc

def make_changelog(new_file:str, old_file:str):
    
    def writelog(fp, title:str, dict:dict):
        temps = f'----------------   {title}:   ----------------\n\n'
        for k,v in dict.items():
            os,ob = utils.hm(v[0])
            ns,nb = utils.hm(v[1])
            temps += f'{k}:\n    old:'
            if ob:
                temps += f'\n{os}\n'
            else:
                temps += f'      {os}'
            temps += '\n    current:'
            if nb:
                temps += f'\n{ns}\n\n'
            else:
                temps += f'  {ns}\n\n'
        
        fp.write(temps)
    
    hjson_dic_new = utils.load_hjson(rc.path, new_file)
    hjson_dic_old = utils.load_hjson(rc.path, old_file)

    dic_new = utils.flat_dict(hjson_dic_new)
    dic_old = utils.flat_dict(hjson_dic_old)
    
    dic_edit = {}
    dic_add = {}
    dic_remove = {}
    for nk,nv in dic_new.items():
        o = dic_old.get(nk)
        if o is not None:
            if o != nv:
                dic_edit[nk] = o, nv
        else:
            dic_add[nk] = '', nv
    for o in dic_old.keys():
        if o not in dic_new:
            dic_remove[o] = dic_old[o], ''

    t = '_'.join(time.asctime().replace(':', ' ').split(' '))
    tlog_file = rc.log_path + t + '_log.txt'

    with open(tlog_file, 'x', encoding='utf-8') as log:
        writelog(log, 'Changed', dic_edit)
        writelog(log, 'Added', dic_add)
        writelog(log, 'Removed', dic_remove)