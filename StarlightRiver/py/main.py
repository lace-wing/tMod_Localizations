import changelog
import updater
import uncomment
import read_config_from_file as rc

def main():
  rc.load()
  if rc.request == 'change log':
    changelog.make_changelog(rc.input, rc.old)
  elif rc.request == 'update':
    updater.update(rc.input, rc.extract, rc.output)
    changelog.make_changelog(rc.output, rc.input)
  elif rc.request == 'uncomment':
    uncomment.uncomment(rc.input, rc.output)

if __name__ == '__main__':
  main()
    
