import utils
import hjson
from collections import OrderedDict as o_dict
import read_config_from_file as rc

def update(latest:str, extract:str, update:str):
  ext = utils.load_hjson(rc.path, extract)
  ext = utils.flat_dict(ext)
  src = utils.load_hjson(rc.path, latest)
  src = utils.flat_dict(src)
  for key, val in src.items():
    if key in ext and utils.is_translated(val):
      ext[key] = val
  
  s = encode(ext)
  with open(rc.path + update, 'w', encoding='utf-8') as fp:
    fp.write(s)

def encode(extract_dict:o_dict):
  rst = o_dict()
  for key, val in extract_dict.items():
    lkey = key.split('.')
    lkey[0] += '.' + lkey[1]
    lkey.remove(lkey[1])
    
    trst = rst
    for i in lkey[:-1]:
      if i in trst:
        trst = trst[i]
      else:
        trst[i] = o_dict()
        trst = trst[i]
    trst[lkey[-1]] = val
    
  ext_str = hjson.dumps(rst).replace('\n  ', '\n').removeprefix('{\n').removesuffix('\n}')
  return ext_str