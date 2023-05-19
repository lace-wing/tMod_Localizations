import utils
import hjson
from collections import OrderedDict as o_dict
import read_config_from_file as rc

def uncomment(input:str, output:str):
  s = ''
  with open(rc.path + input, 'r', encoding='utf-8') as i:
    for line in i.readlines():
      line = line[:line.find('#')].rstrip() + '\n'
      s += line
  with open(rc.path + output, 'w', encoding='utf-8') as o:
    o.write(s)
