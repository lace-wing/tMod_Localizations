import hjson

# 配置文件路径
config_json_path = './StarlightRiver/py/config.hjson'

def load():
    with open(config_json_path, 'r', encoding='utf-8') as conjson:
        config = hjson.load(conjson)
        global request, path, log_path, old, input, extract, output
        
        request = config['request']
        path = config['path']
        log_path = config['log path']
        old = config['old']
        input = config['input']
        extract = config['extract']
        output = config ['output']
        
    import os
    if not os.path.exists(log_path):
        os.makedirs(log_path)
    return config

