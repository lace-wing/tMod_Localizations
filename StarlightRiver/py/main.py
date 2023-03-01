import changelog
import utils
import updater
import read_config_from_file as rc


if __name__ == '__main__':
  rc.load()
  if rc.request == 'change log':
    changelog.make_changelog(rc.latest, rc.old)
  elif rc.request == 'update':
    updater.update(rc.latest, rc.extract, rc.update)
    changelog.make_changelog(rc.update, rc.latest)