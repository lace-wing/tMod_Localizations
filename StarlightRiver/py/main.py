import changelog
import json

config_json_path = './StarlightRiver/py/config.json'

def init_config():
  with open(config_json_path, 'r', encoding='utf-8') as conjson:
    config = json.load(conjson)
    
    global config_path
    config_path = config['path']
    
    global config_old
    config_old = config['old']
    
    global config_new
    config_new = config['new']
    
    global config_log
    config_log = config['log_path']
    
    import os
    if not os.path.exists(config_log):
      os.makedirs(config_log)


if __name__ == '__main__':
  init_config()
  changelog.make_changelog(config_path, config_new, config_old, config_log)